namespace frontend.Layout;

using Avalonia.Controls;
using Avalonia.Layout;
using frontend.Elements;
using frontend.Views;

public class AppLayout : Grid
{
    private readonly Border _contentArea;
    private readonly Sidebar _sidebar;

    public AppLayout(Window parentWindow)
    {
        ColumnDefinitions = new ColumnDefinitions("Auto, *");
        RowDefinitions = new RowDefinitions("Auto, *");
        Background = AppColors.Background;

        // Header
        var header = new Header(parentWindow);
        Grid.SetColumn(header, 0);
        Grid.SetRow(header, 0);
        Grid.SetColumnSpan(header, 2);

        // Sidebar
        _sidebar = new Sidebar();
        _sidebar.NavigationRequested += OnNavigationRequested;
        Grid.SetColumn(_sidebar, 0);
        Grid.SetRow(_sidebar, 1);

        // Content Area
        _contentArea = new Border
        {
            Background = AppColors.Background,
            Padding = new Avalonia.Thickness(24)
        };
        Grid.SetColumn(_contentArea, 1);
        Grid.SetRow(_contentArea, 1);

        Children.Add(header);
        Children.Add(_sidebar);
        Children.Add(_contentArea);

        // Cargar vista inicial
        LoadView("home");
    }

    private void OnNavigationRequested(object? sender, string route)
    {
        LoadView(route);
    }

    private void LoadView(string route)
    {
        Control view = route switch
        {
            "home" => new HomeView(),
            "estudiantes" => new EstudiantesView(),
            "docentes" => new DocentesView(),
            "asignaturas" => new AsignaturasView(),
            "grados" => new GradosView(),
            "matriculas" => new MatriculasView(),
            "asignaciones" => new AsignacionesView(),
            "calificaciones" => new CalificacionesView(),
            _ => new HomeView()
        };

        _contentArea.Child = view;
    }
}