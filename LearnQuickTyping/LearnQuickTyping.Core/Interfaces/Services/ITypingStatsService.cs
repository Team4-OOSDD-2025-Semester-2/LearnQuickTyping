namespace LearnQuickTyping.Core.Interfaces.Services;

public interface ITypingStatsService
{
    double CalculateWordPerMinute(int characterCount, TimeSpan timeTaken);
}