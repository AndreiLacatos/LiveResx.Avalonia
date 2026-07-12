using System;
using System.Collections.Generic;
using System.Globalization;
using LiveResx.Avalonia;
using Xunit;

namespace LiveResx.Avalonia.Tests;

public class LocalizedResourceTests
{
    [Fact]
    public void Constructor_StoresValues()
    {
        var values = new Dictionary<CultureInfo, string>
        {
            [new CultureInfo("en")] = "Hello"
        };
        var resource = new LocalizedResource<string>("Greeting", values);
        resource.Refresh(new CultureInfo("en"));

        Assert.Equal("Hello", resource.Value);
    }

    [Fact]
    public void Value_ReturnsExactMatch()
    {
        var values = new Dictionary<CultureInfo, string>
        {
            [new CultureInfo("en")] = "Hello",
            [new CultureInfo("de")] = "Hallo"
        };
        var resource = new LocalizedResource<string>("Greeting", values);
        resource.Refresh(new CultureInfo("de"));

        Assert.Equal("Hallo", resource.Value);
    }

    [Fact]
    public void Value_FallsBackToInvariant()
    {
        var values = new Dictionary<CultureInfo, string>
        {
            [CultureInfo.InvariantCulture] = "Fallback"
        };
        var resource = new LocalizedResource<string>("Greeting", values);
        resource.Refresh(new CultureInfo("de-DE"));

        Assert.Equal("Fallback", resource.Value);
    }

    [Fact]
    public void Value_ParentChain_FallsBackToNeutral()
    {
        var values = new Dictionary<CultureInfo, string>
        {
            [new CultureInfo("de")] = "Hallo"
        };
        var resource = new LocalizedResource<string>("Greeting", values, FallbackBehavior.ParentChain);
        resource.Refresh(new CultureInfo("de-DE"));

        Assert.Equal("Hallo", resource.Value);
    }

    [Fact]
    public void Value_ParentChain_FallsBackToInvariant()
    {
        var values = new Dictionary<CultureInfo, string>
        {
            [CultureInfo.InvariantCulture] = "Fallback"
        };
        var resource = new LocalizedResource<string>("Greeting", values, FallbackBehavior.ParentChain);
        resource.Refresh(new CultureInfo("de-DE"));

        Assert.Equal("Fallback", resource.Value);
    }

    [Fact]
    public void Value_ReturnsDefault_WhenNoFallbackAvailable()
    {
        var values = new Dictionary<CultureInfo, string>();
        var resource = new LocalizedResource<string>("Greeting", values);
        resource.Refresh(new CultureInfo("de-DE"));

        Assert.Null(resource.Value);
    }

    [Fact]
    public void Refresh_RaisesPropertyChanged()
    {
        var values = new Dictionary<CultureInfo, string>
        {
            [new CultureInfo("en")] = "Hello"
        };
        var resource = new LocalizedResource<string>("Greeting", values);

        var raised = false;
        resource.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName == "Value")
                raised = true;
        };

        resource.Refresh(new CultureInfo("en"));

        Assert.True(raised);
    }

    [Fact]
    public void SameCulture_DoesNotRaisePropertyChanged()
    {
        var values = new Dictionary<CultureInfo, string>
        {
            [new CultureInfo("en")] = "Hello"
        };
        var resource = new LocalizedResource<string>("Greeting", values);
        resource.Refresh(new CultureInfo("en"));

        var callCount = 0;
        resource.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName == "Value")
                callCount++;
        };
        resource.Refresh(new CultureInfo("en"));

        Assert.Equal(0, callCount);
    }

    [Fact]
    public void Constructor_ThrowsOnNullName()
    {
        var ex = Assert.Throws<ArgumentException>(() =>
            new LocalizedResource<string>(null!, new Dictionary<CultureInfo, string>()));
        Assert.Contains("Name must not be empty", ex.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void Constructor_ThrowsOnEmptyName()
    {
        var ex = Assert.Throws<ArgumentException>(() =>
            new LocalizedResource<string>("", new Dictionary<CultureInfo, string>()));
        Assert.Contains("Name must not be empty", ex.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void Constructor_ThrowsOnNullValues()
    {
        var ex = Assert.Throws<ArgumentNullException>(() =>
            new LocalizedResource<string>("Greeting", null!));
        Assert.Equal("values", ex.ParamName);
    }
}
