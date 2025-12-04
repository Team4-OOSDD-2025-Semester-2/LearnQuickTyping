using LearnQuickTyping.App.ViewModels;

namespace LearnQuickTyping.App.Views;

public partial class LyricsResult : ContentPage
{
    private readonly LyricsExerciseViewModel _viewModel;

    public LyricsResult(LyricsExerciseViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }
}
