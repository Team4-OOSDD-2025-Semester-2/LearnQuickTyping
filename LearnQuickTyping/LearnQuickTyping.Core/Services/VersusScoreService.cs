using LearnQuickTyping.Core.Interfaces.Services;
using LearnQuickTyping.Core.Models;

namespace LearnQuickTyping.Core.Services
{
    public class VersusScoreService: IVersusScoreService
    {
        private readonly ITypingStatsService _typingStatsService;

        public VersusScoreService(ITypingStatsService typingStatsService)
        {
            _typingStatsService = typingStatsService;
        }

        public VersusResult CalculateResult(string playerName, string targetText, string typedText, TimeSpan timeTaken)
        {
            int errors = 0;
            for (int i = 0; i < Math.Min(targetText.Length, typedText.Length); i++) //Loop through text based on shortest text length
            {
                if (targetText[i] != typedText[i]) errors++; //If character does not match it's an error
            }
            errors += Math.Abs(targetText.Length - typedText.Length); //If text is too long or short add errors

            double wpm = _typingStatsService.CalculateWordsPerMinuteSingleWord(typedText.Length, timeTaken);

            return new VersusResult
            {
                PlayerName = playerName,
                WordsPerMinute = wpm,
                Errors = errors,
                TimeTaken = timeTaken
            };
        }

        public string DetermineWinner(VersusResult player1, VersusResult player2)
        {
            if (player1.FinalScore > player2.FinalScore) return player1.PlayerName;
            if (player2.FinalScore > player1.FinalScore) return player2.PlayerName;
            return "Tie";
        }
    }
}
