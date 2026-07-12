using System;
using System.Collections.Generic;
using LiveResx.Avalonia.SourceGenerators.Models;
using Microsoft.CodeAnalysis;

namespace LiveResx.Avalonia.SourceGenerators.Generators;

/// <summary>
/// Emits <c>LRX001</c> warnings when two or more definitions would produce a
/// property with the same name on <see cref="DynamicResources"/>.
/// Collisions are detected across both <c>.resx</c> resource keys and
/// <see cref="ILocalizedResource{T}"/> implementor class names.
/// </summary>
internal static class ResourceDiagnosticEmitter
{
    /// <summary>
    /// Diagnostic descriptor for the duplicate / conflicting resource name rule.
    /// </summary>
    internal static readonly DiagnosticDescriptor DuplicateResourceKeyRule = new(
        id: "LRX001",
        title: "Duplicate resource key",
        messageFormat: "Resource key '{0}' is defined by both '{1}' and '{2}'. Remove or rename one to avoid a compilation error.",
        category: "LiveResx.Avalonia",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true);

    /// <summary>
    /// Inspects the discovered resource designer types and custom resource
    /// descriptors, then reports <c>LRX001</c> for every name that appears
    /// in more than one source.
    /// </summary>
    /// <param name="ctx">The source production context used to report diagnostics.</param>
    /// <param name="designerTypes">Resource designer types discovered in the compilation.</param>
    /// <param name="customTypes"><see cref="ILocalizedResource{T}"/> implementors discovered in the compilation.</param>
    internal static void Emit(
        SourceProductionContext ctx,
        IReadOnlyList<ResourceDesignerType> designerTypes,
        IReadOnlyList<LocalizedResourceDescriptor> customTypes)
    {
        var seen = new Dictionary<string, List<string>>(StringComparer.Ordinal);

        foreach (var dt in designerTypes)
        {
            foreach (var key in dt.ResourceKeys)
            {
                AddSource(seen, key, dt.FullTypeName);
            }
        }

        foreach (var ct in customTypes)
        {
            AddSource(seen, ct.GetterName, ct.ImplementorFullName);
        }

        foreach (var kvp in seen)
        {
            if (kvp.Value.Count > 1)
            {
                ctx.ReportDiagnostic(Diagnostic.Create(
                    DuplicateResourceKeyRule,
                    Location.None,
                    kvp.Key,
                    kvp.Value[0],
                    kvp.Value[1]));
            }
        }
    }

    private static void AddSource(
        Dictionary<string, List<string>> map,
        string name,
        string source)
    {
        if (!map.TryGetValue(name, out var list))
        {
            map[name] = list = new List<string>();
        }

        list.Add(source);
    }
}
