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

        // Emit a DynamicResources class with one static DynamicTranslation
        // property per discovered resource key.
        context.RegisterSourceOutput(
            resourceTypes,
            (ctx, types) => DynamicResourcesGenerator.Emit(ctx, ts, types));

        // Emit a [ModuleInitializer] that registers all discovered
        // DynamicResources into DynamicLocalization on startup.
        context.RegisterSourceOutput(
            resourceTypes,
            (ctx, types) => LiveResxRegistrationGenerator.Emit(ctx, ts, types));

        // Emit the TranslateExtension markup extension for XAML data-binding.
        context.RegisterSourceOutput(
            context.CompilationProvider,
            (ctx, _) => TranslateExtensionGenerator.Emit(ctx, ts));
    }

    internal void ConfigureDependencies(GeneratorDependencies dependencies)
    {
        _dependencies ??= dependencies;
    }
}
