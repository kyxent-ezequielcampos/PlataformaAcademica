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

public class AsignaturasView : StackPanel
{
    private readonly ApiService _apiService;
    private readonly StackPanel _listaAsignaturas;
    private readonly Border _formContainer;
    private bool _isEditing = false;
    private int _editingId = 0;

    public AsignaturasView()
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
            Text = "📖 Gestión de Asignaturas",
            FontSize = 32,
            FontWeight = FontWeight.ExtraBold,
            Foreground = new LinearGradientBrush
            {
                StartPoint = new Avalonia.RelativePoint(0, 0, Avalonia.RelativeUnit.Relative),
                EndPoint = new Avalonia.RelativePoint(1, 0, Avalonia.RelativeUnit.Relative),
                GradientStops = new GradientStops
                {
                    new GradientStop(Color.Parse("#A78BFA"), 0),
                    new GradientStop(Color.Parse("#8B5CF6"), 1)
                }
            },
            VerticalAlignment = VerticalAlignment.Center
        };
        Grid.SetColumn(title, 0);

        var btnNuevo = new StyledButton("+ Nueva Asignatura", ButtonStyle.Secondary);
        btnNuevo.Click += (s, e) => MostrarFormulario();
        Grid.SetColumn(btnNuevo, 1);

        header.Children.Add(title);
        header.Children.Add(btnNuevo);
        Children.Add(header);

        // Contenedor del formulario (inicialmente oculto)
        _formContainer = new StyledCard(padding: 32)
        {
            IsVisible = false
        };
        Children.Add(_formContainer);

        // Lista de asignaturas
        _listaAsignaturas = new StackPanel { Spacing = 12 };
        var scrollViewer = new ScrollViewer
        {
            Content = _listaAsignaturas,
            MaxHeight = 600
        };
        Children.Add(scrollViewer);

        CargarAsignaturas();
    }

    private async void CargarAsignaturas()
    {
        _listaAsignaturas.Children.Clear();

        var asignaturas = await _apiService.GetAsync<List<Asignatura>>("/asignaturas");

        if (asignaturas == null || !asignaturas.Any())
        {
            _listaAsignaturas.Children.Add(new TextBlock
            {
                Text = "No hay asignaturas registradas",
                FontSize = 14,
                Foreground = AppColors.TextSecondary
            });
            return;
        }

        foreach (var asignatura in asignaturas)
        {
            _listaAsignaturas.Children.Add(CrearCardAsignatura(asignatura));
        }
    }

    private Border CrearCardAsignatura(Asignatura asignatura)
    {
        var mainGrid = new Grid
        {
            ColumnDefinitions = new ColumnDefinitions("Auto, *, Auto"),
            RowDefinitions = new RowDefinitions("Auto, Auto")
        };

        // Icono de asignatura con gradiente
        var icon = new Border
        {
            Width = 56,
            Height = 56,
            CornerRadius = new Avalonia.CornerRadius(28),
            Background = new LinearGradientBrush
            {
                StartPoint = new Avalonia.RelativePoint(0, 0, Avalonia.RelativeUnit.Relative),
                EndPoint = new Avalonia.RelativePoint(1, 1, Avalonia.RelativeUnit.Relative),
                GradientStops = new GradientStops
                {
                    new GradientStop(Color.Parse("#A78BFA"), 0),
                    new GradientStop(Color.Parse("#8B5CF6"), 1)
                }
            },
            Child = new TextBlock
            {
                Text = "📚",
                FontSize = 26,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            }
        };
        Grid.SetColumn(icon, 0);
        Grid.SetRow(icon, 0);
        Grid.SetRowSpan(icon, 2);

        // Información principal
        var infoPanel = new StackPanel
        {
            Spacing = 8,
            Margin = new Avalonia.Thickness(16, 0, 0, 0),
            VerticalAlignment = VerticalAlignment.Center
        };

        var titlePanel = new StackPanel
        {
            Orientation = Orientation.Horizontal,
            Spacing = 12
        };

        titlePanel.Children.Add(new TextBlock
        {
            Text = asignatura.Nombre,
            FontSize = 19,
            FontWeight = FontWeight.ExtraBold,
            Foreground = AppColors.TextPrimary
        });

        titlePanel.Children.Add(new Border
        {
            Background = AppColors.SecondaryLight,
            CornerRadius = new Avalonia.CornerRadius(14),
            Padding = new Avalonia.Thickness(10, 5),
            Child = new TextBlock
            {
                Text = asignatura.Codigo,
                FontSize = 12,
                FontWeight = FontWeight.SemiBold,
                Foreground = AppColors.Secondary
            }
        });

        infoPanel.Children.Add(titlePanel);

        var detailsGrid = new Grid
        {
            ColumnDefinitions = new ColumnDefinitions("Auto, 20, Auto"),
            RowDefinitions = new RowDefinitions("Auto, Auto")
        };

        // Primera fila de detalles
        var creditosPanel = new StackPanel
        {
            Orientation = Orientation.Horizontal,
            Spacing = 6
        };
        creditosPanel.Children.Add(new TextBlock
        {
            Text = "🎯",
            FontSize = 14,
            VerticalAlignment = VerticalAlignment.Center
        });
        creditosPanel.Children.Add(new TextBlock
        {
            Text = $"{asignatura.Creditos} créditos",
            FontSize = 14,
            FontWeight = FontWeight.Medium,
            Foreground = AppColors.Secondary,
            VerticalAlignment = VerticalAlignment.Center
        });
        Grid.SetColumn(creditosPanel, 0);
        Grid.SetRow(creditosPanel, 0);

        var statusPanel = new StackPanel
        {
            Orientation = Orientation.Horizontal,
            Spacing = 6
        };
        statusPanel.Children.Add(new TextBlock
        {
            Text = "✅",
            FontSize = 14,
            VerticalAlignment = VerticalAlignment.Center
        });
        statusPanel.Children.Add(new TextBlock
        {
            Text = "Activa",
            FontSize = 14,
            Foreground = AppColors.Success,
            FontWeight = FontWeight.Medium,
            VerticalAlignment = VerticalAlignment.Center
        });
        Grid.SetColumn(statusPanel, 2);
        Grid.SetRow(statusPanel, 0);

        // Segunda fila - Descripción
        if (!string.IsNullOrEmpty(asignatura.Descripcion))
        {
            var descripcionText = new TextBlock
            {
                Text = asignatura.Descripcion.Length > 100 
                    ? asignatura.Descripcion.Substring(0, 100) + "..."
                    : asignatura.Descripcion,
                FontSize = 13,
                Foreground = AppColors.TextSecondary,
                TextWrapping = TextWrapping.Wrap,
                MaxWidth = 400
            };
            Grid.SetColumn(descripcionText, 0);
            Grid.SetRow(descripcionText, 1);
            Grid.SetColumnSpan(descripcionText, 3);
            detailsGrid.Children.Add(descripcionText);
        }

        detailsGrid.Children.Add(creditosPanel);
        detailsGrid.Children.Add(statusPanel);

        infoPanel.Children.Add(detailsGrid);

        Grid.SetColumn(infoPanel, 1);
        Grid.SetRow(infoPanel, 0);
        Grid.SetRowSpan(infoPanel, 2);

        // Botones de acción
        var actions = new StackPanel
        {
            Orientation = Orientation.Horizontal,
            Spacing = 8,
            VerticalAlignment = VerticalAlignment.Center
        };

        var btnEditar = new StyledIconButton("✏️", IconButtonStyle.Secondary);
        var btnEliminar = new StyledIconButton("🗑️", IconButtonStyle.Danger);

        btnEditar.Click += (s, e) => EditarAsignatura(asignatura);

        btnEliminar.Click += async (s, e) =>
        {
            var confirm = await frontend.Utils.MessageBox.ShowConfirmAsync(
                TopLevel.GetTopLevel(this) as Window ?? new Window(),
                "Confirmar eliminación",
                $"¿Está seguro de que desea eliminar la asignatura {asignatura.Nombre}?"
            );

            if (confirm)
            {
                var success = await _apiService.DeleteAsync($"/asignaturas/{asignatura.IdAsignatura}");
                if (success) 
                {
                    CargarAsignaturas();
                    await frontend.Utils.MessageBox.ShowInfoAsync(
                        TopLevel.GetTopLevel(this) as Window ?? new Window(),
                        "✅ Éxito",
                        "Asignatura eliminada correctamente."
                    );
                }
                else
                {
                    await frontend.Utils.MessageBox.ShowInfoAsync(
                        TopLevel.GetTopLevel(this) as Window ?? new Window(),
                        "❌ Error",
                        "No se pudo eliminar la asignatura."
                    );
                }
            }
        };

        actions.Children.Add(btnEditar);
        actions.Children.Add(btnEliminar);

        Grid.SetColumn(actions, 2);
        Grid.SetRow(actions, 0);
        Grid.SetRowSpan(actions, 2);

        mainGrid.Children.Add(icon);
        mainGrid.Children.Add(infoPanel);
        mainGrid.Children.Add(actions);

        return new StyledCard(mainGrid, 24)
        {
            Margin = new Avalonia.Thickness(0, 0, 0, 12)
        };
    }

    private void MostrarFormulario(Asignatura? asignatura = null)
    {
        _isEditing = asignatura != null;
        _editingId = asignatura?.IdAsignatura ?? 0;

        var scrollViewer = new ScrollViewer
        {
            MaxHeight = 600,
            HorizontalScrollBarVisibility = Avalonia.Controls.Primitives.ScrollBarVisibility.Disabled,
            VerticalScrollBarVisibility = Avalonia.Controls.Primitives.ScrollBarVisibility.Auto
        };

        var form = new StackPanel { Spacing = 20 };

        // Header del formulario
        var headerPanel = new StackPanel
        {
            Spacing = 8,
            Margin = new Avalonia.Thickness(0, 0, 0, 20)
        };

        headerPanel.Children.Add(new TextBlock
        {
            Text = _isEditing ? "✏️ Editar Asignatura" : "➕ Nueva Asignatura",
            FontSize = 26,
            FontWeight = FontWeight.ExtraBold,
            Foreground = new LinearGradientBrush
            {
                StartPoint = new Avalonia.RelativePoint(0, 0, Avalonia.RelativeUnit.Relative),
                EndPoint = new Avalonia.RelativePoint(1, 0, Avalonia.RelativeUnit.Relative),
                GradientStops = new GradientStops
                {
                    new GradientStop(Color.Parse("#A78BFA"), 0),
                    new GradientStop(Color.Parse("#8B5CF6"), 1)
                }
            }
        });

        headerPanel.Children.Add(new TextBlock
        {
            Text = "Complete la información de la asignatura",
            FontSize = 14,
            Foreground = AppColors.TextSecondary
        });

        form.Children.Add(headerPanel);

        // Grid para organizar los campos
        var fieldsGrid = new Grid
        {
            ColumnDefinitions = new ColumnDefinitions("*, 20, *"),
            RowDefinitions = new RowDefinitions("Auto, Auto, Auto")
        };

        // TEXTBOX CODIGO
        var txtCodigo = new TextBox
        {
            Watermark = "Código de la asignatura",
            Text = asignatura?.Codigo ?? "",
            Height = 50,
            FontSize = 15,
            FontWeight = FontWeight.Medium,
            Padding = new Avalonia.Thickness(16, 14),
            CornerRadius = new Avalonia.CornerRadius(12),
            BorderThickness = new Avalonia.Thickness(2),
            Background = Brushes.White,
            Foreground = Brushes.Black,
            BorderBrush = new SolidColorBrush(Color.Parse("#E5E7EB")),
            CaretBrush = new SolidColorBrush(Color.Parse("#FF6B9D")),
            SelectionBrush = new SolidColorBrush(Color.Parse("#FFE5EF")),
            SelectionForegroundBrush = Brushes.Black
        };
        txtCodigo.GotFocus += (s, e) => { txtCodigo.BorderBrush = new SolidColorBrush(Color.Parse("#FF6B9D")); txtCodigo.BorderThickness = new Avalonia.Thickness(3); txtCodigo.Background = Brushes.White; txtCodigo.Foreground = Brushes.Black; };
        txtCodigo.LostFocus += (s, e) => { txtCodigo.BorderBrush = new SolidColorBrush(Color.Parse("#E5E7EB")); txtCodigo.BorderThickness = new Avalonia.Thickness(2); txtCodigo.Background = Brushes.White; txtCodigo.Foreground = Brushes.Black; };

        // TEXTBOX NOMBRE
        var txtNombre = new TextBox
        {
            Watermark = "Nombre de la asignatura",
            Text = asignatura?.Nombre ?? "",
            Height = 50,
            FontSize = 15,
            FontWeight = FontWeight.Medium,
            Padding = new Avalonia.Thickness(16, 14),
            CornerRadius = new Avalonia.CornerRadius(12),
            BorderThickness = new Avalonia.Thickness(2),
            Background = Brushes.White,
            Foreground = Brushes.Black,
            BorderBrush = new SolidColorBrush(Color.Parse("#E5E7EB")),
            CaretBrush = new SolidColorBrush(Color.Parse("#FF6B9D")),
            SelectionBrush = new SolidColorBrush(Color.Parse("#FFE5EF")),
            SelectionForegroundBrush = Brushes.Black
        };
        txtNombre.GotFocus += (s, e) => { txtNombre.BorderBrush = new SolidColorBrush(Color.Parse("#FF6B9D")); txtNombre.BorderThickness = new Avalonia.Thickness(3); txtNombre.Background = Brushes.White; txtNombre.Foreground = Brushes.Black; };
        txtNombre.LostFocus += (s, e) => { txtNombre.BorderBrush = new SolidColorBrush(Color.Parse("#E5E7EB")); txtNombre.BorderThickness = new Avalonia.Thickness(2); txtNombre.Background = Brushes.White; txtNombre.Foreground = Brushes.Black; };

        // TEXTBOX DESCRIPCION (multilinea)
        var txtDescripcion = new TextBox
        {
            Watermark = "Descripción detallada de la asignatura",
            Text = asignatura?.Descripcion ?? "",
            Height = 100,
            FontSize = 15,
            FontWeight = FontWeight.Medium,
            Padding = new Avalonia.Thickness(16, 14),
            CornerRadius = new Avalonia.CornerRadius(12),
            BorderThickness = new Avalonia.Thickness(2),
            Background = Brushes.White,
            Foreground = Brushes.Black,
            BorderBrush = new SolidColorBrush(Color.Parse("#E5E7EB")),
            CaretBrush = new SolidColorBrush(Color.Parse("#FF6B9D")),
            SelectionBrush = new SolidColorBrush(Color.Parse("#FFE5EF")),
            SelectionForegroundBrush = Brushes.Black,
            TextWrapping = TextWrapping.Wrap,
            AcceptsReturn = true,
            VerticalContentAlignment = Avalonia.Layout.VerticalAlignment.Top
        };
        txtDescripcion.GotFocus += (s, e) => { txtDescripcion.BorderBrush = new SolidColorBrush(Color.Parse("#FF6B9D")); txtDescripcion.BorderThickness = new Avalonia.Thickness(3); txtDescripcion.Background = Brushes.White; txtDescripcion.Foreground = Brushes.Black; };
        txtDescripcion.LostFocus += (s, e) => { txtDescripcion.BorderBrush = new SolidColorBrush(Color.Parse("#E5E7EB")); txtDescripcion.BorderThickness = new Avalonia.Thickness(2); txtDescripcion.Background = Brushes.White; txtDescripcion.Foreground = Brushes.Black; };

        // NUMERICUPDOWN CREDITOS
        var numCreditos = new NumericUpDown
        {
            Minimum = 1,
            Maximum = 10,
            Value = asignatura?.Creditos ?? 1,
            Height = 50,
            FontSize = 15,
            FontWeight = FontWeight.Medium,
            Padding = new Avalonia.Thickness(16, 0),
            CornerRadius = new Avalonia.CornerRadius(12),
            BorderThickness = new Avalonia.Thickness(2),
            Background = Brushes.White,
            Foreground = Brushes.Black,
            BorderBrush = new SolidColorBrush(Color.Parse("#E5E7EB")),
            ShowButtonSpinner = true,
            AllowSpin = true,
            ButtonSpinnerLocation = Location.Right
        };
        numCreditos.GotFocus += (s, e) => { numCreditos.BorderBrush = new SolidColorBrush(Color.Parse("#FF6B9D")); numCreditos.BorderThickness = new Avalonia.Thickness(3); numCreditos.Background = Brushes.White; numCreditos.Foreground = Brushes.Black; };
        numCreditos.LostFocus += (s, e) => { numCreditos.BorderBrush = new SolidColorBrush(Color.Parse("#E5E7EB")); numCreditos.BorderThickness = new Avalonia.Thickness(2); numCreditos.Background = Brushes.White; numCreditos.Foreground = Brushes.Black; };

        // Primera fila - Código y Créditos
        var lblCodigo = new StyledLabel("Código *", LabelStyle.Title);
        Grid.SetColumn(lblCodigo, 0);
        Grid.SetRow(lblCodigo, 0);
        fieldsGrid.Children.Add(lblCodigo);

        Grid.SetColumn(txtCodigo, 0);
        Grid.SetRow(txtCodigo, 0);
        txtCodigo.Margin = new Avalonia.Thickness(0, 25, 0, 15);
        fieldsGrid.Children.Add(txtCodigo);

        var lblCreditos = new StyledLabel("Créditos *", LabelStyle.Title);
        Grid.SetColumn(lblCreditos, 2);
        Grid.SetRow(lblCreditos, 0);
        fieldsGrid.Children.Add(lblCreditos);

        Grid.SetColumn(numCreditos, 2);
        Grid.SetRow(numCreditos, 0);
        numCreditos.Margin = new Avalonia.Thickness(0, 25, 0, 15);
        fieldsGrid.Children.Add(numCreditos);

        // Segunda fila - Nombre (span completo)
        var lblNombre = new StyledLabel("Nombre *", LabelStyle.Title);
        Grid.SetColumn(lblNombre, 0);
        Grid.SetRow(lblNombre, 1);
        fieldsGrid.Children.Add(lblNombre);

        Grid.SetColumn(txtNombre, 0);
        Grid.SetRow(txtNombre, 1);
        Grid.SetColumnSpan(txtNombre, 3);
        txtNombre.Margin = new Avalonia.Thickness(0, 25, 0, 15);
        fieldsGrid.Children.Add(txtNombre);

        // Tercera fila - Descripción (span completo)
        var lblDescripcion = new StyledLabel("Descripción", LabelStyle.Title);
        Grid.SetColumn(lblDescripcion, 0);
        Grid.SetRow(lblDescripcion, 2);
        fieldsGrid.Children.Add(lblDescripcion);

        Grid.SetColumn(txtDescripcion, 0);
        Grid.SetRow(txtDescripcion, 2);
        Grid.SetColumnSpan(txtDescripcion, 3);
        txtDescripcion.Margin = new Avalonia.Thickness(0, 25, 0, 15);
        fieldsGrid.Children.Add(txtDescripcion);

        form.Children.Add(fieldsGrid);

        // Mensaje de error
        var errorText = new TextBlock
        {
            Foreground = AppColors.Danger,
            FontSize = 14,
            FontWeight = FontWeight.Medium,
            IsVisible = false,
            Margin = new Avalonia.Thickness(0, 10, 0, 0),
            Padding = new Avalonia.Thickness(12, 8),
            Background = new SolidColorBrush(Color.Parse("#fef2f2"))
        };
        form.Children.Add(errorText);

        // Botones de acción
        var btnActions = new StackPanel
        {
            Orientation = Orientation.Horizontal,
            Spacing = 12,
            HorizontalAlignment = HorizontalAlignment.Right,
            Margin = new Avalonia.Thickness(0, 20, 0, 0)
        };

        var btnCancelar = new StyledButton("Cancelar", ButtonStyle.Outline)
        {
            Width = 130
        };

        var btnGuardar = new StyledButton(_isEditing ? "📚 Actualizar" : "📚 Guardar", ButtonStyle.Secondary)
        {
            Width = 160
        };

        btnCancelar.Click += (s, e) => _formContainer.IsVisible = false;

        btnGuardar.Click += async (s, e) =>
        {
            errorText.IsVisible = false;
            btnGuardar.IsEnabled = false;
            btnGuardar.Content = "⏳ Guardando...";

            try
            {
                if (string.IsNullOrWhiteSpace(txtCodigo.Text) ||
                    string.IsNullOrWhiteSpace(txtNombre.Text))
                {
                    errorText.Text = "⚠️ Los campos marcados con * son obligatorios";
                    errorText.IsVisible = true;
                    return;
                }

                var data = new
                {
                    codigo = txtCodigo.Text?.Trim(),
                    nombre = txtNombre.Text?.Trim(),
                    descripcion = txtDescripcion.Text?.Trim(),
                    creditos = (int)(numCreditos.Value ?? 1)
                };

                bool success;
                if (_isEditing)
                {
                    success = await _apiService.PutAsync($"/asignaturas/{_editingId}", data);
                }
                else
                {
                    var result = await _apiService.PostAsync<object, object>("/asignaturas", data);
                    success = result != null;
                }

                if (success)
                {
                    _formContainer.IsVisible = false;
                    CargarAsignaturas();
                    
                    await frontend.Utils.MessageBox.ShowInfoAsync(
                        TopLevel.GetTopLevel(this) as Window ?? new Window(),
                        "✅ Éxito",
                        $"Asignatura {(_isEditing ? "actualizada" : "creada")} correctamente."
                    );
                }
                else
                {
                    errorText.Text = "❌ Error al guardar la asignatura. Intente nuevamente.";
                    errorText.IsVisible = true;
                }
            }
            catch (Exception ex)
            {
                errorText.Text = $"❌ Error inesperado: {ex.Message}";
                errorText.IsVisible = true;
            }
            finally
            {
                btnGuardar.IsEnabled = true;
                btnGuardar.Content = _isEditing ? "📚 Actualizar" : "📚 Guardar";
            }
        };

        btnActions.Children.Add(btnCancelar);
        btnActions.Children.Add(btnGuardar);
        form.Children.Add(btnActions);

        scrollViewer.Content = form;
        _formContainer.Child = scrollViewer;
        _formContainer.IsVisible = true;
    }

    private void EditarAsignatura(Asignatura asignatura)
    {
        MostrarFormulario(asignatura);
    }

}
