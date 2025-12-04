using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LearnQuickTyping.Core.Data.Repositories;
using LearnQuickTyping.Core.Interfaces;
using LearnQuickTyping.Core.Interfaces.Repositories;
using LearnQuickTyping.Core.Interfaces.Services;
using LearnQuickTyping.Core.Models;
using System.Collections.ObjectModel;

namespace LearnQuickTyping.App.ViewModels;

public partial class LyricsExerciseViewModel : BaseViewModel
{
    private readonly ILyricsRepository _lyricsRepository;
    private readonly ITypingStatsService _statsService;
    private readonly ITypeControlService _typeControl;
    private readonly IDispatcherTimer _timer;

    private DateTime _startTime;
    private bool _isTiming;

    // Holds all lines of the selected text
    private string[] _currentLyricsLines;

    // Tracks which line is currently being displayed
    private int _currentLineIndex = 0;

    // Tracks score
    private int _correctLines = 0;
    private int _totalLines = 0;

    // Track total time across all lines
    private TimeSpan _totalElapsedTime = TimeSpan.Zero;

    [ObservableProperty]
    private ObservableCollection<string> _lyricsTitles;

    [ObservableProperty]
    private string? _selectedLyricsTitle;

    [ObservableProperty]
    private string? _targetWord;

    [ObservableProperty]
    private string? _inputText;

    [ObservableProperty]
    private string? _timeDisplay = "Current time: 0,00s";

    [ObservableProperty]
    private string? _wpmDisplay = "Current Words Per Minute: 0";

    [ObservableProperty]
    private string? _resultMessage;

    [ObservableProperty]
    private Color _resultColor = Colors.Black;

    [ObservableProperty]
    private bool _isStartButtonVisible = false;

    [ObservableProperty]
    private bool _isExerciseVisible = false;

    [ObservableProperty]
    private bool _isResultVisible = false;

    public event Action<List<LetterStatus>> RequestLetterUpdate;
    public event Action ExerciseStarted;
    public event Action ExerciseCompleted;

    public LyricsExerciseViewModel(
        ILyricsRepository lyricsRepository,
        ITypingStatsService statsService,
        ITypeControlService typeControl)
    {
        _lyricsRepository = lyricsRepository;
        _statsService = statsService;
        _typeControl = typeControl;

        LyricsTitles = new ObservableCollection<string>(_lyricsRepository.GetAllLyricsTitles());

        _timer = Application.Current.Dispatcher.CreateTimer();
        _timer.Interval = TimeSpan.FromMilliseconds(50);
        _timer.Tick += OnTimerTick;
    }

    partial void OnSelectedLyricsTitleChanged(string value)
    {
        if (value == null)
            return;

        int index = LyricsTitles.IndexOf(value);

        var lines = _lyricsRepository.GetLyricsByIndex(index);

        // Fetch all lines of the chosen text
        _currentLyricsLines = _lyricsRepository.GetLyricsByIndex(index);
        _currentLineIndex = 0;

        // Reset score tracking
        _correctLines = 0;
        _totalLines = _currentLyricsLines.Length;

        // Display only the first line
        TargetWord = _currentLyricsLines[_currentLineIndex];

        _typeControl.TargetText = TargetWord;
        InputText = string.Empty;

        // Show start button and hide exercise
        IsStartButtonVisible = true;
        IsExerciseVisible = false;

        RequestLetterUpdate?.Invoke(_typeControl.GetLetterStatuses());
    }

    [RelayCommand]
    public void InitializeExercise()
    {
        if (SelectedLyricsTitle == null)
        {
            ResultMessage = "Select a lyric first!";
            ResultColor = Colors.Red;
            return;
        }

        StopTimer();
        TimeDisplay = "Current time: 0,00s";
        WpmDisplay = "Current Words Per Minute: 0";
        InputText = string.Empty;
        ResultMessage = string.Empty;
        IsStartButtonVisible = false;
        IsExerciseVisible = false;
        IsResultVisible = false;

        LoadNewLyric();
    }

    [RelayCommand]
    public void StartExercise()
    {
        // Hide picker and start button
        IsStartButtonVisible = false;
        IsExerciseVisible = true;

        // Reset total time when starting new exercise
        _totalElapsedTime = TimeSpan.Zero;

        ExerciseStarted?.Invoke();
    }

    private void LoadNewLyric()
    {
        OnSelectedLyricsTitleChanged(SelectedLyricsTitle);
    }

    partial void OnInputTextChanged(string value)
    {
        if (!_isTiming && !string.IsNullOrEmpty(value))
        {
            StartTimer();
        }

        _typeControl.CheckTyping(value ?? string.Empty);

        RequestLetterUpdate?.Invoke(_typeControl.GetLetterStatuses());
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

            double wpm = _statsService.CalculateWordsPerMinuteText(InputText ?? string.Empty, elapsed);
            WpmDisplay = $"Current words per minute: {wpm:F2}";
        }
    }

    [RelayCommand]
    private void CompleteTyping()
    {
        StopTimer();
        var elapsed = DateTime.Now - _startTime;

        // Add this line's time to total
        _totalElapsedTime += elapsed;

        double wpm = _statsService.CalculateWordsPerMinuteText(InputText ?? string.Empty, elapsed);

        TimeDisplay = $"Time: {elapsed.TotalSeconds:F2} seconds";
        WpmDisplay = $"Words Per Minute: {wpm:F2}";

        // Check if input matches target
        bool isCorrect = InputText == TargetWord;

        if (isCorrect)
        {
            ResultMessage = "Correct!";
            ResultColor = Colors.Green;
            _correctLines++;
        }
        else
        {
            ResultMessage = "Incorrect - but moving to next line";
            ResultColor = Colors.Orange;
        }

        // Always move to next line, regardless of correctness
        InputText = string.Empty;
        _currentLineIndex++;

        if (_currentLineIndex < _currentLyricsLines.Length)
        {
            // Show next line
            TargetWord = _currentLyricsLines[_currentLineIndex];
            _typeControl.TargetText = TargetWord;

            RequestLetterUpdate?.Invoke(_typeControl.GetLetterStatuses());

            // Clear result message after a short delay and restart timer
            ResultMessage = string.Empty;
        }
        else
        {
            // All lines completed - show result
            ShowResult(_totalElapsedTime, wpm);

            IsExerciseVisible = false;
            IsResultVisible = true;

            ExerciseCompleted?.Invoke();
        }
    }

    private void ShowResult(TimeSpan totalTime, double averageWpm)
    {
        double accuracy = (_correctLines / (double)_totalLines) * 100;

        string grade = accuracy switch
        {
            >= 90 => "Excellent! 🌟",
            >= 75 => "Great job! 👍",
            >= 60 => "Good effort! 💪",
            >= 40 => "Keep practicing! 📝",
            _ => "Try again! 🎯"
        };

        // Single line version for better display
        ResultMessage = $"Score: {_correctLines}/{_totalLines} ({accuracy:F1}%) | Time: {totalTime.TotalSeconds:F2}s | WPM: {averageWpm:F2} | {grade}";

        ResultColor = accuracy >= 75 ? Colors.Green :
                      accuracy >= 50 ? Colors.Orange : Colors.Red;
    }

    [RelayCommand]
    public void ContinueToNextText()
    {
        // Hide result
        IsResultVisible = false;

        // Reset status
        ResultMessage = string.Empty;
        TargetWord = string.Empty;
        InputText = string.Empty;

        // Show picker and Start button again
        IsStartButtonVisible = true;
        IsExerciseVisible = false;

        // Reset current line index
        _currentLineIndex = 0;
        _currentLyricsLines = null;

        // Event for UI if needed
        ExerciseCompleted?.Invoke();
    }
}