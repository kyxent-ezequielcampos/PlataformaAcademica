namespace frontend.Elements;

using Avalonia.Controls;
using Avalonia.Media;

public enum IconButtonStyle
{
    Primary,
    Success,
    Danger,
    Warning,
    Info,
    Secondary
}

public class StyledIconButton : Button
{
    public StyledIconButton(string icon, IconButtonStyle style = IconButtonStyle.Primary)
    {
        Content = icon;
        Width = 44;
        Height = 44;
        FontSize = 18;
        CornerRadius = new Avalonia.CornerRadius(12);
        BorderThickness = new Avalonia.Thickness(2);
        Cursor = new Avalonia.Input.Cursor(Avalonia.Input.StandardCursorType.Hand);
        
        ApplyStyle(style);
    }

    private void ApplyStyle(IconButtonStyle style)
    {
        switch (style)
        {
            case IconButtonStyle.Primary:
                Background = AppColors.PrimaryLight;
                Foreground = AppColors.Primary;
                BorderBrush = AppColors.Primary;
                break;
            case IconButtonStyle.Success:
                Background = AppColors.SuccessLight;
                Foreground = AppColors.Success;
                BorderBrush = AppColors.Success;
                break;
            case IconButtonStyle.Danger:
                Background = AppColors.DangerLight;
                Foreground = AppColors.Danger;
                BorderBrush = AppColors.Danger;
                break;
            case IconButtonStyle.Warning:
                Background = AppColors.WarningLight;
                Foreground = AppColors.Warning;
                BorderBrush = AppColors.Warning;
                break;
            case IconButtonStyle.Info:
                Background = AppColors.InfoLight;
                Foreground = AppColors.Info;
                BorderBrush = AppColors.Info;
                break;
            case IconButtonStyle.Secondary:
                Background = AppColors.SecondaryLight;
                Foreground = AppColors.Secondary;
                BorderBrush = AppColors.Secondary;
                break;
        }
    }
}
