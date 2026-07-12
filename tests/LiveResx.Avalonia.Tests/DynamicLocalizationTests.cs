using System.Globalization;
using System.Resources;
using LiveResx.Avalonia;
using Xunit;

namespace LiveResx.Avalonia.Tests;

public class DynamicLocalizationTests
{
    public DynamicLocalizationTests()
    {
        DynamicLocalization.Instance.SwitchLocale(new CultureInfo("en"));
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

        // After Register, SwitchLocale(StartingCulture) is called, so Text is available.
        Assert.NotNull(translation.Text);
    }

    [Fact]
    public void SwitchLocale_UpdatesTranslation()
    {
        var translation = CreateTranslation("Hello");

        DynamicLocalization.Instance.Register(
            [translation],
            _ => { });

        DynamicLocalization.Instance.SwitchLocale(new CultureInfo("en"));
        Assert.Equal("Hello, World!", translation.Text);

        DynamicLocalization.Instance.SwitchLocale(new CultureInfo("de"));
        Assert.Equal("Hallo, Welt!", translation.Text);
    }

    [Fact]
    public void SwitchLocale_InvokesCallback()
    {
        CultureInfo? callbackCulture = null;

        DynamicLocalization.Instance.Register(
            [],
            culture => { callbackCulture = culture; });

        var de = new CultureInfo("de");
        DynamicLocalization.Instance.SwitchLocale(de);
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

        DynamicLocalization.Instance.SwitchLocale(new CultureInfo("de"));

        Assert.Equal("Hallo, Welt!", t1.Text);
        Assert.Equal("Hallo, Welt!", t2.Text);
    }

    // ─── Locale property + INPC ───────────────────────────────

    [Fact]
    public void Locale_AfterRegister_EqualsCurrentUICulture()
    {
        DynamicLocalization.Instance.Register([], _ => { });

        Assert.Equal(CultureInfo.CurrentUICulture, DynamicLocalization.Instance.Locale);
    }

    [Fact]
    public void SwitchLocale_UpdatesLocaleProperty()
    {
        DynamicLocalization.Instance.Register([], _ => { });

        var expected = new CultureInfo("de");
        DynamicLocalization.Instance.SwitchLocale(expected);

        Assert.Equal(expected, DynamicLocalization.Instance.Locale);
    }

    [Fact]
    public void SwitchLocale_RaisesPropertyChanged()
    {
        DynamicLocalization.Instance.Register([], _ => { });

        // Pick a culture guaranteed to differ from the current one
        var current = DynamicLocalization.Instance.Locale;
        var target = current.Name == "de" ? new CultureInfo("fr") : new CultureInfo("de");

        var raisedProperty = string.Empty;
        DynamicLocalization.Instance.PropertyChanged += (_, e) =>
            raisedProperty = e.PropertyName!;

        DynamicLocalization.Instance.SwitchLocale(target);

        Assert.Equal("Locale", raisedProperty);
    }

    [Fact]
    public void SwitchLocale_SameLocale_DoesNotRaisePropertyChanged()
    {
        DynamicLocalization.Instance.Register([], _ => { });

        // First, switch to a culture different from the current one
        var current = DynamicLocalization.Instance.Locale;
        var target = current.Name == "de" ? new CultureInfo("fr") : new CultureInfo("de");
        DynamicLocalization.Instance.SwitchLocale(target);

        var callCount = 0;
        DynamicLocalization.Instance.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName == "Locale")
                callCount++;
        };

        // Switch to the same culture again — should be a no-op
        DynamicLocalization.Instance.SwitchLocale(target);

        Assert.Equal(0, callCount);
    }

    private static DynamicTranslation CreateTranslation(string key = "Hello")
    {
        return new DynamicTranslation(key, () => new ResourceManager(typeof(TestResources)));
    }
}
