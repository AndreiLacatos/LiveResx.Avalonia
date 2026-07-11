using System.Globalization;
using System.Resources;
using LiveResx.Avalonia;
using Xunit;

namespace LiveResx.Avalonia.Tests;

public class DynamicTranslationTests
{
    [Fact]
    public void Constructor_ThrowsOnNullKey()
    {
        var ex = Assert.Throws<ArgumentException>(() =>
            new DynamicTranslation(null!, () => null!));
        Assert.Contains("Resource key must not be empty", ex.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void Constructor_ThrowsOnEmptyKey()
    {
        var ex = Assert.Throws<ArgumentException>(() =>
            new DynamicTranslation("", () => null!));
        Assert.Contains("Resource key must not be empty", ex.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void Constructor_ThrowsOnWhitespaceKey()
    {
        var ex = Assert.Throws<ArgumentException>(() =>
            new DynamicTranslation("   ", () => null!));
        Assert.Contains("Resource key must not be empty", ex.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void Constructor_ThrowsOnNullFactory()
    {
        var ex = Assert.Throws<ArgumentNullException>(() =>
            new DynamicTranslation("Key", null!));
        Assert.Equal("resourceManagerFactory", ex.ParamName);
    }

    [Fact]
    public void Constructor_SucceedsWithValidArgs()
    {
        var translation = new DynamicTranslation("Key", () => new ResourceManager(typeof(TestResources)));
        Assert.NotNull(translation);
    }

    [Fact]
    public void Text_ReturnsEmptyForMissingKey()
    {
        var translation = new DynamicTranslation("NonExistentKey", () => new ResourceManager(typeof(TestResources)));
        translation.Refresh(new CultureInfo("en"));
        Assert.Equal(string.Empty, translation.Text);
    }

    [Fact]
    public void Text_ReturnsValueAfterRefresh()
    {
        var translation = new DynamicTranslation("Hello", () => new ResourceManager(typeof(TestResources)));
        translation.Refresh(new CultureInfo("en"));
        Assert.Equal("Hello, World!", translation.Text);
    }

    [Fact]
    public void Text_ReturnsLocalizedValue()
    {
        var translation = new DynamicTranslation("Hello", () => new ResourceManager(typeof(TestResources)));
        translation.Refresh(new CultureInfo("de"));
        Assert.Equal("Hallo, Welt!", translation.Text);
    }

    [Fact]
    public void Refresh_RaisesPropertyChanged()
    {
        var translation = new DynamicTranslation("Hello", () => new ResourceManager(typeof(TestResources)));
        var raised = false;
        translation.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName == "Text")
                raised = true;
        };
        translation.Refresh(new CultureInfo("en"));
        Assert.True(raised);
    }

    [Fact]
    public void ResourceManager_IsLazilyInitialized()
    {
        var factoryCalled = false;
        var translation = new DynamicTranslation("Hello", () =>
        {
            factoryCalled = true;
            return new ResourceManager(typeof(TestResources));
        });

        Assert.False(factoryCalled, "Factory should not be called before Text is accessed");

        // Access Text, which triggers factory
        translation.Refresh(new CultureInfo("en"));
        var _ = translation.Text;
        Assert.True(factoryCalled, "Factory should be called on first Text access");
    }
}
