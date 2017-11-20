using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Views;
using SchoolQuizMachine.Models.Services;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.ViewManagement;
using GalaSoft.MvvmLight.Threading;
using Windows.System;

namespace SchoolQuizMachine.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        GpioButtonService startButtons;
        private RelayCommand _startButtonCommand;
        private RelayCommand _exitButtonCommand;
        private readonly INavigationService NavigationService;

        public RelayCommand StartButtonCommand
        {
            get
            {
                if (_startButtonCommand == null)
                {
                    _startButtonCommand = new RelayCommand(() => NavigateToQuiz());
                }
                return _startButtonCommand;
            }
        }

        public RelayCommand ExitButtonCommand
        {
            get
            {
                if (_exitButtonCommand == null)
                {
                    _exitButtonCommand = new RelayCommand(() => NavigateToExit());
                }
                return _exitButtonCommand;
            }
        }

        public MainViewModel(INavigationService navigationService)
        {
            startButtons = new GpioButtonService();
            DispatcherHelper.Initialize();
            NavigationService = navigationService;

            if (startButtons != null)
            {
                startButtons.ButtonOnePressed += StartButtons_ButtonOnePressed;
                startButtons.ButtonThreePressed += StartButtons_ButtonThreePressed;
            }
        }

        private void StartButtons_ButtonThreePressed(object sender, EventArgs e)
        {
            NavigateToExit();
        }

        private void StartButtons_ButtonOnePressed(object sender, EventArgs e)
        {
            NavigateToQuiz();
        }

        private async void NavigateToExit()
        {
            ViewModelLocator.ResetViewModels();
            await startButtons.Dispose();
            ShutdownManager.BeginShutdown(ShutdownKind.Shutdown, TimeSpan.FromSeconds(0));
        }

        private async void NavigateToQuiz()
        {
            ViewModelLocator.ResetViewModels();
            await startButtons.Dispose();
            await DispatcherHelper.RunAsync(() => NavigationService.NavigateTo("QuizPage"));
        }
    }
}
