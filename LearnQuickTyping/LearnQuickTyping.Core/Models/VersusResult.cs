namespace LearnQuickTyping.Core.Models;

public class VersusResult
{
    public string PlayerName { get; set; }
    public double WordsPerMinute { get; set; }
    public TimeSpan TimeTaken { get; set; }
    public int Errors { get; set; }
    public double FinalScore => WordsPerMinute - Errors;
}