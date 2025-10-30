namespace frontend.Layout;

using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using frontend.Elements;
using frontend.Services;
using frontend.Views;
using frontend.Config;

public class Header : Border
{
    public Header(Window parentWindow)
    {
        Height = AppConfig.UI.HeaderHeight;
        Background = AppColors.Surface;
        BorderBrush = AppColors.BorderLight;
        BorderThickness = new Avalonia.Thickness(0, 0, 0, 2);
        BoxShadow = new BoxShadows(new BoxShadow
        {
            Blur = 10,
            Color = Color.Parse("#10000010"),
            OffsetY = 2
        });

        var grid = new Grid
        {
            ColumnDefinitions = new ColumnDefinitions("*, Auto"),
            Margin = new Avalonia.Thickness(32, 0)
        };

        // Título con gradiente
        var title = new TextBlock
        {
            Text = AppConfig.App.Name,
            FontSize = 22,
            FontWeight = FontWeight.ExtraBold,
            Foreground = new LinearGradientBrush
            {
                StartPoint = new Avalonia.RelativePoint(0, 0, Avalonia.RelativeUnit.Relative),
                EndPoint = new Avalonia.RelativePoint(1, 0, Avalonia.RelativeUnit.Relative),
                GradientStops = new GradientStops
                {
                    new GradientStop(Color.Parse("#FF6B9D"), 0),
                    new GradientStop(Color.Parse("#A78BFA"), 1)
                }
            },
            VerticalAlignment = VerticalAlignment.Center
        };
        Grid.SetColumn(title, 0);

        // Panel de usuario
        var userPanel = new StackPanel
        {
            Orientation = Orientation.Horizontal,
            Spacing = 16,
            VerticalAlignment = VerticalAlignment.Center
        };

        var authService = new AuthService();
        var user = authService.CurrentUser;

        // Avatar del usuario
        var avatar = new Border
        {
            Width = 40,
            Height = 40,
            CornerRadius = new Avalonia.CornerRadius(20),
            Background = new LinearGradientBrush
            {
                StartPoint = new Avalonia.RelativePoint(0, 0, Avalonia.RelativeUnit.Relative),
                EndPoint = new Avalonia.RelativePoint(1, 1, Avalonia.RelativeUnit.Relative),
                GradientStops = new GradientStops
                {
                    new GradientStop(Color.Parse("#FF6B9D"), 0),
                    new GradientStop(Color.Parse("#A78BFA"), 1)
                }
            },
            Child = new TextBlock
            {
                Text = user?.NombreUsuario?.Substring(0, 1).ToUpper() ?? "U",
                FontSize = 16,
                FontWeight = FontWeight.Bold,
                Foreground = Brushes.White,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            }
        };

        var userInfo = new StackPanel
        {
            Spacing = 2,
            VerticalAlignment = VerticalAlignment.Center
        };

        userInfo.Children.Add(new TextBlock
        {
            Text = user?.NombreUsuario ?? "Usuario",
            FontSize = 14,
            FontWeight = FontWeight.SemiBold,
            Foreground = AppColors.TextPrimary
        });

        userInfo.Children.Add(new TextBlock
        {
            Text = user?.Rol ?? "Usuario",
            FontSize = 12,
            FontWeight = FontWeight.Medium,
            Foreground = AppColors.TextSecondary
        });

        var logoutBtn = new StyledButton("Cerrar Sesión", ButtonStyle.Outline)
        {
            Width = 130,
            Height = 40
        };

        logoutBtn.Click += (s, e) =>
        {
            authService.ClearSession();
            var loginWindow = new LoginWindow();
            loginWindow.Show();
            parentWindow.Close();
        };

        userPanel.Children.Add(avatar);
        userPanel.Children.Add(userInfo);
        userPanel.Children.Add(logoutBtn);
        Grid.SetColumn(userPanel, 1);

        grid.Children.Add(title);
        grid.Children.Add(userPanel);

        Child = grid;
    }
}