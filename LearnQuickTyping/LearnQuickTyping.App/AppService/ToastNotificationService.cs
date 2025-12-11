using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using LearnQuickTyping.Core.Interfaces.Services;

namespace LearnQuickTyping.App.Services;

public class ToastNotificationService : INotificationService
{
    public async Task ShowSuccessAsync(string message)
    {
        await ShowToastAsync(message);
    }

    public async Task ShowErrorAsync(string message)
    {
        await ShowToastAsync(message);
    }

    private async Task ShowToastAsync(string message)
    {
        try
        {
            var toast = Toast.Make(message, ToastDuration.Short, 14);
            await toast.Show();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Toast error: {ex.Message}");
        }
    }
}