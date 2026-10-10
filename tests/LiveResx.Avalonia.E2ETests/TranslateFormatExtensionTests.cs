using System.Globalization;
using Avalonia.Controls;
using Avalonia.Data;
using LiveResx.Avalonia;
using Xunit;

namespace LiveResx.Avalonia.E2ETests;

[Collection(DynamicLocalizationCollection.Name)]
public class TranslateFormatExtensionTests
{
    [Fact]
    public void CultureSwitch_ShouldUpdateFormattedText_WhenArgumentsAreUnchanged()
    {
        DynamicLocalization.Instance.SwitchLocale(new CultureInfo("en"));

        var extension = new TranslateFormatExtension
        {
            Template = DynamicResources.Greeting,
            Bindings = { new Binding(nameof(GreetingViewModel.Name)) },
        };
        var textBlock = new TextBlock
        {
            DataContext = new GreetingViewModel { Name = "Ada" },
        };
        textBlock.Bind(TextBlock.TextProperty, (BindingBase)extension.ProvideValue(null!));

        Assert.Equal("Hello Ada!", textBlock.Text);

        // Only the culture changes; the argument binding never fires again, so the
        // binding to the template's Text is the only thing that can re-run the format.
        DynamicLocalization.Instance.SwitchLocale(new CultureInfo("de"));

        Assert.Equal("Hallo Ada!", textBlock.Text);
    }

    private sealed class GreetingViewModel
    {
        public string Name { get; init; } = string.Empty;
    }
}
