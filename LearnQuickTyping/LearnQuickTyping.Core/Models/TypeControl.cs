using System;
using System.Collections.Generic;

namespace LearnQuickTyping.Core.Models
{
    public class TypeControl
    {
        // Data
        public string TargetText { get; set; } = string.Empty;  // What needs to be typed
        public string TypedText { get; set; } = string.Empty;   // What has been typed

        // Event to inform MainPage
        public event Action<List<LetterStatus>>? StatusChanged;

        // Call this method every time the user types
        public void CheckTyping(string typedText)
        {
            TypedText = typedText;
            var statuses = GetLetterStatuses();
            StatusChanged?.Invoke(statuses);  // Notify MainPage
        }

        // Check each letter and return status
        public List<LetterStatus> GetLetterStatuses()
        {
            var statuses = new List<LetterStatus>();

            // Loop through each letter of TargetText
            for (int i = 0; i < TargetText.Length; i++)
            {
                if (i < TypedText.Length)
                {
                    // Letter has already been typed - check if correct
                    bool isCorrect = TargetText[i] == TypedText[i];
                    statuses.Add(new LetterStatus
                    {
                        Character = TargetText[i],
                        Status = isCorrect ? Status.Correct : Status.Incorrect
                    });
                }
                else
                {
                    // Letter still needs to be typed
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

    // Information about one letter
    public class LetterStatus
    {
        public char Character { get; set; }  // The letter itself
        public Status Status { get; set; }   // Correct/Incorrect/Pending
    }

    // Possible statuses
    public enum Status
    {
        Correct,    // Green
        Incorrect,  // Red
        Pending     // Gray
    }
}