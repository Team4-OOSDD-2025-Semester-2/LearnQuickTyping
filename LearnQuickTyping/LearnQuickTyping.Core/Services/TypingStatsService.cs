using LearnQuickTyping.Core.Interfaces.Repositories;
using LearnQuickTyping.Core.Interfaces.Services;

namespace LearnQuickTyping.Core.Services;

public class TypingStatsService : ITypingStatsService
{
    private const double CharactersPerWordAverage = 6.8;
    private readonly ITextRepository _textRepository;

    public TypingStatsService(ITextRepository textRepository)
    {
        _textRepository = textRepository;
    }

    public double CalculateWordsPerMinuteSingleWord(int characterCount, TimeSpan timeTaken)
    {
        if (timeTaken.TotalMinutes == 0) return 0;

        double wordCount = characterCount / CharactersPerWordAverage;
        return wordCount / timeTaken.TotalMinutes;
    }

    public double CalculateWordsPerMinuteText(string text, TimeSpan timeTaken)
    {
        if (timeTaken.TotalMinutes == 0) return 0;

        int wordCount = _textRepository.CountWords(text);
        return wordCount / timeTaken.TotalMinutes;
    }

    public int GetTotalPracticeWordCount()
    {
        return _textRepository.GetWordCount();
    }
}