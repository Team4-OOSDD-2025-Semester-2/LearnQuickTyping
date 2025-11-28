using LearnQuickTyping.App.Views;

namespace LearnQuickTyping.App;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        Routing.RegisterRoute(nameof(Views.WordExercise), typeof(Views.WordExercise));
        Routing.RegisterRoute(nameof(Views.VersusView), typeof(Views.VersusView));
        Routing.RegisterRoute(nameof(Views.VersusResultView), typeof(Views.VersusResultView));
    }
}