namespace LearnQuickTyping.Core.Models;

public class TextResult
{
    public double WordsPerMinute { get; set; }
    public TimeSpan TimeTaken { get; set; }
    public int Errors { get; set; }
    public int Accuracy { get; set; }
}

