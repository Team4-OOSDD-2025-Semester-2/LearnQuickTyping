using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LearnQuickTyping.App.Views;
using LearnQuickTyping.Core.Data.Repositories;
using LearnQuickTyping.Core.Interfaces;
using LearnQuickTyping.Core.Interfaces.Repositories;
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
        private readonly IExerciseResultRepository _exerciseResultRepository;

        [ObservableProperty]
        private TextResult? _result;

        [ObservableProperty]
        private string _difficulty = string.Empty;

        [ObservableProperty]
        private FormattedString _typedTextFormatted = new FormattedString();

        [ObservableProperty]
        private FormattedString _originalTextFormatted = new FormattedString();

        [ObservableProperty]
        private bool _isThresholdMet = false;

        [ObservableProperty]
        private int _exerciseCount;

        public TextExerciseResultViewModel(
            ITextEcerciseScoreService scoreService,
            ITypeControlService typeControl, 
            IExerciseResultRepository exerciseResultRepository)
        {
            _scoreService = scoreService;
            _typeControl = typeControl;
            _exerciseResultRepository = exerciseResultRepository;
        }

        partial void OnResultChanged(TextResult? value)
        {
            
            if (value == null || string.IsNullOrEmpty(Difficulty))
            {
                return;
            }

           
            var mainPage = Application.Current?.Windows.FirstOrDefault()?.Page;

            if ((Difficulty.Equals("Beginner", StringComparison.OrdinalIgnoreCase)) ||
                (Difficulty.Equals("Intermediate", StringComparison.OrdinalIgnoreCase)) ||
                (Difficulty.Equals("Advanced", StringComparison.OrdinalIgnoreCase)))
            {
                if ((value.WordsPerMinute >= 50 && value.Accuracy >= 80) ||
                    (value.WordsPerMinute >= 47 && value.Accuracy >= 85) ||
                    (value.WordsPerMinute >= 43 && value.Accuracy >= 90) ||
                    (value.WordsPerMinute >= 38 && value.Accuracy >= 95))
                {
                   // Query the database for the current count async
                    Task.Run(async () =>
                    {
                        var count = await GetExerciseCountForDifficultyAsync();

                        MainThread.BeginInvokeOnMainThread(() =>
                        {
                            ExerciseCount = count;
                            if (ExerciseCount >= 5)
                            {
                                IsThresholdMet = true;
                                mainPage?.DisplayAlert(
                                    "Well Done!",
                                    $"You are doing great, we suggest you move up a level!",
                                    "OK");
                            }
                        });
                    });

                    GenerateMarkedTexts(value);
                }
                else
                {
                    GenerateMarkedTexts(value);
                }
            }
            else
            {
                IsThresholdMet = false;
                if (value != null)
                {
                    GenerateMarkedTexts(value);
                }
            }
        }

        partial void OnDifficultyChanged(string value)
        {
            // If Result is already set when Difficulty arrives, trigger OnResultChanged again
            if (Result != null && !string.IsNullOrEmpty(value))
            {
                OnResultChanged(Result);
            }
        }

        private async Task<int> GetExerciseCountForDifficultyAsync()
        {
            
            if (!Enum.TryParse<DifficultyLevel>(Difficulty, true, out var difficultyLevel))
            {
                return 0;
            }

            var count = await _exerciseResultRepository.GetCountByDifficultyAsync(ExerciseType.Text, difficultyLevel);
            return count;
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

        [RelayCommand]
        private async Task GoToNextDifficulty()
        {
            if (Difficulty.Equals("Beginner", StringComparison.OrdinalIgnoreCase))
            {
                await Shell.Current.GoToAsync($"{nameof(TextExercise)}?difficulty=Intermediate");
            }
            else if (Difficulty.Equals("Intermediate", StringComparison.OrdinalIgnoreCase))
            {
                await Shell.Current.GoToAsync($"{nameof(TextExercise)}?difficulty=Advanced");
            }
            else if (Difficulty.Equals("Advanced", StringComparison.OrdinalIgnoreCase))
            {
                await Shell.Current.GoToAsync($"{nameof(TextExercise)}?difficulty=Expert");

            }
        }

    }
}
