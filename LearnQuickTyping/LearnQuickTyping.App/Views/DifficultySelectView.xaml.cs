namespace LearnQuickTyping.App.Views;

public partial class DifficultySelectView : ContentPage
{
	public DifficultySelectView()
	{
		InitializeComponent();
	}

    private async void OnDifficultySelected(object sender, EventArgs e)
    {
        var button = (Button)sender;
        var difficulty = button.CommandParameter as string;

        await Shell.Current.GoToAsync($"{nameof(TextExercise)}?difficulty={difficulty}");
    }

    private async void OnIntroductionTextClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(TextExercise));
    }
}