using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LearnQuickTyping.App.Views;
using LearnQuickTyping.Core.Interfaces;
using LearnQuickTyping.Core.Interfaces.Repositories;
using LearnQuickTyping.Core.Interfaces.Services;
using LearnQuickTyping.Core.Models;

namespace LearnQuickTyping.App.ViewModels;

public partial class VersusViewModel : BaseViewModel
{
    private readonly IWordRepository _wordRepository;
    private readonly ITypingStatsService _statsService;
    private readonly ITypeControlService _typeControl;
    private readonly IDispatcherTimer _timer;

    private DateTime _startTime;
    private bool _isTiming;
    private string _sharedTargetWord = string.Empty; // Same Word

    // Temporary result
    private VersusResult? _playerOneResult;
    private VersusResult? _playerTwoResult;

    [ObservableProperty]
    private string _currentPlayerName = "Player 1";

    [ObservableProperty]
    private string _targetWord = string.Empty;

    [ObservableProperty]
    private string _inputText = string.Empty;

    [ObservableProperty]
    private string _timeDisplay = "Time: 0.00s";

    [ObservableProperty]
    private string _resultMessage;

    [ObservableProperty]
    private Color _resultColor = Colors.Black;

    [ObservableProperty]

    private bool _isPlayerOneTurn = true;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsNotTurnOverlayVisible))]
    private bool _isTurnOverlayVisible;

    public bool IsNotTurnOverlayVisible => !IsTurnOverlayVisible;

    [ObservableProperty]
    private string _turnMessage = string.Empty;

    public event Action<List<LetterStatus>>? RequestLetterUpdate;

    public VersusViewModel(
        IWordRepository wordRepository,
        ITypingStatsService statsService,
        ITypeControlService typeControl)
    {
        _wordRepository = wordRepository;
        _statsService = statsService;
        _typeControl = typeControl;

        _timer = Application.Current.Dispatcher.CreateTimer();
        _timer.Interval = TimeSpan.FromMilliseconds(50);
        _timer.Tick += OnTimerTick;
    }

    [RelayCommand]
    public void InitializeVersus()
    {
        // Reset
        _isPlayerOneTurn = true;
        _playerOneResult = null;
        _playerTwoResult = null;

        // Use same words
        _sharedTargetWord = _wordRepository.GetRandomWord();

        SetupTurn("Player 1");
    }

    [RelayCommand]
    private void StartTurn() // Start turn connected to overlay
    {
        IsTurnOverlayVisible = false;
    }

    private void SetupTurn(string playerName)
    {
        StopTimer();
        CurrentPlayerName = playerName;

        // Same word for both players
        TargetWord = _sharedTargetWord;

        _typeControl.TargetText = TargetWord;
        _typeControl.TypedText = string.Empty;
        InputText = string.Empty;
        TimeDisplay = "Time: 0.00s";

        RequestLetterUpdate?.Invoke(_typeControl.GetLetterStatuses());

        TurnMessage = $"{playerName}'s Turn \nPress enter to begin.";
        IsTurnOverlayVisible = true;
    }

    partial void OnInputTextChanged(string value)
    {
        if (IsTurnOverlayVisible) return;

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
            TimeDisplay = $"Time: {elapsed.TotalSeconds:F2}s";
        }
    }

    [RelayCommand]
    private async Task CompleteTyping()
    {
        var elapsed = DateTime.Now - _startTime;

        // Validate text
        if (InputText != TargetWord)
        {
            ResultMessage = "Try again!.";
            ResultColor = Colors.Red;
            return;
        }
        StopTimer();

        // Calculate result
        double wpm = _statsService.CalculateWordPerMinute(InputText.Length, elapsed);
        var result = new VersusResult
        {
            PlayerName = CurrentPlayerName,
            TimeTaken = elapsed,
            WordsPerMinute = wpm,
        };

        if (_isPlayerOneTurn)
        {
            // End player 1 turn
            _playerOneResult = result;
            _isPlayerOneTurn = false;

            SetupTurn("Player 2");
        }
        else
        {
            // End player 2 turn
            _playerTwoResult = result;

            var navigationParameter = new Dictionary<string, object>
            {
                { "Player1Result", _playerOneResult! },
                { "Player2Result", _playerTwoResult! }
            };

            await Shell.Current.GoToAsync(nameof(VersusResultView), navigationParameter);
        }
    }
}