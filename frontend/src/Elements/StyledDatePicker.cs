namespace frontend.Elements;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using System;

public class StyledDatePicker : DatePicker
{
    public StyledDatePicker(DateTime? selectedDate = null)
    {
        SelectedDate = selectedDate ?? DateTime.Now.AddYears(-18);
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
