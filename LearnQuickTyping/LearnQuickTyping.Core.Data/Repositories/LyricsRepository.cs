using LearnQuickTyping.Core.Interfaces.Repositories;

namespace LearnQuickTyping.Core.Data.Repositories;

public class lyricsRepository : ILyricsRepository
{
    private readonly string[] _practiceLyrics = new string[]
    {
        "When you walk through a storm, hold your head up high" +
        "And don't be afraid of the dark" +
        "At the end of the storm is a golden sky" +
        "And the sweet silver song of the lark " 
    };

    public string GetLyrics()
    {
        var random = new Random();
        int index = random.Next(_practiceLyrics.Length);
        return _practiceLyrics[index];
    }
}
