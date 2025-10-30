namespace frontend.Layout;

using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using frontend.Elements;
using frontend.Services;
using frontend.Views;

public class Header : Border
{
    public Header(Window parentWindow)
    {
        Height = 70;
        Background = Colors.Surface;
        BorderBrush = Colors.Border;
        BorderThickness = new Avalonia.Thickness(0, 0, 0, 1);

        var grid = new Grid
        {
            ColumnDefinitions = new ColumnDefinitions("*, Auto"),
            Margin = new Avalonia.Thickness(24, 0)
        };

        // Título
        var title = new TextBlock
        {
            Text = "Sistema de Gestión Académica",
            FontSize = 20,
            FontWeight = FontWeight.Bold,
            Foreground = Colors.TextPrimary,
            VerticalAlignment = VerticalAlignment.Center
        };
        Grid.SetColumn(title, 0);

        // Panel de usuario
        var userPanel = new StackPanel
        {
            Orientation = Orientation.Horizontal,
            Spacing = 12,
            VerticalAlignment = VerticalAlignment.Center
        };

        var authService = new AuthService();
        var user = authService.CurrentUser;

        var userName = new TextBlock
        {
            Text = user?.NombreUsuario ?? "Usuario",
            FontSize = 14,
            FontWeight = FontWeight.Medium,
            Foreground = Colors.TextPrimary,
            VerticalAlignment = VerticalAlignment.Center
        };

        var logoutBtn = new StyledButton("Cerrar Sesión", false)
        {
            Width = 120,
            Height = 36
        };

        logoutBtn.Click += (s, e) =>
        {
            authService.ClearSession();
            var loginWindow = new LoginWindow();
            loginWindow.Show();
            parentWindow.Close();
        };

        userPanel.Children.Add(userName);
        userPanel.Children.Add(logoutBtn);
        Grid.SetColumn(userPanel, 1);

        grid.Children.Add(title);
        grid.Children.Add(userPanel);

        Child = grid;
    }
}