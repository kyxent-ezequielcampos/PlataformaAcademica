namespace frontend.Layout;

using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using frontend.Elements;
using frontend.Config;
using System;

public class Sidebar : Border
{
    public event EventHandler<string>? NavigationRequested;
    private Button? _activeButton;

    public Sidebar()
    {
        Width = AppConfig.UI.SidebarWidth;
        Background = new LinearGradientBrush
        {
            StartPoint = new Avalonia.RelativePoint(0, 0, Avalonia.RelativeUnit.Relative),
            EndPoint = new Avalonia.RelativePoint(0, 1, Avalonia.RelativeUnit.Relative),
            GradientStops = new GradientStops
            {
                new GradientStop(Color.Parse("#2D1B3D"), 0),
                new GradientStop(Color.Parse("#1F1329"), 1)
            }
        };

        var stack = new StackPanel
        {
            Spacing = 6,
            Margin = new Avalonia.Thickness(16, 24)
        };

        // Logo/Título con gradiente
        var logoPanel = new StackPanel
        {
            Spacing = 8,
            Margin = new Avalonia.Thickness(8, 0, 0, 40)
        };

        var logo = new TextBlock
        {
            Text = "🎓 Academia",
            FontSize = 26,
            FontWeight = FontWeight.ExtraBold,
            Foreground = Brushes.White,
        };

        var subtitle = new TextBlock
        {
            Text = "Sistema de Gestión",
            FontSize = 12,
            FontWeight = FontWeight.Medium,
            Foreground = new SolidColorBrush(Color.Parse("#A78BFA")),
            Opacity = 0.8
        };

        logoPanel.Children.Add(logo);
        logoPanel.Children.Add(subtitle);
        stack.Children.Add(logoPanel);

        // Menú items
        AddMenuItem(stack, "🏠", "Inicio", "home", true);
        
        // Sección de Gestión Básica
        AddSectionTitle(stack, "GESTIÓN BÁSICA");
        AddMenuItem(stack, "👨‍🎓", "Estudiantes", "estudiantes");
        AddMenuItem(stack, "👨‍🏫", "Docentes", "docentes");
        AddMenuItem(stack, "📖", "Asignaturas", "asignaturas");
        AddMenuItem(stack, "🎓", "Grados", "grados");
        
        // Sección de Gestión Académica
        AddSectionTitle(stack, "GESTIÓN ACADÉMICA");
        AddMenuItem(stack, "📝", "Matrículas", "matriculas");
        AddMenuItem(stack, "👨‍🏫", "Asignaciones", "asignaciones");
        AddMenuItem(stack, "📊", "Calificaciones", "calificaciones");

        Child = stack;
    }

    private void AddMenuItem(StackPanel parent, string icon, string text, string route, bool isActive = false)
    {
        var btn = new Button
        {
            Background = isActive ? new SolidColorBrush(Color.Parse("#FF6B9D")) : Brushes.Transparent,
            Foreground = Brushes.White,
            BorderThickness = new Avalonia.Thickness(0),
            Height = 52,
            HorizontalAlignment = HorizontalAlignment.Stretch,
            HorizontalContentAlignment = HorizontalAlignment.Left,
            Padding = new Avalonia.Thickness(16, 0),
            FontSize = 15,
            FontWeight = isActive ? FontWeight.SemiBold : FontWeight.Medium,
            Cursor = new Avalonia.Input.Cursor(Avalonia.Input.StandardCursorType.Hand),
            CornerRadius = new Avalonia.CornerRadius(12),
            Margin = new Avalonia.Thickness(0, 3)
        };

        var contentPanel = new StackPanel
        {
            Orientation = Orientation.Horizontal,
            Spacing = 12
        };

        contentPanel.Children.Add(new TextBlock
        {
            Text = icon,
            FontSize = 20,
            VerticalAlignment = VerticalAlignment.Center
        });

        contentPanel.Children.Add(new TextBlock
        {
            Text = text,
            FontSize = 15,
            VerticalAlignment = VerticalAlignment.Center
        });

        btn.Content = contentPanel;

        if (isActive)
        {
            _activeButton = btn;
        }

        // Efectos hover
        btn.PointerEntered += (s, e) =>
        {
            if (btn != _activeButton)
            {
                btn.Background = new SolidColorBrush(Color.Parse("#3D2B4D"));
            }
        };

        btn.PointerExited += (s, e) =>
        {
            if (btn != _activeButton)
            {
                btn.Background = Brushes.Transparent;
            }
        };

        btn.Click += (s, e) =>
        {
            // Desactivar botón anterior
            if (_activeButton != null)
            {
                _activeButton.Background = Brushes.Transparent;
                _activeButton.FontWeight = FontWeight.Medium;
            }

            // Activar botón actual
            btn.Background = new SolidColorBrush(Color.Parse("#FF6B9D"));
            btn.FontWeight = FontWeight.SemiBold;
            _activeButton = btn;

            NavigationRequested?.Invoke(this, route);
        };

        parent.Children.Add(btn);
    }

    private void AddSectionTitle(StackPanel parent, string title)
    {
        var titleBlock = new TextBlock
        {
            Text = title,
            FontSize = 11,
            FontWeight = FontWeight.Bold,
            Foreground = new SolidColorBrush(Color.Parse("#A78BFA")),
            Opacity = 0.6,
            Margin = new Avalonia.Thickness(16, 20, 0, 8)
        };
        parent.Children.Add(titleBlock);
    }
}
