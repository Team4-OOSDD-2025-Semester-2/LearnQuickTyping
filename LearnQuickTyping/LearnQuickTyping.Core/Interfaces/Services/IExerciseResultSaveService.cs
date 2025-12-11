using LearnQuickTyping.Core.Models;

namespace LearnQuickTyping.Core.Interfaces.Services;

public interface IExerciseResultSaveService
{
    Task<bool> SaveResultAsync(
        double wordsPerMinute,
        int accuracy,
        TimeSpan timeTaken,
        int errors,
        ExerciseType exerciseType,
        DifficultyLevel difficultyLevel);

    string FormatTimeTaken(TimeSpan timeTaken);
    string FormatDate(DateTime date);
}