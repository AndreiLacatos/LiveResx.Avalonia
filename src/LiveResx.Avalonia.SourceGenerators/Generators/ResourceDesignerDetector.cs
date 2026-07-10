using System.Collections.Generic;
using System.Threading;
using LiveResx.Avalonia.SourceGenerators.Models;
using Microsoft.CodeAnalysis;

namespace LiveResx.Avalonia.SourceGenerators.Generators;

/// <summary>
/// Detects resource Designer types in the compilation by matching the well-known
/// fingerprint produced by <c>ResXFileCodeGenerator</c> / <c>PublicResXFileCodeGenerator</c>:
/// a type with static <c>ResourceManager</c> and <c>Culture</c> properties plus
/// one or more static <see cref="string"/> properties representing resource keys.
/// </summary>
internal static class ResourceDesignerDetector
{
    /// <summary>
    /// Walks all types in the <paramref name="compilation"/> (both source and
    /// metadata references) and returns those that match the resource designer
    /// fingerprint.
    /// </summary>
    internal static IReadOnlyList<ResourceDesignerType> Detect(
        Compilation compilation, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();

        var results = new List<ResourceDesignerType>();

        foreach (var type in WalkAllTypes(compilation.GlobalNamespace, ct))
        {
            var members = type.GetMembers();
            if (!IsResourceDesignerType(members))
                continue;

            var keys = GetResourceKeys(members);
            if (keys.Count == 0)
                continue;

            var ns = type.ContainingNamespace?.ToDisplayString() ?? string.Empty;

            results.Add(new ResourceDesignerType(
                fullTypeName: string.IsNullOrEmpty(ns) ? type.Name : $"{ns}.{type.Name}",
                @namespace: ns,
                typeName: type.Name,
                resourceKeys: keys));
        }

        return results;
    }

    /// <summary>
    /// Recursively enumerates all named type symbols reachable from the given
    /// <paramref name="symbol"/>, which may be a namespace (yielding all types
    /// within) or a named type (yielding the type itself and its nested types).
    /// <para />
    /// This respects <see cref="ISymbol.GetMembers"/> on both
    /// <see cref="INamespaceSymbol"/> (child namespaces and types) and
    /// <see cref="INamedTypeSymbol"/> (nested types). Non-type members such as
    /// properties and methods are never returned; they are transparently skipped
    /// by the <c>is INamespaceOrTypeSymbol</c> filter.
    /// <para />
    /// The traversal is depth-first and yields <see cref="INamedTypeSymbol"/>
    /// instances for every type in every referenced assembly and all source files
    /// contributed to the <see cref="Compilation"/>. The recursion eagerly
    /// enumerates <c>GetMembers()</c> on each node; for large compilations with
    /// deep nesting, the call stack depth matches the nesting depth.
    /// </summary>
    /// <param name="symbol">The root namespace or type to start the walk from.
    /// Typically <c>compilation.GlobalNamespace</c> for a full-compilation sweep.</param>
    /// <param name="ct">Cancellation token checked at each recursive step.</param>
    private static IEnumerable<INamedTypeSymbol> WalkAllTypes(
        INamespaceOrTypeSymbol symbol, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();

        if (symbol is INamedTypeSymbol namedType)
            yield return namedType;

        foreach (var member in symbol.GetMembers())
        {
            if (member is INamespaceOrTypeSymbol child)
            {
                foreach (var nested in WalkAllTypes(child, ct))
                    yield return nested;
            }
        }
    }

    /// <summary>
    /// Determines whether <paramref name="type"/> matches the resource designer
    /// fingerprint: a type that exposes both a static <c>ResourceManager</c>
    /// property (returning <see cref="System.Resources.ResourceManager"/>) and a
    /// static <c>Culture</c> property (returning
    /// <see cref="System.Globalization.CultureInfo"/>).
    /// <para />
    /// Detection relies exclusively on symbol metadata — no <see cref="SemanticModel"/>
    /// or syntax tree analysis is performed. The property return type is verified
    /// by comparing <see cref="ITypeSymbol.Name"/> to the well-known BCL type
    /// names <c>"ResourceManager"</c> and <c>"CultureInfo"</c>. This is a reliable
    /// heuristic because no user-defined type is expected to shadow these BCL
    /// identifiers on a type that also carries the matching property names.
    /// <para />
    /// Only <see cref="IPropertySymbol"/> members where
    /// <see cref="IPropertySymbol.IsStatic"/> is <c>true</c> are considered;
    /// instance properties, fields, methods, and nested types are ignored.
    /// The fingerprint is deliberately conservative: both properties must be
    /// present for a match. Types with only one of the two are rejected.
    /// </summary>
    private static bool IsResourceDesignerType(IReadOnlyList<ISymbol> members)
    {
        var hasResourceManager = false;
        var hasCulture = false;

        foreach (var member in members)
        {
            if (member is not IPropertySymbol prop || !prop.IsStatic)
                continue;

            if (prop.Name == "ResourceManager" && prop.Type.Name == "ResourceManager")
                hasResourceManager = true;
            else if (prop.Name == "Culture" && prop.Type.Name == "CultureInfo")
                hasCulture = true;
        }

        return hasResourceManager && hasCulture;
    }

    /// <summary>
    /// Extracts the resource key names from a confirmed resource designer
    /// <paramref name="type"/> by collecting all static string property members
    /// that are not the well-known <c>ResourceManager</c> or <c>Culture</c>
    /// properties.
    /// <para />
    /// Selection criteria (all must hold):
    /// <list type="bullet">
    ///   <item><description>The member is an <see cref="IPropertySymbol"/>.</description></item>
    ///   <item><description><see cref="IPropertySymbol.IsStatic"/> is <c>true</c>.</description></item>
    ///   <item><description><see cref="ISymbol.DeclaredAccessibility"/> is
    ///   <see cref="Accessibility.Public"/> or <see cref="Accessibility.Internal"/> —
    ///   matches both <c>PublicResXFileCodeGenerator</c> and
    ///   <c>InternalResXFileCodeGenerator</c> output.</description></item>
    ///   <item><description><see cref="IPropertySymbol.Type"/> has
    ///   <see cref="ITypeSymbol.SpecialType"/> equal to
    ///   <see cref="SpecialType.System_String"/>.</description></item>
    ///   <item><description>The property name is neither <c>"ResourceManager"</c>
    ///   nor <c>"Culture"</c>.</description></item>
    /// </list>
    /// <para />
    /// This method follows the convention that the property name in the generated
    /// <c>Resources.Designer.cs</c> matches the resource key in the <c>.resx</c>
    /// file — a guarantee provided by <c>ResXFileCodeGenerator</c> and
    /// <c>PublicResXFileCodeGenerator</c>. No attempt is made to verify that the
    /// getter body calls <c>ResourceManager.GetString(key, ...)</c>; doing so
    /// would require <see cref="SemanticModel"/> for source types and is
    /// impossible for metadata references.
    /// </summary>
    private static IReadOnlyList<string> GetResourceKeys(IReadOnlyList<ISymbol> members)
    {
        var keys = new List<string>();

        foreach (var member in members)
        {
            if (member is not IPropertySymbol prop)
                continue;

            if (!prop.IsStatic)
                continue;

            if (prop.DeclaredAccessibility != Accessibility.Public &&
                prop.DeclaredAccessibility != Accessibility.Internal)
                continue;

            if (prop.Type.SpecialType != SpecialType.System_String)
                continue;

            if (prop.Name == "ResourceManager" || prop.Name == "Culture")
                continue;

            keys.Add(prop.Name);
        }

        return keys;
    }
}
