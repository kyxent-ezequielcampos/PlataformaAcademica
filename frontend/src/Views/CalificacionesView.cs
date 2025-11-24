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

public class CalificacionesView : StackPanel
{
    private readonly ApiService _apiService;
    private readonly StackPanel _listaCalificaciones;
    private readonly Border _formContainer;
    private bool _isEditing = false;
    private int _editingId = 0;
    private List<Inscripcion> _inscripciones = new();

    public CalificacionesView()
    {
        Spacing = 20;
        _apiService = new ApiService();

        var header = new Grid
        {
            ColumnDefinitions = new ColumnDefinitions("*, Auto")
        };

        var title = new TextBlock
        {
            Text = "📊 Gestión de Calificaciones",
            FontSize = 32,
            FontWeight = FontWeight.ExtraBold,
            Foreground = new LinearGradientBrush
            {
                StartPoint = new Avalonia.RelativePoint(0, 0, Avalonia.RelativeUnit.Relative),
                EndPoint = new Avalonia.RelativePoint(1, 0, Avalonia.RelativeUnit.Relative),
                GradientStops = new GradientStops
                {
                    new GradientStop(Color.Parse("#F59E0B"), 0),
                    new GradientStop(Color.Parse("#D97706"), 1)
                }
            },
            VerticalAlignment = VerticalAlignment.Center
        };
        Grid.SetColumn(title, 0);

        var actionsPanel = new StackPanel
        {
            Orientation = Orientation.Horizontal,
            Spacing = 12,
            VerticalAlignment = VerticalAlignment.Center
        };

        var btnReporte = new StyledButton("📄 Generar Reporte", ButtonStyle.Outline);
        btnReporte.Click += async (s, e) => await GenerarReporteNotas();

        var btnNuevo = new StyledButton("+ Nueva Calificación", ButtonStyle.Primary);
        btnNuevo.Click += async (s, e) => await MostrarFormulario();

        actionsPanel.Children.Add(btnReporte);
        actionsPanel.Children.Add(btnNuevo);
        Grid.SetColumn(actionsPanel, 1);

        header.Children.Add(title);
        header.Children.Add(actionsPanel);
        Children.Add(header);

        _formContainer = new StyledCard(padding: 32) { IsVisible = false };
        Children.Add(_formContainer);

        _listaCalificaciones = new StackPanel { Spacing = 12 };
        var scrollViewer = new ScrollViewer
        {
            Content = _listaCalificaciones,
            MaxHeight = 600
        };
        Children.Add(scrollViewer);

        CargarCalificaciones();
    }

    private async void CargarCalificaciones()
    {
        _listaCalificaciones.Children.Clear();
        var calificaciones = await _apiService.GetAsync<List<Calificacion>>("/calificaciones");

        if (calificaciones == null || !calificaciones.Any())
        {
            _listaCalificaciones.Children.Add(new TextBlock
            {
                Text = "No hay calificaciones registradas",
                FontSize = 14,
                Foreground = AppColors.TextSecondary
            });
            return;
        }

        // Agrupar por estudiante para mostrar promedios
        var porEstudiante = calificaciones.GroupBy(c => c.NombreEstudiante);

        foreach (var grupo in porEstudiante)
        {
            var promedio = grupo.Average(c => (double)c.Nota);
            _listaCalificaciones.Children.Add(CrearCardEstudiante(grupo.Key ?? "Estudiante", promedio, grupo.ToList()));
        }
    }

    private Border CrearCardEstudiante(string nombreEstudiante, double promedio, List<Calificacion> calificaciones)
    {
        var mainPanel = new StackPanel { Spacing = 12 };

        // Header del estudiante con promedio
        var headerGrid = new Grid
        {
            ColumnDefinitions = new ColumnDefinitions("Auto, *, Auto")
        };

        var promedioColor = promedio >= 60 ? "#10B981" : "#EF4444";
        var icon = new Border
        {
            Width = 56,
            Height = 56,
            CornerRadius = new Avalonia.CornerRadius(28),
            Background = new SolidColorBrush(Color.Parse(promedioColor)),
            Child = new TextBlock
            {
                Text = promedio.ToString("0.0"),
                FontSize = 18,
                FontWeight = FontWeight.Bold,
                Foreground = Brushes.White,
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
            Text = nombreEstudiante,
            FontSize = 20,
            FontWeight = FontWeight.Bold,
            Foreground = AppColors.TextPrimary
        });

        infoPanel.Children.Add(new TextBlock
        {
            Text = $"📊 Promedio General: {promedio:0.00}",
            FontSize = 14,
            Foreground = AppColors.TextSecondary
        });

        Grid.SetColumn(infoPanel, 1);

        var badge = new Border
        {
            Background = new SolidColorBrush(Color.Parse(promedioColor)),
            CornerRadius = new Avalonia.CornerRadius(8),
            Padding = new Avalonia.Thickness(12, 6),
            VerticalAlignment = VerticalAlignment.Center,
            Child = new TextBlock
            {
                Text = promedio >= 60 ? "✅ Aprobado" : "❌ Reprobado",
                FontSize = 13,
                FontWeight = FontWeight.Bold,
                Foreground = Brushes.White
            }
        };
        Grid.SetColumn(badge, 2);

        headerGrid.Children.Add(icon);
        headerGrid.Children.Add(infoPanel);
        headerGrid.Children.Add(badge);

        mainPanel.Children.Add(headerGrid);

        // Separador
        mainPanel.Children.Add(new Border
        {
            Height = 1,
            Background = new SolidColorBrush(Color.Parse("#E5E7EB")),
            Margin = new Avalonia.Thickness(0, 8, 0, 8)
        });

        // Lista de calificaciones individuales
        foreach (var calificacion in calificaciones.OrderBy(c => c.Periodo))
        {
            mainPanel.Children.Add(CrearItemCalificacion(calificacion));
        }

        return new StyledCard(mainPanel, 24);
    }

    private Grid CrearItemCalificacion(Calificacion calificacion)
    {
        var grid = new Grid
        {
            ColumnDefinitions = new ColumnDefinitions("*, Auto, Auto, Auto"),
            Margin = new Avalonia.Thickness(0, 4, 0, 4),
            VerticalAlignment = VerticalAlignment.Center
        };

        var asignaturaPanel = new StackPanel { 
            Spacing = 2,
            VerticalAlignment = VerticalAlignment.Center
        };
        asignaturaPanel.Children.Add(new TextBlock
        {
            Text = calificacion.NombreAsignatura ?? "Asignatura",
            FontSize = 15,
            FontWeight = FontWeight.SemiBold,
            Foreground = AppColors.TextPrimary
        });
        asignaturaPanel.Children.Add(new TextBlock
        {
            Text = $"📅 {calificacion.Periodo}",
            FontSize = 12,
            Foreground = AppColors.TextSecondary
        });
        Grid.SetColumn(asignaturaPanel, 0);

        var notaColor = calificacion.Nota >= 60 ? "#10B981" : "#EF4444";
        var notaBadge = new Border
        {
            Background = new SolidColorBrush(Color.Parse(notaColor)),
            CornerRadius = new Avalonia.CornerRadius(6),
            Padding = new Avalonia.Thickness(10, 4),
            Margin = new Avalonia.Thickness(8, 0, 0, 0),
            VerticalAlignment = VerticalAlignment.Center,
            Child = new TextBlock
            {
                Text = calificacion.Nota.ToString("0.00"),
                FontSize = 14,
                FontWeight = FontWeight.Bold,
                Foreground = Brushes.White,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            }
        };
        Grid.SetColumn(notaBadge, 1);

        var btnEditar = new StyledIconButton("✏️", IconButtonStyle.Primary)
        {
            Margin = new Avalonia.Thickness(8, 0, 0, 0),
            VerticalAlignment = VerticalAlignment.Center,
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalContentAlignment = VerticalAlignment.Center,
            HorizontalContentAlignment = HorizontalAlignment.Center
        };
        btnEditar.Click += async (s, e) => await EditarCalificacion(calificacion);
        Grid.SetColumn(btnEditar, 2);

        var btnEliminar = new StyledIconButton("🗑️", IconButtonStyle.Danger)
        {
            Margin = new Avalonia.Thickness(4, 0, 4, 0),
            VerticalAlignment = VerticalAlignment.Center,
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalContentAlignment = VerticalAlignment.Center,
            HorizontalContentAlignment = HorizontalAlignment.Center
        };
        btnEliminar.Click += async (s, e) =>
        {
            var confirm = await frontend.Utils.MessageBox.ShowConfirmAsync(
                TopLevel.GetTopLevel(this) as Window ?? new Window(),
                "Confirmar eliminación",
                $"¿Está seguro de que desea eliminar esta calificación?"
            );

            if (confirm)
            {
                var success = await _apiService.DeleteAsync($"/calificaciones/{calificacion.IdCalificacion}");
                if (success)
                {
                    CargarCalificaciones();
                    await frontend.Utils.MessageBox.ShowInfoAsync(
                        TopLevel.GetTopLevel(this) as Window ?? new Window(),
                        "✅ Éxito",
                        "Calificación eliminada correctamente."
                    );
                }
            }
        };
        Grid.SetColumn(btnEliminar, 3);

        grid.Children.Add(asignaturaPanel);
        grid.Children.Add(notaBadge);
        grid.Children.Add(btnEditar);
        grid.Children.Add(btnEliminar);

        return grid;
    }

    private async System.Threading.Tasks.Task MostrarFormulario(Calificacion? calificacion = null)
    {
        _isEditing = calificacion != null;
        _editingId = calificacion?.IdCalificacion ?? 0;

        _inscripciones = await _apiService.GetAsync<List<Inscripcion>>("/inscripciones") ?? new();

        var form = new StackPanel { Spacing = 20 };

        form.Children.Add(new TextBlock
        {
            Text = _isEditing ? "✏️ Editar Calificación" : "➕ Nueva Calificación",
            FontSize = 24,
            FontWeight = FontWeight.Bold,
            Foreground = new LinearGradientBrush
            {
                StartPoint = new Avalonia.RelativePoint(0, 0, Avalonia.RelativeUnit.Relative),
                EndPoint = new Avalonia.RelativePoint(1, 0, Avalonia.RelativeUnit.Relative),
                GradientStops = new GradientStops
                {
                    new GradientStop(Color.Parse("#F59E0B"), 0),
                    new GradientStop(Color.Parse("#D97706"), 1)
                }
            }
        });

            var cmbInscripcion = new ComboBox
            {
                PlaceholderText = "Seleccione estudiante-asignatura *",
                ItemsSource = _inscripciones,
                Height = 45,
                FontSize = 14,
                IsEnabled = !_isEditing
            };

            if (_inscripciones.Any())
            {
                cmbInscripcion.ItemTemplate = new Avalonia.Controls.Templates.FuncDataTemplate<Inscripcion>((insc, _) =>
                    new TextBlock
                    {
                        Text = insc == null
                            ? ""   // IMPORTANTE: NO usar "Sin datos"
                            : $"{insc.NombreEstudiante} - {insc.NombreAsignatura}"
                    }
                );

                if (_isEditing && calificacion != null)
                {
                    cmbInscripcion.SelectedItem = _inscripciones
                        .FirstOrDefault(i => i.IdInscripcion == calificacion.IdInscripcion);
                }
            }
            else
            {
                cmbInscripcion.SelectedItem = null;
            }


        var txtPeriodo = new TextBox
        {
            Watermark = "Periodo (ej: Parcial 1, Final) *",
            Text = calificacion?.Periodo ?? "",
            Height = 45,
            FontSize = 14
        };

        var txtNota = new TextBox
        {
            Watermark = "Nota (0-100) *",
            Text = calificacion?.Nota.ToString() ?? "",
            Height = 45,
            FontSize = 14
        };

        var txtObservaciones = new TextBox
        {
            Watermark = "Observaciones (opcional)",
            Text = calificacion?.Observaciones ?? "",
            Height = 80,
            FontSize = 14,
            AcceptsReturn = true,
            TextWrapping = Avalonia.Media.TextWrapping.Wrap
        };

        form.Children.Add(new StyledLabel("Estudiante - Asignatura *", LabelStyle.Title));
        form.Children.Add(cmbInscripcion);
        form.Children.Add(new StyledLabel("Periodo *", LabelStyle.Title));
        form.Children.Add(txtPeriodo);
        form.Children.Add(new StyledLabel("Nota (0-100) *", LabelStyle.Title));
        form.Children.Add(txtNota);
        form.Children.Add(new StyledLabel("Observaciones", LabelStyle.Title));
        form.Children.Add(txtObservaciones);

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
            if (cmbInscripcion.SelectedItem == null || string.IsNullOrWhiteSpace(txtPeriodo.Text) || 
                string.IsNullOrWhiteSpace(txtNota.Text))
            {
                errorText.Text = "⚠️ Todos los campos marcados con * son obligatorios";
                errorText.IsVisible = true;
                return;
            }

            if (!decimal.TryParse(txtNota.Text, out decimal nota) || nota < 0 || nota > 100)
            {
                errorText.Text = "⚠️ La nota debe ser un número entre 0 y 100";
                errorText.IsVisible = true;
                return;
            }

            var inscripcionSeleccionada = (Inscripcion)cmbInscripcion.SelectedItem;

            var data = new
            {
                idInscripcion = inscripcionSeleccionada.IdInscripcion,
                periodo = txtPeriodo.Text?.Trim(),
                nota = nota,
                observaciones = txtObservaciones.Text?.Trim()
            };

            bool success;
            if (_isEditing)
            {
                success = await _apiService.PutAsync($"/calificaciones/{_editingId}", data);
            }
            else
            {
                var result = await _apiService.PostAsync<object, object>("/calificaciones", data);
                success = result != null;
            }

            if (success)
            {
                _formContainer.IsVisible = false;
                CargarCalificaciones();
                await frontend.Utils.MessageBox.ShowInfoAsync(
                    TopLevel.GetTopLevel(this) as Window ?? new Window(),
                    "✅ Éxito",
                    $"Calificación {(_isEditing ? "actualizada" : "creada")} correctamente."
                );
            }
            else
            {
                errorText.Text = "❌ Error al guardar. Verifique que no exista ya una calificación para este periodo.";
                errorText.IsVisible = true;
            }
        };

        btnActions.Children.Add(btnCancelar);
        btnActions.Children.Add(btnGuardar);
        form.Children.Add(btnActions);

        _formContainer.Child = form;
        _formContainer.IsVisible = true;
    }

    private async System.Threading.Tasks.Task EditarCalificacion(Calificacion calificacion)
    {
        await MostrarFormulario(calificacion);
    }

    private async System.Threading.Tasks.Task GenerarReporteNotas()
    {
        // Crear diálogo para seleccionar estudiante y ciclo
        var dialog = new Window
        {
            Title = "Generar Reporte de Notas",
            Width = 500,
            Height = 415,
            WindowStartupLocation = WindowStartupLocation.CenterOwner,
            CanResize = false
        };

        var mainPanel = new StackPanel
        {
            Margin = new Avalonia.Thickness(30),
            Spacing = 20
        };

        mainPanel.Children.Add(new TextBlock
        {
            Text = "📄 Generar Reporte de Notas",
            FontSize = 20,
            FontWeight = FontWeight.Bold,
            Foreground = new SolidColorBrush(Color.Parse("#F59E0B"))
        });

        // Cargar estudiantes
        var estudiantes = await _apiService.GetAsync<List<Estudiante>>("/estudiantes") ?? new();
        estudiantes.Insert(0, new Estudiante { IdEstudiante = 0, Nombres = "Seleccione un estudiante", Apellidos = "" });

        var cmbEstudiante = new ComboBox
        {
            ItemsSource = estudiantes,
            SelectedIndex = 0,
            Height = 45,
            FontSize = 14
        };
        cmbEstudiante.ItemTemplate = new Avalonia.Controls.Templates.FuncDataTemplate<Estudiante>((est, _) =>
            new TextBlock { Text = est.IdEstudiante == 0 ? est.Nombres : $"{est.Nombres} {est.Apellidos}" }
        );

        var txtCiclo = new TextBox
        {
            Watermark = "Ciclo escolar (ej: 2025)",
            Text = DateTime.Now.Year.ToString(),
            Height = 45,
            FontSize = 14
        };

        var errorText = new TextBlock
        {
            Foreground = AppColors.Danger,
            FontSize = 14,
            IsVisible = false
        };

        var btnActions = new StackPanel
        {
            Orientation = Orientation.Horizontal,
            Spacing = 12,
            HorizontalAlignment = HorizontalAlignment.Right,
            Margin = new Avalonia.Thickness(0, 20, 0, 0)
        };

        var btnCancelar = new StyledButton("Cancelar", ButtonStyle.Outline);
        btnCancelar.Click += (s, e) => dialog.Close();

        var btnGenerar = new StyledButton("📄 Generar PDF", ButtonStyle.Primary);
        btnGenerar.Click += async (s, e) =>
        {
            var estudianteSeleccionado = (Estudiante?)cmbEstudiante.SelectedItem;
            if (estudianteSeleccionado == null || estudianteSeleccionado.IdEstudiante == 0 || string.IsNullOrWhiteSpace(txtCiclo.Text))
            {
                errorText.Text = "⚠️ Debe seleccionar un estudiante y especificar el ciclo escolar";
                errorText.IsVisible = true;
                return;
            }

            var pdfBytes = await _apiService.DownloadPdfAsync($"/reportes/notas/{estudianteSeleccionado.IdEstudiante}?cicloEscolar={txtCiclo.Text}");
            
            if (pdfBytes != null && pdfBytes.Length > 0)
            {
                var saveDialog = new SaveFileDialog
                {
                    Title = "Guardar Reporte de Notas",
                    DefaultExtension = "pdf",
                    InitialFileName = $"Reporte_Notas_{estudianteSeleccionado.Nombres}_{estudianteSeleccionado.Apellidos}_{txtCiclo.Text}.pdf"
                };
                saveDialog.Filters.Add(new FileDialogFilter { Name = "PDF", Extensions = { "pdf" } });

                var result = await saveDialog.ShowAsync(dialog);
                if (!string.IsNullOrEmpty(result))
                {
                    await System.IO.File.WriteAllBytesAsync(result, pdfBytes);
                    dialog.Close();
                    await frontend.Utils.MessageBox.ShowInfoAsync(
                        TopLevel.GetTopLevel(this) as Window ?? new Window(),
                        "✅ Éxito",
                        "Reporte generado y guardado correctamente."
                    );
                }
            }
            else
            {
                errorText.Text = "❌ No se pudo generar el reporte. Verifique que el estudiante tenga calificaciones.";
                errorText.IsVisible = true;
            }
        };

        btnActions.Children.Add(btnCancelar);
        btnActions.Children.Add(btnGenerar);

        mainPanel.Children.Add(new StyledLabel("Estudiante", LabelStyle.Title));
        mainPanel.Children.Add(cmbEstudiante);
        mainPanel.Children.Add(new StyledLabel("Ciclo Escolar", LabelStyle.Title));
        mainPanel.Children.Add(txtCiclo);
        mainPanel.Children.Add(errorText);
        mainPanel.Children.Add(btnActions);

        dialog.Content = mainPanel;
        await dialog.ShowDialog(TopLevel.GetTopLevel(this) as Window ?? new Window());
    }
}