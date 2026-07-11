using System.Globalization;
using System.Resources;
using LiveResx.Avalonia;
using Xunit;

namespace LiveResx.Avalonia.Tests;

public class DynamicLocalizationTests
{
    public DynamicLocalizationTests()
    {
        DynamicLocalization.Instance.SwitchCulture(new CultureInfo("en"));
    }

    [Fact]
    public void Instance_IsSingleton()
    {
        var instance1 = DynamicLocalization.Instance;
        var instance2 = DynamicLocalization.Instance;
        Assert.Same(instance1, instance2);
    }

    [Fact]
    public void Register_SwitchesToStartingCulture()
    {
        var translation = CreateTranslation();

        DynamicLocalization.Instance.Register(
            [translation],
            _ => { });

        // After Register, SwitchCulture(StartingCulture) is called, so Text is available.
        Assert.NotNull(translation.Text);
    }

    [Fact]
    public void SwitchCulture_UpdatesTranslation()
    {
        var translation = CreateTranslation("Hello");

        DynamicLocalization.Instance.Register(
            [translation],
            _ => { });

        DynamicLocalization.Instance.SwitchCulture(new CultureInfo("en"));
        Assert.Equal("Hello, World!", translation.Text);

        DynamicLocalization.Instance.SwitchCulture(new CultureInfo("de"));
        Assert.Equal("Hallo, Welt!", translation.Text);
    }

    [Fact]
    public void SwitchCulture_InvokesCallback()
    {
        CultureInfo? callbackCulture = null;

        DynamicLocalization.Instance.Register(
            [],
            culture => { callbackCulture = culture; });

        var de = new CultureInfo("de");
        DynamicLocalization.Instance.SwitchCulture(de);
        Assert.Equal(de, callbackCulture);
    }

    [Fact]
    public void EmptyRegistration_DoesNotThrow()
    {
        DynamicLocalization.Instance.Register([], _ => { });
        // No exception means success
    }

    [Fact]
    public void MultipleRegisters_AccumulateTranslations()
    {
        var t1 = CreateTranslation("Hello");
        var t2 = CreateTranslation("Hello");

        DynamicLocalization.Instance.Register([t1], _ => { });
        DynamicLocalization.Instance.Register([t2], _ => { });

        DynamicLocalization.Instance.SwitchCulture(new CultureInfo("de"));

        Assert.Equal("Hallo, Welt!", t1.Text);
        Assert.Equal("Hallo, Welt!", t2.Text);
    }

    private static DynamicTranslation CreateTranslation(string key = "Hello")
    {
        return new DynamicTranslation(key, () => new ResourceManager(typeof(TestResources)));
    }
}
