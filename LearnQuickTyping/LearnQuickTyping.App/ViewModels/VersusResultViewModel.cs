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
    private string _winnerMessage;

    [ObservableProperty]
    private Color _winnerColor;

    public VersusResultViewModel(IVersusScoreService scoreService)
    {
        _scoreService = scoreService;
    }

    // When final player has finished determine winner
    partial void OnPlayer2ResultChanged(VersusResult value)
    {
        DetermineWinner();
    }

    private void DetermineWinner()
    {
        if (Player1Result == null || Player2Result == null) return;

        string winnerName = _scoreService.DetermineWinner(Player1Result, Player2Result);

        if (winnerName == "Tie")
        {
            WinnerMessage = "It's a Tie!";
            WinnerColor = Colors.Orange;
        }
        else
        {
            WinnerMessage = $"{winnerName} Wins!";
            WinnerColor = Colors.Green;
        }
    }

    [RelayCommand]
    private async Task GoHome()
    {
        await Shell.Current.GoToAsync("///StartPage");
    }
}