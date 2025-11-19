namespace frontend.Views;

using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using frontend.Elements;
using frontend.Models;
using frontend.Services;
using System;
using System.Collections.Generic;
using System.Linq;

public class MatriculasView : StackPanel
{
    private readonly ApiService _apiService;
    private readonly StackPanel _listaMatriculas;
    private readonly Border _formContainer;
    private bool _isEditing = false;
    private int _editingId = 0;
    private List<Estudiante> _estudiantes = new();
    private List<Grado> _grados = new();

    public MatriculasView()
    {
        Spacing = 20;
        _apiService = new ApiService();

        var header = new Grid
        {
            ColumnDefinitions = new ColumnDefinitions("*, Auto")
        };

        var title = new TextBlock
        {
            Text = "📝 Gestión de Matrículas",
            FontSize = 32,
            FontWeight = FontWeight.ExtraBold,
            Foreground = new LinearGradientBrush
            {
                StartPoint = new Avalonia.RelativePoint(0, 0, Avalonia.RelativeUnit.Relative),
                EndPoint = new Avalonia.RelativePoint(1, 0, Avalonia.RelativeUnit.Relative),
                GradientStops = new GradientStops
                {
                    new GradientStop(Color.Parse("#10B981"), 0),
                    new GradientStop(Color.Parse("#059669"), 1)
                }
            },
            VerticalAlignment = VerticalAlignment.Center
        };
        Grid.SetColumn(title, 0);

        var btnNuevo = new StyledButton("+ Nueva Matrícula", ButtonStyle.Primary);
        btnNuevo.Click += async (s, e) => await MostrarFormulario();
        Grid.SetColumn(btnNuevo, 1);

        header.Children.Add(title);
        header.Children.Add(btnNuevo);
        Children.Add(header);

        _formContainer = new StyledCard(padding: 32) { IsVisible = false };
        Children.Add(_formContainer);

        _listaMatriculas = new StackPanel { Spacing = 12 };
        var scrollViewer = new ScrollViewer
        {
            Content = _listaMatriculas,
            MaxHeight = 600
        };
        Children.Add(scrollViewer);

        CargarMatriculas();
    }

    private async void CargarMatriculas()
    {
        _listaMatriculas.Children.Clear();
        var matriculas = await _apiService.GetAsync<List<Matricula>>("/matriculas");

        if (matriculas == null || !matriculas.Any())
        {
            _listaMatriculas.Children.Add(new TextBlock
            {
                Text = "No hay matrículas registradas",
                FontSize = 14,
                Foreground = AppColors.TextSecondary
            });
            return;
        }

        foreach (var matricula in matriculas)
        {
            _listaMatriculas.Children.Add(CrearCardMatricula(matricula));
        }
    }

    private Border CrearCardMatricula(Matricula matricula)
    {
        var mainGrid = new Grid
        {
            ColumnDefinitions = new ColumnDefinitions("Auto, *, Auto")
        };

        var icon = new Border
        {
            Width = 48,
            Height = 48,
            CornerRadius = new Avalonia.CornerRadius(24),
            Background = new LinearGradientBrush
            {
                StartPoint = new Avalonia.RelativePoint(0, 0, Avalonia.RelativeUnit.Relative),
                EndPoint = new Avalonia.RelativePoint(1, 1, Avalonia.RelativeUnit.Relative),
                GradientStops = new GradientStops
                {
                    new GradientStop(Color.Parse("#10B981"), 0),
                    new GradientStop(Color.Parse("#059669"), 1)
                }
            },
            Child = new TextBlock
            {
                Text = "📝",
                FontSize = 24,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            }
        };
        Grid.SetColumn(icon, 0);

        var infoPanel = new StackPanel
        {
            Spacing = 4,
            Margin = new Avalonia.Thickness(16, 0, 0, 0),
            VerticalAlignment = VerticalAlignment.Center
        };

        infoPanel.Children.Add(new TextBlock
        {
            Text = matricula.NombreEstudiante ?? "Estudiante",
            FontSize = 18,
            FontWeight = FontWeight.Bold,
            Foreground = AppColors.TextPrimary
        });

        infoPanel.Children.Add(new TextBlock
        {
            Text = $"🎓 {matricula.NombreGrado} • 📅 {matricula.CicloEscolar} • {matricula.Estado}",
            FontSize = 14,
            Foreground = AppColors.TextSecondary
        });

        Grid.SetColumn(infoPanel, 1);

        var actions = new StackPanel
        {
            Orientation = Orientation.Horizontal,
            Spacing = 8,
            VerticalAlignment = VerticalAlignment.Center
        };

        var btnEditar = new StyledIconButton("✏️", IconButtonStyle.Primary);
        var btnEliminar = new StyledIconButton("🗑️", IconButtonStyle.Danger);

        btnEditar.Click += async (s, e) => await EditarMatricula(matricula);
        btnEliminar.Click += async (s, e) =>
        {
            var confirm = await frontend.Utils.MessageBox.ShowConfirmAsync(
                TopLevel.GetTopLevel(this) as Window ?? new Window(),
                "Confirmar eliminación",
                $"¿Está seguro de que desea eliminar esta matrícula?"
            );

            if (confirm)
            {
                var success = await _apiService.DeleteAsync($"/matriculas/{matricula.IdMatricula}");
                if (success)
                {
                    CargarMatriculas();
                    await frontend.Utils.MessageBox.ShowInfoAsync(
                        TopLevel.GetTopLevel(this) as Window ?? new Window(),
                        "✅ Éxito",
                        "Matrícula eliminada correctamente."
                    );
                }
            }
        };

        actions.Children.Add(btnEditar);
        actions.Children.Add(btnEliminar);
        Grid.SetColumn(actions, 2);

        mainGrid.Children.Add(icon);
        mainGrid.Children.Add(infoPanel);
        mainGrid.Children.Add(actions);

        return new StyledCard(mainGrid, 20);
    }

    private async System.Threading.Tasks.Task MostrarFormulario(Matricula? matricula = null)
    {
        _isEditing = matricula != null;
        _editingId = matricula?.IdMatricula ?? 0;

        _estudiantes = await _apiService.GetAsync<List<Estudiante>>("/estudiantes") ?? new();
        _grados = await _apiService.GetAsync<List<Grado>>("/grados") ?? new();

        var form = new StackPanel { Spacing = 20 };

        form.Children.Add(new TextBlock
        {
            Text = _isEditing ? "✏️ Editar Matrícula" : "➕ Nueva Matrícula",
            FontSize = 24,
            FontWeight = FontWeight.Bold,
            Foreground = new LinearGradientBrush
            {
                StartPoint = new Avalonia.RelativePoint(0, 0, Avalonia.RelativeUnit.Relative),
                EndPoint = new Avalonia.RelativePoint(1, 0, Avalonia.RelativeUnit.Relative),
                GradientStops = new GradientStops
                {
                    new GradientStop(Color.Parse("#10B981"), 0),
                    new GradientStop(Color.Parse("#059669"), 1)
                }
            }
        });

        _estudiantes.Insert(0, new Estudiante { IdEstudiante = 0, Nombres = "Seleccione un estudiante *", Apellidos = "" });

        var cmbEstudiante = new ComboBox
        {
            ItemsSource = _estudiantes,
            Height = 45,
            SelectedIndex = 0,
            FontSize = 14
        };

        cmbEstudiante.ItemTemplate = new Avalonia.Controls.Templates.FuncDataTemplate<Estudiante>((est, _) =>
        new TextBlock{
                Text = est.IdEstudiante == 0 ? est.Nombres : $"{est.Nombres} {est.Apellidos}"
            }
        
        
        
      );

        // Si está editando, seleccionar el estudiante real
        if (_isEditing && matricula != null)
        {
            cmbEstudiante.SelectedItem = _estudiantes.FirstOrDefault(e => e.IdEstudiante == matricula.IdEstudiante);
        }
        
        
        _grados.Insert(0, new Grado { IdGrado = 0, Nombre = "Seleccione un grado *" });

        var cmbGrado = new ComboBox
        {
            ItemsSource = _grados,
            Height = 45,
            FontSize = 14,
            SelectedIndex = 0
        };
        cmbGrado.ItemTemplate = new Avalonia.Controls.Templates.FuncDataTemplate<Grado>((g, _) =>
         new TextBlock { Text = g.Nombre }
         );
        

        var txtCiclo = new TextBox
        {
            Watermark = "Ciclo escolar (ej: 2025) *",
            Text = matricula?.CicloEscolar ?? DateTime.Now.Year.ToString(),
            Height = 45,
            FontSize = 14
        };

        form.Children.Add(new StyledLabel("Estudiante *", LabelStyle.Title));
        form.Children.Add(cmbEstudiante);
        form.Children.Add(new StyledLabel("Grado *", LabelStyle.Title));
        form.Children.Add(cmbGrado);
        form.Children.Add(new StyledLabel("Ciclo Escolar *", LabelStyle.Title));
        form.Children.Add(txtCiclo);

        var errorText = new TextBlock
        {
            Foreground = AppColors.Danger,
            FontSize = 14,
            IsVisible = false
        };
        form.Children.Add(errorText);

        var btnActions = new StackPanel
        {
            Orientation = Orientation.Horizontal,
            Spacing = 12,
            HorizontalAlignment = HorizontalAlignment.Right
        };

        var btnCancelar = new StyledButton("Cancelar", ButtonStyle.Outline);
        var btnGuardar = new StyledButton(_isEditing ? "💾 Actualizar" : "💾 Guardar", ButtonStyle.Primary);

        btnCancelar.Click += (s, e) => _formContainer.IsVisible = false;
        btnGuardar.Click += async (s, e) =>
        {
            if (cmbEstudiante.SelectedItem == null || cmbGrado.SelectedItem == null || string.IsNullOrWhiteSpace(txtCiclo.Text))
            {
                errorText.Text = "⚠️ Todos los campos son obligatorios";
                errorText.IsVisible = true;
                return;
            }

            var estudianteSeleccionado = (Estudiante)cmbEstudiante.SelectedItem;
            var gradoSeleccionado = (Grado)cmbGrado.SelectedItem;

            var data = new
            {
                idEstudiante = estudianteSeleccionado.IdEstudiante,
                idGrado = gradoSeleccionado.IdGrado,
                cicloEscolar = txtCiclo.Text?.Trim()
            };

            bool success;
            if (_isEditing)
            {
                success = await _apiService.PutAsync($"/matriculas/{_editingId}", data);
            }
            else
            {
                var result = await _apiService.PostAsync<object, object>("/matriculas", data);
                success = result != null;
            }

            if (success)
            {
                _formContainer.IsVisible = false;
                CargarMatriculas();
                await frontend.Utils.MessageBox.ShowInfoAsync(
                    TopLevel.GetTopLevel(this) as Window ?? new Window(),
                    "✅ Éxito",
                    $"Matrícula {(_isEditing ? "actualizada" : "creada")} correctamente."
                );
            }
            else
            {
                errorText.Text = "❌ Error al guardar";
                errorText.IsVisible = true;
            }
        };

        btnActions.Children.Add(btnCancelar);
        btnActions.Children.Add(btnGuardar);
        form.Children.Add(btnActions);

        _formContainer.Child = form;
        _formContainer.IsVisible = true;
    }

    private async System.Threading.Tasks.Task EditarMatricula(Matricula matricula)
    {
        await MostrarFormulario(matricula);
    }
}
