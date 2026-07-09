using Microsoft.CodeAnalysis;

namespace LiveResx.Avalonia.SourceGenerators;

[Generator]
public class DynamicTranslationGenerator : IIncrementalGenerator
{
    private GeneratorDependencies? _dependencies;

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        ConfigureDependencies(GeneratorDependencies.Default);
        context.RegisterSourceOutput(
            context.CompilationProvider,
            (ctx, _) => ctx.AddSource(
                "LiveResx.Avalonia.POC.g.cs",
                """
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
                    
                    public sealed class TranslateExtension
                    {
                        private readonly DynamicTranslation _translation;
                    
                        public TranslateExtension(DynamicTranslation translation)
                        {
                            _translation = translation;
                        }
                    
                        public object ProvideValue(IServiceProvider serviceProvider)
                        {
                            return new global::Avalonia.Data.Binding(nameof(DynamicTranslation.Text))
                            {
                                Source = _translation,
                            };
                        }
                    }
                }
                """));
    }

    internal void ConfigureDependencies(GeneratorDependencies dependencies)
    {
        _dependencies ??= dependencies;
    }
}
