using System.Globalization;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using LiveResx.Avalonia;

namespace Playground;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        var custom = new LocalizedResource<string[]>(
            "MyCustomResources",
            new Dictionary<CultureInfo, string[]>
            {
                { new CultureInfo("en"), ["Hello", "English"] },
                { new CultureInfo("de"), ["Hallo", "Deutsch"] },
            });
        DynamicLocalization.Instance.RegisterResource(custom);
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow
            {
                DataContext = new MainWindowViewModel(),
            };
        }

        base.OnFrameworkInitializationCompleted();
    }
}