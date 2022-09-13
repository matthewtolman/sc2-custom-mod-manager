using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace SC2_Avalonia_UI;

public partial class AboutWindow : Window
{
    public AboutWindow()
    {
        InitializeComponent();
#if DEBUG
        this.AttachDevTools();
#endif
        // AboutLabel.Content = $"About {Consts.AppName} v{Consts.Version}";
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}