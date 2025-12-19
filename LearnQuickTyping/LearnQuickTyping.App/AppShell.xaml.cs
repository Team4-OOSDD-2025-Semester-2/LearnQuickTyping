using LearnQuickTyping.App.Views;

namespace LearnQuickTyping.App;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        Routing.RegisterRoute(nameof(Views.WordExercise), typeof(Views.WordExercise));
        Routing.RegisterRoute(nameof(Views.TextExercise), typeof(Views.TextExercise));
        Routing.RegisterRoute(nameof(Views.VersusView), typeof(Views.VersusView));
        Routing.RegisterRoute(nameof(Views.VersusResultView), typeof(Views.VersusResultView));
        Routing.RegisterRoute(nameof(Views.TextExerciseResultView), typeof(Views.TextExerciseResultView));
        Routing.RegisterRoute(nameof(Views.LyricsExercise), typeof(Views.LyricsExercise));
        Routing.RegisterRoute(nameof(Views.LyricsResult), typeof(Views.LyricsResult));
        Routing.RegisterRoute(nameof(Views.DifficultySelectView), typeof(Views.DifficultySelectView));
        Routing.RegisterRoute(nameof(Views.ProgressPage), typeof(Views.ProgressPage));
    }
}