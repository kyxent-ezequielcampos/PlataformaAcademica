namespace frontend.Views;

using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using frontend.Elements;
using frontend.Services;
using frontend.Models;
using System.Collections.Generic;

public class HomeView : StackPanel
{
    private readonly ApiService _apiService;
    private readonly Grid _cardsGrid;

    public HomeView()
    {
        Spacing = 24;
        _apiService = new ApiService();

        var authService = new AuthService();
        var user = authService.CurrentUser;

        // Título de bienvenida con gradiente
        Children.Add(new TextBlock
        {
            Text = $"¡Bienvenido, {user?.NombreUsuario}! 👋",
            FontSize = 36,
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
            }
        });

        Children.Add(new TextBlock
        {
            Text = $"Rol: {user?.Rol}",
            FontSize = 17,
            FontWeight = FontWeight.Medium,
            Foreground = AppColors.TextSecondary,
            Margin = new Avalonia.Thickness(0, -16, 0, 0)
        });

        // Cards con estadísticas
        _cardsGrid = new Grid
        {
            ColumnDefinitions = new ColumnDefinitions("*, *, *, *"),
            ColumnSpacing = 20,
            Margin = new Avalonia.Thickness(0, 24, 0, 0)
        };

        // Inicializar con valores por defecto
        AddStatCard(_cardsGrid, 0, "👨‍🎓 Estudiantes", "...", AppColors.Primary);
        AddStatCard(_cardsGrid, 1, "👨‍🏫 Docentes", "...", AppColors.Success);
        AddStatCard(_cardsGrid, 2, "📖 Asignaturas", "...", AppColors.Info);
        AddStatCard(_cardsGrid, 3, "📊 Sistema", "✓", AppColors.Secondary);

        Children.Add(_cardsGrid);

        // Información adicional
        Children.Add(new StyledCard(padding: 36)
        {
            Margin = new Avalonia.Thickness(0, 24, 0, 0),
            Child = new StackPanel
            {
                Spacing = 18,
                Children =
                {
                    new TextBlock
                    {
                        Text = "📌 Panel de Control",
                        FontSize = 24,
                        FontWeight = FontWeight.ExtraBold,
                        Foreground = AppColors.TextPrimary
                    },
                    new TextBlock
                    {
                        Text = "Utilice el menú lateral para navegar entre las diferentes secciones del sistema.",
                        FontSize = 16,
                        FontWeight = FontWeight.Medium,
                        Foreground = AppColors.TextSecondary,
                        TextWrapping = TextWrapping.Wrap
                    },
                    new TextBlock
                    {
                        Text = "• Gestione estudiantes, docentes y asignaturas\n• Visualice estadísticas en tiempo real\n• Administre el sistema académico de forma eficiente",
                        FontSize = 15,
                        Foreground = AppColors.TextSecondary,
                        TextWrapping = TextWrapping.Wrap,
                        Margin = new Avalonia.Thickness(0, 8, 0, 0),
                        LineHeight = 24
                    }
                }
            }
        });

        // Cargar estadísticas reales
        CargarEstadisticas();
    }

    private async void CargarEstadisticas()
    {
        try
        {
            // Cargar estudiantes
            var estudiantes = await _apiService.GetAsync<List<Estudiante>>("/estudiantes");
            var countEstudiantes = estudiantes?.Count ?? 0;
            ActualizarCard(0, countEstudiantes.ToString());

            // Cargar docentes
            var docentes = await _apiService.GetAsync<List<Docente>>("/docentes");
            var countDocentes = docentes?.Count ?? 0;
            ActualizarCard(1, countDocentes.ToString());

            // Cargar asignaturas
            var asignaturas = await _apiService.GetAsync<List<Asignatura>>("/asignaturas");
            var countAsignaturas = asignaturas?.Count ?? 0;
            ActualizarCard(2, countAsignaturas.ToString());

            // Estado del sistema
            var isHealthy = await _apiService.TestConnectionAsync();
            ActualizarCard(3, isHealthy ? "✓ Online" : "✗ Offline");
        }
        catch
        {
            // En caso de error, mantener los valores por defecto
            ActualizarCard(0, "Error");
            ActualizarCard(1, "Error");
            ActualizarCard(2, "Error");
            ActualizarCard(3, "✗ Error");
        }
    }

    private void ActualizarCard(int cardIndex, string newValue)
    {
        if (cardIndex < _cardsGrid.Children.Count)
        {
            var card = _cardsGrid.Children[cardIndex] as Border;
            if (card?.Child is StackPanel stackPanel && stackPanel.Children.Count > 1)
            {
                if (stackPanel.Children[1] is TextBlock valueText)
                {
                    valueText.Text = newValue;
                }
            }
        }
    }

    private void AddStatCard(Grid parent, int column, string title, string value, SolidColorBrush color)
    {
        var card = new StyledCard(padding: 28)
        {
            Child = new StackPanel
            {
                Spacing = 16,
                Children =
                {
                    new TextBlock
                    {
                        Text = title,
                        FontSize = 15,
                        FontWeight = FontWeight.Medium,
                        Foreground = AppColors.TextSecondary
                    },
                    new TextBlock
                    {
                        Text = value,
                        FontSize = 40,
                        FontWeight = FontWeight.ExtraBold,
                        Foreground = color
                    }
                }
            }
        };

        Grid.SetColumn(card, column);
        parent.Children.Add(card);
    }
}