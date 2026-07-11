using System.Globalization;
using System.Reactive;
using System.Reactive.Linq;
using ReactiveUI;
using LiveResx.Avalonia;

namespace Playground;


public class MainWindowViewModel : ReactiveObject
{
    public MainWindowViewModel()
    {
        LocaleLabel = DynamicLocalization.Instance.ObservableLocale().Select(c => c.DisplayName);
        SetEnglish = ReactiveCommand.Create(() =>
        {
            DynamicLocalization.Instance.SwitchLocale(new CultureInfo("en"));
        });
        SetGerman = ReactiveCommand.Create(() =>
        {
            DynamicLocalization.Instance.SwitchLocale(new CultureInfo("de"));
        });
        SetFrench = ReactiveCommand.Create(() =>
        {
            DynamicLocalization.Instance.SwitchLocale(new CultureInfo("fr"));
        });
    }

    public IObservable<string> LocaleLabel { get; }
    public ReactiveCommand<Unit, Unit> SetEnglish { get; }
    public ReactiveCommand<Unit, Unit> SetGerman { get; }
    public ReactiveCommand<Unit, Unit> SetFrench { get; }
}
