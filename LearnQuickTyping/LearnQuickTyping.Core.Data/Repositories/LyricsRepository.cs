using LearnQuickTyping.Core.Interfaces.Repositories;

namespace LearnQuickTyping.Core.Data.Repositories;

public class LyricsRepository : ILyricsRepository
{
    private readonly List<string[]> _lyricsCollection = new List<string[]>
    {
        new string[]
        {
            "When you walk through a storm,",
            "hold your head up high",
            "And don't be afraid of the dark",
            "At the end of the storm is a golden sky", 
            "And the sweet silver song of the lark"
        },

        new string[]
        {
            "Is this the real life?",
            "Is this just fantasy?",
            "Caught in a landslide,",
            "No escape from reality"

        }
    };

    public IEnumerable<string> GetAllLyricsTitles()
    {
        return new List<string>
        {
            "You'll Never Walk Alone",
            "Bohemian Rhapsody"
        };
    }
    public string[] GetLyricsByIndex(int index)
    {
        if (index < 0 || index >= _lyricsCollection.Count)
            throw new ArgumentOutOfRangeException(nameof(index));

        return _lyricsCollection[index];
    }

}
