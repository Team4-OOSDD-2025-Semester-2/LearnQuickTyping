namespace LearnQuickTyping.App.Views;

public partial class Main : ContentPage
{
	private readonly string [] words = new string []
	{
		"example", "typing", "quick", "learn", "keyboard", "practice", "speed", "accuracy", "challenge", "improve"
	};

    public Main()
	{
		InitializeComponent();
		DisplayRandomWord();
	}

	private void DisplayRandomWord()
		{
		var random = new Random();
		int index = random.Next(words.Length);
		ExampleWord.Text = words[index];
    }

    private void OnEntryDone(object sender, EventArgs e)
    {
        var entry = (Entry)sender;
		string text = entry.Text ?? string.Empty;

		CompareStringsOnEnter();
		
    }

	void CompareStringsOnEnter()
    {
		if (InputText.Text == ExampleWord.Text)
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
