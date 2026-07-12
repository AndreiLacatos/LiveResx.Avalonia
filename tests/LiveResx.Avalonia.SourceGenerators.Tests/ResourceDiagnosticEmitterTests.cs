using LiveResx.Avalonia.SourceGenerators.Generators;
using Microsoft.CodeAnalysis;

namespace LiveResx.Avalonia.SourceGenerators.Tests;

public class ResourceDiagnosticEmitterTests
{
    [Fact]
    public void DuplicateAcrossDesigners_EmitsLRX001()
    {
        var source =
            """
            using System.Resources;
            using System.Globalization;

            namespace Translations
            {
                public class ResA
                {
                    private static ResourceManager resourceMan;
                    private static CultureInfo resourceCulture;

                    public static ResourceManager ResourceManager
                    {
                        get
                        {
                            if (resourceMan is null)
                                resourceMan = new ResourceManager("Translations.ResA",
                                    typeof(ResA).Assembly);
                            return resourceMan;
                        }
                    }

                    public static CultureInfo Culture
                    {
                        get => resourceCulture;
                        set => resourceCulture = value;
                    }

                    public static string Greeting
                    {
                        get => ResourceManager.GetString("Greeting", resourceCulture) ?? string.Empty;
                    }
                }

                public class ResB
                {
                    private static ResourceManager resourceMan;
                    private static CultureInfo resourceCulture;

                    public static ResourceManager ResourceManager
                    {
                        get
                        {
                            if (resourceMan is null)
                                resourceMan = new ResourceManager("Translations.ResB",
                                    typeof(ResB).Assembly);
                            return resourceMan;
                        }
                    }

                    public static CultureInfo Culture
                    {
                        get => resourceCulture;
                        set => resourceCulture = value;
                    }

                    public static string Greeting
                    {
                        get => ResourceManager.GetString("Greeting", resourceCulture) ?? string.Empty;
                    }
                }
            }
            """;

        var (driver, _) = SourceGenerationRunner.Run(source);
        var genDiagnostics = driver.GetRunResult().Diagnostics;
        var lrx001 = genDiagnostics.Where(d => d.Id == "LRX001").ToList();

        Assert.Single(lrx001);
        Assert.Contains("Greeting", lrx001[0].GetMessage());
    }

    [Fact]
    public void DuplicateAcrossDesignerAndCustom_EmitsLRX001()
    {
        var source =
            """
            using System.Resources;
            using System.Globalization;
            using System.Collections.Generic;
            using LiveResx.Avalonia;

            namespace Translations
            {
                public class ResA
                {
                    private static ResourceManager resourceMan;
                    private static CultureInfo resourceCulture;

                    public static ResourceManager ResourceManager
                    {
                        get
                        {
                            if (resourceMan is null)
                                resourceMan = new ResourceManager("Translations.ResA",
                                    typeof(ResA).Assembly);
                            return resourceMan;
                        }
                    }

                    public static CultureInfo Culture
                    {
                        get => resourceCulture;
                        set => resourceCulture = value;
                    }

                    public static string CustomLabels
                    {
                        get => ResourceManager.GetString("CustomLabels", resourceCulture) ?? string.Empty;
                    }
                }

                internal sealed class CustomLabels : ILocalizedResource<string>
                {
                    public IReadOnlyDictionary<CultureInfo, string> Values { get; } =
                        new Dictionary<CultureInfo, string>
                        {
                            { CultureInfo.InvariantCulture, "fallback" }
                        }.AsReadOnly();
                }
            }
            """;

        var (driver, _) = SourceGenerationRunner.Run(
            source,
            customDetectorOverride: CustomResourceDetector.Detect);
        var genDiagnostics = driver.GetRunResult().Diagnostics;
        var lrx001 = genDiagnostics.Where(d => d.Id == "LRX001").ToList();

        Assert.Single(lrx001);
        Assert.Contains("CustomLabels", lrx001[0].GetMessage());
    }

    [Fact]
    public void NoDuplicate_NoLRX001()
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

                    public static string Goodbye
                    {
                        get => ResourceManager.GetString("Goodbye", resourceCulture) ?? string.Empty;
                    }
                }
            }
            """;

        var (driver, _) = SourceGenerationRunner.Run(source);
        var genDiagnostics = driver.GetRunResult().Diagnostics;

        Assert.DoesNotContain(genDiagnostics, d => d.Id == "LRX001");
    }
}
