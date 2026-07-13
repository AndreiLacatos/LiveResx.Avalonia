using System;
using System.Text;
using Microsoft.CodeAnalysis;

namespace LiveResx.Avalonia.SourceGenerators.Generators;

/// <summary>
/// Generates the <c>TranslateExtension</c> markup extension class that enables
/// data-binding to <see cref="DynamicTranslation"/> instances in Avalonia XAML.
/// </summary>
internal static class TranslateExtensionGenerator
{
    /// <summary>
    /// Emits the <c>TranslateExtension.g.cs</c> source file containing the
    /// <c>LiveResx.Avalonia.TranslateExtension</c> class.
    /// </summary>
    internal static void Emit(SourceProductionContext ctx, DateTimeOffset timestamp)
    {
        var sb = new StringBuilder();
        sb.Append(GeneratorHeader.Generate(timestamp));
        sb.AppendLine("namespace LiveResx.Avalonia");
        sb.AppendLine("{");
        sb.AppendLine("    /// <summary>");
        sb.AppendLine("    /// Provides a markup extension that returns a data-binding to a <see cref=\"DynamicTranslation\"/> instance,");
        sb.AppendLine("    /// enabling automatic UI updates when the application culture changes.");
        sb.AppendLine("    /// </summary>");
        sb.AppendLine("    " + GeneratorHeader.GeneratedCodeAttribute);
        sb.AppendLine("    [global::System.Diagnostics.DebuggerNonUserCode]");
        sb.AppendLine("    [global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]");
        sb.AppendLine("    public sealed class TranslateExtension");
        sb.AppendLine("    {");
        sb.AppendLine("        private readonly global::LiveResx.Avalonia.DynamicTranslation _translation;");
        sb.AppendLine();
        sb.AppendLine("        /// <summary>");
        sb.AppendLine("        /// Initializes a new instance of the <see cref=\"TranslateExtension\"/> class.");
        sb.AppendLine("        /// </summary>");
        sb.AppendLine("        /// <param name=\"translation\">The dynamic translation to bind to.</param>");
        sb.AppendLine("        public TranslateExtension(global::LiveResx.Avalonia.DynamicTranslation translation)");
        sb.AppendLine("        {");
        sb.AppendLine("            _translation = translation;");
        sb.AppendLine("        }");
        sb.AppendLine();
        sb.AppendLine("        /// <summary>");
        sb.AppendLine("        /// Provides the value for the markup extension, returning a data-binding");
        sb.AppendLine("        /// to the <c>Text</c> property of the underlying <see cref=\"DynamicTranslation\"/>.");
        sb.AppendLine("        /// </summary>");
        sb.AppendLine("        /// <param name=\"serviceProvider\">The service provider for the markup extension.</param>");
        sb.AppendLine("        /// <returns>A <see cref=\"global::Avalonia.Data.Binding\"/> to <c>DynamicTranslation.Text</c>.</returns>");
        sb.AppendLine("        public object ProvideValue(global::System.IServiceProvider serviceProvider)");
        sb.AppendLine("        {");
        sb.AppendLine("            return new global::Avalonia.Data.Binding(\"Text\")");
        sb.AppendLine("            {");
        sb.AppendLine("                Source = _translation,");
        sb.AppendLine("            };");
        sb.AppendLine("        }");
        sb.AppendLine("    }");
        sb.Append("}");
        ctx.AddSource("LiveResx.Avalonia.TranslateExtension.g.cs", sb.ToString());
    }
}
