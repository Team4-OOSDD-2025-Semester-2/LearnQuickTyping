namespace LearnQuickTyping.App;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        Routing.RegisterRoute(nameof(Views.WordExercise), typeof(Views.WordExercise));
        Routing.RegisterRoute(nameof(Views.LyricsExercise), typeof(Views.LyricsExercise));
    }
}