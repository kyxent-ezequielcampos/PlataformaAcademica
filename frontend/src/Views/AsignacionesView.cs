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

public class AsignacionesView : StackPanel
{
    private readonly ApiService _apiService;
    private readonly StackPanel _listaAsignaciones;
    private readonly Border _formContainer;
    private bool _isEditing = false;
    private int _editingId = 0;
    private List<Docente> _docentes = new();
    private List<Asignatura> _asignaturas = new();
    private List<Grado> _grados = new();

    public AsignacionesView()
    {
        Spacing = 20;
        _apiService = new ApiService();

        var header = new Grid
        {
            ColumnDefinitions = new ColumnDefinitions("*, Auto")
        };

        var title = new TextBlock
        {
            Text = "👨‍🏫 Asignaciones Docentes",
            FontSize = 32,
            FontWeight = FontWeight.ExtraBold,
            Foreground = new LinearGradientBrush
            {
                StartPoint = new Avalonia.RelativePoint(0, 0, Avalonia.RelativeUnit.Relative),
                EndPoint = new Avalonia.RelativePoint(1, 0, Avalonia.RelativeUnit.Relative),
                GradientStops = new GradientStops
                {
                    new GradientStop(Color.Parse("#EC4899"), 0),
                    new GradientStop(Color.Parse("#DB2777"), 1)
                }
            },
            VerticalAlignment = VerticalAlignment.Center
        };
        Grid.SetColumn(title, 0);

        var btnNuevo = new StyledButton("+ Nueva Asignación", ButtonStyle.Primary);
        btnNuevo.Click += async (s, e) => await MostrarFormulario();
        Grid.SetColumn(btnNuevo, 1);

        header.Children.Add(title);
        header.Children.Add(btnNuevo);
        Children.Add(header);

        _formContainer = new StyledCard(padding: 32) { IsVisible = false };
        Children.Add(_formContainer);

        _listaAsignaciones = new StackPanel { Spacing = 12 };
        var scrollViewer = new ScrollViewer
        {
            Content = _listaAsignaciones,
            MaxHeight = 600
        };
        Children.Add(scrollViewer);

        CargarAsignaciones();
    }

    private async void CargarAsignaciones()
    {
        _listaAsignaciones.Children.Clear();
        var asignaciones = await _apiService.GetAsync<List<Asignacion>>("/asignaciones");

        if (asignaciones == null || !asignaciones.Any())
        {
            _listaAsignaciones.Children.Add(new TextBlock
            {
                Text = "No hay asignaciones registradas",
                FontSize = 14,
                Foreground = AppColors.TextSecondary
            });
            return;
        }

        foreach (var asignacion in asignaciones)
        {
            _listaAsignaciones.Children.Add(CrearCardAsignacion(asignacion));
        }
    }

    private Border CrearCardAsignacion(Asignacion asignacion)
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
                    new GradientStop(Color.Parse("#EC4899"), 0),
                    new GradientStop(Color.Parse("#DB2777"), 1)
                }
            },
            Child = new TextBlock
            {
                Text = "👨‍🏫",
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
            Text = asignacion.NombreDocente ?? "Docente",
            FontSize = 18,
            FontWeight = FontWeight.Bold,
            Foreground = AppColors.TextPrimary
        });

        infoPanel.Children.Add(new TextBlock
        {
            Text = $"📚 {asignacion.NombreAsignatura} • 🎓 {asignacion.NombreGrado} • 📅 {asignacion.CicloEscolar}",
            FontSize = 14,
            Foreground = AppColors.TextSecondary
        });

        Grid.SetColumn(infoPanel, 1);

        var actions = new StackPanel
        {
            Orientation = Orientation.Horizontal,
            Spacing = 8,
            VerticalAlignment = VerticalAlignment.Center,
            HorizontalAlignment = HorizontalAlignment.Center
        };

        var btnEditar = new StyledIconButton("✏️", IconButtonStyle.Primary);
        var btnEliminar = new StyledIconButton("🗑️", IconButtonStyle.Danger);
        btnEditar.HorizontalContentAlignment = HorizontalAlignment.Center;
        btnEditar.VerticalContentAlignment = VerticalAlignment.Center;

        btnEliminar.HorizontalContentAlignment = HorizontalAlignment.Center;
        btnEliminar.VerticalContentAlignment = VerticalAlignment.Center;

        btnEditar.Click += async (s, e) => await EditarAsignacion(asignacion);
        btnEliminar.Click += async (s, e) =>
        {
            var confirm = await frontend.Utils.MessageBox.ShowConfirmAsync(
                TopLevel.GetTopLevel(this) as Window ?? new Window(),
                "Confirmar eliminación",
                $"¿Está seguro de que desea eliminar esta asignación?"
            );

            if (confirm)
            {
                var success = await _apiService.DeleteAsync($"/asignaciones/{asignacion.IdAsignacion}");
                if (success)
                {
                    CargarAsignaciones();
                    await frontend.Utils.MessageBox.ShowInfoAsync(
                        TopLevel.GetTopLevel(this) as Window ?? new Window(),
                        "✅ Éxito",
                        "Asignación eliminada correctamente."
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

    private async System.Threading.Tasks.Task MostrarFormulario(Asignacion? asignacion = null)
    {
        _isEditing = asignacion != null;
        _editingId = asignacion?.IdAsignacion ?? 0;

        _docentes = await _apiService.GetAsync<List<Docente>>("/docentes") ?? new();
        _asignaturas = await _apiService.GetAsync<List<Asignatura>>("/asignaturas") ?? new();
        _grados = await _apiService.GetAsync<List<Grado>>("/grados") ?? new();

        var form = new StackPanel { Spacing = 20 };

        form.Children.Add(new TextBlock
        {
            Text = _isEditing ? "✏️ Editar Asignación" : "➕ Nueva Asignación",
            FontSize = 24,
            FontWeight = FontWeight.Bold,
            Foreground = new LinearGradientBrush
            {
                StartPoint = new Avalonia.RelativePoint(0, 0, Avalonia.RelativeUnit.Relative),
                EndPoint = new Avalonia.RelativePoint(1, 0, Avalonia.RelativeUnit.Relative),
                GradientStops = new GradientStops
                {
                    new GradientStop(Color.Parse("#EC4899"), 0),
                    new GradientStop(Color.Parse("#DB2777"), 1)
                }
            }
        });
        _docentes = await _apiService.GetAsync<List<Docente>>("/docentes") ?? new();

        // Insertar placeholder correcto (Docente, no Grado)
        _docentes.Insert(0, new Docente
        {
            IdDocente = 0,
            Nombres = "Seleccione un docente *",
            Apellidos = ""
        });

        var cmbDocente = new ComboBox
        {
            ItemsSource = _docentes,
            Height = 45,
            FontSize = 14,
            SelectedIndex = 0
        };

        if (_docentes.Any())
        {
            cmbDocente.ItemTemplate = new Avalonia.Controls.Templates.FuncDataTemplate<Docente>((doc, _) =>
                new TextBlock { Text = $"{doc.Nombres} {doc.Apellidos}".Trim() }
            );

            if (_isEditing && asignacion != null)
            {
                cmbDocente.SelectedItem = _docentes
                    .FirstOrDefault(d => d.IdDocente == asignacion.IdDocente);
            }
        }


        _asignaturas = await _apiService.GetAsync<List<Asignatura>>("/asignaturas") ?? new();

        // Insertar placeholder correcto
        _asignaturas.Insert(0, new Asignatura
        {
            IdAsignatura = 0,
            Nombre = "Seleccione una asignatura *"
        });

        var cmbAsignatura = new ComboBox
        {
            ItemsSource = _asignaturas,
            Height = 45,
            FontSize = 14,
            SelectedIndex = 0
        };

        if (_asignaturas.Any())
        {
            cmbAsignatura.ItemTemplate = new Avalonia.Controls.Templates.FuncDataTemplate<Asignatura>((asig, _) =>
                new TextBlock { Text = asig.Nombre }
            );

            // Seleccionar al editar
            if (_isEditing && asignacion != null)
            {
                cmbAsignatura.SelectedItem = _asignaturas
                    .FirstOrDefault(a => a.IdAsignatura == asignacion.IdAsignatura);
            }
        }


        _grados = await _apiService.GetAsync<List<Grado>>("/grados") ?? new();

        // Insertar placeholder correcto
        _grados.Insert(0, new Grado
        {
            IdGrado = 0,
            Nombre = "Seleccione un grado *"
        });

        var cmbGrado = new ComboBox
        {
            ItemsSource = _grados,
            Height = 45,
            FontSize = 14,
            SelectedIndex = 0
        };

        if (_grados.Any())
        {
            cmbGrado.ItemTemplate = new Avalonia.Controls.Templates.FuncDataTemplate<Grado>((grd, _) =>
                new TextBlock { Text = grd.Nombre }
            );

            // Seleccionar al editar
            if (_isEditing && asignacion != null)
            {
                cmbGrado.SelectedItem = _grados
                    .FirstOrDefault(g => g.IdGrado == asignacion.IdGrado);
            }
        }


        var txtCiclo = new TextBox
        {
            Watermark = "Ciclo escolar (ej: 2025) *",
            Text = asignacion?.CicloEscolar ?? DateTime.Now.Year.ToString(),
            Height = 45,
            FontSize = 14
        };

        form.Children.Add(new StyledLabel("Docente *", LabelStyle.Title));
        form.Children.Add(cmbDocente);
        form.Children.Add(new StyledLabel("Asignatura *", LabelStyle.Title));
        form.Children.Add(cmbAsignatura);
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
            if (cmbDocente.SelectedItem == null || cmbAsignatura.SelectedItem == null || 
                cmbGrado.SelectedItem == null || string.IsNullOrWhiteSpace(txtCiclo.Text))
            {
                errorText.Text = "⚠️ Todos los campos son obligatorios";
                errorText.IsVisible = true;
                return;
            }

            var docenteSeleccionado = (Docente)cmbDocente.SelectedItem;
            var asignaturaSeleccionada = (Asignatura)cmbAsignatura.SelectedItem;
            var gradoSeleccionado = (Grado)cmbGrado.SelectedItem;

            var data = new
            {
                idDocente = docenteSeleccionado.IdDocente,
                idAsignatura = asignaturaSeleccionada.IdAsignatura,
                idGrado = gradoSeleccionado.IdGrado,
                cicloEscolar = txtCiclo.Text?.Trim()
            };

            bool success;
            if (_isEditing)
            {
                success = await _apiService.PutAsync($"/asignaciones/{_editingId}", data);
            }
            else
            {
                var result = await _apiService.PostAsync<object, object>("/asignaciones", data);
                success = result != null;
            }

            if (success)
            {
                _formContainer.IsVisible = false;
                CargarAsignaciones();
                await frontend.Utils.MessageBox.ShowInfoAsync(
                    TopLevel.GetTopLevel(this) as Window ?? new Window(),
                    "✅ Éxito",
                    $"Asignación {(_isEditing ? "actualizada" : "creada")} correctamente."
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

    private async System.Threading.Tasks.Task EditarAsignacion(Asignacion asignacion)
    {
        await MostrarFormulario(asignacion);
    }
}
