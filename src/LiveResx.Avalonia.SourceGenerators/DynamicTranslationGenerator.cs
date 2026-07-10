using System;
using System.Threading;
using LiveResx.Avalonia.SourceGenerators.Generators;
using LiveResx.Avalonia.SourceGenerators.Models;
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

        // Phase 2: Detect resource designer types in the compilation.
        // Consumed by Phase 3 (DynamicResources) and Phase 4 (Registration).
        var resourceTypes = context.CompilationProvider
            .Select((compilation, ct) =>
                deps.ResourceDesignerDetector(compilation, ct));

        // Phase 3: DynamicResources — one property per discovered resource key.
        context.RegisterSourceOutput(
            resourceTypes,
            (ctx, types) => DynamicResourcesGenerator.Emit(ctx, ts, types));

        // Phase 1: TranslateExtension — standalone markup extension, no resource discovery needed.
        context.RegisterSourceOutput(
            context.CompilationProvider,
            (ctx, _) => TranslateExtensionGenerator.Emit(ctx, ts));

        // Legacy POC — will be replaced by proper __LiveResxRegistration
        // generator in future phases.
        context.RegisterSourceOutput(
            context.CompilationProvider,
            (ctx, _) => EmitPocFallback(ctx, ts));
    }

    internal void ConfigureDependencies(GeneratorDependencies dependencies)
    {
        _dependencies ??= dependencies;
    }

    private static void EmitPocFallback(SourceProductionContext ctx, DateTimeOffset timestamp)
    {
        ctx.AddSource("LiveResx.Avalonia.POC.g.cs",
            GeneratorHeader.Generate(timestamp) + """
            namespace LiveResx.Avalonia
            {
                internal static class __LiveResxRegistration
                {
                    [System.Runtime.CompilerServices.ModuleInitializer]
                    internal static void Initialize()
                    {
                        DynamicLocalization.Instance.Register(
                            new[]
                            {
                                DynamicResources.HelloWorld
                            },
                            culture =>
                            {
                                Translations.Resources.Culture = culture;
                            });
                    }
                }
            }
            """);
    }
}
