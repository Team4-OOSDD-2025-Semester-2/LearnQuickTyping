namespace LearnQuickTyping.Core.Models;

public class TextResult
{
    public double WordsPerMinute { get; set; }
    public TimeSpan TimeTaken { get; set; }
    public int Errors { get; set; }
    public int Accuracy { get; set; }
    public string OriginalText { get; set; } = string.Empty;
    public string TypedText { get; set; } = string.Empty;
}

