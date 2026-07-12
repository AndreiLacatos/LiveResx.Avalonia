using System;
using System.Collections.Generic;
using System.Threading;
using LiveResx.Avalonia.SourceGenerators.Models;
using Microsoft.CodeAnalysis;

namespace LiveResx.Avalonia.SourceGenerators.Generators;

/// <summary>
/// Detects types in the compilation that implement <see cref="ILocalizedResource{T}"/>
/// and produces descriptors for each valid implementation.
/// </summary>
internal static class CustomResourceDetector
{
    /// <summary>
    /// Walks all types in the <paramref name="compilation"/> and returns descriptors
    /// for each concrete (non-abstract, non-static, non-generic) class that implements
    /// <c>LiveResx.Avalonia.ILocalizedResource&lt;T&gt;</c>.
    /// </summary>
    internal static IReadOnlyList<LocalizedResourceDescriptor> Detect(
        Compilation compilation, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();

        // Resolve the ILocalizedResource<T> interface symbol from the compilation's
        // metadata. If it cannot be resolved (e.g. during a design-time build where
        // references aren't fully loaded), return empty immediately.
        var interfaceSymbol = compilation.GetTypeByMetadataName(
            "LiveResx.Avalonia.ILocalizedResource`1");
        if (interfaceSymbol == null)
            return Array.Empty<LocalizedResourceDescriptor>();

        var results = new List<LocalizedResourceDescriptor>();

        foreach (var type in WalkAllTypes(compilation.GlobalNamespace, ct))
        {
            ct.ThrowIfCancellationRequested();

            // Must be a concrete (non-static, non-abstract) class
            if (type.TypeKind != TypeKind.Class)
                continue;

            if (type.IsStatic || type.IsAbstract)
                continue;

            // Must not be an open generic type
            if (type.TypeArguments.Length > 0 && SymbolEqualityComparer.Default.Equals(type.OriginalDefinition, type))
                continue;

            // Find the matching ILocalizedResource<T> in the interface list
            foreach (var iface in type.AllInterfaces)
            {
                // Use SymbolEqualityComparer for reliable matching even during
                // design-time builds where metadata resolution may be incomplete.
                if (!SymbolEqualityComparer.Default.Equals(iface.OriginalDefinition, interfaceSymbol))
                    continue;

                if (iface.TypeArguments.Length != 1)
                    continue;

                // Found a valid implementation
                var valueType = iface.TypeArguments[0];
                var getterName = type.Name;

                var implementorFullName = type.ToDisplayString(
                    new SymbolDisplayFormat(
                        typeQualificationStyle:
                            SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces,
                        globalNamespaceStyle:
                            SymbolDisplayGlobalNamespaceStyle.Included));

                var valueTypeFullName = valueType.ToDisplayString(
                    new SymbolDisplayFormat(
                        typeQualificationStyle:
                            SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces,
                        globalNamespaceStyle:
                            SymbolDisplayGlobalNamespaceStyle.Included));

                results.Add(new LocalizedResourceDescriptor(
                    getterName,
                    valueTypeFullName,
                    implementorFullName));

                break; // only the first matching interface
            }
        }

        return results;
    }

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
}
