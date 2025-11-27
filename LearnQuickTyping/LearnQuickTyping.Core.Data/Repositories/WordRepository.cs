using LearnQuickTyping.Core.Interfaces.Repositories;

namespace LearnQuickTyping.Core.Data.Repositories;

public class WordRepository : IWordRepository
{
    private readonly string[] _practiceWords = new string[]
    {
        "Example", "Typing", "Quick", "Learn", "Keyboard",
        "Practice", "Speed", "Accuracy", "Challenge", "Improve"
    };

    public string GetRandomWord()
    {
        var random = new Random();
        int index = random.Next(_practiceWords.Length);
        return _practiceWords[index];
    }
}