using System.ComponentModel;
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

        _viewModel.PropertyChanged += OnViewModelPropertyChanged;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _viewModel.InitializeVersusCommand.Execute(null);
    }

    private void OnViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(VersusViewModel.IsTurnOverlayVisible))
        {
            Dispatcher.DispatchDelayed(TimeSpan.FromMilliseconds(50), () => //Delay overlay
            {
                if (_viewModel.IsTurnOverlayVisible)
                {
                    Invisible.Focus(); //Entry to start turn is selected
                }
                else
                {
                    TypingEntry.Focus(); //Entry to write exercise is selected
                }
            });
        }
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