using LearnQuickTyping.Core.Interfaces.Services;

namespace LearnQuickTyping.Core.Services;

public class TypingStatsService : ITypingStatsService
{
    private const double CharactersPerWordAverage = 6.8;

    public double CalculateWordPerMinute(int characterCount, TimeSpan timeTaken)
    {
        if (timeTaken.TotalMinutes == 0) return 0;

        double wordCount = characterCount / CharactersPerWordAverage;
        return wordCount / timeTaken.TotalMinutes;
    }
}