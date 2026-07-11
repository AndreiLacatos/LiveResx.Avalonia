using System.Globalization;
using System.Reactive;
using System.Reactive.Linq;
using LiveResx.Avalonia;
using ReactiveUI;

namespace ValidationDemo;

public class MainWindowViewModel : ReactiveObject
{
    public MainWindowViewModel()
    {
        SetEnglish = ReactiveCommand.Create(() =>
        {
            SwitchLocale("en");
        });
        SetGerman = ReactiveCommand.Create(() =>
        {
            SwitchLocale("de");
        });
        SetFrench = ReactiveCommand.Create(() =>
        {
            SwitchLocale("fr");
        });

        // Reactive validation pipeline:
        //   WhenAnyValue  →  Throttle(300ms)  →  Select(error or null)
        //   →  ToObservable() / Return  →  Switch  →  ToProperty
        _errorMessage = this.WhenAnyValue(x => x.UserName)
            .Throttle(TimeSpan.FromMilliseconds(300))
            .Select(name => string.IsNullOrWhiteSpace(name)
                ? DynamicResources.FieldRequired.ToObservable()
                : Observable.Return(string.Empty))
            .Switch()
            .Select(msg => string.IsNullOrEmpty(msg) ? null : msg)
            .ToProperty(this, x => x.ErrorMessage);
    }

    private static void SwitchLocale(string locale)
        => DynamicLocalization.Instance.SwitchLocale(new CultureInfo(locale));

    public string? UserName
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    }

    private readonly ObservableAsPropertyHelper<string?> _errorMessage;
    public string? ErrorMessage => _errorMessage.Value;

    public ReactiveCommand<Unit, Unit> SetEnglish { get; }
    public ReactiveCommand<Unit, Unit> SetGerman { get; }
    public ReactiveCommand<Unit, Unit> SetFrench { get; }
}
