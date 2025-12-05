using LearnQuickTyping.App.ViewModels;

namespace LearnQuickTyping.App.Views;

public partial class LyricsResult : ContentPage
{
    public LyricsResult(LyricsExerciseViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}