using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using LearnQuickTyping.Core.Interfaces;
using LearnQuickTyping.Core.Interfaces.Repositories;
using LearnQuickTyping.Core.Interfaces.Services;
using LearnQuickTyping.Core.Data.Repositories;
using LearnQuickTyping.Core.Services;
using LearnQuickTyping.App.ViewModels;
using LearnQuickTyping.App.Views;

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

        builder.Services.AddSingleton<ITypingStatsService, TypingStatsService>();
        builder.Services.AddTransient<ITypeControlService, TypeControlService>();
        builder.Services.AddTransient<IVersusScoreService, VersusScoreService>();
        builder.Services.AddTransient<ITextEcerciseScoreService, TextExerciseScoreService>();

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

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}