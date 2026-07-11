using System.Globalization;
using LiveResx.Avalonia;
using Xunit;

namespace LiveResx.Avalonia.E2ETests;

public class E2ESmokeTests
{
    [Fact]
    public void DynamicResources_ShouldBeGenerated()
    {
        // If this compiles and runs, the source generator detected the
        // resource designer, emitted DynamicResources, and the code compiled.
        var translation = DynamicResources.HelloWorld;
        Assert.NotNull(translation);
    }

    [Fact]
    public void CultureSwitch_ShouldUpdateTranslationText()
    {
        var translation = DynamicResources.HelloWorld;

        DynamicLocalization.Instance.SwitchCulture(new CultureInfo("en"));
        Assert.Equal("Hello", translation.Text);

        DynamicLocalization.Instance.SwitchCulture(new CultureInfo("de"));
        Assert.Equal("Hallo", translation.Text);
    }
}
