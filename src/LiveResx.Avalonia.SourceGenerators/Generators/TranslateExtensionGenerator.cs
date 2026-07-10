using System;
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
        ctx.AddSource("LiveResx.Avalonia.TranslateExtension.g.cs",
            GeneratorHeader.Generate(timestamp) + """
            namespace LiveResx.Avalonia
            {
                /// <summary>
                /// Provides a markup extension that returns a data-binding to a <see cref="DynamicTranslation"/> instance,
                /// enabling automatic UI updates when the application culture changes.
                /// </summary>
                public sealed class TranslateExtension
                {
                    private readonly global::LiveResx.Avalonia.DynamicTranslation _translation;

                    /// <summary>
                    /// Initializes a new instance of the <see cref="TranslateExtension"/> class.
                    /// </summary>
                    /// <param name="translation">The dynamic translation to bind to.</param>
                    public TranslateExtension(global::LiveResx.Avalonia.DynamicTranslation translation)
                    {
                        _translation = translation;
                    }

                    /// <summary>
                    /// Provides the value for the markup extension, returning a data-binding
                    /// to the <c>Text</c> property of the underlying <see cref="DynamicTranslation"/>.
                    /// </summary>
                    /// <param name="serviceProvider">The service provider for the markup extension.</param>
                    /// <returns>A <see cref="global::Avalonia.Data.Binding"/> to <c>DynamicTranslation.Text</c>.</returns>
                    public object ProvideValue(global::System.IServiceProvider serviceProvider)
                    {
                        return new global::Avalonia.Data.Binding("Text")
                        {
                            Source = _translation,
                        };
                    }
                }
            }
            """);
    }
}
