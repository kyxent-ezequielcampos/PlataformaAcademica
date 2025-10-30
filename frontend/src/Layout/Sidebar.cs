namespace frontend.Layout;

using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using frontend.Elements;
using System;

public class Sidebar : Border
{
    public event EventHandler<string>? NavigationRequested;

    public Sidebar()
    {
        Width = 250;
        Background = Colors.Sidebar;

        var stack = new StackPanel
        {
            Spacing = 8,
            Margin = new Avalonia.Thickness(16)
        };

        // Logo/Título
        var logo = new TextBlock
        {
            Text = "📚 Academia",
            FontSize = 24,
            FontWeight = FontWeight.Bold,
            Foreground = Brushes.White,
            Margin = new Avalonia.Thickness(0, 0, 0, 32)
        };
        stack.Children.Add(logo);

        // Menú items
        AddMenuItem(stack, "🏠 Inicio", "home");
        AddMenuItem(stack, "👨‍🎓 Estudiantes", "estudiantes");
        AddMenuItem(stack, "👨‍🏫 Docentes", "docentes");
        AddMenuItem(stack, "📖 Asignaturas", "asignaturas");

        Child = stack;
    }

    private void AddMenuItem(StackPanel parent, string text, string route)
    {
        var btn = new Button
        {
            Content = text,
            Background = Brushes.Transparent,
            Foreground = Brushes.White,
            BorderThickness = new Avalonia.Thickness(0),
            Height = 48,
            HorizontalAlignment = HorizontalAlignment.Stretch,
            HorizontalContentAlignment = HorizontalAlignment.Left,
            Padding = new Avalonia.Thickness(16, 0),
            FontSize = 15,
            Cursor = new Avalonia.Input.Cursor(Avalonia.Input.StandardCursorType.Hand)
        };

        btn.Click += (s, e) => NavigationRequested?.Invoke(this, route);
        parent.Children.Add(btn);
    }
}
