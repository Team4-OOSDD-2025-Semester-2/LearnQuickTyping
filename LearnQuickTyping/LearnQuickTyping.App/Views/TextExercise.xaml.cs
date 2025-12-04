using LearnQuickTyping.App.ViewModels;
using LearnQuickTyping.Core.Models;
using Span = Microsoft.Maui.Controls.Span;

namespace LearnQuickTyping.App.Views;

public partial class TextExercise : ContentPage
{
    private readonly TextExerciseViewModel _viewModel;

    private List<LetterStatus>? _pendingStatuses;
    private List<Span> _currentLineSpans = new();
    private FormattedString? _currentLineFormatted;
    private bool _isUpdatingLetters; // Prevent concurrent updates

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

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        // Clean up event handlers to prevent memory leaks
        _viewModel.RequestLetterUpdate -= UpdateLetterDisplay;
        _viewModel.OnLineChanged -= UpdateLineDisplay;
        _viewModel.PropertyChanged -= OnViewModelPropertyChanged;
    }

    private void OnViewModelPropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
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
            try
            {
                PreviousLineLabel.Text = prev;
                NextLineLabel.Text = next;

                // Pre-allocate with capacity for better performance
                _currentLineFormatted = new FormattedString();
                _currentLineSpans = new List<Span>(curr?.Length ?? 0);

                if (!string.IsNullOrEmpty(curr))
                {
                    double fontSize = CurrentLineLabel.FontSize;

                    foreach (char c in curr)
                    {
                        var span = new Span
                        {
                            Text = c.ToString(),
                            TextColor = Colors.Gray,
                            FontSize = fontSize
                        };
                        _currentLineSpans.Add(span);
                        _currentLineFormatted.Spans.Add(span);
                    }
                }

                CurrentLineLabel.FormattedText = _currentLineFormatted;
                TypedTextLabel.Text = string.Empty;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"UpdateLineDisplay error: {ex.Message}");
            }
        });
    }

    private void UpdateLetterDisplay(List<LetterStatus> statuses)
    {
        // If an update is already running, save this one for later and return
        if (_isUpdatingLetters)
        {
            _pendingStatuses = statuses;
            return;
        }

        _isUpdatingLetters = true;
        _pendingStatuses = null; // Clear pending since we are processing one now

        MainThread.BeginInvokeOnMainThread(() =>
        {
            try
            {
                if (_currentLineSpans == null || _currentLineSpans.Count == 0 || statuses == null)
                {
                    // _isUpdatingLetters will be reset in finally block
                    return;
                }

                int limit = Math.Min(statuses.Count, _currentLineSpans.Count);

                for (int i = 0; i < limit; i++)
                {
                    var status = statuses[i];
                    var span = _currentLineSpans[i];

                    // Optimization from Step 2
                    if (status.Status == Status.Pending &&
                        span.TextColor == Colors.Gray &&
                        span.TextDecorations == TextDecorations.None)
                    {
                        break;
                    }

                    Color targetColor = status.Status switch
                    {
                        Status.Correct => Colors.Green,
                        Status.Incorrect => Colors.Red,
                        _ => Colors.Gray
                    };

                    TextDecorations targetDecoration = status.Status == Status.Incorrect
                        ? TextDecorations.Underline
                        : TextDecorations.None;

                    if (span.TextColor != targetColor)
                        span.TextColor = targetColor;

                    if (span.TextDecorations != targetDecoration)
                        span.TextDecorations = targetDecoration;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"UpdateLetterDisplay error: {ex.Message}");
            }
            finally
            {
                _isUpdatingLetters = false;

                // Check if a new update arrived while we were busy
                if (_pendingStatuses != null)
                {
                    var nextUpdate = _pendingStatuses;
                    _pendingStatuses = null;
                    // Recursively call to process the pending update
                    UpdateLetterDisplay(nextUpdate);
                }
            }
        });
    }

    private void FocusEntry(Entry entry)
    {
        Dispatcher.Dispatch(async () =>
        {
            await Task.Delay(100);
            entry.Focus();
        });
    }

    private void OnTextChanged(object? sender, TextChangedEventArgs e)
    {
        string currentText = e.NewTextValue ?? string.Empty;
        string oldText = e.OldTextValue ?? string.Empty;

        // Update TypedTextLabel based on what was added
        if (currentText.Length == 0)
        {
            TypedTextLabel.Text = "";
        }
        else if (currentText.Length > oldText.Length)
        {
            // Append only the newly added characters
            string addedText = currentText.Substring(oldText.Length);
            TypedTextLabel.Text += addedText;
        }
        // Note: We don't handle deletion in TypedTextLabel since it's display-only

        // Update ViewModel - this triggers the typing check
        _viewModel.TypedText = TypedTextLabel.Text ?? string.Empty;
    }
}