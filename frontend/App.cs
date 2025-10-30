using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using frontend.Views;
using frontend.Services;

namespace frontend;

public class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            // Verificar si hay sesión guardada
            var authService = new AuthService();
            
            if (authService.IsAuthenticated())
            {
                desktop.MainWindow = new MainWindow();
            }
            else
            {
                desktop.MainWindow = new LoginWindow();
            }
        }

        base.OnFrameworkInitializationCompleted();
    }
}
