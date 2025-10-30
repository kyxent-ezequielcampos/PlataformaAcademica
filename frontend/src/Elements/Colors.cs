namespace frontend.Elements;

using Avalonia.Media;

public static class AppColors
{
    // Paleta Candy - Colores vibrantes y modernos
    public static readonly SolidColorBrush Primary = new(Color.Parse("#FF6B9D")); // Rosa candy
    public static readonly SolidColorBrush PrimaryHover = new(Color.Parse("#FF4D88"));
    public static readonly SolidColorBrush PrimaryLight = new(Color.Parse("#FFE5EF"));
    
    public static readonly SolidColorBrush Secondary = new(Color.Parse("#A78BFA")); // Púrpura suave
    public static readonly SolidColorBrush SecondaryHover = new(Color.Parse("#8B5CF6"));
    public static readonly SolidColorBrush SecondaryLight = new(Color.Parse("#F3F0FF"));
    
    public static readonly SolidColorBrush Success = new(Color.Parse("#4ADE80")); // Verde menta
    public static readonly SolidColorBrush SuccessLight = new(Color.Parse("#DCFCE7"));
    
    public static readonly SolidColorBrush Warning = new(Color.Parse("#FBBF24")); // Amarillo candy
    public static readonly SolidColorBrush WarningLight = new(Color.Parse("#FEF3C7"));
    
    public static readonly SolidColorBrush Danger = new(Color.Parse("#FB7185")); // Rojo suave
    public static readonly SolidColorBrush DangerLight = new(Color.Parse("#FFE4E6"));
    
    public static readonly SolidColorBrush Info = new(Color.Parse("#60A5FA")); // Azul cielo
    public static readonly SolidColorBrush InfoLight = new(Color.Parse("#DBEAFE"));
    
    // Fondos y superficies
    public static readonly SolidColorBrush Background = new(Color.Parse("#FFF5F7")); // Rosa muy claro
    public static readonly SolidColorBrush Surface = new(Color.Parse("#FFFFFF"));
    public static readonly SolidColorBrush SurfaceHover = new(Color.Parse("#FAFAFA"));
    
    // Bordes
    public static readonly SolidColorBrush Border = new(Color.Parse("#F0E5E9"));
    public static readonly SolidColorBrush BorderLight = new(Color.Parse("#F9F5F7"));
    
    // Textos
    public static readonly SolidColorBrush TextPrimary = new(Color.Parse("#1F2937"));
    public static readonly SolidColorBrush TextSecondary = new(Color.Parse("#6B7280"));
    public static readonly SolidColorBrush TextTertiary = new(Color.Parse("#9CA3AF"));
    
    // Sidebar con gradiente candy
    public static readonly SolidColorBrush Sidebar = new(Color.Parse("#2D1B3D")); // Púrpura oscuro
    public static readonly SolidColorBrush SidebarHover = new(Color.Parse("#3D2B4D"));
}