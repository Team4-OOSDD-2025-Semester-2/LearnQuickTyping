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

        [ObservableProperty]
        private string _recommendedLevel = string.Empty;

        [ObservableProperty]
        private bool _isIntroductionTest = false;

        public TextExerciseResultViewModel(
            ITextEcerciseScoreService scoreService,
            ITypeControlService typeControl)
        {
            _scoreService = scoreService;
            _typeControl = typeControl;
        }

        partial void OnResultChanged(TextResult? value)
        {
            if (value == null) return;

            GenerateMarkedTexts(value);
        }

        partial void OnDifficultyChanged(string value)
        {
            // Check if this is an introduction test (multiple variations)
            string normalizedDifficulty = value?.Trim().ToLowerInvariant() ?? "";

            IsIntroductionTest = normalizedDifficulty == "introduction text" ||
                                normalizedDifficulty == "introduction";

            if (IsIntroductionTest && Result != null)
            {
                // Calculate recommended level based on performance
                string recommended = CalculateRecommendedLevel(Result);
                RecommendedLevel = recommended;

                // Show recommendation alert
                ShowRecommendationAlert(recommended);
            }
        }

        private string CalculateRecommendedLevel(TextResult result)
        {
            double wpm = result.WordsPerMinute;
            int accuracy = result.Accuracy;

            if (wpm > 80 && accuracy >= 95)
            {
                return "Expert";
            }
            else if (wpm >= 60 && accuracy >= 90)
            {
                return "Advanced";
            }
            else if ((wpm <= 60 && accuracy >= 80) ||
                    (wpm > 30 && accuracy >= 80))
            {
                return "Intermediate";
            }
            else
            {
                return "Beginner";
            }
        }

        private async void ShowRecommendationAlert(string recommendedLevel)
        {
            var mainPage = Application.Current?.Windows.FirstOrDefault()?.Page;
            if (mainPage == null) return;

            string message = GetRecommendationMessage(recommendedLevel);
            string title = "Well Done!";

            bool startRecommended = await mainPage.DisplayAlert(
                title,
                message,
                "View results",
                "Go to Home");

            if (startRecommended)
            {
                // Navigate back to result
                return;
            }
            else
            {
                // Go back to home
                await Shell.Current.GoToAsync("///StartPage");
            }
        }

        private string GetRecommendationMessage(string level)
        {
            string recommendation = level switch
            {
                "Expert" => "Your suggested level is Expert.",
                "Advanced" => "Your suggested level is Advanced.",
                "Intermediate" => "Your suggested level is Intermediate.",
                "Beginner" => "Your suggested level is Beginner."
            };

            return recommendation;
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
        private async Task StartRecommendedLevel()
        {
            if (!string.IsNullOrEmpty(RecommendedLevel))
            {
                await Shell.Current.GoToAsync($"{nameof(TextExercise)}?difficulty={RecommendedLevel}");
            }
        }
    }
}
