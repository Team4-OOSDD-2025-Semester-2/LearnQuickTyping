namespace LearnQuickTyping.App
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            Routing.RegisterRoute("WordPractice", typeof(Views.Main));
        }
    }
}
