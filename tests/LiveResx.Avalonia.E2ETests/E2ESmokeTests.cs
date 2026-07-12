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

        DynamicLocalization.Instance.SwitchLocale(new CultureInfo("en"));
        Assert.Equal("Hello", translation.Text);

        DynamicLocalization.Instance.SwitchLocale(new CultureInfo("de"));
        Assert.Equal("Hallo", translation.Text);
    }

    [Fact]
    public void TranslationToObservable_EmitsCurrentValueOnSubscribe()
    {
        var translation = DynamicResources.HelloWorld;
        DynamicLocalization.Instance.SwitchLocale(new CultureInfo("en"));

        string? result = null;
        using (translation.ToObservable().Subscribe(v => result = v))
        {
            Assert.Equal("Hello", result);
        }
    }

    [Fact]
    public void TranslationToObservable_EmitsUpdatedValueOnCultureSwitch()
    {
        var translation = DynamicResources.HelloWorld;
        DynamicLocalization.Instance.SwitchLocale(new CultureInfo("en"));

        var results = new List<string>();
        using (translation.ToObservable().Subscribe(results.Add))
        {
            DynamicLocalization.Instance.SwitchLocale(new CultureInfo("de"));
            Assert.Contains("Hallo", results);
        }
    }

    [Fact]
    public void Locale_Property_ReturnsCurrentCulture()
    {
        DynamicLocalization.Instance.SwitchLocale(new CultureInfo("en"));
        Assert.Equal(new CultureInfo("en"), DynamicLocalization.Instance.Locale);

        DynamicLocalization.Instance.SwitchLocale(new CultureInfo("de"));
        Assert.Equal(new CultureInfo("de"), DynamicLocalization.Instance.Locale);
    }

    [Fact]
    public void ObservableLocale_EmitsCurrentValueOnSubscribe()
    {
        DynamicLocalization.Instance.SwitchLocale(new CultureInfo("en"));

        CultureInfo? result = null;
        using (DynamicLocalization.Instance.ObservableLocale().Subscribe(v => result = v))
        {
            Assert.Equal(new CultureInfo("en"), result);
        }
    }

    [Fact]
    public void ObservableLocale_EmitsOnSwitchLocale()
    {
        DynamicLocalization.Instance.SwitchLocale(new CultureInfo("en"));

        var results = new List<CultureInfo>();
        using (DynamicLocalization.Instance.ObservableLocale().Subscribe(results.Add))
        {
            DynamicLocalization.Instance.SwitchLocale(new CultureInfo("de"));
            Assert.Contains(new CultureInfo("de"), results);
        }
    }
}
