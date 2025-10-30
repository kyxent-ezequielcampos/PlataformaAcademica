namespace frontend.Elements;

using Avalonia.Controls;
using Avalonia.Media;

public class StyledLabel : TextBlock
{
    public StyledLabel(string text, bool isTitle = false)
    {
        Text = text;
        FontWeight = isTitle ? FontWeight.SemiBold : FontWeight.Normal;
        FontSize = isTitle ? 16 : 14;
        Foreground = isTitle ? Colors.TextPrimary : Colors.TextSecondary;
        Margin = new Avalonia.Thickness(0, 0, 0, 8);
    }
}