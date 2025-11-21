using System;
using System.Collections.Generic;

namespace LearnQuickTyping.Core.Models
{
    public class TypeControle
    {
        // Data
        public string TargetText { get; set; }  // Wat moet getypt worden
        public string TypedText { get; set; }   // Wat is getypt

        // Event om MainPage te informeren
        public event Action<List<LetterStatus>> StatusChanged;

        // Deze methode roep je aan elke keer dat gebruiker typt
        public void CheckTyping(string typedText)
        {
            TypedText = typedText;
            var statuses = GetLetterStatuses();
            StatusChanged?.Invoke(statuses);  // Vertel MainPage
        }

        // Controleer elke letter en geef status terug
        public List<LetterStatus> GetLetterStatuses()
        {
            var statuses = new List<LetterStatus>();

            // Loop door elke letter van TargetText
            for (int i = 0; i < TargetText.Length; i++)
            {
                if (i < TypedText.Length)
                {
                    // Letter is al getypt - controleer of correct
                    bool isCorrect = TargetText[i] == TypedText[i];

                    statuses.Add(new LetterStatus
                    {
                        Character = TargetText[i],
                        Status = isCorrect ? Status.Correct : Status.Incorrect
                    });
                }
                else
                {
                    // Letter moet nog getypt worden
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

    // Info over één letter
    public class LetterStatus
    {
        public char Character { get; set; }  // De letter zelf
        public Status Status { get; set; }   // Correct/Incorrect/Pending
    }

    // Mogelijke statussen
    public enum Status
    {
        Correct,    // Groen
        Incorrect,  // Rood
        Pending     // Grijs
    }
}
