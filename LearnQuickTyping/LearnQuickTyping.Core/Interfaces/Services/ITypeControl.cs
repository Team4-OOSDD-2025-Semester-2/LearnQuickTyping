using LearnQuickTyping.Core.Models;

namespace LearnQuickTyping.Core.Interfaces;

public interface ITypeControl
{
    string TargetText { get; set; }
    string TypedText { get; set; }

    void CheckTyping(string typedText);
    List<LetterStatus> GetLetterStatuses();
}