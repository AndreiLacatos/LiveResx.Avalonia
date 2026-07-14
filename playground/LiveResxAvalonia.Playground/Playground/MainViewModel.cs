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
        CustomResources = DynamicLocalization.Instance.GetResource<string[]>("MyCustomResources").ToObservable();
        UserName = Observable.Interval(TimeSpan.FromSeconds(1))
            .StartWith(0L)
            .Select(tick => tick % 2 == 0 ? "John" : "Doe");
    }

    public IObservable<string[]> CustomResources { get; }
    public IObservable<string> LocaleLabel { get; }
    public ReactiveCommand<Unit, Unit> SetEnglish { get; }
    public ReactiveCommand<Unit, Unit> SetGerman { get; }
    public ReactiveCommand<Unit, Unit> SetFrench { get; }
    public IObservable<string> UserName { get; }
}
