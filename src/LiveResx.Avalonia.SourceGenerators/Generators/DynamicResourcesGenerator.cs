using System;
using System.Collections.Generic;
using System.Text;
using LiveResx.Avalonia.SourceGenerators.Models;
using Microsoft.CodeAnalysis;

namespace LiveResx.Avalonia.SourceGenerators.Generators;

/// <summary>
/// Emits the <c>LiveResx.Avalonia.DynamicResources</c> static class with
/// <see cref="DynamicTranslation"/> properties for each discovered resource designer
/// and unwrapped <c>LocalizedResource&lt;T&gt;</c> getters for each
/// <see cref="ILocalizedResource{T}"/> implementor.
/// </summary>
internal static class DynamicResourcesGenerator
{
    /// <summary>
    /// Emits the <c>LiveResx.Avalonia.DynamicResources.g.cs</c> source file.
    /// </summary>
    /// <param name="ctx">The source production context.</param>
    /// <param name="timestamp">The timestamp captured in <see cref="GeneratorDependencies.TimestampProvider"/>.</param>
    /// <param name="designerTypes">The resource designer types detected in the compilation.</param>
    /// <param name="customTypes">The <see cref="ILocalizedResource{T}"/> implementors detected in the compilation.</param>
    internal static void Emit(
        SourceProductionContext ctx,
        DateTimeOffset timestamp,
        IReadOnlyList<ResourceDesignerType> designerTypes,
        IReadOnlyList<LocalizedResourceDescriptor> customTypes)
    {
        var sb = new StringBuilder();
        sb.Append(GeneratorHeader.Generate(timestamp));
        sb.AppendLine("namespace LiveResx.Avalonia");
        sb.AppendLine("{");
        sb.AppendLine("    " + GeneratorHeader.GeneratedCodeAttribute);
        sb.AppendLine("    [global::System.Diagnostics.DebuggerNonUserCode]");
        sb.AppendLine("    [global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]");
        sb.AppendLine("    public static class DynamicResources");
        sb.AppendLine("    {");

        // Emit resx-based DynamicTranslation properties
        foreach (var type in designerTypes)
        {
            foreach (var key in type.ResourceKeys)
            {
                sb.AppendLine($"        public static global::LiveResx.Avalonia.DynamicTranslation {key} {{ get; }} = new global::LiveResx.Avalonia.DynamicTranslation(");
                sb.AppendLine($"            \"{key}\",");
                sb.AppendLine($"            () => global::{type.FullTypeName}.ResourceManager);");
            }
        }

        // Emit custom resource backing fields and unwrapped getters
        foreach (var custom in customTypes)
        {
            var fieldName = $"s_{custom.GetterName}";
            sb.AppendLine();
            sb.AppendLine($"        internal static readonly global::LiveResx.Avalonia.LocalizedResource<{custom.ValueTypeFullName}> {fieldName} =");
            sb.AppendLine($"            new global::LiveResx.Avalonia.LocalizedResource<{custom.ValueTypeFullName}>(");
            sb.AppendLine($"                \"{custom.GetterName}\",");
            sb.AppendLine($"                new global::System.Collections.Generic.Dictionary<global::System.Globalization.CultureInfo, {custom.ValueTypeFullName}>(");
            sb.AppendLine($"                    new {custom.ImplementorFullName}().Values),");
            sb.AppendLine($"                global::LiveResx.Avalonia.FallbackBehavior.Invariant);");
            sb.AppendLine();
            sb.AppendLine($"        public static global::LiveResx.Avalonia.LocalizedResource<{custom.ValueTypeFullName}> {custom.GetterName} => {fieldName};");
        }

        sb.AppendLine("    }");
        sb.AppendLine("}");
        ctx.AddSource("LiveResx.Avalonia.DynamicResources.g.cs", sb.ToString());
    }
}
