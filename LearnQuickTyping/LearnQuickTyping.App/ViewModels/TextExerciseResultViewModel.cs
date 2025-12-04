using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;

namespace LearnQuickTyping.App.ViewModels
{
    public partial class TextExerciseResultViewModel : BaseViewModel
    {

        [RelayCommand]
        private async Task GoHome()
        {
            await Shell.Current.GoToAsync("///StartPage");
        }
    }
}
