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
    private string _currentLineText = string.Empty;
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

        Dispatcher.DispatchDelayed(TimeSpan.FromMilliseconds(100), () =>
        {
            Invisible.Focus();
        });
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
        if (e.PropertyName == nameof(_viewModel.IsStartScreenVisible))
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
                PreviousLineLabel.Text = prev;
                NextLineLabel.Text = next;

                // Store the text in our variable
                _currentLineText = curr ?? string.Empty;

                // Clear FormattedText first to ensure Text displays
                CurrentLineLabel.FormattedText = null;
                CurrentLineLabel.Text = _currentLineText;

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
        if (_isUpdatingLetters) return;
        _isUpdatingLetters = true;

        MainThread.BeginInvokeOnMainThread(() =>
        {
            try
            {
                string fullText = _currentLineText;

                if (string.IsNullOrEmpty(fullText) || statuses == null || statuses.Count == 0)
                {
                    // If we have text but no statuses yet, just show the plain text
                    if (!string.IsNullOrEmpty(fullText))
                    {
                        CurrentLineLabel.FormattedText = null;
                        CurrentLineLabel.Text = fullText;
                    }
                    return;
                }

                var newFormattedString = new FormattedString();

                var currentStatus = statuses[0].Status;
                int startIndex = 0;

                for (int i = 1; i < statuses.Count; i++)
                {
                    if (statuses[i].Status != currentStatus)
                    {
                        int length = i - startIndex;
                        // Safely extract substring
                        if (startIndex + length <= fullText.Length)
                        {
                            string segment = fullText.Substring(startIndex, length);
                            newFormattedString.Spans.Add(CreateSpan(segment, currentStatus));
                        }

                        currentStatus = statuses[i].Status;
                        startIndex = i;
                    }
                }

                // Add remaining text
                if (startIndex < fullText.Length)
                {
                    string remainingText = fullText.Substring(startIndex);
                    newFormattedString.Spans.Add(CreateSpan(remainingText, currentStatus));
                }

                // Apply the new formatting
                CurrentLineLabel.Text = null; // Clear plain text
                CurrentLineLabel.FormattedText = newFormattedString;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"UpdateLetterDisplay error: {ex.Message}");
            }
            finally
            {
                _isUpdatingLetters = false;
            }
        });
    }

    // Helper method to keep style consistent
    private Span CreateSpan(string text, Status status)
    {
        var span = new Span { Text = text, FontSize = CurrentLineLabel.FontSize };

        switch (status)
        {
            case Status.Correct:
                span.TextColor = Colors.Green;
                break;
            case Status.Incorrect:
                span.TextColor = Colors.Red;
                span.TextDecorations = TextDecorations.Underline;
                break;
            default: // Pending
                span.TextColor = Colors.Gray;
                break;
        }
        return span;
    }

    private void OnTextChanged(object? sender, TextChangedEventArgs e)
    {
        string currentText = e.NewTextValue ?? string.Empty;
        string oldText = e.OldTextValue ?? string.Empty;

        // Prevent whole text deletions using shortcuts
        if (currentText.Length < oldText.Length)
        {
            // Don't update anything; ignore the deletion attempt
            return;
        }

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

        // Update ViewModel - this triggers the typing check
        _viewModel.TypedText = TypedTextLabel.Text ?? string.Empty;
    }
}