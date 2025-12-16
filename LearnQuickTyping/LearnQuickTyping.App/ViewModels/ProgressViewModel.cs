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
}