using System.Globalization;
using System.Reactive;
using System.Reactive.Linq;
using Avalonia.Media;
using LiveResx.Avalonia;
using ReactiveUI;

namespace CustomResourcesDemo;

public class MainWindowViewModel : ReactiveObject
{
    public MainWindowViewModel()
    {
        SetEnglish = ReactiveCommand.Create(() => SwitchLocale("en"));
        SetGerman = ReactiveCommand.Create(() => SwitchLocale("de"));
        SetFrench = ReactiveCommand.Create(() => SwitchLocale("fr"));

        // Reactive pipeline for the accent color custom resource.
        // ToObservable() is an extension method emitted by the source generator
        // when ReactiveUI is detected in the compilation.
        _accentBrush = DynamicResources.AccentColor.ToObservable()
            .Select(color => new SolidColorBrush(color))
            .ToProperty(this, x => x.AccentBrush, new SolidColorBrush(Colors.Transparent));

        // Derive a hex string for display from the same observable.
        _accentHex = DynamicResources.AccentColor.ToObservable()
            .Select(color => color.ToString())
            .ToProperty(this, x => x.AccentHex, "#000000");
    }

    private static void SwitchLocale(string locale)
        => DynamicLocalization.Instance.SwitchLocale(new CultureInfo(locale));

    private readonly ObservableAsPropertyHelper<SolidColorBrush> _accentBrush;
    public SolidColorBrush AccentBrush => _accentBrush.Value;

    private readonly ObservableAsPropertyHelper<string> _accentHex;
    public string AccentHex => _accentHex.Value;

    public ReactiveCommand<Unit, Unit> SetEnglish { get; }
    public ReactiveCommand<Unit, Unit> SetGerman { get; }
    public ReactiveCommand<Unit, Unit> SetFrench { get; }
}
