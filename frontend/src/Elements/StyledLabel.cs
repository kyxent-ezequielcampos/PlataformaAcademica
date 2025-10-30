namespace frontend.Elements;

using Avalonia.Controls;
using Avalonia.Media;

public enum LabelStyle
{
    Normal,
    Title,
    Subtitle,
    Caption
}

public class StyledLabel : TextBlock
{
    public StyledLabel(string text, LabelStyle style = LabelStyle.Normal)
    {
        Text = text;
        Margin = new Avalonia.Thickness(0, 0, 0, 8);
        
        switch (style)
        {
            case LabelStyle.Title:
                FontWeight = FontWeight.SemiBold;
                FontSize = 15;
                Foreground = AppColors.TextPrimary;
                break;
            case LabelStyle.Subtitle:
                FontWeight = FontWeight.Medium;
                FontSize = 14;
                Foreground = AppColors.TextSecondary;
                break;
            case LabelStyle.Caption:
                FontWeight = FontWeight.Normal;
                FontSize = 12;
                Foreground = AppColors.TextTertiary;
                break;
            default:
                FontWeight = FontWeight.Normal;
                FontSize = 14;
                Foreground = AppColors.TextSecondary;
                break;
        }
    }
}