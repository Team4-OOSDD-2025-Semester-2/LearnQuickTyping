namespace LearnQuickTyping.App.Views;

public partial class Main : ContentPage
{
    private DateTime startTime;
    private DateTime endTime;
    private Boolean isTiming = false;
    private TimeSpan elapsedTime = TimeSpan.Zero;
    private IDispatcherTimer realTimeTimer;

    private readonly string[] PracticeWords = new string[]
    {
        "example", "typing", "quick", "learn", "keyboard", "practice", "speed", "accuracy", "challenge", "improve"
    };

    public Main()
    {
        InitializeComponent();
        DisplayRandomWord();
        SetupRealTimeTimer();
    }

    // Display a random word from the list above
    private void DisplayRandomWord()
    {
        var random = new Random();
        int index = random.Next(PracticeWords.Length);
        PracticeWord.Text = PracticeWords[index];
    }

    #region Time Related Methods

    #region START STOP RESET TIMER
    // Start timing typing
    private void StartTiming(object sender, TextChangedEventArgs e)
    {
        if (!isTiming && !string.IsNullOrEmpty(e.NewTextValue))
        {
            startTime = DateTime.Now;
            isTiming = true;
            realTimeTimer.Start();
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
            realTimeTimer.Stop();
        }
        return elapsedTime;
    }

    // Reset timer
    void ResetTiming()
    {
        isTiming = false;
        elapsedTime = TimeSpan.Zero;
        realTimeTimer.Stop();
    }
    #endregion

    #region REAL TIME TIMER IN SECONDS
    // Setup for Real-Time Timer
    private void SetupRealTimeTimer()
    {
        // Create a timer that runs on the UI thread to avoid cross-threading issues
            realTimeTimer = Dispatcher.CreateTimer();
        // Set the timer to trigger every 50 milliseconds (20 times per second)
            realTimeTimer.Interval = TimeSpan.FromMilliseconds(50); // Updates every 50ms
        // Connect the timer to our event handler method
            realTimeTimer.Tick += OnRealTimeTimerTick;
    }

    // Real-time typing performance monitor
    private void OnRealTimeTimerTick(object sender, EventArgs e)
    {
        if (isTiming)
        {
            // Calculate current elapsed time
            var currentElapsed = DateTime.Now - startTime;
            // Calculate current WPM
            int typedCharacters = InputText.Text.Length;
            double tempWPM = CalculateWordPerMinute(typedCharacters, currentElapsed);
            
            // Display seconds and WPM
            SecondsResult.Text = $"Current time: {currentElapsed.TotalSeconds:F2}s";
            WordsPerMinuteResult.Text = $"Current words per minute: {tempWPM:F2}";
        }
    }
    #endregion

    // WPM Calculations
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
            SecondsResult.Text = $"Time: {timeTaken.TotalSeconds:F2} seconds";
            WordsPerMinuteResult.Text = $"Words Per Minute: {wpm:F2}";
            TextResult.Text = $"Correct!";
            TextResult.TextColor = Colors.Green;

            DisplayRandomWord();
        }
        else
        {
            SecondsResult.Text = $"Time: {timeTaken.TotalSeconds:F2} seconds";
            WordsPerMinuteResult.Text = $"Words Per Minute: {wpm:F2}";
            TextResult.Text = "Try Again!";
            TextResult.TextColor = Colors.Red;
        }
        InputText.Text = string.Empty;
    }
}