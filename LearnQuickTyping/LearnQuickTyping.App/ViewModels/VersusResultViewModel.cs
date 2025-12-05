using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LearnQuickTyping.Core.Interfaces.Services;
using LearnQuickTyping.Core.Models;

namespace LearnQuickTyping.App.ViewModels;

[QueryProperty(nameof(Player1Result), "Player1Result")]
[QueryProperty(nameof(Player2Result), "Player2Result")]
public partial class VersusResultViewModel : BaseViewModel
{
    private readonly IVersusScoreService _scoreService;

    [ObservableProperty]
    private VersusResult _player1Result;

    [ObservableProperty]
    private VersusResult _player2Result;

    [ObservableProperty]
    private bool _isPlayer1Winner;

    [ObservableProperty]
    private bool _isPlayer2Winner;

    [ObservableProperty]
    private bool _isTie;

    public VersusResultViewModel(IVersusScoreService scoreService)
    {
        _scoreService = scoreService;
    }

    partial void OnPlayer1ResultChanged(VersusResult value) => CheckWinner();
    partial void OnPlayer2ResultChanged(VersusResult value) => CheckWinner();

    private void CheckWinner()
    {
        if (Player1Result == null || Player2Result == null) return;

        var winnerName = _scoreService.DetermineWinner(Player1Result, Player2Result);

        if (winnerName == "Tie")
        {
            IsTie = true;
            IsPlayer1Winner = true;
            IsPlayer2Winner = true;
        }
        else
        {
            IsTie = false;
            IsPlayer1Winner = winnerName == Player1Result.PlayerName;
            IsPlayer2Winner = winnerName == Player2Result.PlayerName;
        }
    }

    [RelayCommand]
    private async Task GoHome()
    {
        await Shell.Current.GoToAsync("///StartPage");
    }
}