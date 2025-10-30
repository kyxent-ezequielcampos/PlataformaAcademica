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
        Height = 44;
        Padding = new Avalonia.Thickness(24, 0);
        CornerRadius = new Avalonia.CornerRadius(12);
        FontSize = 14;
        FontWeight = FontWeight.SemiBold;
        HorizontalContentAlignment = HorizontalAlignment.Center;
        VerticalContentAlignment = VerticalAlignment.Center;
        Cursor = new Avalonia.Input.Cursor(Avalonia.Input.StandardCursorType.Hand);
        BorderThickness = new Avalonia.Thickness(2);

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