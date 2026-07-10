using System;
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

        // TranslateExtension — standalone markup extension, no resource discovery needed.
        context.RegisterSourceOutput(
            context.CompilationProvider,
            (ctx, _) => TranslateExtensionGenerator.Emit(ctx, ts));

        // Legacy POC — will be replaced by proper DynamicResources + __LiveResxRegistration
        // generators in future phases.
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
                public static class DynamicResources
                {
                    public static DynamicTranslation HelloWorld { get; } = new DynamicTranslation(
                        "HelloWorld",
                        () => Translations.Resources.ResourceManager);
                }

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
