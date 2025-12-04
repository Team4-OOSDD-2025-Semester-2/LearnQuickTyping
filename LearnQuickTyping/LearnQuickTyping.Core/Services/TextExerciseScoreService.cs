using LearnQuickTyping.Core.Interfaces.Services;
using LearnQuickTyping.Core.Models;

namespace LearnQuickTyping.Core.Services
{
    public class TextExerciseScoreService : ITextEcerciseScoreService
    {
        private readonly ITypingStatsService _typingStatsService;

        public TextExerciseScoreService(ITypingStatsService typingStatsService)
        {
            _typingStatsService = typingStatsService;
        }

        public TextResult CalculateResult(string targetText, string typedText, TimeSpan timeTaken)
        {
            int errors = _typingStatsService.GetMistakeCount();
            errors += Math.Abs(targetText.Length - typedText.Length); //If text is too long or short add errors

            double wpm = _typingStatsService.CalculateWordsPerMinuteText(typedText.Length, timeTaken);

            int accuracy = _typingStatsService.CalculateAccuracy();

            return new TextResult
            {
                WordsPerMinute = wpm,
                Errors = errors,
                TimeTaken = timeTaken,
                Accuracy = accuracy
            };
        }
    }
}