using LiveResx.Avalonia.SourceGenerators.Generators;
using Microsoft.CodeAnalysis;

namespace LiveResx.Avalonia.SourceGenerators.Tests;

public class ToObservableExtensionGeneratorTests
{
    private const string ResourceDesignerSource = """
        using System.Resources;
        using System.Globalization;

        namespace Translations
        {
            public class Resources
            {
                private static ResourceManager resourceMan;
                private static CultureInfo resourceCulture;

                public static ResourceManager ResourceManager
                {
                    get
                    {
                        if (resourceMan is null)
                            resourceMan = new ResourceManager("Translations.Resources",
                                typeof(Resources).Assembly);
                        return resourceMan;
                    }
                }

                public static CultureInfo Culture
                {
                    get => resourceCulture;
                    set => resourceCulture = value;
                }

                public static string HelloWorld
                {
                    get => ResourceManager.GetString("HelloWorld", resourceCulture) ?? string.Empty;
                }
            }
        }
        """;

    [Fact]
    public void ToObservableExtension_NotEmitted_WhenNoReactiveDeps()
    {
        var (driver, _) = SourceGenerationRunner.Run(
            ResourceDesignerSource,
            reactiveDetectorOverride: (_, _) => false);

        var generatedTrees = driver.GetRunResult().GeneratedTrees;
        var hasExtension = generatedTrees.Any(t =>
            t.FilePath.EndsWith("DynamicTranslationExtensions.g.cs"));

        Assert.False(hasExtension,
            "ToObservable extension should NOT be emitted without reactive dependencies");
    }

    [Fact]
    public void ToObservableExtension_Emitted_WhenReactiveDepsPresent()
    {
        var (driver, _) = SourceGenerationRunner.Run(
            ResourceDesignerSource,
            reactiveDetectorOverride: (_, _) => true);

        var generatedTrees = driver.GetRunResult().GeneratedTrees;
        var hasExtension = generatedTrees.Any(t =>
            t.FilePath.EndsWith("DynamicTranslationExtensions.g.cs"));

        Assert.True(hasExtension,
            "ToObservable extension SHOULD be emitted when reactive dependencies are present");

        // Also verify the generated source contains both extension methods
        var extensionTree = generatedTrees.First(t =>
            t.FilePath.EndsWith("DynamicTranslationExtensions.g.cs"));
        var sourceText = extensionTree.GetText().ToString();

        Assert.Contains("DynamicTranslationExtensions", sourceText);
        Assert.Contains("DynamicLocalizationExtensions", sourceText);
        Assert.Contains("LocalizedResourceExtensions", sourceText);
        Assert.Contains("ObservableLocale", sourceText);
        Assert.Contains("ToObservable", sourceText);
    }

    [Fact]
    public async Task ToObservableExtension_ShouldGenerateExpectedOutput()
    {
        var (driver, _) = SourceGenerationRunner.Run(
            ResourceDesignerSource,
            reactiveDetectorOverride: (_, _) => true);

        await Verify(driver).UseDirectory(TestConstants.SnapshotsDirectory);
    }
}
