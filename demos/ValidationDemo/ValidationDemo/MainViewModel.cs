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
        Locale = "en";

        SetEnglish = ReactiveCommand.Create(() =>
        {
            Locale = "en";
            SwitchCulture("en");
        });
        SetGerman = ReactiveCommand.Create(() =>
        {
            Locale = "de";
            SwitchCulture("de");
        });
        SetFrench = ReactiveCommand.Create(() =>
        {
            Locale = "fr";
            SwitchCulture("fr");
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

    private static void SwitchCulture(string locale)
        => DynamicLocalization.Instance.SwitchCulture(new CultureInfo(locale));

    public string Locale { get; set => this.RaiseAndSetIfChanged(ref field, value); }

    string? _userName;
    public string? UserName
    {
        get => _userName;
        set => this.RaiseAndSetIfChanged(ref _userName, value);
    }

    private readonly ObservableAsPropertyHelper<string?> _errorMessage;
    public string? ErrorMessage => _errorMessage.Value;

    public ReactiveCommand<Unit, Unit> SetEnglish { get; }
    public ReactiveCommand<Unit, Unit> SetGerman { get; }
    public ReactiveCommand<Unit, Unit> SetFrench { get; }
}
