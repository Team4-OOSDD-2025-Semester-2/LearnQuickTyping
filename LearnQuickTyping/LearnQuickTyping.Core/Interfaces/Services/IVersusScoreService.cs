using LearnQuickTyping.Core.Models;

namespace LearnQuickTyping.Core.Interfaces.Services
{
    public interface IVersusScoreService
    {
        VersusResult CalculateResult(string playerName, string targetText, string typedText, TimeSpan timeTaken);
        string DetermineWinner(VersusResult player1, VersusResult player2);
    }
}
