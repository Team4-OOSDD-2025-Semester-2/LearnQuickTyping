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

    // Houdt alle regels van de geselecteerde tekst
    private string[] _currentLyricsLines;

    // Houdt bij welke regel momenteel getoond wordt
    private int _currentLineIndex = 0;


    [ObservableProperty]
    private ObservableCollection<string> _lyricsTitles;

    [ObservableProperty]
    private string _selectedLyricsTitle;

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

    public object InitializeVersusCommand { get; internal set; }

    public event Action<List<LetterStatus>> RequestLetterUpdate;

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

        // Haal alle regels van de gekozen tekst op
        _currentLyricsLines = _lyricsRepository.GetLyricsByIndex(index);
        _currentLineIndex = 0;

        // Toon alleen de eerste regel
        TargetWord = _currentLyricsLines[_currentLineIndex];

        _typeControl.TargetText = TargetWord;
        InputText = string.Empty;

        RequestLetterUpdate?.Invoke(_typeControl.GetLetterStatuses());
    }


    [RelayCommand]
    public void InitializeExercise()
    {
        if (SelectedLyricsTitle == null)
        {
            ResultMessage = "Select a text first!";
            ResultColor = Colors.Red;
            return;
        }

        StopTimer();
        TimeDisplay = "Current time: 0,00s";
        WpmDisplay = "Current Words Per Minute: 0";
        InputText = string.Empty;
        ResultMessage = string.Empty;

        LoadNewLyric();
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

            double wpm = _statsService.CalculateWordPerMinute(InputText?.Length ?? 0, elapsed);
            WpmDisplay = $"Current words per minute: {wpm:F2}";
        }
    }

    [RelayCommand]
    private void CompleteTyping()
    {
        StopTimer();
        var elapsed = DateTime.Now - _startTime;
        double wpm = _statsService.CalculateWordPerMinute(InputText?.Length ?? 0, elapsed);

        TimeDisplay = $"Time: {elapsed.TotalSeconds:F2} seconds";
        WpmDisplay = $"Words Per Minute: {wpm:F2}";

        if (InputText == TargetWord)
        {
            ResultMessage = "Correct!";
            ResultColor = Colors.Green;
            InputText = string.Empty;
            _currentLineIndex++;

            if (_currentLineIndex < _currentLyricsLines.Length)
            {
                // volgende regel tonen
                TargetWord = _currentLyricsLines[_currentLineIndex];
                _typeControl.TargetText = TargetWord;

                RequestLetterUpdate?.Invoke(_typeControl.GetLetterStatuses());
            }
            else
            {
                // tekst is klaar
                ResultMessage = "Text complete!";
                ResultColor = Colors.Green;
            }
        }
        else
        {
            ResultMessage = "Try Again!";
            ResultColor = Colors.Red;
            InputText = string.Empty;
        }

    }

}