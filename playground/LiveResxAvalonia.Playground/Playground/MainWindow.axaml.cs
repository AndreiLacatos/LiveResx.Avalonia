using Avalonia.Controls;
using LiveResx.Avalonia;

namespace Playground;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        var str = DynamicResources.HelloWorld;
        Console.WriteLine(str.Text);
        InitializeComponent();
    }
}