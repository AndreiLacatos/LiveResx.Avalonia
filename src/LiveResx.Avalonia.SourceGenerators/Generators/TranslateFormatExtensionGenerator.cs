using System;
using System.Text;
using Microsoft.CodeAnalysis;

namespace LiveResx.Avalonia.SourceGenerators.Generators;

/// <summary>
/// Generates the <c>TranslateFormatExtension</c> markup extension class that supports
/// composite string formatting with data-bound arguments for <see cref="DynamicTranslation"/>
/// instances in Avalonia XAML.
/// </summary>
internal static class TranslateFormatExtensionGenerator
{
    /// <summary>
    /// Emits the <c>TranslateFormatExtension.g.cs</c> source file containing the
    /// <c>LiveResx.Avalonia.TranslateFormatExtension</c> class.
    /// </summary>
    internal static void Emit(SourceProductionContext ctx, DateTimeOffset timestamp)
    {
        var sb = new StringBuilder();
        sb.Append(GeneratorHeader.Generate(timestamp));
        sb.AppendLine("namespace LiveResx.Avalonia");
        sb.AppendLine("{");
        sb.AppendLine("    /// <summary>");
        sb.AppendLine("    /// Provides a markup extension that returns a <see cref=\"global::Avalonia.Data.MultiBinding\"/>");
        sb.AppendLine("    /// combining a format-template <see cref=\"DynamicTranslation\"/> with additional data-bound");
        sb.AppendLine("    /// arguments, enabling localized composite strings that update automatically when the");
        sb.AppendLine("    /// application culture changes.");
        sb.AppendLine("    /// </summary>");
        sb.AppendLine("    " + GeneratorHeader.GeneratedCodeAttribute);
        sb.AppendLine("    [global::System.Diagnostics.DebuggerNonUserCode]");
        sb.AppendLine("    [global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]");
        sb.AppendLine("    public sealed class TranslateFormatExtension");
        sb.AppendLine("    {");
        sb.AppendLine("        /// <summary>");
        sb.AppendLine("        /// Gets or sets the <see cref=\"DynamicTranslation\"/> that provides the format template string.");
        sb.AppendLine("        /// </summary>");
        sb.AppendLine("        public global::LiveResx.Avalonia.DynamicTranslation Template { get; set; }");
        sb.AppendLine();
        sb.AppendLine("        /// <summary>");
        sb.AppendLine("        /// Gets the collection of bindings that supply the format arguments.");
        sb.AppendLine("        /// </summary>");
        sb.AppendLine("        [global::Avalonia.Metadata.Content]");
        sb.AppendLine("        public global::System.Collections.Generic.IList<global::Avalonia.Data.BindingBase> Bindings { get; } =");
        sb.AppendLine("            new global::System.Collections.Generic.List<global::Avalonia.Data.BindingBase>();");
        sb.AppendLine();
        sb.AppendLine("        /// <summary>");
        sb.AppendLine("        /// Provides the value for the markup extension, returning a <see cref=\"global::Avalonia.Data.MultiBinding\"/>");
        sb.AppendLine("        /// that applies <see cref=\"string.Format(string, object[])\"/> using the template from");
        sb.AppendLine("        /// <see cref=\"Template\"/> and the values from <see cref=\"Bindings\"/>.");
        sb.AppendLine("        /// </summary>");
        sb.AppendLine("        /// <param name=\"serviceProvider\">The service provider for the markup extension.</param>");
        sb.AppendLine("        /// <returns>A <see cref=\"global::Avalonia.Data.MultiBinding\"/> that produces the formatted string.</returns>");
        sb.AppendLine("        public object ProvideValue(global::System.IServiceProvider serviceProvider)");
        sb.AppendLine("        {");
        sb.AppendLine("            var mb = new global::Avalonia.Data.MultiBinding();");
        sb.AppendLine();
        sb.AppendLine("            foreach (var binding in Bindings)");
        sb.AppendLine("            {");
        sb.AppendLine("                mb.Bindings.Add(binding);");
        sb.AppendLine("            }");
        sb.AppendLine();
        sb.AppendLine("            mb.Bindings.Add(new global::Avalonia.Data.ReflectionBinding(\"Text\"));");
        sb.AppendLine("            mb.Converter = new TranslationTemplateConverter(Template);");
        sb.AppendLine("            return mb;");
        sb.AppendLine("        }");
        sb.AppendLine();
        sb.AppendLine("        private sealed class TranslationTemplateConverter : global::Avalonia.Data.Converters.IMultiValueConverter");
        sb.AppendLine("        {");
        sb.AppendLine("            private readonly global::LiveResx.Avalonia.DynamicTranslation _key;");
        sb.AppendLine();
        sb.AppendLine("            public TranslationTemplateConverter(global::LiveResx.Avalonia.DynamicTranslation key)");
        sb.AppendLine("            {");
        sb.AppendLine("                _key = key;");
        sb.AppendLine("            }");
        sb.AppendLine();
        sb.AppendLine("            public object Convert(");
        sb.AppendLine("                global::System.Collections.Generic.IList<object> values,");
        sb.AppendLine("                global::System.Type targetType,");
        sb.AppendLine("                object parameter,");
        sb.AppendLine("                global::System.Globalization.CultureInfo culture)");
        sb.AppendLine("            {");
        sb.AppendLine("                object[] args;");
        sb.AppendLine();
        sb.AppendLine("                if (values.Count <= 1)");
        sb.AppendLine("                {");
        sb.AppendLine("                    args = global::System.Array.Empty<object>();");
        sb.AppendLine("                }");
        sb.AppendLine("                else");
        sb.AppendLine("                {");
        sb.AppendLine("                    args = new object[values.Count - 1];");
        sb.AppendLine();
        sb.AppendLine("                    for (var i = 0; i < args.Length; i++)");
        sb.AppendLine("                    {");
        sb.AppendLine("                        args[i] = values[i];");
        sb.AppendLine("                    }");
        sb.AppendLine("                }");
        sb.AppendLine("                var template = _key.Text;");
        sb.AppendLine("                return string.Format(template, args);");
        sb.AppendLine("            }");
        sb.AppendLine("        }");
        sb.AppendLine("    }");
        sb.Append("}");
        ctx.AddSource("LiveResx.Avalonia.TranslateFormatExtension.g.cs", sb.ToString());
    }
}
