using System;
using Microsoft.CodeAnalysis;

namespace LiveResx.Avalonia.SourceGenerators.Generators;

/// <summary>
/// Emits the <c>LiveResx.Avalonia.DynamicTranslationExtensions</c> class with a
/// <c>ToObservable()</c> extension method for <see cref="DynamicTranslation"/>.
/// Only emitted when the compilation references <c>System.Reactive</c>,
/// <c>ReactiveUI</c>, or <c>ReactiveUI.Avalonia</c>.
/// </summary>
internal static class ToObservableExtensionGenerator
{
    /// <summary>
    /// Emits the <c>LiveResx.Avalonia.DynamicTranslationExtensions.g.cs</c> source file.
    /// </summary>
    internal static void Emit(SourceProductionContext ctx, DateTimeOffset timestamp)
    {
        ctx.AddSource("LiveResx.Avalonia.DynamicTranslationExtensions.g.cs",
            GeneratorHeader.Generate(timestamp) + """
            using System.Reactive.Linq;

            namespace LiveResx.Avalonia
            {
                /// <summary>
                /// Extension methods for <see cref="DynamicTranslation"/> that provide
                /// reactive observable access to translation values.
                /// </summary>
                public static class DynamicTranslationExtensions
                {
                    /// <summary>
                    /// Returns an observable stream of <see cref="DynamicTranslation.Text"/>
                    /// values. Immediately emits the current value upon subscription, then
                    /// emits the new value each time the culture changes.
                    /// </summary>
                    /// <param name="translation">The dynamic translation to observe.</param>
                    /// <returns>An observable that yields the current <see cref="DynamicTranslation.Text"/>
                    /// followed by all subsequent values on culture switch.</returns>
                    public static global::System.IObservable<string> ToObservable(
                        this global::LiveResx.Avalonia.DynamicTranslation translation)
                    {
                        return global::System.Reactive.Linq.Observable.Defer(() =>
                            global::System.Reactive.Linq.Observable
                                .Return(translation.Text)
                                .Concat(global::System.Reactive.Linq.Observable
                                    .FromEventPattern<
                                        global::System.ComponentModel.PropertyChangedEventHandler,
                                        global::System.ComponentModel.PropertyChangedEventArgs>(
                                        h => translation.PropertyChanged += h,
                                        h => translation.PropertyChanged -= h)
                                    .Where(e => e.EventArgs.PropertyName == nameof(global::LiveResx.Avalonia.DynamicTranslation.Text))
                                    .Select(_ => translation.Text)));
                    }
                }
            }
            """);
    }
}
