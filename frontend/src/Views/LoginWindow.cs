namespace frontend.Views;

using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using frontend.Elements;
using frontend.Models;
using frontend.Services;
using frontend.Config;
using System;

public class LoginWindow : Window
{
    private readonly TextBox _usernameBox;
    private readonly TextBox _passwordBox;
    private readonly TextBlock _errorText;
    private readonly ApiService _apiService;
    private readonly AuthService _authService;

    public LoginWindow()
    {
        Title = "Iniciar Sesión - Sistema Académico";
        WindowState = WindowState.Maximized;
        WindowStartupLocation = WindowStartupLocation.CenterScreen;
        CanResize = true;

        _apiService = new ApiService();
        _authService = new AuthService();

        // Fondo fullscreen con gradiente candy
        var backgroundGrid = new Grid
        {
            Background = new LinearGradientBrush
            {
                StartPoint = new Avalonia.RelativePoint(0, 0, Avalonia.RelativeUnit.Relative),
                EndPoint = new Avalonia.RelativePoint(1, 1, Avalonia.RelativeUnit.Relative),
                GradientStops = new GradientStops
                {
                    new GradientStop(Color.Parse("#FF6B9D"), 0),
                    new GradientStop(Color.Parse("#A78BFA"), 0.5),
                    new GradientStop(Color.Parse("#60A5FA"), 1)
                }
            }
        };

        var mainPanel = new StackPanel
        {
            VerticalAlignment = VerticalAlignment.Center,
            HorizontalAlignment = HorizontalAlignment.Center,
            Spacing = 0
        };

        // USERNAME TEXTBOX - FONDO BLANCO Y TEXTO NEGRO SIEMPRE
        _usernameBox = new TextBox
        {
            Watermark = "Ingrese su usuario",
            Height = 54,
            FontSize = 15,
            FontWeight = FontWeight.Medium,
            Padding = new Avalonia.Thickness(16, 14),
            Margin = new Avalonia.Thickness(0, 0, 0, 20),
            CornerRadius = new Avalonia.CornerRadius(12),
            BorderThickness = new Avalonia.Thickness(2),
            // COLORES FIJOS - NUNCA CAMBIAN
            Background = Brushes.White,
            Foreground = Brushes.Black,
            BorderBrush = new SolidColorBrush(Color.Parse("#E5E7EB")),
            CaretBrush = new SolidColorBrush(Color.Parse("#FF6B9D")),
            SelectionBrush = new SolidColorBrush(Color.Parse("#FFE5EF")),
            SelectionForegroundBrush = Brushes.Black
        };

        // Solo cambiar el borde en focus
        _usernameBox.GotFocus += (s, e) =>
        {
            _usernameBox.BorderBrush = new SolidColorBrush(Color.Parse("#FF6B9D"));
            _usernameBox.BorderThickness = new Avalonia.Thickness(3);
            // FORZAR colores para que no cambien
            _usernameBox.Background = Brushes.White;
            _usernameBox.Foreground = Brushes.Black;
        };

        _usernameBox.LostFocus += (s, e) =>
        {
            _usernameBox.BorderBrush = new SolidColorBrush(Color.Parse("#E5E7EB"));
            _usernameBox.BorderThickness = new Avalonia.Thickness(2);
            // FORZAR colores para que no cambien
            _usernameBox.Background = Brushes.White;
            _usernameBox.Foreground = Brushes.Black;
        };

        // PASSWORD TEXTBOX - FONDO BLANCO Y TEXTO NEGRO SIEMPRE
        _passwordBox = new TextBox
        {
            Watermark = "Ingrese su contraseña",
            PasswordChar = '•',
            Height = 54,
            FontSize = 15,
            FontWeight = FontWeight.Medium,
            Padding = new Avalonia.Thickness(16, 14),
            Margin = new Avalonia.Thickness(0, 0, 0, 20),
            CornerRadius = new Avalonia.CornerRadius(12),
            BorderThickness = new Avalonia.Thickness(2),
            // COLORES FIJOS - NUNCA CAMBIAN
            Background = Brushes.White,
            Foreground = Brushes.Black,
            BorderBrush = new SolidColorBrush(Color.Parse("#E5E7EB")),
            CaretBrush = new SolidColorBrush(Color.Parse("#FF6B9D")),
            SelectionBrush = new SolidColorBrush(Color.Parse("#FFE5EF")),
            SelectionForegroundBrush = Brushes.Black
        };

        // Solo cambiar el borde en focus
        _passwordBox.GotFocus += (s, e) =>
        {
            _passwordBox.BorderBrush = new SolidColorBrush(Color.Parse("#FF6B9D"));
            _passwordBox.BorderThickness = new Avalonia.Thickness(3);
            // FORZAR colores para que no cambien
            _passwordBox.Background = Brushes.White;
            _passwordBox.Foreground = Brushes.Black;
        };

        _passwordBox.LostFocus += (s, e) =>
        {
            _passwordBox.BorderBrush = new SolidColorBrush(Color.Parse("#E5E7EB"));
            _passwordBox.BorderThickness = new Avalonia.Thickness(2);
            // FORZAR colores para que no cambien
            _passwordBox.Background = Brushes.White;
            _passwordBox.Foreground = Brushes.Black;
        };

        var card = new Border
        {
            Background = new SolidColorBrush(Color.Parse("#FFFFFF")),
            CornerRadius = new Avalonia.CornerRadius(32),
            BoxShadow = new BoxShadows(new BoxShadow { 
                Blur = 60, 
                Color = Color.Parse("#00000050"),
                OffsetX = 0,
                OffsetY = 20
            }),
            Padding = new Avalonia.Thickness(60, 50, 60, 50),
            MinWidth = 520,
            MaxWidth = 620,
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center,
            Child = new StackPanel
            {
                Spacing = 22,
                Children =
                {
                    new TextBlock
                    {
                        Text = "🎓 Sistema Académico",
                        FontSize = 44,
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
                        },
                        HorizontalAlignment = HorizontalAlignment.Center,
                        Margin = new Avalonia.Thickness(0,0,0,12)
                    },
                    new TextBlock
                    {
                        Text = "Iniciar Sesión",
                        FontSize = 26,
                        FontWeight = FontWeight.SemiBold,
                        Foreground = new SolidColorBrush(Color.Parse("#1F2937")),
                        HorizontalAlignment = HorizontalAlignment.Center,
                        Margin = new Avalonia.Thickness(0,0,0,36)
                    },
                    new TextBlock
                    {
                        Text = "Usuario",
                        FontSize = 14,
                        FontWeight = FontWeight.SemiBold,
                        Foreground = new SolidColorBrush(Color.Parse("#374151")),
                        Margin = new Avalonia.Thickness(0, 0, 0, 8)
                    },
                    _usernameBox,
                    new TextBlock
                    {
                        Text = "Contraseña",
                        FontSize = 14,
                        FontWeight = FontWeight.SemiBold,
                        Foreground = new SolidColorBrush(Color.Parse("#374151")),
                        Margin = new Avalonia.Thickness(0, 0, 0, 8)
                    },
                    _passwordBox,
                    (_errorText = new TextBlock
                    {
                        Foreground = new SolidColorBrush(Color.Parse("#DC2626")),
                        FontSize = 13,
                        TextWrapping = TextWrapping.Wrap,
                        IsVisible = false,
                        Margin = new Avalonia.Thickness(0, -8, 0, 0)
                    }),
                    CreateLoginButton()
                }
            }
        };

        mainPanel.Children.Add(card);
        backgroundGrid.Children.Add(mainPanel);
        Content = backgroundGrid;
    }

    private Button CreateLoginButton()
    {
        var btn = new Button
        {
            Content = "Iniciar Sesión",
            HorizontalAlignment = HorizontalAlignment.Stretch,
            Height = 56,
            FontSize = 17,
            FontWeight = FontWeight.Bold,
            Background = new LinearGradientBrush
            {
                StartPoint = new Avalonia.RelativePoint(0, 0, Avalonia.RelativeUnit.Relative),
                EndPoint = new Avalonia.RelativePoint(1, 0, Avalonia.RelativeUnit.Relative),
                GradientStops = new GradientStops
                {
                    new GradientStop(Color.Parse("#FF6B9D"), 0),
                    new GradientStop(Color.Parse("#FF4D88"), 1)
                }
            },
            Foreground = Brushes.White,
            CornerRadius = new Avalonia.CornerRadius(14),
            Margin = new Avalonia.Thickness(0, 20, 0, 0),
            Cursor = new Avalonia.Input.Cursor(Avalonia.Input.StandardCursorType.Hand),
            BorderThickness = new Avalonia.Thickness(0)
        };

        btn.Click += async (s, e) =>
        {
            _errorText.IsVisible = false;
            btn.IsEnabled = false;
            btn.Content = "Iniciando...";

            try
            {
                if (string.IsNullOrWhiteSpace(_usernameBox.Text) ||
                    string.IsNullOrWhiteSpace(_passwordBox.Text))
                {
                    ShowError("Por favor complete todos los campos");
                    return;
                }

                // Verificar conexión con el backend
                var connectionTest = await _apiService.TestConnectionAsync();
                if (!connectionTest)
                {
                    ShowError("No se puede conectar con el servidor en http://localhost:5130. Verifique que el backend esté ejecutándose.");
                    return;
                }

                var loginDto = new LoginDto
                {
                    NombreUsuario = _usernameBox.Text ?? "",
                    Contrasena = _passwordBox.Text ?? ""
                };

                Console.WriteLine($"🔐 Intentando login con usuario: {loginDto.NombreUsuario}");

                var usuario = await _apiService.PostAsync<LoginDto, Usuario>(
                    "/usuarios/login",
                    loginDto
                );

                if (usuario != null && usuario.Activo)
                {
                    _authService.SaveSession(usuario);
                    var mainWindow = new MainWindow();
                    mainWindow.Show();
                    Close();
                }
                else
                {
                    ShowError("Credenciales inválidas. Por favor intente nuevamente.");
                }
            }
            catch (Exception ex)
            {
                ShowError($"Error inesperado: {ex.Message}");
            }
            finally
            {
                btn.IsEnabled = true;
                btn.Content = "Iniciar Sesión";
            }
        };

        return btn;
    }

    private void ShowError(string message)
    {
        _errorText.Text = message;
        _errorText.IsVisible = true;
    }
}