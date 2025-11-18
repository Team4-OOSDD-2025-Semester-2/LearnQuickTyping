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
}