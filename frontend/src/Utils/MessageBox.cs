namespace frontend.Utils;

using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using frontend.Elements;
using System.Threading.Tasks;

public static class MessageBox
{
    public static async Task<bool> ShowConfirmAsync(Window parent, string title, string message)
    {
        var dialog = new Window
        {
            Title = title,
            Width = 400,
            Height = 200,
            WindowStartupLocation = WindowStartupLocation.CenterOwner,
            CanResize = false
        };

        var result = false;

        var content = new StackPanel
        {
            Spacing = 20,
            Margin = new Avalonia.Thickness(20)
        };

        content.Children.Add(new TextBlock
        {
            Text = message,
            FontSize = 14,
            Foreground = AppColors.TextPrimary,
            TextWrapping = TextWrapping.Wrap
        });

        var buttons = new StackPanel
        {
            Orientation = Orientation.Horizontal,
            Spacing = 10,
            HorizontalAlignment = HorizontalAlignment.Right
        };

        var btnYes = new StyledButton("Sí", ButtonStyle.Primary) { Width = 80 };
        btnYes.Click += (s, e) =>
        {
            result = true;
            dialog.Close();
        };

        var btnNo = new StyledButton("No", ButtonStyle.Outline) { Width = 80 };
        btnNo.Click += (s, e) =>
        {
            result = false;
            dialog.Close();
        };

        buttons.Children.Add(btnNo);
        buttons.Children.Add(btnYes);
        content.Children.Add(buttons);

        dialog.Content = content;

        await dialog.ShowDialog(parent);
        return result;
    }

    public static async Task ShowInfoAsync(Window parent, string title, string message)
    {
        var dialog = new Window
        {
            Title = title,
            Width = 400,
            Height = 180,
            WindowStartupLocation = WindowStartupLocation.CenterOwner,
            CanResize = false
        };

        var content = new StackPanel
        {
            Spacing = 20,
            Margin = new Avalonia.Thickness(20)
        };

        content.Children.Add(new TextBlock
        {
            Text = message,
            FontSize = 14,
            Foreground = AppColors.TextPrimary,
            TextWrapping = TextWrapping.Wrap
        });

        var btnOk = new StyledButton("Aceptar", ButtonStyle.Primary) 
        { 
            Width = 80,
            HorizontalAlignment = HorizontalAlignment.Right
        };
        btnOk.Click += (s, e) => dialog.Close();

        content.Children.Add(btnOk);
        dialog.Content = content;

        await dialog.ShowDialog(parent);
    }
}