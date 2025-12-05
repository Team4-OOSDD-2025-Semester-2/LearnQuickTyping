using LearnQuickTyping.Core.Interfaces;
using LearnQuickTyping.Core.Interfaces.Repositories;
using LearnQuickTyping.Core.Interfaces.Services;
using LearnQuickTyping.Core.Models;

namespace LearnQuickTyping.Core.Services;

public class TypingStatsService : ITypingStatsService
{
    private const double CharactersPerWordAverage = 6.8;
    private readonly ITextRepository _textRepository;

    private int _totalMistakes = 0;
    private int _previousTypedLength = 0;

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

    public int GetMistakeCount()
    {
        return _totalMistakes;
    }

    public void TrackMistakes(string typedText, string targetText)
    {
        int currentLength = typedText?.Length ?? 0;

        // Recalculate total mistakes from scratch each time
        _totalMistakes = 0;

        for (int i = 0; i < currentLength && i < targetText.Length; i++)
        {
            if (typedText[i] != targetText[i])
            {
                _totalMistakes++;
            }
        }

        _previousTypedLength = currentLength;
    }

    public void ResetMistakes()
    {
        _totalMistakes = 0;
        _previousTypedLength = 0;
    }

    public int CalculateAccuracy()
        {
        if (_previousTypedLength == 0) return 100;
        int correctChars = _previousTypedLength - _totalMistakes;
        return (int)((correctChars / (double)_previousTypedLength) * 100);
    }
}