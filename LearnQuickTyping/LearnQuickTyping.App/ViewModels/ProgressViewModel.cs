using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LearnQuickTyping.Core.Interfaces.Repositories;
using LearnQuickTyping.Core.Models;
using System.Collections.ObjectModel;

namespace LearnQuickTyping.App.ViewModels;

public partial class ProgressViewModel : BaseViewModel
{
    private readonly IExerciseResultRepository _repository;

    [ObservableProperty]
    private ObservableCollection<ExerciseResult> _recentResults = new();

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(NoResults))]
    private bool _hasResults;

    public bool NoResults => !HasResults && !IsLoading;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(NoResults))]
    private bool _isLoading;

    [ObservableProperty]
    private string _noResultsMessage = "No results available.";

    public ProgressViewModel(IExerciseResultRepository repository)
    {
        _repository = repository;
        Title = "Progress";
    }

    [RelayCommand]
    public async Task LoadProgressAsync()
    {
        if (IsLoading) return;

        try
        {
            IsLoading = true;

            var results = await _repository.GetRecentAsync(10);

            RecentResults.Clear();
            foreach (var result in results)
            {
                RecentResults.Add(result);
            }

            HasResults = RecentResults.Count > 0;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error loading progress: {ex.Message}");
            HasResults = false;
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task GoHome()
    {
        await Shell.Current.GoToAsync("///StartPage");
    }

    // Helper properties for display formatting
    public static string FormatExerciseType(ExerciseType type) => type switch
    {
        ExerciseType.Word => "Word",
        ExerciseType.Text => "Text",
        ExerciseType.Versus => "Versus",
        ExerciseType.Lyrics => "Karaoke",
        _ => type.ToString()
    };

    public static string FormatDifficulty(DifficultyLevel level) => level switch
    {
        DifficultyLevel.Beginner => "Beginner",
        DifficultyLevel.Intermediate => "Intermediate",
        DifficultyLevel.Advanced => "Advanced",
        DifficultyLevel.Expert => "Expert",
        _ => level.ToString()
    };

    public static string FormatTimeTaken(string timeTaken)
    {
        // Format: mm-ss-fff -> mm:ss
        if (string.IsNullOrEmpty(timeTaken)) return "00:00";

        var parts = timeTaken.Split('-');
        if (parts.Length >= 2)
        {
            return $"{parts[0]}:{parts[1]}";
        }
        return timeTaken;
    }
}