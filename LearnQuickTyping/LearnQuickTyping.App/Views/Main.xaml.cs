namespace LearnQuickTyping.App.Views;

public partial class Main : ContentPage
{
	private readonly string [] PracticeWords = new string []
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

    // When the user presses Enter after typing
    private void OnEntryDone(object sender, EventArgs e)
    {
        var entry = (Entry)sender;

		CompareStringsOnEnter();
    }

    // Compare the input text with the example word
    void CompareStringsOnEnter()
    {
		if (InputText.Text == PracticeWord.Text)
		{
			Result.Text = "Correct!";
			Result.TextColor = Colors.Green;
        }
		else
		{
			Result.Text = "Try Again!";
			Result.TextColor = Colors.Red;
        }
        InputText.Text = string.Empty;
    }
}
