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

public class GradosView : StackPanel
{
    private readonly ApiService _apiService;
    private readonly StackPanel _listaGrados;
    private readonly Border _formContainer;
    private bool _isEditing = false;
    private int _editingId = 0;

    public GradosView()
    {
        Spacing = 20;
        _apiService = new ApiService();

        // Header
        var header = new Grid
        {
            ColumnDefinitions = new ColumnDefinitions("*, Auto")
        };

        var title = new TextBlock
        {
            Text = "🎓 Gestión de Grados",
            FontSize = 32,
            FontWeight = FontWeight.ExtraBold,
            Foreground = new LinearGradientBrush
            {
                StartPoint = new Avalonia.RelativePoint(0, 0, Avalonia.RelativeUnit.Relative),
                EndPoint = new Avalonia.RelativePoint(1, 0, Avalonia.RelativeUnit.Relative),
                GradientStops = new GradientStops
                {
                    new GradientStop(Color.Parse("#6366F1"), 0),
                    new GradientStop(Color.Parse("#8B5CF6"), 1)
                }
            },
            VerticalAlignment = VerticalAlignment.Center
        };
        Grid.SetColumn(title, 0);

        var btnNuevo = new StyledButton("+ Nuevo Grado", ButtonStyle.Primary);
        btnNuevo.Click += (s, e) => MostrarFormulario();
        Grid.SetColumn(btnNuevo, 1);

        header.Children.Add(title);
        header.Children.Add(btnNuevo);
        Children.Add(header);

        // Contenedor del formulario
        _formContainer = new StyledCard(padding: 32)
        {
            IsVisible = false
        };
        Children.Add(_formContainer);

        // Lista de grados
        _listaGrados = new StackPanel { Spacing = 12 };
        var scrollViewer = new ScrollViewer
        {
            Content = _listaGrados,
            MaxHeight = 600
        };
        Children.Add(scrollViewer);

        CargarGrados();
    }

    private async void CargarGrados()
    {
        _listaGrados.Children.Clear();

        var grados = await _apiService.GetAsync<List<Grado>>("/grados");

        if (grados == null || !grados.Any())
        {
            _listaGrados.Children.Add(new TextBlock
            {
                Text = "No hay grados registrados",
                FontSize = 14,
                Foreground = AppColors.TextSecondary
            });
            return;
        }

        foreach (var grado in grados)
        {
            _listaGrados.Children.Add(CrearCardGrado(grado));
        }
    }

    private Border CrearCardGrado(Grado grado)
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
                    new GradientStop(Color.Parse("#6366F1"), 0),
                    new GradientStop(Color.Parse("#8B5CF6"), 1)
                }
            },
            Child = new TextBlock
            {
                Text = "🎓",
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
            Text = grado.Nombre,
            FontSize = 18,
            FontWeight = FontWeight.Bold,
            Foreground = AppColors.TextPrimary
        });

        var detailsText = new TextBlock
        {
            FontSize = 14,
            Foreground = AppColors.TextSecondary
        };

        var details = new List<string>();
        if (!string.IsNullOrEmpty(grado.Nivel)) details.Add($"📚 {grado.Nivel}");
        if (!string.IsNullOrEmpty(grado.Seccion)) details.Add($"📋 Sección {grado.Seccion}");
        detailsText.Text = string.Join(" • ", details);
        infoPanel.Children.Add(detailsText);

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

        btnEditar.Click += (s, e) => EditarGrado(grado);
        btnEliminar.Click += async (s, e) =>
        {
            var confirm = await frontend.Utils.MessageBox.ShowConfirmAsync(
                TopLevel.GetTopLevel(this) as Window ?? new Window(),
                "Confirmar eliminación",
                $"¿Está seguro de que desea eliminar el grado {grado.Nombre}?"
            );

            if (confirm)
            {
                var success = await _apiService.DeleteAsync($"/grados/{grado.IdGrado}");
                if (success)
                {
                    CargarGrados();
                    await frontend.Utils.MessageBox.ShowInfoAsync(
                        TopLevel.GetTopLevel(this) as Window ?? new Window(),
                        "✅ Éxito",
                        "Grado eliminado correctamente."
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

    private void MostrarFormulario(Grado? grado = null)
    {
        _isEditing = grado != null;
        _editingId = grado?.IdGrado ?? 0;

        var form = new StackPanel { Spacing = 20 };

        form.Children.Add(new TextBlock
        {
            Text = _isEditing ? "✏️ Editar Grado" : "➕ Nuevo Grado",
            FontSize = 24,
            FontWeight = FontWeight.Bold,
            Foreground = new LinearGradientBrush
            {
                StartPoint = new Avalonia.RelativePoint(0, 0, Avalonia.RelativeUnit.Relative),
                EndPoint = new Avalonia.RelativePoint(1, 0, Avalonia.RelativeUnit.Relative),
                GradientStops = new GradientStops
                {
                    new GradientStop(Color.Parse("#6366F1"), 0),
                    new GradientStop(Color.Parse("#8B5CF6"), 1)
                }
            }
        });

        var txtNombre = new TextBox
        {
            Watermark = "Nombre del grado *",
            Text = grado?.Nombre ?? "",
            Height = 45,
            FontSize = 14
        };

        var txtNivel = new TextBox
        {
            Watermark = "Nivel (ej: Primaria, Secundaria)",
            Text = grado?.Nivel ?? "",
            Height = 45,
            FontSize = 14
        };

        var txtSeccion = new TextBox
        {
            Watermark = "Sección (ej: A, B, C)",
            Text = grado?.Seccion ?? "",
            Height = 45,
            FontSize = 14
        };

        form.Children.Add(new StyledLabel("Nombre *", LabelStyle.Title));
        form.Children.Add(txtNombre);
        form.Children.Add(new StyledLabel("Nivel", LabelStyle.Title));
        form.Children.Add(txtNivel);
        form.Children.Add(new StyledLabel("Sección", LabelStyle.Title));
        form.Children.Add(txtSeccion);

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
            if (string.IsNullOrWhiteSpace(txtNombre.Text))
            {
                errorText.Text = "⚠️ El nombre es obligatorio";
                errorText.IsVisible = true;
                return;
            }

            var data = new
            {
                nombre = txtNombre.Text?.Trim(),
                nivel = txtNivel.Text?.Trim(),
                seccion = txtSeccion.Text?.Trim()
            };

            bool success;
            if (_isEditing)
            {
                success = await _apiService.PutAsync($"/grados/{_editingId}", data);
            }
            else
            {
                var result = await _apiService.PostAsync<object, object>("/grados", data);
                success = result != null;
            }

            if (success)
            {
                _formContainer.IsVisible = false;
                CargarGrados();
                await frontend.Utils.MessageBox.ShowInfoAsync(
                    TopLevel.GetTopLevel(this) as Window ?? new Window(),
                    "✅ Éxito",
                    $"Grado {(_isEditing ? "actualizado" : "creado")} correctamente."
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

    private void EditarGrado(Grado grado)
    {
        MostrarFormulario(grado);
    }
}
