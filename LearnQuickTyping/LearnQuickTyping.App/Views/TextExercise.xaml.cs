using LearnQuickTyping.App.ViewModels;
using LearnQuickTyping.Core.Models;
using Span = Microsoft.Maui.Controls.Span;

namespace LearnQuickTyping.App.Views;

public partial class TextExercise : ContentPage
{
    private readonly TextExerciseViewModel _viewModel;

    private List<Span> _currentLineSpans = new();
    private FormattedString _currentLineFormatted;

    public TextExercise(TextExerciseViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;

        _viewModel.RequestLetterUpdate += UpdateLetterDisplay;
        _viewModel.OnLineChanged += UpdateLineDisplay;
        _viewModel.PropertyChanged += OnViewModelPropertyChanged;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _viewModel.InitializeExerciseCommand.Execute(null);
        FocusEntry(InputEntry);
    }

    private void OnViewModelPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(_viewModel.IsTurnOverlayVisible))
        {
            if (_viewModel.IsTurnOverlayVisible)
                FocusEntry(Invisible);
            else
                FocusEntry(InputEntry);
        }
    }

    private void UpdateLineDisplay(string prev, string curr, string next)
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            PreviousLineLabel.Text = prev;
            NextLineLabel.Text = next;

            _currentLineFormatted = new FormattedString();
            _currentLineSpans.Clear();

            if (!string.IsNullOrEmpty(curr))
            {
                foreach (char c in curr)
                {
                    var span = new Span
                    {
                        Text = c.ToString(),
                        TextColor = Colors.Gray,
                        FontSize = CurrentLineLabel.FontSize
                    };
                    _currentLineSpans.Add(span);
                    _currentLineFormatted.Spans.Add(span);
                }
            }

            CurrentLineLabel.FormattedText = _currentLineFormatted;
            TypedTextLabel.Text = string.Empty;
        });
    }

    private void UpdateLetterDisplay(List<LetterStatus> statuses)
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            if (_currentLineSpans == null || _currentLineSpans.Count == 0) return;
            if (statuses == null) return;

            int limit = Math.Min(statuses.Count, _currentLineSpans.Count);

            for (int i = 0; i < limit; i++)
            {
                var status = statuses[i];
                var span = _currentLineSpans[i];

                var targetColor = status.Status switch
                {
                    Status.Correct => Colors.Green,
                    Status.Incorrect => Colors.Red,
                    _ => Colors.Gray
                };

                var targetDecoration = status.Status == Status.Incorrect
                    ? TextDecorations.Underline
                    : TextDecorations.None;

                if (span.TextColor != targetColor)
                    span.TextColor = targetColor;

                if (span.TextDecorations != targetDecoration)
                    span.TextDecorations = targetDecoration;
            }
        });
    }

    private void FocusEntry(Entry entry)
    {
        Dispatcher.Dispatch(async () =>
        {
            await Task.Delay(100); // Small delay to ensure UI is rendered
            entry.Focus();
        });
    }

    private void OnTextChanged(object sender, TextChangedEventArgs e)
    {
        string currentText = e.NewTextValue ?? string.Empty;
        string oldText = e.OldTextValue ?? string.Empty;

        // Check if text was added 
        if (currentText.Length == 0)
        {
            TypedTextLabel.Text = "";
        }
        else if (currentText.Length > oldText.Length)
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