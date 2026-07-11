using System.Globalization;
using System.Resources;

namespace LiveResx.Avalonia.E2ETests;

internal class Resources
{
    private static ResourceManager resourceMan;
    private static CultureInfo resourceCulture;

    internal static ResourceManager ResourceManager
    {
        get
        {
            if (resourceMan is null)
                resourceMan = new ResourceManager(
                    "LiveResx.Avalonia.E2ETests.Resources",
                    typeof(Resources).Assembly);
            return resourceMan;
        }
    }

    internal static CultureInfo Culture
    {
        get => resourceCulture;
        set => resourceCulture = value;
    }

    internal static string HelloWorld =>
        ResourceManager.GetString("HelloWorld", resourceCulture) ?? string.Empty;
}
