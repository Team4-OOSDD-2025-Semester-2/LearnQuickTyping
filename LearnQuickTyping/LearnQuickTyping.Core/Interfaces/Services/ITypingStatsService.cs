namespace LearnQuickTyping.Core.Interfaces.Services;

public interface ITypingStatsService
{
    double CalculateWordsPerMinuteSingleWord(int characterCount, TimeSpan timeTaken);
    double CalculateWordsPerMinuteText(string text, TimeSpan timeTaken);
    int GetTotalPracticeWordCount();
    int GetMistakeCount();
    void TrackMistakes(string typedText, string targetText);

    void ResetMistakes();

}