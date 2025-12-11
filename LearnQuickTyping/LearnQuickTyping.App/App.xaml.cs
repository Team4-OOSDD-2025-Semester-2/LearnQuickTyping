using LearnQuickTyping.Core.Data.Database;

namespace LearnQuickTyping.App
{
    public partial class App : Application
    {
        private readonly SqliteSchemaMigrator _migrator;

        public App(SqliteSchemaMigrator migrator)
        {
            InitializeComponent();
            _migrator = migrator;
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new AppShell());
        }

        protected override async void OnStart()
        {
            base.OnStart();
            await _migrator.MigrateAsync();
        }
    }
}