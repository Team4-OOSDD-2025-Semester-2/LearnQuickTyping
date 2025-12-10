namespace LearnQuickTyping.App.Views;

public partial class DifficultySelectView : ContentPage
{
	public DifficultySelectView()
	{
		InitializeComponent();
	}

    private async void OnBeginnerDifficultyClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(TextExercise));
    }

    private async void OnIntermediateDifficultyClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(TextExercise));
    }

    private async void OnAdvancedDifficultyClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(TextExercise));
    }

    private async void OnExpertDifficultyClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(TextExercise));
    }
}