namespace LearnQuickTyping.App.Views;

public partial class Main : ContentPage
{
    private DateTime startTime;
    private DateTime endTime;
    private Boolean isTiming = false;
    private TimeSpan elapsedTime = TimeSpan.Zero;

    private readonly string[] PracticeWords = new string[]
    {
        "example", "typing", "quick", "learn", "keyboard", "practice", "speed", "accuracy", "challenge", "improve"
    };

    public Main()
    {
        InitializeComponent();
        DisplayRandomWord();
    }

    // Display a random word from the list above
    private void DisplayRandomWord()
    {
        var random = new Random();
        int index = random.Next(PracticeWords.Length);
        PracticeWord.Text = PracticeWords[index];
    }

    #region WPM calculations
    // Start timing typing
    private void StartTiming(object sender, TextChangedEventArgs e)
    {
        if (!isTiming && !string.IsNullOrEmpty(e.NewTextValue))
        {
            startTime = DateTime.Now;
            isTiming = true;
        }
    }

    // Stop timing typing
    TimeSpan StopTiming()
    {
        if (isTiming)
        {
            endTime = DateTime.Now;
            isTiming = false;
            elapsedTime = endTime - startTime;
        }
        return elapsedTime;
    }

    void ResetTiming()
    {
        isTiming = false;
        elapsedTime = TimeSpan.Zero;
    }

    double CalculateWordPerMinute(int characterCount, TimeSpan timeTaken)
    {
        if (timeTaken.TotalMinutes == 0) return 0;

        // 6.8 characters = 1 word
        double wordCount = characterCount / 6.8; // Average amount of characters in list "PracticeWords"

        return wordCount / timeTaken.TotalMinutes;
    }

    #endregion

    // When the user presses Enter after typing
    private void OnEntryDone(object sender, EventArgs e)
    {
        var entry = (Entry)sender;

        CompareStringsOnEnter(StopTiming());
    }

    // Compare the input text with the example word
    void CompareStringsOnEnter(TimeSpan timeTaken)
    {
        int typedCharacters = InputText.Text.Length;
        double wpm = CalculateWordPerMinute(typedCharacters, timeTaken);
        ResetTiming();

        if (InputText.Text == PracticeWord.Text)
        {
            Result.Text = $"Correct! \r\nTime: {timeTaken.TotalSeconds:F2} seconds \r\nWords Per Minute: {wpm:F2}";
            Result.TextColor = Colors.Green;

            DisplayRandomWord();
        }
        else
        {
            Result.Text = "Try Again!";
            Result.TextColor = Colors.Red;
        }
        InputText.Text = string.Empty;
    }
}