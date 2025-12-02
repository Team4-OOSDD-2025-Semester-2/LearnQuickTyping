using LearnQuickTyping.App.ViewModels;

namespace LearnQuickTyping.App.Views;

public partial class VersusResultView : ContentPage
{
    public VersusResultView(VersusResultViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override bool OnBackButtonPressed()
    {
        return true;
    }
}