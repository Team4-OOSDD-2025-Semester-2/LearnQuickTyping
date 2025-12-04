using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LearnQuickTyping.Core.Interfaces.Services;
using LearnQuickTyping.Core.Models;


namespace LearnQuickTyping.App.ViewModels
{
    [QueryProperty(nameof(Result), "Result")]
    public partial class TextExerciseResultViewModel : BaseViewModel
    {
        private readonly ITextEcerciseScoreService _scoreService;

        [ObservableProperty]
        private TextResult? _result;

        public TextExerciseResultViewModel(ITextEcerciseScoreService scoreService)
        {
            _scoreService = scoreService;
        }
                
        [RelayCommand]
        private async Task GoHome()
        {
            await Shell.Current.GoToAsync("///StartPage");
        }
    }
}
