using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LearnQuickTyping.App.Views;
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
    private readonly IExerciseResultSaveService _saveService;

    private DateTime _startTime;
    private bool _isTiming;

    private string[] _currentLyricsLines;
    private int _currentLineIndex;
    private int _correctLines;
    private int _totalLines;
    private TimeSpan _totalElapsedTime;
    private int _totalCharactersTyped;
    private int _totalErrors;

    [ObservableProperty]
    private ObservableCollection<string> _lyricsTitles;

    [ObservableProperty]
    private string _selectedLyricsTitle;

    [ObservableProperty]
    private string _targetWord;

    [ObservableProperty]
    private string _inputText;

    [ObservableProperty]
    private string _timeDisplay = "Current time: 0.00s";

    [ObservableProperty]
    private string _wpmDisplay = "Words per minute: 0";

    [ObservableProperty]
    private string _resultMessage;

    [ObservableProperty]
    private Color _resultColor = Colors.Black;

    [ObservableProperty]
    private bool _isStartButtonVisible = false;

    [ObservableProperty]
    private bool _isExerciseVisible = false;

    public event Action<List<LetterStatus>> RequestLetterUpdate;
    public event Action ExerciseStarted;
    public event Action ExerciseCompleted;
    public event Action NavigateToResults;

    public LyricsExerciseViewModel(
        ILyricsRepository lyricsRepository,
        ITypingStatsService statsService,
        ITypeControlService typeControl,
        IExerciseResultSaveService saveService)
    {
        _lyricsRepository = lyricsRepository;
        _statsService = statsService;
        _typeControl = typeControl;
        _saveService = saveService;

        LyricsTitles = new ObservableCollection<string>(_lyricsRepository.GetAllLyricsTitles());

        _timer = Application.Current.Dispatcher.CreateTimer();
        _timer.Interval = TimeSpan.FromMilliseconds(50);
        _timer.Tick += OnTimerTick;
    }

    partial void OnSelectedLyricsTitleChanged(string value)
    {
        if (string.IsNullOrEmpty(value)) return;

        int index = LyricsTitles.IndexOf(value);
        _currentLyricsLines = _lyricsRepository.GetLyricsByIndex(index);
        _currentLineIndex = 0;
        _correctLines = 0;
        _totalLines = _currentLyricsLines.Length;

        TargetWord = _currentLyricsLines[_currentLineIndex];
        _typeControl.TargetText = TargetWord;
        InputText = string.Empty;

        IsStartButtonVisible = true;
        IsExerciseVisible = false;

        RequestLetterUpdate?.Invoke(_typeControl.GetLetterStatuses());
    }

    [RelayCommand]
    public void StartExercise()
    {
        IsStartButtonVisible = false;
        IsExerciseVisible = true;

        _totalElapsedTime = TimeSpan.Zero;
        _totalCharactersTyped = 0;
        _totalErrors = 0;
        _isTiming = false;

        ExerciseStarted?.Invoke();
    }

    partial void OnInputTextChanged(string value)
    {
        if (!_isTiming && !string.IsNullOrEmpty(value))
        {
            _startTime = DateTime.Now;
            _isTiming = true;
            _timer.Start();
        }

        _typeControl.CheckTyping(value ?? string.Empty);
        RequestLetterUpdate?.Invoke(_typeControl.GetLetterStatuses());
    }

    private void OnTimerTick(object sender, EventArgs e)
    {
        if (!_isTiming) return;

        var elapsed = DateTime.Now - _startTime;
        TimeDisplay = $"Current time: {elapsed.TotalSeconds:F2}s";

        int totalChars = _totalCharactersTyped + (InputText?.Length ?? 0);
        double wpm = _statsService.CalculateWordsPerMinuteText(
           totalChars,
           _totalElapsedTime + elapsed);
        WpmDisplay = $"Words per minute: {wpm:F2}";
    }

    [RelayCommand]
    public void CompleteTyping()
    {
        StopTimer();
        var elapsed = DateTime.Now - _startTime;

        _totalElapsedTime += elapsed;
        _totalCharactersTyped += InputText?.Length ?? 0;

        bool isCorrect = InputText == TargetWord;
        ResultMessage = isCorrect ? "Correct!" : "Incorrect - moving to next line";
        ResultColor = isCorrect ? Colors.Green : Colors.Orange;

        if (isCorrect)
        {
            _correctLines++;
        }
        else
        {
            // Count errors for this line
            string typed = InputText ?? string.Empty;
            for (int i = 0; i < Math.Min(TargetWord.Length, typed.Length); i++)
            {
                if (TargetWord[i] != typed[i]) _totalErrors++;
            }
            _totalErrors += Math.Abs(TargetWord.Length - typed.Length);
        }

        InputText = string.Empty;
        _currentLineIndex++;

        if (_currentLineIndex < _currentLyricsLines.Length)
        {
            TargetWord = _currentLyricsLines[_currentLineIndex];
            _typeControl.TargetText = TargetWord;
            RequestLetterUpdate?.Invoke(_typeControl.GetLetterStatuses());
            ResultMessage = string.Empty;
            _isTiming = false;
        }
        else
        {
            // If exercise completed, show result
            ShowResult();
            IsExerciseVisible = false;

            MainThread.BeginInvokeOnMainThread(() =>
            {
                NavigateToResults?.Invoke();
            });
        }
    }

    private void StopTimer()
    {
        _isTiming = false;
        _timer.Stop();
    }

    private async Task ShowResult()
    {
        double accuracy = (_correctLines / (double)_totalLines) * 100;
        double averageWpm = _statsService.CalculateWordsPerMinuteText(
           _totalCharactersTyped,
           _totalElapsedTime);

        TimeDisplay = $"Total time: {_totalElapsedTime.TotalSeconds:F2}s";
        WpmDisplay = $"Average WPM: {averageWpm:F2}";

        string grade = accuracy switch
        {
            >= 90 => "Excellent! 🌟",
            >= 75 => "Great job! 👍",
            >= 60 => "Good effort! 💪",
            >= 40 => "Keep practicing! 📝",
            _ => "Try again! 🎯"
        };

        ResultMessage = $"Score: {_correctLines}/{_totalLines} correct ({accuracy:F1}%)\n\n{grade}";
        ResultColor = accuracy >= 75 ? Colors.Green :
                      accuracy >= 50 ? Colors.Orange : Colors.Red;

        // Save result to database
        await _saveService.SaveResultAsync(
            averageWpm,
            (int)accuracy,
            _totalElapsedTime,
            _totalErrors,
            ExerciseType.Karaoke,
            DifficultyLevel.None);
    }

    [RelayCommand]
    public async void NewSong()
    {
        await Shell.Current.GoToAsync(nameof(LyricsExercise));
    }

    [RelayCommand]
    public async Task GoHome()
    {
        await Shell.Current.GoToAsync("///StartPage");
    }
}