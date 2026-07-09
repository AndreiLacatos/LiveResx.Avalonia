using System.Globalization;
using System.Reactive;
using ReactiveUI;

namespace Playground;


public class MainWindowViewModel : ReactiveObject
{
    public MainWindowViewModel()
    {
        Locale = "en";

        SetEnglish = ReactiveCommand.Create(() =>
        {
            Locale = "en";
            LiveResx.Avalonia.DynamicLocalization.Instance.SwitchCulture(new CultureInfo("en"));
        });
        SetGerman = ReactiveCommand.Create(() =>
        {
            Locale = "de";
            LiveResx.Avalonia.DynamicLocalization.Instance.SwitchCulture(new CultureInfo("de"));
        });
        SetFrench = ReactiveCommand.Create(() =>
        {
            Locale = "fr";
            LiveResx.Avalonia.DynamicLocalization.Instance.SwitchCulture(new CultureInfo("fr"));
        });
    }

    public string Locale { get; set => this.RaiseAndSetIfChanged(ref field, value); }

    public ReactiveCommand<Unit, Unit> SetEnglish { get; }
    public ReactiveCommand<Unit, Unit> SetGerman { get; }
    public ReactiveCommand<Unit, Unit> SetFrench { get; }
}
