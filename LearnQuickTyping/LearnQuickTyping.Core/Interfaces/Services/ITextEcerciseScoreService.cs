using LearnQuickTyping.Core.Models;


namespace LearnQuickTyping.Core.Interfaces.Services
{
    public interface ITextEcerciseScoreService
    {
        TextResult CalculateResult(string targetText, string typedText, TimeSpan timeTaken);
    }
}
