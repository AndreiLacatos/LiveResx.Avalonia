using System;
using System.Text;
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
        var sb = new StringBuilder();
        sb.Append(GeneratorHeader.Generate(timestamp));
        sb.AppendLine("namespace LiveResx.Avalonia");
        sb.AppendLine("{");
        sb.AppendLine("    /// <summary>");
        sb.AppendLine("    /// Extension methods for <see cref=\"DynamicTranslation\"/> that provide");
        sb.AppendLine("    /// reactive observable access to translation values.");
        sb.AppendLine("    /// </summary>");
        sb.AppendLine("    " + GeneratorHeader.GeneratedCodeAttribute);
        sb.AppendLine("    [global::System.Diagnostics.DebuggerNonUserCode]");
        sb.AppendLine("    [global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]");
        sb.AppendLine("    public static class DynamicTranslationExtensions");
        sb.AppendLine("    {");
        sb.AppendLine("        /// <summary>");
        sb.AppendLine("        /// Returns an observable stream of <see cref=\"DynamicTranslation.Text\"/>");
        sb.AppendLine("        /// values. Immediately emits the current value upon subscription, then");
        sb.AppendLine("        /// emits the new value each time the culture changes.");
        sb.AppendLine("        /// </summary>");
        sb.AppendLine("        /// <param name=\"translation\">The dynamic translation to observe.</param>");
        sb.AppendLine("        /// <returns>An observable that yields the current <see cref=\"DynamicTranslation.Text\"/>");
        sb.AppendLine("        /// followed by all subsequent values on culture switch.</returns>");
        sb.AppendLine("        public static global::System.IObservable<string> ToObservable(");
        sb.AppendLine("            this global::LiveResx.Avalonia.DynamicTranslation translation)");
        sb.AppendLine("        {");
        sb.AppendLine("            return global::System.Reactive.Linq.Observable.Defer(() =>");
        sb.AppendLine("                global::System.Reactive.Linq.Observable.Concat(");
        sb.AppendLine("                    global::System.Reactive.Linq.Observable.Return(translation.Text),");
        sb.AppendLine("                    global::System.Reactive.Linq.Observable.Select(");
        sb.AppendLine("                        global::System.Reactive.Linq.Observable.Where(");
        sb.AppendLine("                            global::System.Reactive.Linq.Observable.FromEventPattern<");
        sb.AppendLine("                                global::System.ComponentModel.PropertyChangedEventHandler,");
        sb.AppendLine("                                global::System.ComponentModel.PropertyChangedEventArgs>(");
        sb.AppendLine("                                h => translation.PropertyChanged += h,");
        sb.AppendLine("                                h => translation.PropertyChanged -= h),");
        sb.AppendLine("                            e => e.EventArgs.PropertyName == nameof(global::LiveResx.Avalonia.DynamicTranslation.Text)),");
        sb.AppendLine("                        _ => translation.Text)));");
        sb.AppendLine("        }");
        sb.AppendLine("    }");
        sb.AppendLine();
        sb.AppendLine("    /// <summary>");
        sb.AppendLine("    /// Extension methods for <see cref=\"DynamicLocalization\"/> that provide");
        sb.AppendLine("    /// reactive observable access to the current locale.");
        sb.AppendLine("    /// </summary>");
        sb.AppendLine("    " + GeneratorHeader.GeneratedCodeAttribute);
        sb.AppendLine("    [global::System.Diagnostics.DebuggerNonUserCode]");
        sb.AppendLine("    [global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]");
        sb.AppendLine("    public static class DynamicLocalizationExtensions");
        sb.AppendLine("    {");
        sb.AppendLine("        /// <summary>");
        sb.AppendLine("        /// Returns an observable stream of <see cref=\"DynamicLocalization.Locale\"/>");
        sb.AppendLine("        /// values. Immediately emits the current value upon subscription, then");
        sb.AppendLine("        /// emits the new value each time <see cref=\"DynamicLocalization.SwitchLocale\"/>");
        sb.AppendLine("        /// is called.");
        sb.AppendLine("        /// </summary>");
        sb.AppendLine("        /// <param name=\"localization\">The localization service to observe.</param>");
        sb.AppendLine("        /// <returns>An observable that yields the current <see cref=\"DynamicLocalization.Locale\"/>");
        sb.AppendLine("        /// followed by all subsequent values when the locale switches.</returns>");
        sb.AppendLine("        public static global::System.IObservable<global::System.Globalization.CultureInfo> ObservableLocale(");
        sb.AppendLine("            this global::LiveResx.Avalonia.DynamicLocalization localization)");
        sb.AppendLine("        {");
        sb.AppendLine("            return global::System.Reactive.Linq.Observable.Defer(() =>");
        sb.AppendLine("                global::System.Reactive.Linq.Observable.Concat(");
        sb.AppendLine("                    global::System.Reactive.Linq.Observable.Return(localization.Locale),");
        sb.AppendLine("                    global::System.Reactive.Linq.Observable.Select(");
        sb.AppendLine("                        global::System.Reactive.Linq.Observable.Where(");
        sb.AppendLine("                            global::System.Reactive.Linq.Observable.FromEventPattern<");
        sb.AppendLine("                                global::System.ComponentModel.PropertyChangedEventHandler,");
        sb.AppendLine("                                global::System.ComponentModel.PropertyChangedEventArgs>(");
        sb.AppendLine("                                h => localization.PropertyChanged += h,");
        sb.AppendLine("                                h => localization.PropertyChanged -= h),");
        sb.AppendLine("                            e => e.EventArgs.PropertyName == nameof(global::LiveResx.Avalonia.DynamicLocalization.Locale)),");
        sb.AppendLine("                        _ => localization.Locale)));");
        sb.AppendLine("        }");
        sb.AppendLine("    }");
        sb.AppendLine();
        sb.AppendLine("    /// <summary>");
        sb.AppendLine("    /// Extension methods for <see cref=\"LocalizedResource{T}\"/> that provide");
        sb.AppendLine("    /// reactive observable access to resource values.");
        sb.AppendLine("    /// </summary>");
        sb.AppendLine("    " + GeneratorHeader.GeneratedCodeAttribute);
        sb.AppendLine("    [global::System.Diagnostics.DebuggerNonUserCode]");
        sb.AppendLine("    [global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]");
        sb.AppendLine("    public static class LocalizedResourceExtensions");
        sb.AppendLine("    {");
        sb.AppendLine("        /// <summary>");
        sb.AppendLine("        /// Returns an observable stream of <see cref=\"LocalizedResource{T}.Value\"/>");
        sb.AppendLine("        /// values. Immediately emits the current value upon subscription, then");
        sb.AppendLine("        /// emits the new value each time the culture changes.");
        sb.AppendLine("        /// </summary>");
        sb.AppendLine("        /// <param name=\"resource\">The localized resource to observe.</param>");
        sb.AppendLine("        /// <returns>An observable that yields the current <see cref=\"LocalizedResource{T}.Value\"/>");
        sb.AppendLine("        /// followed by all subsequent values on culture switch.</returns>");
        sb.AppendLine("        public static global::System.IObservable<T> ToObservable<T>(");
        sb.AppendLine("            this global::LiveResx.Avalonia.LocalizedResource<T> resource)");
        sb.AppendLine("        {");
        sb.AppendLine("            return global::System.Reactive.Linq.Observable.Defer(() =>");
        sb.AppendLine("                global::System.Reactive.Linq.Observable.Concat(");
        sb.AppendLine("                    global::System.Reactive.Linq.Observable.Return(resource.Value),");
        sb.AppendLine("                    global::System.Reactive.Linq.Observable.Select(");
        sb.AppendLine("                        global::System.Reactive.Linq.Observable.Where(");
        sb.AppendLine("                            global::System.Reactive.Linq.Observable.FromEventPattern<");
        sb.AppendLine("                                global::System.ComponentModel.PropertyChangedEventHandler,");
        sb.AppendLine("                                global::System.ComponentModel.PropertyChangedEventArgs>(");
        sb.AppendLine("                                h => resource.PropertyChanged += h,");
        sb.AppendLine("                                h => resource.PropertyChanged -= h),");
        sb.AppendLine("                            e => e.EventArgs.PropertyName == nameof(global::LiveResx.Avalonia.LocalizedResource<T>.Value)),");
        sb.AppendLine("                        _ => resource.Value)));");
        sb.AppendLine("        }");
        sb.AppendLine("    }");
        sb.Append("}");
        ctx.AddSource("LiveResx.Avalonia.DynamicTranslationExtensions.g.cs", sb.ToString());
    }
}
