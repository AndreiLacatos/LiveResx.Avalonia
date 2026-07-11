using System.Resources;

namespace LiveResx.Avalonia.Tests;

internal static class TestResources
{
    private static ResourceManager s_resourceManager;

    internal static ResourceManager ResourceManager =>
        s_resourceManager ??= new ResourceManager(typeof(TestResources));
}
