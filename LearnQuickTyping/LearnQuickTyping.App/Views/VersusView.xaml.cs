using LearnQuickTyping.App.ViewModels;
using LearnQuickTyping.Core.Models;

namespace LearnQuickTyping.App.Views;

public partial class VersusView : ContentPage
{
    private readonly VersusViewModel _viewModel;

    public VersusView(VersusViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;

        _viewModel.RequestLetterUpdate += UpdateLetterDisplay;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _viewModel.InitializeVersusCommand.Execute(null);
    }

    private void UpdateLetterDisplay(List<LetterStatus> statuses)
    {
        var formattedString = new FormattedString();

        foreach (var letterStatus in statuses)
        {
            var span = new Span
            {
                Text = letterStatus.Character.ToString(),
                FontSize = PracticeWordLabel.FontSize
            };

            span.TextColor = letterStatus.Status switch
            {
                Status.Correct => Colors.Green,
                Status.Incorrect => Colors.Red,
                Status.Pending => Colors.Gray,
                _ => Colors.Black
            };

            formattedString.Spans.Add(span);
        }

        PracticeWordLabel.FormattedText = formattedString;
    }
}