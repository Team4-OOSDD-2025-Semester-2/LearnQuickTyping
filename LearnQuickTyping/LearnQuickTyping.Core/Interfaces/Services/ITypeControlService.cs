using LearnQuickTyping.Core.Models;

namespace LearnQuickTyping.Core.Interfaces;

public interface ITypeControlService
{
    string TargetText { get; set; }
    string TypedText { get; set; }

    void CheckTyping(string typedText);
    List<LetterStatus> GetLetterStatuses();
}