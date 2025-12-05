using LearnQuickTyping.App.ViewModels;
using LearnQuickTyping.Core.Models;
using Span = Microsoft.Maui.Controls.Span;

namespace LearnQuickTyping.App.Views;

public partial class TextExercise : ContentPage
{
    private readonly TextExerciseViewModel _viewModel;

    public TextExercise(TextExerciseViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;

        _viewModel.RequestLetterUpdate += UpdateLetterDisplay;
        _viewModel.PropertyChanged += OnViewModelPropertyChanged;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _viewModel.InitializeExerciseCommand.Execute(null);

        Dispatcher.DispatchDelayed(TimeSpan.FromMilliseconds(100), () =>
        {
            Invisible.Focus();
        });
    }

    private void OnViewModelPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(_viewModel.IsTurnOverlayVisible))
        {
            Dispatcher.DispatchDelayed(TimeSpan.FromMilliseconds(50), () =>
            {
                if (_viewModel.IsStartScreenVisible)
                {
                    Invisible.Focus();
                }
                else
                {
                    InputEntry.Focus();
                }
            });
        }
    }

    private void UpdateLineDisplay(string prev, string curr, string next)
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            try
            {
                FocusEntry(Invisible);
            }
            else
            {
                FocusEntry(InputEntry);
            }
        }
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

            span.TextDecorations = letterStatus.Status switch
            {
                Status.Correct => TextDecorations.None,
                Status.Incorrect => TextDecorations.Underline,
                _ => TextDecorations.None
            };

            formattedString.Spans.Add(span);
        }

        PracticeTextLabel.FormattedText = formattedString;
    }

    private void OnTextChanged(object? sender, TextChangedEventArgs e)
    {
        string currentText = e.NewTextValue ?? string.Empty;
        string oldText = e.OldTextValue ?? string.Empty;

        // Check if text was added 
        if (currentText.Length > oldText.Length)
        {
            // Get the newly added characters
            string addedText = currentText.Substring(oldText.Length);

            // Append to the label
            TypedTextLabel.Text += addedText;
        }

        // Update ViewModel to compare based on TypedTextLabel content
        _viewModel.TypedText = TypedTextLabel.Text ?? string.Empty;
    }
}