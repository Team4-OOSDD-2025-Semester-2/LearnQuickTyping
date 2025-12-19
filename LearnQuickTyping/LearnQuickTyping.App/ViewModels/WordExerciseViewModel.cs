using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LearnQuickTyping.Core.Interfaces;
using LearnQuickTyping.Core.Interfaces.Repositories;
using LearnQuickTyping.Core.Interfaces.Services;
using LearnQuickTyping.Core.Models;

namespace LearnQuickTyping.App.ViewModels;

public partial class WordExerciseViewModel : BaseViewModel
{
    private readonly IWordRepository _wordRepository;
    private readonly ITypingStatsService _statsService;
    private readonly ITypeControlService _typeControl;
    private readonly IDispatcherTimer _timer;
    private readonly IExerciseResultSaveService _saveService;

    private DateTime _startTime;
    private bool _isTiming;

    [ObservableProperty]
    private string _targetWord;

    [ObservableProperty]
    private string _inputText;

    [ObservableProperty]
    private string _timeDisplay = "Current time: 0,00s";

    [ObservableProperty]
    private string _wpmDisplay = "Current Words Per Minute: 0";

    [ObservableProperty]
    private string _resultMessage;

    [ObservableProperty]
    private Color _resultColor = Colors.Black;

    public event Action<List<LetterStatus>> RequestLetterUpdate;

    public WordExerciseViewModel(
        IWordRepository wordRepository,
        ITypingStatsService statsService,
        ITypeControlService typeControl,
        IExerciseResultSaveService saveService)
    {
        _wordRepository = wordRepository;
        _statsService = statsService;
        _typeControl = typeControl;
        _saveService = saveService;

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
        ResultMessage = string.Empty;

        LoadNewWord();
    }

    private void LoadNewWord()
    {
        TargetWord = _wordRepository.GetRandomWord();
        _typeControl.TargetText = TargetWord;
        _typeControl.TypedText = string.Empty;
        InputText = string.Empty;

        RequestLetterUpdate?.Invoke(_typeControl.GetLetterStatuses());
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

            double wpm = _statsService.CalculateWordsPerMinuteSingleWord(InputText?.Length ?? 0, elapsed);
            WpmDisplay = $"Current words per minute: {wpm:F2}";
        }
    }

    [RelayCommand]
    private async Task CompleteTyping()
    {
        StopTimer();
        var elapsed = DateTime.Now - _startTime;
        double wpm = _statsService.CalculateWordsPerMinuteSingleWord(InputText?.Length ?? 0, elapsed);

        TimeDisplay = $"Time: {elapsed.TotalSeconds:F2} seconds";
        WpmDisplay = $"Words Per Minute: {wpm:F2}";

        if (InputText == TargetWord)
        {
            ResultMessage = "Correct!";
            ResultColor = Colors.Green;

            await _saveService.SaveResultAsync(
                wpm, 100, elapsed, 0,
                ExerciseType.Word, DifficultyLevel.None);

            LoadNewWord();
        }
        else
        {
            ResultMessage = "Try Again!";
            ResultColor = Colors.Red;

            // Calculate errors including length difference
            int errors = CalculateErrors(TargetWord, InputText ?? string.Empty);
            int accuracy = CalculateAccuracy(TargetWord, InputText ?? string.Empty, errors);

            await _saveService.SaveResultAsync(
                wpm, accuracy, elapsed, errors,
                ExerciseType.Word, DifficultyLevel.None);

            InputText = string.Empty;
            _isTiming = false;
        }
    }

    private int CalculateErrors(string target, string typed)
    {
        int errors = 0;

        // Count character mismatch
        int minLength = Math.Min(target.Length, typed.Length);
        for (int i = 0; i < minLength; i++)
        {
            if (target[i] != typed[i])
                errors++;
        }

        // Count length difference as mistake
        errors += Math.Abs(target.Length - typed.Length);

        return errors;
    }

    private int CalculateAccuracy(string target, string typed, int errors)
    {
        if (target.Length == 0) return 0;

        int correctChars = target.Length - errors;
        return Math.Max(0, (int)((correctChars / (double)target.Length) * 100));
    }
}