namespace frontend.Elements;

using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Layout;

public class StyledButton : Button
{
    public StyledButton(string text, bool isPrimary = true)
    {
        Content = text;
        Height = 40;
        Padding = new Avalonia.Thickness(24, 0);
        CornerRadius = new Avalonia.CornerRadius(6);
        FontSize = 14;
        FontWeight = FontWeight.Medium;
        HorizontalContentAlignment = HorizontalAlignment.Center;
        VerticalContentAlignment = VerticalAlignment.Center;
        Cursor = new Avalonia.Input.Cursor(Avalonia.Input.StandardCursorType.Hand);

        if (isPrimary)
        {
            Background = Colors.Primary;
            Foreground = Brushes.White;
        }
        else
        {
            Background = Colors.Secondary;
            Foreground = Brushes.White;
        }
    }
}