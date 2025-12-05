using LearnQuickTyping.Core.Interfaces;
using LearnQuickTyping.Core.Models;

namespace LearnQuickTyping.Core.Services;

public class TypeControlService : ITypeControlService
{
    public string TargetText { get; set; } = string.Empty;
    public string TypedText { get; set; } = string.Empty;

    public void CheckTyping(string typedText)
    {
        TypedText = typedText;
    }

    public List<LetterStatus> GetLetterStatuses()
    {
        var statuses = new List<LetterStatus>(TargetText.Length);

        for (int i = 0; i < TargetText.Length; i++)
        {
            if (i < TypedText.Length)
            {
                bool isCorrect = TargetText[i] == TypedText[i];
                statuses.Add(new LetterStatus
                {
                    Character = TargetText[i],
                    Status = isCorrect ? Status.Correct : Status.Incorrect
                });
            }
            else
            {
                statuses.Add(new LetterStatus
                {
                    Character = TargetText[i],
                    Status = Status.Pending
                });
            }
        }

        return statuses;
    }
}