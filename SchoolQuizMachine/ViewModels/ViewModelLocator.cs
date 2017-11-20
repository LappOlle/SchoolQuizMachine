using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Views;
using Microsoft.Practices.ServiceLocation;
using SchoolQuizMachine.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolQuizMachine.ViewModels
{
    public class ViewModelLocator
    {
        private static string CurrentKey = Guid.NewGuid().ToString();
        private const string StartPage = "StartPage";
        private const string QuizPage = "QuizPage";

        static ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);
            var nav = new NavigationService();

            nav.Configure(StartPage, typeof(StartPage));
            nav.Configure(QuizPage, typeof(QuizPage));

            SimpleIoc.Default.Register<INavigationService>(() => nav);
            SimpleIoc.Default.Register<MainViewModel>();
            SimpleIoc.Default.Register<QuizViewModel>();
        }

        public static MainViewModel Main
        {
            get
            {
                return ServiceLocator.Current.GetInstance<MainViewModel>(CurrentKey);
            }
        }

        public static QuizViewModel Quiz
        {
            get
            {
                return ServiceLocator.Current.GetInstance<QuizViewModel>(CurrentKey);
            }
        }

        public static void ResetViewModels()
        {
            SimpleIoc.Default.Unregister<MainViewModel>(CurrentKey);
            SimpleIoc.Default.Unregister<QuizViewModel>(CurrentKey);
            CurrentKey = Guid.NewGuid().ToString();
        }
            
    }
}
