namespace LearnQuickTyping.Core.Interfaces.Repositories;

public interface ITextRepository
{
    string GetRandomText();
    int GetWordCount();
    int CountWords(string text);
}