using System;
using System.Collections.Generic;
using System.Text;
using LiveResx.Avalonia.SourceGenerators.Models;
using Microsoft.CodeAnalysis;

namespace LiveResx.Avalonia.SourceGenerators.Generators;

/// <summary>
/// Emits the <c>LiveResx.Avalonia.__LiveResxRegistration</c> class with a
/// <see cref="System.Runtime.CompilerServices.ModuleInitializerAttribute"/> that calls
/// <c>DynamicLocalization.Instance.Register</c> with all discovered
/// <see cref="DynamicTranslation"/> properties and the correct culture-setter callback.
/// </summary>
internal static class LiveResxRegistrationGenerator
{
    /// <summary>
    /// Emits the <c>LiveResx.Avalonia.__LiveResxRegistration.g.cs</c> source file.
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
        sb.AppendLine("    internal static class __LiveResxRegistration");
        sb.AppendLine("    {");
        sb.AppendLine("        [System.Runtime.CompilerServices.ModuleInitializer]");
        sb.AppendLine("        internal static void Initialize()");
        sb.AppendLine("        {");

        if (types.Count == 0)
        {
            sb.AppendLine("            global::LiveResx.Avalonia.DynamicLocalization.Instance.Register(");
            sb.AppendLine("                new global::LiveResx.Avalonia.DynamicTranslation[0],");
            sb.AppendLine("                _ => { });");
        }
        else
        {
            sb.AppendLine("            global::LiveResx.Avalonia.DynamicLocalization.Instance.Register(");
            sb.AppendLine("                new global::LiveResx.Avalonia.DynamicTranslation[]");
            sb.AppendLine("                {");

            foreach (var type in types)
            {
                foreach (var key in type.ResourceKeys)
                {
                    sb.AppendLine($"                    global::LiveResx.Avalonia.DynamicResources.{key},");
                }
            }

            sb.AppendLine("                },");
            sb.AppendLine("                culture =>");
            sb.AppendLine("                {");

            foreach (var type in types)
            {
                sb.AppendLine($"                    global::{type.FullTypeName}.Culture = culture;");
            }

            sb.AppendLine("                });");
        }

        sb.AppendLine("        }");
        sb.AppendLine("    }");
        sb.AppendLine("}");
        ctx.AddSource("LiveResx.Avalonia.__LiveResxRegistration.g.cs", sb.ToString());
    }
}
