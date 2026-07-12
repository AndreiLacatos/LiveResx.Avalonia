using Microsoft.CodeAnalysis;

namespace LiveResx.Avalonia.SourceGenerators.Tests;

public class DynamicResourcesGeneratorTests
{
    [Fact]
    public async Task SingleResourceKey_ShouldGenerateExpectedOutput()
    {
        var source =
            """
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
                            {
                                resourceMan = new ResourceManager("Translations.Resources",
                                    typeof(Resources).Assembly);
                            }
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

        var (driver, diagnostics) = SourceGenerationRunner.Run(source);
        Assert.Empty(diagnostics.Where(d => d.Severity == DiagnosticSeverity.Error));
        await Verify(driver).UseDirectory(TestConstants.SnapshotsDirectory);
    }

    [Fact]
    public async Task CustomResource_ShouldGenerateExpectedOutput()
    {
        var source =
            """
            using System.Collections.Generic;
            using System.Globalization;
            using LiveResx.Avalonia;

            namespace Translations
            {
                internal sealed class CustomLabels : ILocalizedResource<string>
                {
                    public IReadOnlyDictionary<CultureInfo, string> Values { get; } =
                        new Dictionary<CultureInfo, string>
                        {
                            { new CultureInfo("en"), "English" },
                            { new CultureInfo("de"), "Deutsch" },
                        }.AsReadOnly();
                }
            }
            """;

        var (driver, diagnostics) = SourceGenerationRunner.Run(
            source,
            customDetectorOverride: LiveResx.Avalonia.SourceGenerators.Generators.CustomResourceDetector.Detect);
        Assert.Empty(diagnostics.Where(d => d.Severity == DiagnosticSeverity.Error));
        await Verify(driver).UseDirectory(TestConstants.SnapshotsDirectory);
    }

    [Fact]
    public async Task InternalResourceDesigner_ShouldGenerateExpectedOutput()
    {
        var source =
            """
            using System.Resources;
            using System.Globalization;

            namespace Translations
            {
                internal class Resources
                {
                    private static ResourceManager resourceMan;
                    private static CultureInfo resourceCulture;

                    internal static ResourceManager ResourceManager
                    {
                        get
                        {
                            if (resourceMan is null)
                                resourceMan = new ResourceManager("Translations.Resources",
                                    typeof(Resources).Assembly);
                            return resourceMan;
                        }
                    }

                    internal static CultureInfo Culture
                    {
                        get => resourceCulture;
                        set => resourceCulture = value;
                    }

                    internal static string HelloWorld
                    {
                        get => ResourceManager.GetString("HelloWorld", resourceCulture) ?? string.Empty;
                    }
                }
            }
            """;

        var (driver, diagnostics) = SourceGenerationRunner.Run(source);
        Assert.Empty(diagnostics.Where(d => d.Severity == DiagnosticSeverity.Error));
        await Verify(driver).UseDirectory(TestConstants.SnapshotsDirectory);
    }
}
