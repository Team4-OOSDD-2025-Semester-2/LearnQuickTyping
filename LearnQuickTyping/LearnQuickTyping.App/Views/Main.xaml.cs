using LearnQuickTyping.Core.Models;

namespace LearnQuickTyping.App.Views;

public partial class Main : ContentPage
{
    private DateTime startTime;
    private DateTime endTime;
    private bool isTiming = false;
    private TimeSpan elapsedTime = TimeSpan.Zero;
    private IDispatcherTimer realTimeTimer = null!;

    // TypeControl for letter-by-letter checking
    private TypeControl typeControl = null!;

    // Store the current target word
    private string currentTargetWord = string.Empty;

    private readonly string[] PracticeWords = new string[]
    {
        "Example", "Typing", "Quick", "Learn", "Keyboard", "Practice", "Speed", "Accuracy", "Challenge", "Improve"
    };

    public Main()
    {
        InitializeComponent();

        // FIRST: Initialize TypeControl
        typeControl = new TypeControl();
        typeControl.StatusChanged += OnTypingStatusChanged;

        // THEN: Call DisplayRandomWord
        DisplayRandomWord();
        SetupRealTimeTimer();
    }

    // Display a random word from the list above
    private void DisplayRandomWord()
    {
        var random = new Random();
        int index = random.Next(PracticeWords.Length);
        string newWord = PracticeWords[index];

        // Store the word in a variable
        currentTargetWord = newWord;

        // Reset the visual display
        PracticeWord.FormattedText = null;
        PracticeWord.Text = newWord;

        // Update TypeControl with new target word
        if (typeControl != null)
        {
            typeControl.TargetText = newWord;
            typeControl.TypedText = string.Empty;

            // Update display with all letters gray (pending)
            var statuses = typeControl.GetLetterStatuses();
            UpdateLetterDisplay(statuses);
        }
    }

    // Event handler for TypeControl updates
    private void OnTypingStatusChanged(List<LetterStatus> statuses)
    {
        // Update the visual display of letters with colors
        UpdateLetterDisplay(statuses);
    }

    // Method to display letters with colors
    private void UpdateLetterDisplay(List<LetterStatus> statuses)
    {
        // Create a FormattedString for colored text
        var formattedString = new FormattedString();

        foreach (var letterStatus in statuses)
        {
            var span = new Span
            {
                Text = letterStatus.Character.ToString(),
                FontSize = PracticeWord.FontSize
            };

            // Choose color based on status
            span.TextColor = letterStatus.Status switch
            {
                Status.Correct => Colors.Green,      // Correct letter - green
                Status.Incorrect => Colors.Red,      // Wrong letter - red
                Status.Pending => Colors.Gray,       // Not yet typed - gray
                _ => Colors.Black
            };

            formattedString.Spans.Add(span);
        }

        // Update the PracticeWord label with colored letters
        PracticeWord.FormattedText = formattedString;
    }

    #region Time Related Methods

    #region START STOP RESET TIMER
    // Start timing typing
    private void StartTiming(object sender, TextChangedEventArgs e)
    {
        // Start timer if not already timing and text is not empty
        if (!isTiming && !string.IsNullOrEmpty(e.NewTextValue))
        {
            startTime = DateTime.Now;
            isTiming = true;
            realTimeTimer.Start();
        }

        // Check typing with TypeControl on every text change
        if (typeControl != null)
        {
            typeControl.CheckTyping(e.NewTextValue ?? string.Empty);
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
    private void OnRealTimeTimerTick(object? sender, EventArgs e)
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

        // Compare with currentTargetWord instead of PracticeWord.Text
        // (PracticeWord.Text may be empty after FormattedText is set)
        if (InputText.Text == currentTargetWord)
        {
            SecondsResult.Text = $"Time: {timeTaken.TotalSeconds:F2} seconds";
            WordsPerMinuteResult.Text = $"Words Per Minute: {wpm:F2}";
            TextResult.Text = $"Correct!";
            TextResult.TextColor = Colors.Green;

            // Display new random word
            DisplayRandomWord();

            // Reset TypeControl for new word
            if (typeControl != null)
            {
                typeControl.TypedText = string.Empty;
            }
        }
        else
        {
            SecondsResult.Text = $"Time: {timeTaken.TotalSeconds:F2} seconds";
            WordsPerMinuteResult.Text = $"Words Per Minute: {wpm:F2}";
            TextResult.Text = "Try Again!";
            TextResult.TextColor = Colors.Red;
        }

        // Clear input field
        InputText.Text = string.Empty;
    }
}
