namespace frontend.Elements;

using Avalonia.Controls;
using Avalonia.Media;

public class StyledCard : Border
{
    public StyledCard(Control? child = null, double padding = 24)
    {
        Background = AppColors.Surface;
        CornerRadius = new Avalonia.CornerRadius(16);
        Padding = new Avalonia.Thickness(padding);
        BoxShadow = new BoxShadows(new BoxShadow
        {
            Blur = 20,
            Color = Color.Parse("#10000020"),
            OffsetY = 4
        });
        
        if (child != null)
        {
            Child = child;
        }
    }
}
