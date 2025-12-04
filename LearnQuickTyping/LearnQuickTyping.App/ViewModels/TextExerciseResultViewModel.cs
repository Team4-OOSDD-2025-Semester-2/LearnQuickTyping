using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LearnQuickTyping.Core.Interfaces;
using LearnQuickTyping.Core.Interfaces.Services;
using LearnQuickTyping.Core.Models;


namespace LearnQuickTyping.App.ViewModels
{
    [QueryProperty(nameof(Result), "Result")]
    public partial class TextExerciseResultViewModel : BaseViewModel
    {
        private readonly ITextEcerciseScoreService _scoreService;
        private readonly ITypeControlService _typeControl;

        [ObservableProperty]
        private TextResult? _result;

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
            if (value != null)
            {
                GenerateFormattedStrings(value);
            }
        }

        private void GenerateFormattedStrings(TextResult result)
        {
            // Set up the type control service
            _typeControl.TargetText = result.OriginalText ?? string.Empty;
            _typeControl.TypedText = result.TypedText ?? string.Empty;

            // Get letter statuses from the service
            var letterStatuses = _typeControl.GetLetterStatuses();

            var typedFormatted = new FormattedString();
            var originalFormatted = new FormattedString();

            string typed = result.TypedText ?? string.Empty;

            // Generate formatted original text using LetterStatus
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

            // Generate formatted typed text
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
    }
}
