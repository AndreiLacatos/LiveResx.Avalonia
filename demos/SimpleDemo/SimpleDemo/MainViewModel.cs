using System.Globalization;
using System.Reactive;
using System.Reactive.Linq;
using ReactiveUI;

namespace SimpleDemo;

public class MainWindowViewModel : ReactiveObject
{
    public MainWindowViewModel()
    {
        Locale = "en";

        SetEnglish = ReactiveCommand.Create<Unit, Unit>(_ =>
        {
            Locale = "en";
            LiveResx.Avalonia.DynamicLocalization.Instance.SwitchLocale(new CultureInfo("en"));
            return Unit.Default;
        });
        SetGerman = ReactiveCommand.Create<Unit, Unit>(_ =>
        {
            Locale = "de";
            LiveResx.Avalonia.DynamicLocalization.Instance.SwitchLocale(new CultureInfo("de"));
            return Unit.Default;
        });
        SetFrench = ReactiveCommand.Create<Unit, Unit>(_ =>
        {
            Locale = "fr";
            LiveResx.Avalonia.DynamicLocalization.Instance.SwitchLocale(new CultureInfo("fr"));
            return Unit.Default;
        });
        Time = Observable.Interval(TimeSpan.FromMinutes(1))
            .StartWith(0L)
            .Select(_ => DateTimeOffset.Now);
    }

    public string Locale { get; set => this.RaiseAndSetIfChanged(ref field, value); }

    public ReactiveCommand<Unit, Unit> SetEnglish { get; }
    public ReactiveCommand<Unit, Unit> SetGerman { get; }
    public ReactiveCommand<Unit, Unit> SetFrench { get; }
    public IObservable<DateTimeOffset> Time { get; }
}
