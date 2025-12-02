namespace LearnQuickTyping.App.Views;

public partial class StartPage : ContentPage
{
    public StartPage()
    {
        InitializeComponent();
    }

    private async void OnWordPracticeClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(WordExercise));
    }

    private async void OnTextPracticeClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(TextExercise));
    }

    private async void OnVersusModeClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(VersusView));
    }

    private async void OnKaraokeModeClicked(object sender, EventArgs e)
    {
        await DisplayAlert("Info", "Not yet implemented.", "OK");
    }
}