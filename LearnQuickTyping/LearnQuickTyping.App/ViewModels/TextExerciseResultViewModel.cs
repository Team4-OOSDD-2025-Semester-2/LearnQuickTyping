using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LearnQuickTyping.App.Views;
using LearnQuickTyping.Core.Interfaces;
using LearnQuickTyping.Core.Interfaces.Services;
using LearnQuickTyping.Core.Models;


namespace LearnQuickTyping.App.ViewModels
{
    [QueryProperty(nameof(Result), "Result")]
    [QueryProperty(nameof(Difficulty), "Difficulty")]
    public partial class TextExerciseResultViewModel : BaseViewModel
    {
        private readonly ITextEcerciseScoreService _scoreService;
        private readonly ITypeControlService _typeControl;

        [ObservableProperty]
        private TextResult? _result;

        [ObservableProperty]
        private string _difficulty = string.Empty;

        [ObservableProperty]
        private FormattedString _typedTextFormatted = new FormattedString();

        [ObservableProperty]
        private FormattedString _originalTextFormatted = new FormattedString();

        public TextExerciseResultViewModel(
            ITextEcerciseScoreService scoreService,
            ITypeControlService typeControl)
        {
            _scoreService = scoreService;
            _typeControl = typeControl;
        }

        partial void OnResultChanged(TextResult? value)
        {
            var mainPage = Application.Current?.Windows.FirstOrDefault()?.Page;

            if (Enum.TryParse<DifficultyLevel>(Difficulty, out var difficultyLevel) && ((value.WordsPerMinute >= 50 && value.Accuracy >= 80) || (value.WordsPerMinute >= 47 && value.Accuracy >= 85) || (value.WordsPerMinute >= 43 && value.Accuracy >= 90) || (value.WordsPerMinute >= 38 && value.Accuracy >= 95)) && difficultyLevel != DifficultyLevel.Expert)
            {
                mainPage?.DisplayAlert(
                    "Well Done!", $"You are doing great, we suggest you move up a level!", "OK");
                GenerateMarkedTexts(value);
            }
            else
            {
                if (value != null)
                {
                    GenerateMarkedTexts(value);
                }
            }

        }

        private void GenerateMarkedTexts(TextResult result)
        {
            // Set up the type control service
            _typeControl.TargetText = result.OriginalText ?? string.Empty;
            _typeControl.TypedText = result.TypedText ?? string.Empty;

            // Get letter statuses from the service
            var letterStatuses = _typeControl.GetLetterStatuses();

            var typedFormatted = new FormattedString();
            var originalFormatted = new FormattedString();

            string typed = result.TypedText ?? string.Empty;

            foreach (var letterStatus in letterStatuses)
            {
                var span = new Span { Text = letterStatus.Character.ToString() };
                
                span.TextColor = letterStatus.Status switch
                {
                    Status.Correct => Colors.Green,
                    Status.Incorrect => Colors.Red,
                    Status.Pending => Colors.Gray,
                    _ => Colors.Black
                };
                
                originalFormatted.Spans.Add(span);
            }

            for (int i = 0; i < typed.Length; i++)
            {
                var span = new Span { Text = typed[i].ToString() };
                
                if (i < letterStatuses.Count)
                {
                    span.TextColor = letterStatuses[i].Status == Status.Correct 
                        ? Colors.Green 
                        : Colors.Red;
                }
                else
                {
                    // Extra characters typed beyond target
                    span.TextColor = Colors.Red;
                }
                
                typedFormatted.Spans.Add(span);
            }

            TypedTextFormatted = typedFormatted;
            OriginalTextFormatted = originalFormatted;
        }
                
        [RelayCommand]
        private async Task GoHome()
        {
            await Shell.Current.GoToAsync("///StartPage");
        }

        [RelayCommand]
        private async Task TryAgain()
        {
            await Shell.Current.GoToAsync($"{nameof(TextExercise)}?difficulty={Difficulty}");
        }
    }
}
