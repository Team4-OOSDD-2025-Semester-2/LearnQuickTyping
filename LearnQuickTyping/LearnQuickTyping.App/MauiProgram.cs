using CommunityToolkit.Maui;
using LearnQuickTyping.App.Services;
using LearnQuickTyping.App.ViewModels;
using LearnQuickTyping.App.Views;
using LearnQuickTyping.Core.Data.Database;
using LearnQuickTyping.Core.Data.Repositories;
using LearnQuickTyping.Core.Interfaces;
using LearnQuickTyping.Core.Interfaces.Database;
using LearnQuickTyping.Core.Interfaces.Repositories;
using LearnQuickTyping.Core.Interfaces.Services;
using LearnQuickTyping.Core.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace LearnQuickTyping.App;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiCommunityToolkit()
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        builder.Services.AddSingleton<IWordRepository, WordRepository>();
        builder.Services.AddSingleton<ITextRepository, TextRepository>();
        builder.Services.AddSingleton<ILyricsRepository, LyricsRepository>();

        // Database
        string dbPath = Path.Combine(FileSystem.AppDataDirectory, "learnquicktyping.db");
        builder.Services.AddSingleton<ISqliteConnectionFactory>(new SqliteConnectionFactory(dbPath));
        builder.Services.AddSingleton<SqliteSchemaMigrator>();
        builder.Services.AddSingleton<IExerciseResultRepository, ExerciseResultRepository>();

        builder.Services.AddSingleton<ITypingStatsService, TypingStatsService>();
        builder.Services.AddTransient<ITypeControlService, TypeControlService>();
        builder.Services.AddTransient<IVersusScoreService, VersusScoreService>();
        builder.Services.AddTransient<ITextEcerciseScoreService, TextExerciseScoreService>();
        builder.Services.AddSingleton<IExerciseResultSaveService, ExerciseResultSaveService>();
        builder.Services.AddSingleton<INotificationService, ToastNotificationService>();


        builder.Services.AddTransient<WordExerciseViewModel>();
        builder.Services.AddTransient<WordExercise>();

        builder.Services.AddTransient<TextExerciseViewModel>();
        builder.Services.AddTransient<TextExercise>();
        
        builder.Services.AddTransient<TextExerciseResultViewModel>();
        builder.Services.AddTransient<TextExerciseResultView>();

        builder.Services.AddTransient<VersusViewModel>();
        builder.Services.AddTransient<VersusView>();

        builder.Services.AddTransient<VersusResultViewModel>();
        builder.Services.AddTransient<VersusResultView>();

        builder.Services.AddTransient<LyricsExerciseViewModel>();
        builder.Services.AddTransient<LyricsResult>();
        builder.Services.AddTransient<LyricsExercise>();

        builder.Services.AddTransient<DifficultySelectView>();

        builder.Services.AddTransient<ProgressViewModel>();
        builder.Services.AddTransient<ProgressPage>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}