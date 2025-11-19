namespace frontend.Elements;

using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Layout;

public enum ButtonStyle
{
    Primary,
    Secondary,
    Success,
    Danger,
    Warning,
    Info,
    Outline
}

public class StyledButton : Button
{
    public StyledButton(string text, ButtonStyle style = ButtonStyle.Primary)
    {
        Content = text;

        // Tamaño y espaciado
        Height = 48;
        Padding = new Avalonia.Thickness(20, 0);

        // Ajuste automático del ancho para evitar que corte el texto
        Width = double.NaN;      // Auto-size
        MinWidth = 135;          // Para evitar recortes en contenedores estrechos

        HorizontalAlignment = HorizontalAlignment.Center;

        // Bordes redondeados (estilo que querías)
        CornerRadius = new Avalonia.CornerRadius(20);

        // Texto
        FontSize = 14;
        FontWeight = FontWeight.SemiBold;

        // Bordes
        BorderThickness = new Avalonia.Thickness(2);

        // Alineación interna
        HorizontalContentAlignment = HorizontalAlignment.Center;
        VerticalContentAlignment = VerticalAlignment.Center;

        // Cursor mano
        Cursor = new Avalonia.Input.Cursor(Avalonia.Input.StandardCursorType.Hand);

        ApplyStyle(style);
    }

    private void ApplyStyle(ButtonStyle style)
    {
        switch (style)
        {
            case ButtonStyle.Primary:
                Background = AppColors.Primary;
                Foreground = Brushes.White;
                BorderBrush = AppColors.Primary;
                break;

            case ButtonStyle.Secondary:
                Background = AppColors.Secondary;
                Foreground = Brushes.White;
                BorderBrush = AppColors.Secondary;
                break;

            case ButtonStyle.Success:
                Background = AppColors.Success;
                Foreground = Brushes.White;
                BorderBrush = AppColors.Success;
                break;

            case ButtonStyle.Danger:
                Background = AppColors.Danger;
                Foreground = Brushes.White;
                BorderBrush = AppColors.Danger;
                break;

            case ButtonStyle.Warning:
                Background = AppColors.Warning;
                Foreground = AppColors.TextPrimary;
                BorderBrush = AppColors.Warning;
                break;

            case ButtonStyle.Info:
                Background = AppColors.Info;
                Foreground = Brushes.White;
                BorderBrush = AppColors.Info;
                break;

            case ButtonStyle.Outline:
                Background = AppColors.Surface;
                Foreground = AppColors.TextSecondary;
                BorderBrush = AppColors.Border;
                break;
        }
    }
}
