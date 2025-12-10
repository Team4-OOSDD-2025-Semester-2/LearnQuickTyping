namespace LearnQuickTyping.Core.Models;

public class ExerciseResult
{
    public int Id { get; set; }

    public string Date { get; set; } = string.Empty;

    public string Time { get; set; } = string.Empty;

    public double WordsPerMinute { get; set; }

    public int Accuracy { get; set; }

    public string TimeTaken { get; set; } = string.Empty;

    public int Errors { get; set; }

    public ExerciseType ExerciseType { get; set; }

    public DifficultyLevel DifficultyLevel { get; set; }
}

public enum ExerciseType
{
    Word,
    Text,
    Versus,
    Lyrics
}

public enum DifficultyLevel
{
    Beginner,
    Intermediate,
    Advanced,
    Expert
}