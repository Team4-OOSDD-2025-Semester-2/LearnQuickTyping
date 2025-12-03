using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LearnQuickTyping.Core.Interfaces;
using LearnQuickTyping.Core.Interfaces.Repositories;
using LearnQuickTyping.Core.Interfaces.Services;
using LearnQuickTyping.Core.Models;
using System.Text.RegularExpressions;

namespace LearnQuickTyping.App.ViewModels;

public partial class TextExerciseViewModel : BaseViewModel
{
    private readonly ITextRepository _textRepository;
    private readonly ITypingStatsService _statsService;
    private readonly ITypeControlService _typeControl;
    private readonly IDispatcherTimer _timer;

    private DateTime _startTime;
    private bool _isTiming;

    private List<string> _sentences = new();
    private int _currentSentenceIndex;

    private int _accumulatedMistakes;
    private int _accumulatedWords;
    private string _allTypedText = string.Empty;

    [ObservableProperty]
    private string _targetText;

    [ObservableProperty]
    private string _inputText;

    [ObservableProperty]
    private string _typedText;

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
    private string _resultMessage;

    [ObservableProperty]
    private Color _resultColor = Colors.Black;

    [ObservableProperty]
    private bool _isTurnOverlayVisible = false;

    [ObservableProperty]
    private bool _isNotTurnOverlayVisible = true;

    [ObservableProperty]
    private string _completeMessage;

    public event Action<List<LetterStatus>> RequestLetterUpdate;
    public event Action<string, string, string> OnLineChanged;

    public TextExerciseViewModel(
        ITextRepository textRepository,
        ITypingStatsService statsService,
        ITypeControlService typeControl)
    {
        _textRepository = textRepository;
        _statsService = statsService;
        _typeControl = typeControl;

        _timer = Application.Current.Dispatcher.CreateTimer();
        _timer.Interval = TimeSpan.FromMilliseconds(50);
        _timer.Tick += OnTimerTick;
    }

    [RelayCommand]
    public void InitializeExercise()
    {
        StopTimer();
        TimeDisplay = "Current time: 0,00s";
        WpmDisplay = "Current Words Per Minute: 0";
        InputText = string.Empty;
        TypedText = string.Empty;
        ResultMessage = string.Empty;
        IsTurnOverlayVisible = false;
        IsNotTurnOverlayVisible = true;

        _accumulatedMistakes = 0;
        _accumulatedWords = 0;
        _allTypedText = string.Empty;
        _statsService.ResetMistakes();

        LoadNewText();
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

            InputText = string.Empty;
            TypedText = string.Empty;

            string prev = _currentSentenceIndex > 0 ? _sentences[_currentSentenceIndex - 1] : "";
            string next = _currentSentenceIndex + 1 < _sentences.Count ? _sentences[_currentSentenceIndex + 1] : "";

            OnLineChanged?.Invoke(prev, TargetText, next);
            ProgressDisplay = $"Sentence: {_currentSentenceIndex + 1}/{_sentences.Count}";

            RequestLetterUpdate?.Invoke(_typeControl.GetLetterStatuses());
        }
        else
        {
            CompleteTyping();
        }
    }

    partial void OnTypedTextChanged(string value)
    {
        if (string.IsNullOrEmpty(TargetText)) return;

        if (!_isTiming && !string.IsNullOrEmpty(value))
        {
            StartTimer();
        }

        string safeValue = value ?? string.Empty;

        _typeControl.CheckTyping(safeValue);
        _statsService.TrackMistakes(safeValue, TargetText);

        RequestLetterUpdate?.Invoke(_typeControl.GetLetterStatuses());

        if (safeValue.Length >= TargetText.Length)
        {
            CompleteSentence();
        }
    }

    private void CompleteSentence()
    {
        _accumulatedMistakes += _statsService.GetMistakeCount();
        _accumulatedWords += CountWords(TargetText);
        _allTypedText += TypedText + " ";

        _statsService.ResetMistakes();

        _currentSentenceIndex++;
        LoadCurrentSentence();
    }

    private int CountWords(string text)
    {
        return string.IsNullOrWhiteSpace(text) ? 0 : text.Split(' ', StringSplitOptions.RemoveEmptyEntries).Length;
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

    private void OnTimerTick(object sender, EventArgs e)
    {
        if (_isTiming)
        {
            var elapsed = DateTime.Now - _startTime;
            TimeDisplay = $"Current time: {elapsed.TotalSeconds:F2}s";

            int currentWords = CountWords(TypedText);
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
    }

    [RelayCommand]
    private void CompleteTyping()
    {
        StopTimer();
        var elapsed = DateTime.Now - _startTime;

        double wpm = elapsed.TotalMinutes > 0 ? _accumulatedWords / elapsed.TotalMinutes : 0;

        int totalChars = _allTypedText.Length;
        int accuracy = 100;
        if (totalChars > 0)
        {
            double errorRate = (double)_accumulatedMistakes / totalChars;
            accuracy = Math.Max(0, (int)((1 - errorRate) * 100));
        }

        TimeDisplay = $"Time: {elapsed.TotalSeconds:F2} seconds";
        WpmDisplay = $"Words Per Minute: {wpm:F2}";
        MistakeCountDisplay = $"Mistakes: {_accumulatedMistakes}";
        AccuracyDisplay = $"Accuracy: {accuracy}%";

        CompleteMessage = $"Exercise Complete!\n\nTime: {elapsed.TotalSeconds:F2}s\nWPM: {wpm:F2}\nMistakes: {_accumulatedMistakes}\nAccuracy: {accuracy}%\n\nPress Enter to continue";

        // Always show result overlay
        IsTurnOverlayVisible = true;
        IsNotTurnOverlayVisible = false;
    }

    [RelayCommand]
    private void StartExercise()
    {
        // Hide overlay
        IsTurnOverlayVisible = false;
        IsNotTurnOverlayVisible = true;
        InitializeExercise();
    }
}