namespace frontend.Elements;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

public class StyledTextBox : TextBox
{
    public StyledTextBox(string placeholder = "", int height = 50)
    {
        // Configuración básica
        Watermark = placeholder;
        Height = height;
        MinHeight = height;
        HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch;
        
        // Padding y bordes
        Padding = new Thickness(16, 14);
        CornerRadius = new CornerRadius(12);
        BorderThickness = new Thickness(2);
        
        // Tipografía
        FontSize = 15;
        FontWeight = FontWeight.Medium;
        
        // COLORES FIJOS - FONDO BLANCO Y TEXTO NEGRO SIEMPRE
        Background = Brushes.White;
        Foreground = Brushes.Black;
        BorderBrush = new SolidColorBrush(Color.Parse("#E5E7EB"));
        
        // Cursor y selección
        CaretBrush = new SolidColorBrush(Color.Parse("#FF6B9D"));
        SelectionBrush = new SolidColorBrush(Color.Parse("#FFE5EF"));
        SelectionForegroundBrush = Brushes.Black;
        
        // Funcionalidad
        IsReadOnly = false;
        IsEnabled = true;
        IsTabStop = true;
        Focusable = true;
        AcceptsTab = false;
        UseFloatingWatermark = false;
        
        // Configuración de texto multilínea
        TextWrapping = height > 50 ? TextWrapping.Wrap : TextWrapping.NoWrap;
        AcceptsReturn = height > 50;
        VerticalContentAlignment = height > 50 
            ? Avalonia.Layout.VerticalAlignment.Top 
            : Avalonia.Layout.VerticalAlignment.Center;

        // Solo cambiar borde en focus - NADA MÁS
        GotFocus += (s, e) =>
        {
            BorderBrush = new SolidColorBrush(Color.Parse("#FF6B9D"));
            BorderThickness = new Thickness(3);
            // FORZAR que los colores no cambien
            Background = Brushes.White;
            Foreground = Brushes.Black;
        };

        LostFocus += (s, e) =>
        {
            BorderBrush = new SolidColorBrush(Color.Parse("#E5E7EB"));
            BorderThickness = new Thickness(2);
            // FORZAR que los colores no cambien
            Background = Brushes.White;
            Foreground = Brushes.Black;
        };
    }
}
