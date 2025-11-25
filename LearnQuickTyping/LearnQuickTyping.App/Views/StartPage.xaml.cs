namespace LearnQuickTyping.App.Views;

public partial class StartPage : ContentPage
{
    public StartPage()
    {
        InitializeComponent();
    }

    private async void OnWordPracticeClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("WordPractice");
    }

    private async void OnTextPracticeClicked(object sender, EventArgs e)
    {
        await DisplayAlert("Info", "Not yet implemented.", "OK");
    }

    private async void OnVersusModeClicked(object sender, EventArgs e)
    {
        await DisplayAlert("Info", "Not yet implemented.", "OK");
    }

    private async void OnKaraokeModeClicked(object sender, EventArgs e)
    {
        await DisplayAlert("Info", "Not yet implemented.", "OK");
    }

    private async void OnCreateProfileClicked(object sender, EventArgs e)
    {
        await DisplayAlert("Info", "Not yet implemented.", "OK");
    }
}