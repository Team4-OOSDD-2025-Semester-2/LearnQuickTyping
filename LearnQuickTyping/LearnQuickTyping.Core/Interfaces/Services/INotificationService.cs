namespace LearnQuickTyping.Core.Interfaces.Services;

public interface INotificationService
{
    Task ShowSuccessAsync(string message);
    Task ShowErrorAsync(string message);
}