namespace LearnQuickTyping.Core.Interfaces.Services;

public interface ITypingStatsService
{
    double CalculateWordsPerMinuteSingleWord(int characterCount, TimeSpan timeTaken);
    double CalculateWordsPerMinuteText(string text, TimeSpan timeTaken);
    int GetTotalPracticeWordCount();
}