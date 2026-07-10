using System;
using System.Collections.Generic;
using System.Text;
using LiveResx.Avalonia.SourceGenerators.Models;
using Microsoft.CodeAnalysis;

namespace LiveResx.Avalonia.SourceGenerators.Generators;

/// <summary>
/// Emits the <c>LiveResx.Avalonia.DynamicResources</c> partial class with one
/// <see cref="DynamicTranslation"/> property per resource key discovered by the
/// <see cref="ResourceDesignerDetector"/>.
/// </summary>
internal static class DynamicResourcesGenerator
{
    /// <summary>
    /// Emits the <c>LiveResx.Avalonia.DynamicResources.g.cs</c> source file.
    /// </summary>
    /// <param name="ctx">The source production context.</param>
    /// <param name="timestamp">The timestamp captured in <see cref="GeneratorDependencies.TimestampProvider"/>.</param>
    /// <param name="types">The resource designer types detected in the compilation.</param>
    internal static void Emit(
        SourceProductionContext ctx,
        DateTimeOffset timestamp,
        IReadOnlyList<ResourceDesignerType> types)
    {
        var sb = new StringBuilder();
        sb.Append(GeneratorHeader.Generate(timestamp));
        sb.AppendLine("namespace LiveResx.Avalonia");
        sb.AppendLine("{");
        sb.AppendLine("    public static class DynamicResources");
        sb.AppendLine("    {");

        foreach (var type in types)
        {
            foreach (var key in type.ResourceKeys)
            {
                sb.AppendLine($"        public static DynamicTranslation {key} {{ get; }} = new DynamicTranslation(");
                sb.AppendLine($"            \"{key}\",");
                sb.AppendLine($"            () => {type.FullTypeName}.ResourceManager);");
            }
        }

        sb.AppendLine("    }");
        sb.AppendLine("}");
        ctx.AddSource("LiveResx.Avalonia.DynamicResources.g.cs", sb.ToString());
    }
}
