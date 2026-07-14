using LiveResx.Avalonia.SourceGenerators.Generators;
using Microsoft.CodeAnalysis;

namespace LiveResx.Avalonia.SourceGenerators;

[Generator]
public class DynamicTranslationGenerator : IIncrementalGenerator
{
    private GeneratorDependencies? _dependencies;

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        ConfigureDependencies(GeneratorDependencies.Default);

        var deps = _dependencies ?? GeneratorDependencies.Default;
        var ts = deps.TimestampProvider();

        // Detect resource designer types by walking the compilation for
        // types that expose static ResourceManager + Culture properties.
        var resourceTypes = context.CompilationProvider
            .Select((compilation, ct) =>
                deps.ResourceDesignerDetector(compilation, ct));

        // Detect ILocalizedResource<T> implementors for custom typed resources.
        var customTypes = context.CompilationProvider
            .Select((compilation, ct) =>
                deps.CustomResourceDetector(compilation, ct));

        // Combine both detections into a single pipeline so that all resource
        // metadata is available when emitting DynamicResources and registration.
        var combined = resourceTypes.Combine(customTypes);

        // Emit a DynamicResources class with one static DynamicTranslation
        // property per discovered resource key, plus unwrapped getters for
        // each ILocalizedResource<T> implementor.
        context.RegisterSourceOutput(
            combined,
            (ctx, pair) => DynamicResourcesGenerator.Emit(ctx, ts, pair.Left, pair.Right));

        // Emit a [ModuleInitializer] that registers all discovered
        // DynamicResources into DynamicLocalization on startup, followed
        // by RegisterResource for each ILocalizedResource<T> implementor.
        context.RegisterSourceOutput(
            combined,
            (ctx, pair) => LiveResxRegistrationGenerator.Emit(ctx, ts, pair.Left, pair.Right));

        // Emit LRX001 warnings for duplicate / conflicting resource names
        // across .resx keys and ILocalizedResource<T> class names.
        context.RegisterSourceOutput(
            combined,
            (ctx, pair) => ResourceDiagnosticEmitter.Emit(ctx, pair.Left, pair.Right));

        // Emit the TranslateExtension markup extension for XAML data-binding.
        context.RegisterSourceOutput(
            context.CompilationProvider,
            (ctx, _) => TranslateExtensionGenerator.Emit(ctx, ts));

        // Emit the TranslateFormatExtension markup extension for composite string formatting
        // with data-bound arguments in XAML.
        context.RegisterSourceOutput(
            context.CompilationProvider,
            (ctx, _) => TranslateFormatExtensionGenerator.Emit(ctx, ts));

        // Detect whether the compilation references System.Reactive, ReactiveUI,
        // or ReactiveUI.Avalonia. If so, emit ToObservable extension methods.
        var hasReactiveDeps = context.CompilationProvider
            .Select((compilation, ct) =>
                deps.ReactiveAssemblyDetector(compilation, ct));

        context.RegisterSourceOutput(
            hasReactiveDeps,
            (ctx, hasRx) =>
            {
                if (hasRx)
                    ToObservableExtensionGenerator.Emit(ctx, ts);
            });
    }

    internal void ConfigureDependencies(GeneratorDependencies dependencies)
    {
        _dependencies ??= dependencies;
    }
}
