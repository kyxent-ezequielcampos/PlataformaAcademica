namespace frontend.Elements;

using Avalonia.Controls;
using Avalonia.Media;

public class StyledTextBox : TextBox
{
    public StyledTextBox(string placeholder = "")
    {
        Watermark = placeholder;
        Height = 40;
        Padding = new Avalonia.Thickness(12);
        CornerRadius = new Avalonia.CornerRadius(6);
        FontSize = 14;
        Background = Colors.Surface;
        BorderBrush = Colors.Border;
        BorderThickness = new Avalonia.Thickness(1);
    }
}