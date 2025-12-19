using LearnQuickTyping.Core.Interfaces.Repositories;
using LearnQuickTyping.Core.Interfaces.Services;
using LearnQuickTyping.Core.Models;

namespace LearnQuickTyping.Core.Services;

public class ExerciseResultSaveService : IExerciseResultSaveService
{
    private readonly IExerciseResultRepository _repository;
    private readonly INotificationService _notificationService;

    public ExerciseResultSaveService(
        IExerciseResultRepository repository,
        INotificationService notificationService)
    {
        _repository = repository;
        _notificationService = notificationService;
    }

    public async Task<bool> SaveResultAsync(
        double wordsPerMinute,
        int accuracy,
        TimeSpan timeTaken,
        int errors,
        ExerciseType exerciseType,
        DifficultyLevel difficultyLevel)
    {
        try
        {
            var now = DateTime.Now;

            var result = new ExerciseResult
            {
                Date = FormatDate(now),
                Time = now.ToString("HH:mm:ss"),
                WordsPerMinute = wordsPerMinute,
                Accuracy = accuracy,
                TimeTaken = FormatTimeTaken(timeTaken),
                Errors = errors,
                ExerciseType = exerciseType,
                DifficultyLevel = difficultyLevel
            };

            await _repository.AddAsync(result);
            await _notificationService.ShowSuccessAsync("Result saved successfully!");
            return true;
        }
        catch (Exception)
        {
            await _notificationService.ShowErrorAsync("Failed to save result.");
            return false;
        }
    }

    public string FormatTimeTaken(TimeSpan timeTaken)
    {
        int totalMinutes = (int)timeTaken.TotalMinutes;
        return $"{totalMinutes:D2}-{timeTaken.Seconds:D2}-{timeTaken.Milliseconds:D3}";
    }

    public string FormatDate(DateTime date) => date.ToString("yyyy-MM-dd");
}