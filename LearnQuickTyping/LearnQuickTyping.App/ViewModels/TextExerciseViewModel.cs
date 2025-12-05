using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LearnQuickTyping.Core.Interfaces;
using LearnQuickTyping.Core.Interfaces.Repositories;
using LearnQuickTyping.Core.Interfaces.Services;
using LearnQuickTyping.Core.Models;
using System.Text.RegularExpressions;
using LearnQuickTyping.App.Views;
using Microsoft.Maui.Layouts;


namespace LearnQuickTyping.App.ViewModels;

public partial class TextExerciseViewModel : BaseViewModel
{
    private readonly ITextRepository _textRepository;
    private readonly ITypingStatsService _statsService;
    private readonly ITypeControlService _typeControl;
    private readonly IDispatcherTimer _timer;

    private DateTime _startTime;
    private bool _isTiming;
    private bool _isTransitioning; // Prevent updates during sentence transition

    private List<string> _sentences = new();
    private int _currentSentenceIndex;

    private int _accumulatedMistakes;
    private int _accumulatedWords;
    private string _allTypedText = string.Empty;

    // Temporary result
    private TextResult? _result;

    public event Action? RequestInputFocus;

    [ObservableProperty]
    private string _targetText = string.Empty;

    [ObservableProperty]
    private string _inputText = string.Empty;

    [ObservableProperty]
    private string _typedText = string.Empty;

    [ObservableProperty]
    private string _timeDisplay = "Current time: 0,00s";

    [ObservableProperty]
    private string _wpmDisplay = "Current Words Per Minute: 0";

    [ObservableProperty]
    private string _mistakeCountDisplay = "Mistakes: 0";

    [ObservableProperty]
    private string _accuracyDisplay = "Accuracy: 100%";

    [ObservableProperty]
    private string _progressDisplay = "Sentence: 0/0";

    [ObservableProperty]
    private string _resultMessage = string.Empty;

    [ObservableProperty]
    private Color _resultColor = Colors.Black;

    [ObservableProperty]
    private bool _isStartScreenVisible = true;

    [ObservableProperty]
    private bool _isNotStartScreenVisible = false;

    [ObservableProperty]
    private string _completeMessage = string.Empty;

    public event Action<List<LetterStatus>>? RequestLetterUpdate;
    public event Action<string, string, string>? OnLineChanged;

    public TextExerciseViewModel(
        ITextRepository textRepository,
        ITypingStatsService statsService,
        ITypeControlService typeControl)
    {
        _textRepository = textRepository;
        _statsService = statsService;
        _typeControl = typeControl;

        _timer = Application.Current!.Dispatcher.CreateTimer();
        _timer.Interval = TimeSpan.FromMilliseconds(100); // Reduced frequency: 100ms instead of 50ms
        _timer.Tick += OnTimerTick;
    }

    [RelayCommand]
    public void InitializeExercise()
    {
        _isTransitioning = true;

        StopTimer();
        TimeDisplay = "Current time: 0,00s";
        WpmDisplay = "Current Words Per Minute: 0";
        MistakeCountDisplay = "Mistakes: 0";
        AccuracyDisplay = "Accuracy: 100%";
        InputText = string.Empty;
        TypedText = string.Empty;
        ResultMessage = string.Empty;

        // Force property change to trigger focus
        IsStartScreenVisible = false;
        IsNotStartScreenVisible = true;

        _accumulatedMistakes = 0;
        _accumulatedWords = 0;
        _allTypedText = string.Empty;
        _statsService.ResetMistakes();

        LoadNewText();

        // Now set it back to show the start screen and trigger PropertyChanged
        IsStartScreenVisible = true;
        IsNotStartScreenVisible = false;

        _isTransitioning = false;
    }

    private void LoadNewText()
    {
        string fullText = _textRepository.GetRandomText();
        _sentences = SplitTextIntoSentences(fullText);

        if (_sentences.Count == 0)
        {
            _sentences.Add("Error loading text. Please try again.");
        }

        _currentSentenceIndex = 0;
        LoadCurrentSentence();
    }

    private List<string> SplitTextIntoSentences(string text)
    {
        var sentences = new List<string>();
        string pattern = @"(?<=[.!?])\s+";
        var parts = Regex.Split(text, pattern);

        foreach (var part in parts)
        {
            if (!string.IsNullOrWhiteSpace(part))
                sentences.Add(part.Trim());
        }
        return sentences;
    }

    private void LoadCurrentSentence()
    {
        if (_currentSentenceIndex < _sentences.Count)
        {
            TargetText = _sentences[_currentSentenceIndex];
            _typeControl.TargetText = TargetText;
            _typeControl.TypedText = string.Empty;

            string prev = _currentSentenceIndex > 0 ? _sentences[_currentSentenceIndex - 1] : "";
            string next = _currentSentenceIndex + 1 < _sentences.Count ? _sentences[_currentSentenceIndex + 1] : "";

            ProgressDisplay = $"Sentence: {_currentSentenceIndex + 1}/{_sentences.Count}";

            // Notify view to update display - do this before clearing input
            OnLineChanged?.Invoke(prev, TargetText, next);
            RequestLetterUpdate?.Invoke(_typeControl.GetLetterStatuses());
        }
        else
        {
            CompleteTyping();
        }
    }

    partial void OnTypedTextChanged(string value)
    {
        // Skip processing during transitions to prevent cascading updates
        if (_isTransitioning || string.IsNullOrEmpty(TargetText))
            return;

        string safeValue = value ?? string.Empty;

        if (!_isTiming && !string.IsNullOrEmpty(safeValue))
        {
            StartTimer();
        }

        _typeControl.CheckTyping(safeValue);
        _statsService.TrackMistakes(safeValue, TargetText);

        RequestLetterUpdate?.Invoke(_typeControl.GetLetterStatuses());

        // Check for sentence completion
        if (safeValue.Length >= TargetText.Length)
        {
            CompleteSentence();
        }
    }

    private void CompleteSentence()
    {
        _isTransitioning = true;

        _accumulatedMistakes += _statsService.GetMistakeCount();
        _accumulatedWords += _textRepository.CountWords(TargetText);
        _allTypedText += TypedText + " ";

        _statsService.ResetMistakes();

        _currentSentenceIndex++;

        // Clear input fields before loading new sentence
        InputText = string.Empty;
        TypedText = string.Empty;

        // Small delay to let UI settle before loading next sentence
        Application.Current?.Dispatcher.Dispatch(() =>
        {
            LoadCurrentSentence();
            _isTransitioning = false;
        });
    }

    private void StartTimer()
    {
        _startTime = DateTime.Now;
        _isTiming = true;
        _timer.Start();
    }

    private void StopTimer()
    {
        _isTiming = false;
        _timer.Stop();
    }

    private void OnTimerTick(object? sender, EventArgs e)
    {
        if (!_isTiming || _isTransitioning)
            return;

        var elapsed = DateTime.Now - _startTime;
        TimeDisplay = $"Current time: {elapsed.TotalSeconds:F2}s";

        int currentWords = _textRepository.CountWords(TypedText);
        int totalWords = _accumulatedWords + currentWords;
        double wpm = elapsed.TotalMinutes > 0 ? totalWords / elapsed.TotalMinutes : 0;

        WpmDisplay = $"Current words per minute: {wpm:F2}";

        int currentMistakes = _statsService.GetMistakeCount();
        int totalMistakes = _accumulatedMistakes + currentMistakes;
        MistakeCountDisplay = $"Mistakes: {totalMistakes}";

        int totalChars = _allTypedText.Length + (TypedText?.Length ?? 0);
        int accuracy = 100;
        if (totalChars > 0)
        {
            double errorRate = (double)totalMistakes / totalChars;
            accuracy = Math.Max(0, (int)((1 - errorRate) * 100));
        }
        AccuracyDisplay = $"Accuracy: {accuracy}%";
    }

    [RelayCommand]
    private async Task CompleteTyping()
    {
        StopTimer();
        var elapsed = DateTime.Now - _startTime;

        // Include current sentence's words in the final count
        int currentWords = _textRepository.CountWords(TypedText);
        int totalWords = _accumulatedWords + currentWords;
        
        double wpm = elapsed.TotalMinutes > 0 ? totalWords / elapsed.TotalMinutes : 0;

        // Include current sentence's mistakes in the final count
        int currentMistakes = _statsService.GetMistakeCount();
        int totalMistakes = _accumulatedMistakes + currentMistakes;
        
        // Include current sentence's characters in the final count
        int totalChars = _allTypedText.Length + (TypedText?.Length ?? 0);
        int accuracy = 100;
        if (totalChars > 0)
        {
            double errorRate = (double)totalMistakes / totalChars;
            accuracy = Math.Max(0, (int)((1 - errorRate) * 100));
        }

        // Combine all text (accumulated + current)
        string fullTypedText = _allTypedText + (TypedText ?? string.Empty);
        string fullOriginalText = string.Join(" ", _sentences);

        var result = new TextResult
        {
            WordsPerMinute = wpm,
            TimeTaken = elapsed,
            Errors = totalMistakes,
            Accuracy = accuracy,
            OriginalText = fullOriginalText,
            TypedText = fullTypedText
        };

        _result = result;

        var navigationParameter = new Dictionary<string, object>
        {
            { "Result", _result! }
        };

        await Shell.Current.GoToAsync(nameof(TextExerciseResultView), navigationParameter);
    }

    [RelayCommand]
    private void StartExercise()
    {
        IsStartScreenVisible = false;
        IsNotStartScreenVisible = true;
    }
}