using System;
using Microsoft.CodeAnalysis;

namespace LiveResx.Avalonia.SourceGenerators.Generators;

    /// <summary>
    /// Emits the <c>LiveResx.Avalonia.DynamicTranslationExtensions.g.cs</c> source file
    /// containing extension methods for <see cref="DynamicTranslation"/>,
    /// <see cref="DynamicLocalization"/>, and <see cref="LocalizedResource{T}"/>
    /// that expose observable streams of their values.
    /// Only emitted when the compilation references <c>System.Reactive</c>,
    /// <c>ReactiveUI</c>, or <c>ReactiveUI.Avalonia</c>.
    /// </summary>
    internal static class ToObservableExtensionGenerator
    {
        /// <summary>
        /// Emits the <c>LiveResx.Avalonia.DynamicTranslationExtensions.g.cs</c> source file
        /// with all three extension classes.
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

                    /// <summary>
                    /// Extension methods for <see cref="DynamicLocalization"/> that provide
                    /// reactive observable access to the current locale.
                    /// </summary>
                    public static class DynamicLocalizationExtensions
                    {
                        /// <summary>
                        /// Returns an observable stream of <see cref="DynamicLocalization.Locale"/>
                        /// values. Immediately emits the current value upon subscription, then
                        /// emits the new value each time <see cref="DynamicLocalization.SwitchLocale"/>
                        /// is called.
                        /// </summary>
                        /// <param name="localization">The localization service to observe.</param>
                        /// <returns>An observable that yields the current <see cref="DynamicLocalization.Locale"/>
                        /// followed by all subsequent values when the locale switches.</returns>
                        public static global::System.IObservable<global::System.Globalization.CultureInfo> ObservableLocale(
                            this global::LiveResx.Avalonia.DynamicLocalization localization)
                        {
                            return global::System.Reactive.Linq.Observable.Defer(() =>
                                global::System.Reactive.Linq.Observable
                                    .Return(localization.Locale)
                                    .Concat(global::System.Reactive.Linq.Observable
                                        .FromEventPattern<
                                            global::System.ComponentModel.PropertyChangedEventHandler,
                                            global::System.ComponentModel.PropertyChangedEventArgs>(
                                            h => localization.PropertyChanged += h,
                                            h => localization.PropertyChanged -= h)
                                        .Where(e => e.EventArgs.PropertyName == nameof(global::LiveResx.Avalonia.DynamicLocalization.Locale))
                                        .Select(_ => localization.Locale)));
                        }
                    }

                    /// <summary>
                    /// Extension methods for <see cref="LocalizedResource{T}"/> that provide
                    /// reactive observable access to resource values.
                    /// </summary>
                    public static class LocalizedResourceExtensions
                    {
                        /// <summary>
                        /// Returns an observable stream of <see cref="LocalizedResource{T}.Value"/>
                        /// values. Immediately emits the current value upon subscription, then
                        /// emits the new value each time the culture changes.
                        /// </summary>
                        /// <param name="resource">The localized resource to observe.</param>
                        /// <returns>An observable that yields the current <see cref="LocalizedResource{T}.Value"/>
                        /// followed by all subsequent values on culture switch.</returns>
                        public static global::System.IObservable<T> ToObservable<T>(
                            this global::LiveResx.Avalonia.LocalizedResource<T> resource)
                        {
                            return global::System.Reactive.Linq.Observable.Defer(() =>
                                global::System.Reactive.Linq.Observable
                                    .Return(resource.Value)
                                    .Concat(global::System.Reactive.Linq.Observable
                                        .FromEventPattern<
                                            global::System.ComponentModel.PropertyChangedEventHandler,
                                            global::System.ComponentModel.PropertyChangedEventArgs>(
                                            h => resource.PropertyChanged += h,
                                            h => resource.PropertyChanged -= h)
                                        .Where(e => e.EventArgs.PropertyName == nameof(global::LiveResx.Avalonia.LocalizedResource<T>.Value))
                                        .Select(_ => resource.Value)));
                        }
                    }
                }
                """);
        }
    }
