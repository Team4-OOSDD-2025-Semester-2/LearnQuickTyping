using LearnQuickTyping.Core.Interfaces.Repositories;

namespace LearnQuickTyping.Core.Data.Repositories;

public class TextRepository : ITextRepository
{
    private readonly string[] _practiceTexts = new string[]
    {
        "Test", "Test2", "Test3"
    };

    public string GetRandomText()
    {
        var random = new Random();
        int index = random.Next(_practiceTexts.Length);
        return _practiceTexts[index];
    }
}