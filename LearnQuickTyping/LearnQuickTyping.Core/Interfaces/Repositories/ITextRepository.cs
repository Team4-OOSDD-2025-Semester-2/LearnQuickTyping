namespace LearnQuickTyping.Core.Interfaces.Repositories;

public interface ITextRepository
{
    string GetRandomText();
    string GetRandomTextByDifficulty(string difficulty);
    int GetWordCount();
    int CountWords(string text);
}