namespace frontend.Elements;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

public class StyledNumericUpDown : NumericUpDown
{
    public StyledNumericUpDown(decimal minimum = 1, decimal maximum = 100, decimal value = 1)
    {
        Minimum = minimum;
        Maximum = maximum;
        Value = value;
        Height = 50;
        MinHeight = 50;
        
        // Tipografía
        FontSize = 15;
        FontWeight = FontWeight.Medium;
        
        // COLORES FIJOS - FONDO BLANCO Y TEXTO NEGRO SIEMPRE
        Background = Brushes.White;
        Foreground = Brushes.Black;
        BorderBrush = new SolidColorBrush(Color.Parse("#E5E7EB"));
        BorderThickness = new Thickness(2);
        CornerRadius = new CornerRadius(12);
        Padding = new Thickness(16, 0);
        
        IsEnabled = true;
        Focusable = true;
        IsTabStop = true;
        ShowButtonSpinner = true;
        AllowSpin = true;
        ButtonSpinnerLocation = Location.Right;

        // Solo cambiar borde en focus
        GotFocus += (s, e) =>
        {
            BorderBrush = new SolidColorBrush(Color.Parse("#FF6B9D"));
            BorderThickness = new Thickness(3);
            Background = Brushes.White;
            Foreground = Brushes.Black;
        };

        LostFocus += (s, e) =>
        {
            BorderBrush = new SolidColorBrush(Color.Parse("#E5E7EB"));
            BorderThickness = new Thickness(2);
            Background = Brushes.White;
            Foreground = Brushes.Black;
        };
    }
}
