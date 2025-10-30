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

public class EstudiantesView : StackPanel
{
    private readonly ApiService _apiService;
    private readonly StackPanel _listaEstudiantes;
    private readonly Border _formContainer;
    private bool _isEditing = false;
    private int _editingId = 0;

    public EstudiantesView()
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
            Text = "👨‍🎓 Gestión de Estudiantes",
            FontSize = 32,
            FontWeight = FontWeight.ExtraBold,
            Foreground = new LinearGradientBrush
            {
                StartPoint = new Avalonia.RelativePoint(0, 0, Avalonia.RelativeUnit.Relative),
                EndPoint = new Avalonia.RelativePoint(1, 0, Avalonia.RelativeUnit.Relative),
                GradientStops = new GradientStops
                {
                    new GradientStop(Color.Parse("#FF6B9D"), 0),
                    new GradientStop(Color.Parse("#FF4D88"), 1)
                }
            },
            VerticalAlignment = VerticalAlignment.Center
        };
        Grid.SetColumn(title, 0);

        var btnNuevo = new StyledButton("+ Nuevo Estudiante", ButtonStyle.Primary);
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

        // Lista de estudiantes
        _listaEstudiantes = new StackPanel { Spacing = 12 };
        var scrollViewer = new ScrollViewer
        {
            Content = _listaEstudiantes,
            MaxHeight = 600
        };
        Children.Add(scrollViewer);

        CargarEstudiantes();
    }

    private async void CargarEstudiantes()
    {
        _listaEstudiantes.Children.Clear();

        var estudiantes = await _apiService.GetAsync<List<Estudiante>>("/estudiantes");

        if (estudiantes == null || !estudiantes.Any())
        {
            _listaEstudiantes.Children.Add(new TextBlock
            {
                Text = "No hay estudiantes registrados",
                FontSize = 14,
                Foreground = AppColors.TextSecondary
            });
            return;
        }

        foreach (var estudiante in estudiantes)
        {
            _listaEstudiantes.Children.Add(CrearCardEstudiante(estudiante));
        }
    }

    private Border CrearCardEstudiante(Estudiante estudiante)
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
                    new GradientStop(Color.Parse("#FF6B9D"), 0),
                    new GradientStop(Color.Parse("#FF4D88"), 1)
                }
            },
            Child = new TextBlock
            {
                Text = $"{estudiante.Nombres.FirstOrDefault()}{estudiante.Apellidos.FirstOrDefault()}",
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

        infoPanel.Children.Add(new TextBlock
        {
            Text = $"{estudiante.Nombres} {estudiante.Apellidos}",
            FontSize = 19,
            FontWeight = FontWeight.ExtraBold,
            Foreground = AppColors.TextPrimary
        });

        var detailsGrid = new Grid
        {
            ColumnDefinitions = new ColumnDefinitions("Auto, 20, Auto"),
            RowDefinitions = new RowDefinitions("Auto, Auto")
        };

        // Primera fila de detalles
        var docIcon = new TextBlock
        {
            Text = "🆔",
            FontSize = 14,
            VerticalAlignment = VerticalAlignment.Center
        };
        Grid.SetColumn(docIcon, 0);
        Grid.SetRow(docIcon, 0);

        var docText = new TextBlock
        {
            Text = estudiante.Documento,
            FontSize = 14,
            Foreground = AppColors.TextSecondary,
            Margin = new Avalonia.Thickness(6, 0, 0, 0),
            VerticalAlignment = VerticalAlignment.Center
        };
        Grid.SetColumn(docText, 0);
        Grid.SetRow(docText, 0);

        var emailIcon = new TextBlock
        {
            Text = "📧",
            FontSize = 14,
            VerticalAlignment = VerticalAlignment.Center
        };
        Grid.SetColumn(emailIcon, 2);
        Grid.SetRow(emailIcon, 0);

        var emailText = new TextBlock
        {
            Text = estudiante.Email ?? "Sin email",
            FontSize = 14,
            Foreground = AppColors.TextSecondary,
            Margin = new Avalonia.Thickness(6, 0, 0, 0),
            VerticalAlignment = VerticalAlignment.Center
        };
        Grid.SetColumn(emailText, 2);
        Grid.SetRow(emailText, 0);

        // Segunda fila de detalles
        var phoneIcon = new TextBlock
        {
            Text = "📞",
            FontSize = 14,
            VerticalAlignment = VerticalAlignment.Center
        };
        Grid.SetColumn(phoneIcon, 0);
        Grid.SetRow(phoneIcon, 1);

        var phoneText = new TextBlock
        {
            Text = estudiante.Telefono ?? "Sin teléfono",
            FontSize = 14,
            Foreground = AppColors.TextSecondary,
            Margin = new Avalonia.Thickness(6, 0, 0, 0),
            VerticalAlignment = VerticalAlignment.Center
        };
        Grid.SetColumn(phoneText, 0);
        Grid.SetRow(phoneText, 1);

        var ageIcon = new TextBlock
        {
            Text = "🎂",
            FontSize = 14,
            VerticalAlignment = VerticalAlignment.Center
        };
        Grid.SetColumn(ageIcon, 2);
        Grid.SetRow(ageIcon, 1);

        var age = DateTime.Now.Year - estudiante.FechaNacimiento.Year;
        var ageText = new TextBlock
        {
            Text = $"{age} años",
            FontSize = 14,
            Foreground = AppColors.TextSecondary,
            Margin = new Avalonia.Thickness(6, 0, 0, 0),
            VerticalAlignment = VerticalAlignment.Center
        };
        Grid.SetColumn(ageText, 2);
        Grid.SetRow(ageText, 1);

        detailsGrid.Children.Add(docIcon);
        detailsGrid.Children.Add(docText);
        detailsGrid.Children.Add(emailIcon);
        detailsGrid.Children.Add(emailText);
        detailsGrid.Children.Add(phoneIcon);
        detailsGrid.Children.Add(phoneText);
        detailsGrid.Children.Add(ageIcon);
        detailsGrid.Children.Add(ageText);

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

        var btnEditar = new StyledIconButton("✏️", IconButtonStyle.Primary);
        var btnEliminar = new StyledIconButton("🗑️", IconButtonStyle.Danger);

        btnEditar.Click += (s, e) => EditarEstudiante(estudiante);

        btnEliminar.Click += async (s, e) =>
        {
            var confirm = await frontend.Utils.MessageBox.ShowConfirmAsync(
                TopLevel.GetTopLevel(this) as Window ?? new Window(),
                "Confirmar eliminación",
                $"¿Está seguro de que desea eliminar al estudiante {estudiante.Nombres} {estudiante.Apellidos}?"
            );

            if (confirm)
            {
                var success = await _apiService.DeleteAsync($"/estudiantes/{estudiante.IdEstudiante}");
                if (success) 
                {
                    CargarEstudiantes();
                    await frontend.Utils.MessageBox.ShowInfoAsync(
                        TopLevel.GetTopLevel(this) as Window ?? new Window(),
                        "✅ Éxito",
                        "Estudiante eliminado correctamente."
                    );
                }
                else
                {
                    await frontend.Utils.MessageBox.ShowInfoAsync(
                        TopLevel.GetTopLevel(this) as Window ?? new Window(),
                        "❌ Error",
                        "No se pudo eliminar el estudiante."
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

    private void MostrarFormulario(Estudiante? estudiante = null)
    {
        _isEditing = estudiante != null;
        _editingId = estudiante?.IdEstudiante ?? 0;

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
            Text = _isEditing ? "✏️ Editar Estudiante" : "➕ Nuevo Estudiante",
            FontSize = 26,
            FontWeight = FontWeight.ExtraBold,
            Foreground = new LinearGradientBrush
            {
                StartPoint = new Avalonia.RelativePoint(0, 0, Avalonia.RelativeUnit.Relative),
                EndPoint = new Avalonia.RelativePoint(1, 0, Avalonia.RelativeUnit.Relative),
                GradientStops = new GradientStops
                {
                    new GradientStop(Color.Parse("#FF6B9D"), 0),
                    new GradientStop(Color.Parse("#FF4D88"), 1)
                }
            }
        });

        headerPanel.Children.Add(new TextBlock
        {
            Text = "Complete la información del estudiante",
            FontSize = 14,
            Foreground = AppColors.TextSecondary
        });

        form.Children.Add(headerPanel);

        // Grid para organizar los campos en dos columnas
        var fieldsGrid = new Grid
        {
            ColumnDefinitions = new ColumnDefinitions("*, 20, *"),
            RowDefinitions = new RowDefinitions("Auto, Auto, Auto, Auto, Auto")
        };

        // TEXTBOX DOCUMENTO
        var txtDocumento = new TextBox
        {
            Watermark = "Documento del estudiante",
            Text = estudiante?.Documento ?? "",
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
        txtDocumento.GotFocus += (s, e) =>
        {
            txtDocumento.BorderBrush = new SolidColorBrush(Color.Parse("#FF6B9D"));
            txtDocumento.BorderThickness = new Avalonia.Thickness(3);
            txtDocumento.Background = Brushes.White;
            txtDocumento.Foreground = Brushes.Black;
        };
        txtDocumento.LostFocus += (s, e) =>
        {
            txtDocumento.BorderBrush = new SolidColorBrush(Color.Parse("#E5E7EB"));
            txtDocumento.BorderThickness = new Avalonia.Thickness(2);
            txtDocumento.Background = Brushes.White;
            txtDocumento.Foreground = Brushes.Black;
        };

        // TEXTBOX NOMBRES
        var txtNombres = new TextBox
        {
            Watermark = "Nombres completos",
            Text = estudiante?.Nombres ?? "",
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
        txtNombres.GotFocus += (s, e) =>
        {
            txtNombres.BorderBrush = new SolidColorBrush(Color.Parse("#FF6B9D"));
            txtNombres.BorderThickness = new Avalonia.Thickness(3);
            txtNombres.Background = Brushes.White;
            txtNombres.Foreground = Brushes.Black;
        };
        txtNombres.LostFocus += (s, e) =>
        {
            txtNombres.BorderBrush = new SolidColorBrush(Color.Parse("#E5E7EB"));
            txtNombres.BorderThickness = new Avalonia.Thickness(2);
            txtNombres.Background = Brushes.White;
            txtNombres.Foreground = Brushes.Black;
        };

        // TEXTBOX APELLIDOS
        var txtApellidos = new TextBox
        {
            Watermark = "Apellidos completos",
            Text = estudiante?.Apellidos ?? "",
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
        txtApellidos.GotFocus += (s, e) =>
        {
            txtApellidos.BorderBrush = new SolidColorBrush(Color.Parse("#FF6B9D"));
            txtApellidos.BorderThickness = new Avalonia.Thickness(3);
            txtApellidos.Background = Brushes.White;
            txtApellidos.Foreground = Brushes.Black;
        };
        txtApellidos.LostFocus += (s, e) =>
        {
            txtApellidos.BorderBrush = new SolidColorBrush(Color.Parse("#E5E7EB"));
            txtApellidos.BorderThickness = new Avalonia.Thickness(2);
            txtApellidos.Background = Brushes.White;
            txtApellidos.Foreground = Brushes.Black;
        };

        // DATEPICKER FECHA
        var dateFecha = new DatePicker
        {
            SelectedDate = estudiante?.FechaNacimiento ?? System.DateTime.Now.AddYears(-18),
            Height = 50,
            FontSize = 15,
            FontWeight = FontWeight.Medium,
            Padding = new Avalonia.Thickness(16, 0),
            CornerRadius = new Avalonia.CornerRadius(12),
            BorderThickness = new Avalonia.Thickness(2),
            Background = Brushes.White,
            Foreground = Brushes.Black,
            BorderBrush = new SolidColorBrush(Color.Parse("#E5E7EB"))
        };
        dateFecha.GotFocus += (s, e) =>
        {
            dateFecha.BorderBrush = new SolidColorBrush(Color.Parse("#FF6B9D"));
            dateFecha.BorderThickness = new Avalonia.Thickness(3);
            dateFecha.Background = Brushes.White;
            dateFecha.Foreground = Brushes.Black;
        };
        dateFecha.LostFocus += (s, e) =>
        {
            dateFecha.BorderBrush = new SolidColorBrush(Color.Parse("#E5E7EB"));
            dateFecha.BorderThickness = new Avalonia.Thickness(2);
            dateFecha.Background = Brushes.White;
            dateFecha.Foreground = Brushes.Black;
        };

        // TEXTBOX TELEFONO
        var txtTelefono = new TextBox
        {
            Watermark = "Número de teléfono",
            Text = estudiante?.Telefono ?? "",
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
        txtTelefono.GotFocus += (s, e) =>
        {
            txtTelefono.BorderBrush = new SolidColorBrush(Color.Parse("#FF6B9D"));
            txtTelefono.BorderThickness = new Avalonia.Thickness(3);
            txtTelefono.Background = Brushes.White;
            txtTelefono.Foreground = Brushes.Black;
        };
        txtTelefono.LostFocus += (s, e) =>
        {
            txtTelefono.BorderBrush = new SolidColorBrush(Color.Parse("#E5E7EB"));
            txtTelefono.BorderThickness = new Avalonia.Thickness(2);
            txtTelefono.Background = Brushes.White;
            txtTelefono.Foreground = Brushes.Black;
        };

        // TEXTBOX DIRECCION
        var txtDireccion = new TextBox
        {
            Watermark = "Dirección completa",
            Text = estudiante?.Direccion ?? "",
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
        txtDireccion.GotFocus += (s, e) =>
        {
            txtDireccion.BorderBrush = new SolidColorBrush(Color.Parse("#FF6B9D"));
            txtDireccion.BorderThickness = new Avalonia.Thickness(3);
            txtDireccion.Background = Brushes.White;
            txtDireccion.Foreground = Brushes.Black;
        };
        txtDireccion.LostFocus += (s, e) =>
        {
            txtDireccion.BorderBrush = new SolidColorBrush(Color.Parse("#E5E7EB"));
            txtDireccion.BorderThickness = new Avalonia.Thickness(2);
            txtDireccion.Background = Brushes.White;
            txtDireccion.Foreground = Brushes.Black;
        };

        // TEXTBOX EMAIL
        var txtEmail = new TextBox
        {
            Watermark = "Correo electrónico",
            Text = estudiante?.Email ?? "",
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
        txtEmail.GotFocus += (s, e) =>
        {
            txtEmail.BorderBrush = new SolidColorBrush(Color.Parse("#FF6B9D"));
            txtEmail.BorderThickness = new Avalonia.Thickness(3);
            txtEmail.Background = Brushes.White;
            txtEmail.Foreground = Brushes.Black;
        };
        txtEmail.LostFocus += (s, e) =>
        {
            txtEmail.BorderBrush = new SolidColorBrush(Color.Parse("#E5E7EB"));
            txtEmail.BorderThickness = new Avalonia.Thickness(2);
            txtEmail.Background = Brushes.White;
            txtEmail.Foreground = Brushes.Black;
        };

        // Primera fila - Documento y Fecha
        var lblDocumento = new StyledLabel("Documento *", LabelStyle.Title);
        Grid.SetColumn(lblDocumento, 0);
        Grid.SetRow(lblDocumento, 0);
        fieldsGrid.Children.Add(lblDocumento);

        Grid.SetColumn(txtDocumento, 0);
        Grid.SetRow(txtDocumento, 0);
        txtDocumento.Margin = new Avalonia.Thickness(0, 25, 0, 15);
        fieldsGrid.Children.Add(txtDocumento);

        var lblFecha = new StyledLabel("Fecha de Nacimiento *", LabelStyle.Title);
        Grid.SetColumn(lblFecha, 2);
        Grid.SetRow(lblFecha, 0);
        fieldsGrid.Children.Add(lblFecha);

        Grid.SetColumn(dateFecha, 2);
        Grid.SetRow(dateFecha, 0);
        dateFecha.Margin = new Avalonia.Thickness(0, 25, 0, 15);
        fieldsGrid.Children.Add(dateFecha);

        // Segunda fila - Nombres y Teléfono
        var lblNombres = new StyledLabel("Nombres *", LabelStyle.Title);
        Grid.SetColumn(lblNombres, 0);
        Grid.SetRow(lblNombres, 1);
        fieldsGrid.Children.Add(lblNombres);

        Grid.SetColumn(txtNombres, 0);
        Grid.SetRow(txtNombres, 1);
        txtNombres.Margin = new Avalonia.Thickness(0, 25, 0, 15);
        fieldsGrid.Children.Add(txtNombres);

        var lblTelefono = new StyledLabel("Teléfono", LabelStyle.Title);
        Grid.SetColumn(lblTelefono, 2);
        Grid.SetRow(lblTelefono, 1);
        fieldsGrid.Children.Add(lblTelefono);

        Grid.SetColumn(txtTelefono, 2);
        Grid.SetRow(txtTelefono, 1);
        txtTelefono.Margin = new Avalonia.Thickness(0, 25, 0, 15);
        fieldsGrid.Children.Add(txtTelefono);

        // Tercera fila - Apellidos (span completo)
        var lblApellidos = new StyledLabel("Apellidos *", LabelStyle.Title);
        Grid.SetColumn(lblApellidos, 0);
        Grid.SetRow(lblApellidos, 2);
        fieldsGrid.Children.Add(lblApellidos);

        Grid.SetColumn(txtApellidos, 0);
        Grid.SetRow(txtApellidos, 2);
        Grid.SetColumnSpan(txtApellidos, 3);
        txtApellidos.Margin = new Avalonia.Thickness(0, 25, 0, 15);
        fieldsGrid.Children.Add(txtApellidos);

        // Cuarta fila - Dirección (span completo)
        var lblDireccion = new StyledLabel("Dirección", LabelStyle.Title);
        Grid.SetColumn(lblDireccion, 0);
        Grid.SetRow(lblDireccion, 3);
        fieldsGrid.Children.Add(lblDireccion);

        Grid.SetColumn(txtDireccion, 0);
        Grid.SetRow(txtDireccion, 3);
        Grid.SetColumnSpan(txtDireccion, 3);
        txtDireccion.Margin = new Avalonia.Thickness(0, 25, 0, 15);
        fieldsGrid.Children.Add(txtDireccion);

        // Quinta fila - Email (span completo)
        var lblEmail = new StyledLabel("Email", LabelStyle.Title);
        Grid.SetColumn(lblEmail, 0);
        Grid.SetRow(lblEmail, 4);
        fieldsGrid.Children.Add(lblEmail);

        Grid.SetColumn(txtEmail, 0);
        Grid.SetRow(txtEmail, 4);
        Grid.SetColumnSpan(txtEmail, 3);
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

        var btnGuardar = new StyledButton(_isEditing ? "💾 Actualizar" : "💾 Guardar", ButtonStyle.Primary)
        {
            Width = 150
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
                    fechaNacimiento = dateFecha.SelectedDate?.DateTime ?? System.DateTime.Now,
                    direccion = txtDireccion.Text?.Trim(),
                    telefono = txtTelefono.Text?.Trim(),
                    email = txtEmail.Text?.Trim()
                };

                bool success;
                if (_isEditing)
                {
                    success = await _apiService.PutAsync($"/estudiantes/{_editingId}", data);
                }
                else
                {
                    var result = await _apiService.PostAsync<object, object>("/estudiantes", data);
                    success = result != null;
                }

                if (success)
                {
                    _formContainer.IsVisible = false;
                    CargarEstudiantes();
                    
                    await frontend.Utils.MessageBox.ShowInfoAsync(
                        TopLevel.GetTopLevel(this) as Window ?? new Window(),
                        "✅ Éxito",
                        $"Estudiante {(_isEditing ? "actualizado" : "creado")} correctamente."
                    );
                }
                else
                {
                    errorText.Text = "❌ Error al guardar el estudiante. Intente nuevamente.";
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
                btnGuardar.Content = _isEditing ? "💾 Actualizar" : "💾 Guardar";
            }
        };

        btnActions.Children.Add(btnCancelar);
        btnActions.Children.Add(btnGuardar);
        form.Children.Add(btnActions);

        scrollViewer.Content = form;
        _formContainer.Child = scrollViewer;
        _formContainer.IsVisible = true;
    }

    private void EditarEstudiante(Estudiante estudiante)
    {
        MostrarFormulario(estudiante);
    }

}
