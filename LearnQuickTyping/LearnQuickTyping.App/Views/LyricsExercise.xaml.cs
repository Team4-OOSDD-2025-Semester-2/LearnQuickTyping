using LearnQuickTyping.App.ViewModels;
using LearnQuickTyping.Core.Models;

namespace LearnQuickTyping.App.Views;

public partial class LyricsExercise : ContentPage
{
    private readonly LyricsExerciseViewModel _viewModel;

    public LyricsExercise(LyricsExerciseViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;

        _viewModel.RequestLetterUpdate += UpdateLetterDisplay;
        _viewModel.ExerciseStarted += OnExerciseStarted;
        _viewModel.NavigateToResults += OnNavigateToResults;
        _viewModel.ExerciseCompleted += OnExerciseCompleted;
    }

    private void OnExerciseStarted()
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            TextPicker.IsVisible = false;
        });
    }

    private void OnExerciseCompleted()
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            TextPicker.IsVisible = true;
        });
    }

    private async void OnNavigateToResults()
    {
        await MainThread.InvokeOnMainThreadAsync(async () =>
        {
            // Navigate to the result page
            await Navigation.PushAsync(new LyricsResult(_viewModel));
        });
    }

    private void UpdateLetterDisplay(List<LetterStatus> statuses)
    {
        var formattedString = new FormattedString();

        foreach (var letterStatus in statuses)
        {
            var span = new Span
            {
                Text = letterStatus.Character.ToString(),
                FontSize = PracticeTextLabel.FontSize,
                TextColor = letterStatus.Status switch
                {
                    Status.Correct => Colors.Green,
                    Status.Incorrect => Colors.Red,
                    Status.Pending => Colors.Gray,
                    _ => Colors.Black
                }
            };

            formattedString.Spans.Add(span);
        }

        PracticeTextLabel.FormattedText = formattedString;
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        _viewModel.RequestLetterUpdate -= UpdateLetterDisplay;
        _viewModel.ExerciseStarted -= OnExerciseStarted;
        _viewModel.NavigateToResults -= OnNavigateToResults;
        _viewModel.ExerciseCompleted -= OnExerciseCompleted;
    }
}
