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

        _viewModel.ExerciseStarted += OnExerciseStarted;
        _viewModel.ExerciseCompleted += OnExerciseCompleted;
        _viewModel.RequestLetterUpdate += UpdateLetterDisplay;
    }
    protected override void OnAppearing()
    {
        base.OnAppearing();
        _viewModel.InitializeExerciseCommand.Execute(null);
    }
    private void OnExerciseStarted()
    {
        // When the lyric is chosen, the picker disapperead 
        MainThread.BeginInvokeOnMainThread(() =>
        {
            TextPicker.IsVisible = false;
        });
    }
    private void OnExerciseCompleted()
    {
        // When the lyric is completed, the picker appeared 
        MainThread.BeginInvokeOnMainThread(() =>
        {
            TextPicker.IsVisible = true;
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
                FontSize = PracticeTextLabel.FontSize
            };

            span.TextColor = letterStatus.Status switch
            {
                Status.Correct => Colors.Green,
                Status.Incorrect => Colors.Red,
                Status.Pending => Colors.Gray,
                _ => Colors.Black
            };

            formattedString.Spans.Add(span);
        }

        PracticeTextLabel.FormattedText = formattedString;
    }
    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        _viewModel.ExerciseStarted += OnExerciseStarted;
        _viewModel.ExerciseCompleted += OnExerciseCompleted;
        _viewModel.RequestLetterUpdate += UpdateLetterDisplay;
    }
}