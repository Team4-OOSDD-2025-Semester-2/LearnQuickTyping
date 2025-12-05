using LearnQuickTyping.App.ViewModels;

namespace LearnQuickTyping.App.Views;

public partial class TextExerciseResultView : ContentPage
{
	public TextExerciseResultView(TextExerciseResultViewModel viewModel)
	{
		InitializeComponent();
        BindingContext = viewModel;
    }
}