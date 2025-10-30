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

public class DocentesView : StackPanel
{
    private readonly ApiService _apiService;
    private readonly StackPanel _listaDocentes;
    private readonly Border _formContainer;
    private bool _isEditing = false;
    private int _editingId = 0;

    public DocentesView()
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
            Text = "👨‍🏫 Gestión de Docentes",
            FontSize = 32,
            FontWeight = FontWeight.ExtraBold,
            Foreground = new LinearGradientBrush
            {
                StartPoint = new Avalonia.RelativePoint(0, 0, Avalonia.RelativeUnit.Relative),
                EndPoint = new Avalonia.RelativePoint(1, 0, Avalonia.RelativeUnit.Relative),
                GradientStops = new GradientStops
                {
                    new GradientStop(Color.Parse("#4ADE80"), 0),
                    new GradientStop(Color.Parse("#22C55E"), 1)
                }
            },
            VerticalAlignment = VerticalAlignment.Center
        };
        Grid.SetColumn(title, 0);

        var btnNuevo = new StyledButton("+ Nuevo Docente", ButtonStyle.Success);
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

        // Lista de docentes
        _listaDocentes = new StackPanel { Spacing = 12 };
        var scrollViewer = new ScrollViewer
        {
            Content = _listaDocentes,
            MaxHeight = 600
        };
        Children.Add(scrollViewer);

        CargarDocentes();
    }

    private async void CargarDocentes()
    {
        _listaDocentes.Children.Clear();

        var docentes = await _apiService.GetAsync<List<Docente>>("/docentes");

        if (docentes == null || !docentes.Any())
        {
            _listaDocentes.Children.Add(new TextBlock
            {
                Text = "No hay docentes registrados",
                FontSize = 14,
                Foreground = AppColors.TextSecondary
            });
            return;
        }

        foreach (var docente in docentes)
        {
            _listaDocentes.Children.Add(CrearCardDocente(docente));
        }
    }

    private Border CrearCardDocente(Docente docente)
    {
        var mainGrid = new Grid
        {
            ColumnDefinitions = new ColumnDefinitions("Auto, *, Auto"),
            RowDefinitions = new RowDefinitions("Auto, Auto")
        };

        // Avatar/Icono con gradiente
        var avatar = new Border
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
                    new GradientStop(Color.Parse("#4ADE80"), 0),
                    new GradientStop(Color.Parse("#22C55E"), 1)
                }
            },
            Child = new TextBlock
            {
                Text = $"{docente.Nombres.FirstOrDefault()}{docente.Apellidos.FirstOrDefault()}",
                FontSize = 20,
                FontWeight = FontWeight.Bold,
                Foreground = Brushes.White,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            }
        };
        Grid.SetColumn(avatar, 0);
        Grid.SetRow(avatar, 0);
        Grid.SetRowSpan(avatar, 2);

        // Información principal
        var infoPanel = new StackPanel
        {
            Spacing = 6,
            Margin = new Avalonia.Thickness(16, 0, 0, 0),
            VerticalAlignment = VerticalAlignment.Center
        };

        var namePanel = new StackPanel
        {
            Orientation = Orientation.Horizontal,
            Spacing = 8
        };

        namePanel.Children.Add(new TextBlock
        {
            Text = $"{docente.Nombres} {docente.Apellidos}",
            FontSize = 19,
            FontWeight = FontWeight.ExtraBold,
            Foreground = AppColors.TextPrimary
        });

        if (!string.IsNullOrEmpty(docente.Especialidad))
        {
            namePanel.Children.Add(new Border
            {
                Background = AppColors.SuccessLight,
                CornerRadius = new Avalonia.CornerRadius(14),
                Padding = new Avalonia.Thickness(10, 5),
                Child = new TextBlock
                {
                    Text = docente.Especialidad,
                    FontSize = 12,
                    FontWeight = FontWeight.SemiBold,
                    Foreground = AppColors.Success
                }
            });
        }

        infoPanel.Children.Add(namePanel);

        var detailsGrid = new Grid
        {
            ColumnDefinitions = new ColumnDefinitions("Auto, 20, Auto"),
            RowDefinitions = new RowDefinitions("Auto, Auto")
        };

        // Primera fila de detalles
        var docPanel = new StackPanel
        {
            Orientation = Orientation.Horizontal,
            Spacing = 6
        };
        docPanel.Children.Add(new TextBlock
        {
            Text = "🆔",
            FontSize = 14,
            VerticalAlignment = VerticalAlignment.Center
        });
        docPanel.Children.Add(new TextBlock
        {
            Text = docente.Documento,
            FontSize = 14,
            Foreground = AppColors.TextSecondary,
            VerticalAlignment = VerticalAlignment.Center
        });
        Grid.SetColumn(docPanel, 0);
        Grid.SetRow(docPanel, 0);

        var emailPanel = new StackPanel
        {
            Orientation = Orientation.Horizontal,
            Spacing = 6
        };
        emailPanel.Children.Add(new TextBlock
        {
            Text = "📧",
            FontSize = 14,
            VerticalAlignment = VerticalAlignment.Center
        });
        emailPanel.Children.Add(new TextBlock
        {
            Text = docente.Email ?? "Sin email",
            FontSize = 14,
            Foreground = AppColors.TextSecondary,
            VerticalAlignment = VerticalAlignment.Center
        });
        Grid.SetColumn(emailPanel, 2);
        Grid.SetRow(emailPanel, 0);

        // Segunda fila de detalles
        var phonePanel = new StackPanel
        {
            Orientation = Orientation.Horizontal,
            Spacing = 6
        };
        phonePanel.Children.Add(new TextBlock
        {
            Text = "📞",
            FontSize = 14,
            VerticalAlignment = VerticalAlignment.Center
        });
        phonePanel.Children.Add(new TextBlock
        {
            Text = docente.Telefono ?? "Sin teléfono",
            FontSize = 14,
            Foreground = AppColors.TextSecondary,
            VerticalAlignment = VerticalAlignment.Center
        });
        Grid.SetColumn(phonePanel, 0);
        Grid.SetRow(phonePanel, 1);

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
            Text = "Activo",
            FontSize = 14,
            Foreground = AppColors.Success,
            FontWeight = FontWeight.Medium,
            VerticalAlignment = VerticalAlignment.Center
        });
        Grid.SetColumn(statusPanel, 2);
        Grid.SetRow(statusPanel, 1);

        detailsGrid.Children.Add(docPanel);
        detailsGrid.Children.Add(emailPanel);
        detailsGrid.Children.Add(phonePanel);
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

        var btnEditar = new StyledIconButton("✏️", IconButtonStyle.Success);
        var btnEliminar = new StyledIconButton("🗑️", IconButtonStyle.Danger);

        btnEditar.Click += (s, e) => EditarDocente(docente);

        btnEliminar.Click += async (s, e) =>
        {
            var confirm = await frontend.Utils.MessageBox.ShowConfirmAsync(
                TopLevel.GetTopLevel(this) as Window ?? new Window(),
                "Confirmar eliminación",
                $"¿Está seguro de que desea eliminar al docente {docente.Nombres} {docente.Apellidos}?"
            );

            if (confirm)
            {
                var success = await _apiService.DeleteAsync($"/docentes/{docente.IdDocente}");
                if (success) 
                {
                    CargarDocentes();
                    await frontend.Utils.MessageBox.ShowInfoAsync(
                        TopLevel.GetTopLevel(this) as Window ?? new Window(),
                        "✅ Éxito",
                        "Docente eliminado correctamente."
                    );
                }
                else
                {
                    await frontend.Utils.MessageBox.ShowInfoAsync(
                        TopLevel.GetTopLevel(this) as Window ?? new Window(),
                        "❌ Error",
                        "No se pudo eliminar el docente."
                    );
                }
            }
        };

        actions.Children.Add(btnEditar);
        actions.Children.Add(btnEliminar);

        Grid.SetColumn(actions, 2);
        Grid.SetRow(actions, 0);
        Grid.SetRowSpan(actions, 2);

        mainGrid.Children.Add(avatar);
        mainGrid.Children.Add(infoPanel);
        mainGrid.Children.Add(actions);

        return new StyledCard(mainGrid, 24)
        {
            Margin = new Avalonia.Thickness(0, 0, 0, 12)
        };
    }

    private void MostrarFormulario(Docente? docente = null)
    {
        _isEditing = docente != null;
        _editingId = docente?.IdDocente ?? 0;

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
            Text = _isEditing ? "✏️ Editar Docente" : "➕ Nuevo Docente",
            FontSize = 26,
            FontWeight = FontWeight.ExtraBold,
            Foreground = new LinearGradientBrush
            {
                StartPoint = new Avalonia.RelativePoint(0, 0, Avalonia.RelativeUnit.Relative),
                EndPoint = new Avalonia.RelativePoint(1, 0, Avalonia.RelativeUnit.Relative),
                GradientStops = new GradientStops
                {
                    new GradientStop(Color.Parse("#4ADE80"), 0),
                    new GradientStop(Color.Parse("#22C55E"), 1)
                }
            }
        });

        headerPanel.Children.Add(new TextBlock
        {
            Text = "Complete la información del docente",
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

        // TEXTBOX DOCUMENTO
        var txtDocumento = new TextBox
        {
            Watermark = "Documento del docente",
            Text = docente?.Documento ?? "",
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
        txtDocumento.GotFocus += (s, e) => { txtDocumento.BorderBrush = new SolidColorBrush(Color.Parse("#FF6B9D")); txtDocumento.BorderThickness = new Avalonia.Thickness(3); txtDocumento.Background = Brushes.White; txtDocumento.Foreground = Brushes.Black; };
        txtDocumento.LostFocus += (s, e) => { txtDocumento.BorderBrush = new SolidColorBrush(Color.Parse("#E5E7EB")); txtDocumento.BorderThickness = new Avalonia.Thickness(2); txtDocumento.Background = Brushes.White; txtDocumento.Foreground = Brushes.Black; };

        // TEXTBOX NOMBRES
        var txtNombres = new TextBox
        {
            Watermark = "Nombres completos",
            Text = docente?.Nombres ?? "",
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
        txtNombres.GotFocus += (s, e) => { txtNombres.BorderBrush = new SolidColorBrush(Color.Parse("#FF6B9D")); txtNombres.BorderThickness = new Avalonia.Thickness(3); txtNombres.Background = Brushes.White; txtNombres.Foreground = Brushes.Black; };
        txtNombres.LostFocus += (s, e) => { txtNombres.BorderBrush = new SolidColorBrush(Color.Parse("#E5E7EB")); txtNombres.BorderThickness = new Avalonia.Thickness(2); txtNombres.Background = Brushes.White; txtNombres.Foreground = Brushes.Black; };

        // TEXTBOX APELLIDOS
        var txtApellidos = new TextBox
        {
            Watermark = "Apellidos completos",
            Text = docente?.Apellidos ?? "",
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
        txtApellidos.GotFocus += (s, e) => { txtApellidos.BorderBrush = new SolidColorBrush(Color.Parse("#FF6B9D")); txtApellidos.BorderThickness = new Avalonia.Thickness(3); txtApellidos.Background = Brushes.White; txtApellidos.Foreground = Brushes.Black; };
        txtApellidos.LostFocus += (s, e) => { txtApellidos.BorderBrush = new SolidColorBrush(Color.Parse("#E5E7EB")); txtApellidos.BorderThickness = new Avalonia.Thickness(2); txtApellidos.Background = Brushes.White; txtApellidos.Foreground = Brushes.Black; };

        // TEXTBOX ESPECIALIDAD
        var txtEspecialidad = new TextBox
        {
            Watermark = "Especialidad o área",
            Text = docente?.Especialidad ?? "",
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
        txtEspecialidad.GotFocus += (s, e) => { txtEspecialidad.BorderBrush = new SolidColorBrush(Color.Parse("#FF6B9D")); txtEspecialidad.BorderThickness = new Avalonia.Thickness(3); txtEspecialidad.Background = Brushes.White; txtEspecialidad.Foreground = Brushes.Black; };
        txtEspecialidad.LostFocus += (s, e) => { txtEspecialidad.BorderBrush = new SolidColorBrush(Color.Parse("#E5E7EB")); txtEspecialidad.BorderThickness = new Avalonia.Thickness(2); txtEspecialidad.Background = Brushes.White; txtEspecialidad.Foreground = Brushes.Black; };

        // TEXTBOX TELEFONO
        var txtTelefono = new TextBox
        {
            Watermark = "Número de teléfono",
            Text = docente?.Telefono ?? "",
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
        txtTelefono.GotFocus += (s, e) => { txtTelefono.BorderBrush = new SolidColorBrush(Color.Parse("#FF6B9D")); txtTelefono.BorderThickness = new Avalonia.Thickness(3); txtTelefono.Background = Brushes.White; txtTelefono.Foreground = Brushes.Black; };
        txtTelefono.LostFocus += (s, e) => { txtTelefono.BorderBrush = new SolidColorBrush(Color.Parse("#E5E7EB")); txtTelefono.BorderThickness = new Avalonia.Thickness(2); txtTelefono.Background = Brushes.White; txtTelefono.Foreground = Brushes.Black; };

        // TEXTBOX EMAIL
        var txtEmail = new TextBox
        {
            Watermark = "Correo electrónico",
            Text = docente?.Email ?? "",
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
        txtEmail.GotFocus += (s, e) => { txtEmail.BorderBrush = new SolidColorBrush(Color.Parse("#FF6B9D")); txtEmail.BorderThickness = new Avalonia.Thickness(3); txtEmail.Background = Brushes.White; txtEmail.Foreground = Brushes.Black; };
        txtEmail.LostFocus += (s, e) => { txtEmail.BorderBrush = new SolidColorBrush(Color.Parse("#E5E7EB")); txtEmail.BorderThickness = new Avalonia.Thickness(2); txtEmail.Background = Brushes.White; txtEmail.Foreground = Brushes.Black; };

        // Primera fila - Documento y Nombres
        var lblDocumento = new StyledLabel("Documento *", LabelStyle.Title);
        Grid.SetColumn(lblDocumento, 0);
        Grid.SetRow(lblDocumento, 0);
        fieldsGrid.Children.Add(lblDocumento);

        Grid.SetColumn(txtDocumento, 0);
        Grid.SetRow(txtDocumento, 0);
        txtDocumento.Margin = new Avalonia.Thickness(0, 25, 0, 15);
        fieldsGrid.Children.Add(txtDocumento);

        var lblNombres = new StyledLabel("Nombres *", LabelStyle.Title);
        Grid.SetColumn(lblNombres, 2);
        Grid.SetRow(lblNombres, 0);
        fieldsGrid.Children.Add(lblNombres);

        Grid.SetColumn(txtNombres, 2);
        Grid.SetRow(txtNombres, 0);
        txtNombres.Margin = new Avalonia.Thickness(0, 25, 0, 15);
        fieldsGrid.Children.Add(txtNombres);

        // Segunda fila - Apellidos y Especialidad
        var lblApellidos = new StyledLabel("Apellidos *", LabelStyle.Title);
        Grid.SetColumn(lblApellidos, 0);
        Grid.SetRow(lblApellidos, 1);
        fieldsGrid.Children.Add(lblApellidos);

        Grid.SetColumn(txtApellidos, 0);
        Grid.SetRow(txtApellidos, 1);
        txtApellidos.Margin = new Avalonia.Thickness(0, 25, 0, 15);
        fieldsGrid.Children.Add(txtApellidos);

        var lblEspecialidad = new StyledLabel("Especialidad", LabelStyle.Title);
        Grid.SetColumn(lblEspecialidad, 2);
        Grid.SetRow(lblEspecialidad, 1);
        fieldsGrid.Children.Add(lblEspecialidad);

        Grid.SetColumn(txtEspecialidad, 2);
        Grid.SetRow(txtEspecialidad, 1);
        txtEspecialidad.Margin = new Avalonia.Thickness(0, 25, 0, 15);
        fieldsGrid.Children.Add(txtEspecialidad);

        // Tercera fila - Teléfono y Email
        var lblTelefono = new StyledLabel("Teléfono", LabelStyle.Title);
        Grid.SetColumn(lblTelefono, 0);
        Grid.SetRow(lblTelefono, 2);
        fieldsGrid.Children.Add(lblTelefono);

        Grid.SetColumn(txtTelefono, 0);
        Grid.SetRow(txtTelefono, 2);
        txtTelefono.Margin = new Avalonia.Thickness(0, 25, 0, 15);
        fieldsGrid.Children.Add(txtTelefono);

        var lblEmail = new StyledLabel("Email", LabelStyle.Title);
        Grid.SetColumn(lblEmail, 2);
        Grid.SetRow(lblEmail, 2);
        fieldsGrid.Children.Add(lblEmail);

        Grid.SetColumn(txtEmail, 2);
        Grid.SetRow(txtEmail, 2);
        txtEmail.Margin = new Avalonia.Thickness(0, 25, 0, 15);
        fieldsGrid.Children.Add(txtEmail);

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

        var btnGuardar = new StyledButton(_isEditing ? "👨‍🏫 Actualizar" : "👨‍🏫 Guardar", ButtonStyle.Success)
        {
            Width = 170
        };

        btnCancelar.Click += (s, e) => _formContainer.IsVisible = false;

        btnGuardar.Click += async (s, e) =>
        {
            errorText.IsVisible = false;
            btnGuardar.IsEnabled = false;
            btnGuardar.Content = "⏳ Guardando...";

            try
            {
                if (string.IsNullOrWhiteSpace(txtDocumento.Text) ||
                    string.IsNullOrWhiteSpace(txtNombres.Text) ||
                    string.IsNullOrWhiteSpace(txtApellidos.Text))
                {
                    errorText.Text = "⚠️ Los campos marcados con * son obligatorios";
                    errorText.IsVisible = true;
                    return;
                }

                var data = new
                {
                    documento = txtDocumento.Text?.Trim(),
                    nombres = txtNombres.Text?.Trim(),
                    apellidos = txtApellidos.Text?.Trim(),
                    especialidad = txtEspecialidad.Text?.Trim(),
                    telefono = txtTelefono.Text?.Trim(),
                    email = txtEmail.Text?.Trim()
                };

                bool success;
                if (_isEditing)
                {
                    success = await _apiService.PutAsync($"/docentes/{_editingId}", data);
                }
                else
                {
                    var result = await _apiService.PostAsync<object, object>("/docentes", data);
                    success = result != null;
                }

                if (success)
                {
                    _formContainer.IsVisible = false;
                    CargarDocentes();
                    
                    await frontend.Utils.MessageBox.ShowInfoAsync(
                        TopLevel.GetTopLevel(this) as Window ?? new Window(),
                        "✅ Éxito",
                        $"Docente {(_isEditing ? "actualizado" : "creado")} correctamente."
                    );
                }
                else
                {
                    errorText.Text = "❌ Error al guardar el docente. Intente nuevamente.";
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
                btnGuardar.Content = _isEditing ? "👨‍🏫 Actualizar" : "👨‍🏫 Guardar";
            }
        };

        btnActions.Children.Add(btnCancelar);
        btnActions.Children.Add(btnGuardar);
        form.Children.Add(btnActions);

        scrollViewer.Content = form;
        _formContainer.Child = scrollViewer;
        _formContainer.IsVisible = true;
    }

    private void EditarDocente(Docente docente)
    {
        MostrarFormulario(docente);
    }

}
