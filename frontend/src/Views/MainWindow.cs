namespace frontend.Views;

using Avalonia.Controls;
using frontend.Layout;
using frontend.Config;

public class MainWindow : Window
{
    public MainWindow()
    {
        Title = AppConfig.App.Name;
        Width = AppConfig.UI.DefaultWindowWidth;
        Height = AppConfig.UI.DefaultWindowHeight;
        WindowStartupLocation = WindowStartupLocation.CenterScreen;

        Content = new AppLayout(this);
    }
}