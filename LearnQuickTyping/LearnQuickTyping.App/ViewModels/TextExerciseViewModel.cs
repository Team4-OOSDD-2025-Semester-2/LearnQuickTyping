using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LearnQuickTyping.Core.Interfaces;
using LearnQuickTyping.Core.Interfaces.Repositories;
using LearnQuickTyping.Core.Interfaces.Services;
using LearnQuickTyping.Core.Models;

namespace LearnQuickTyping.App.ViewModels;

public partial class TextExerciseViewModel : BaseViewModel
{
    private readonly ITextRepository _textRepository;
    private readonly ITypingStatsService _statsService;
    private readonly ITypeControlService _typeControl;
    private readonly IDispatcherTimer _timer;

    private DateTime _startTime;
    private bool _isTiming;

    [ObservableProperty]
    private string _targetText;

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

    [ObservableProperty]
    private bool _isTurnOverlayVisible = false;

    [ObservableProperty]
    private bool _isNotTurnOverlayVisible = true;

    [ObservableProperty]
    private string _completeMessage;

    private bool _wasCorrect;

    public event Action<List<LetterStatus>> RequestLetterUpdate;

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
        ResultMessage = string.Empty;
        IsTurnOverlayVisible = false;
        IsNotTurnOverlayVisible = true;

        LoadNewText();
    }

    private void LoadNewText()
    {
        TargetText = _textRepository.GetRandomText();
        _typeControl.TargetText = TargetText;
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

            double wpm = _statsService.CalculateWordsPerMinuteText(InputText ?? string.Empty, elapsed);
            WpmDisplay = $"Current words per minute: {wpm:F2}";
        }
    }

    [RelayCommand]
    private void CompleteTyping()
    {
        StopTimer();
        var elapsed = DateTime.Now - _startTime;
        double wpm = _statsService.CalculateWordsPerMinuteText(InputText ?? string.Empty, elapsed);

        TimeDisplay = $"Time: {elapsed.TotalSeconds:F2} seconds";
        WpmDisplay = $"Words Per Minute: {wpm:F2}";

        if (InputText == TargetText)
        {
            ResultMessage = "Correct!";
            ResultColor = Colors.Green;
            CompleteMessage = $"Exercise Complete!\n\nTime: {elapsed.TotalSeconds:F2}s\nWPM: {wpm:F2}\n\nPress Enter to continue";
            _wasCorrect = true;
        }
        else
        {
            ResultMessage = "Try Again!";
            ResultColor = Colors.Red;
            CompleteMessage = $"Incorrect!\n\nTime: {elapsed.TotalSeconds:F2}s\nWPM: {wpm:F2}\n\nPress Enter to try again";
            _wasCorrect = false;
        }

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

        if (_wasCorrect)
        {
            // Load new text for next exercise
            LoadNewText();
        }
        else
        {
            // Clear input to try again
            InputText = string.Empty;
        }

        _isTiming = false;
        ResultMessage = string.Empty;
    }
}