using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Threading;
using GalaSoft.MvvmLight.Views;
using SchoolQuizMachine.Models.Handlers;
using SchoolQuizMachine.Models.Poco;
using SchoolQuizMachine.Models.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace SchoolQuizMachine.ViewModels
{
    public class QuizViewModel : ViewModelBase
    {
        #region Fields

        INavigationService NavigationService;
        private Char[] charsToInitialz = { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L',
            'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z', 'Å', 'Ä', 'Ö' };
        private string messageSetYourInitialz = "Skriv dina initialer:";
        private GpioButtonService ExternalButtons;
        List<QuestionData> QuizRound;

        int QuestionIndex = -1;
        List<bool> scoreList;
        QuizHandler handler;
        char initialOne = 'A';
        char initialTwo = 'A';
        bool initialOneSetted = false;
        bool initialTwoSetted = false;
        bool LoadNextQuestion = true;

        public ObservableCollection<string> HighScoreList { get; set; }

        #endregion

        #region Properties
        private BitmapImage _backgroundImage;
        public BitmapImage BackgroundImage
        {
            get
            {
                return _backgroundImage;
            }
            set
            {
                _backgroundImage = value;
                RaisePropertyChanged(() => BackgroundImage);
            }
        }

        private BitmapImage _questionImage;
        public BitmapImage QuestionImage
        {
            get
            {
                return _questionImage;
            }
            set
            {
                _questionImage = value;
                RaisePropertyChanged(() => QuestionImage);
            }
        }

        private bool _loading = true;
        public bool Loading
        {
            get
            {
                return _loading;
            }
            set
            {
                _loading = value;
                RaisePropertyChanged(() => Loading);
            }
        }

        private string _headerField = "Loading";
        public string HeaderField
        {
            get
            {
                return _headerField;
            }
            set
            {
                _headerField = value;
                RaisePropertyChanged(() => HeaderField);
            }
        }

        private string _questionField = "";
        public string QuestionField
        {
            get
            {
                return _questionField;
            }
            set
            {
                _questionField = value;
                RaisePropertyChanged(() => QuestionField);
            }
        }

        private string _alternativeOneField = "Minska";
        public string AlternativeOneField
        {
            get
            {
                return _alternativeOneField;
            }
            set
            {
                _alternativeOneField = value;
                RaisePropertyChanged(() => AlternativeOneField);
            }
        }

        private string _alternativeTwoField = "Välj";
        public string AlternativeTwoField
        {
            get
            {
                return _alternativeTwoField;
            }
            set
            {
                _alternativeTwoField = value;
                RaisePropertyChanged(() => AlternativeTwoField);
            }
        }

        private string _alternativeThreeField = "Öka";
        public string AlternativeThreeField
        {
            get
            {
                return _alternativeThreeField;
            }
            set
            {
                _alternativeThreeField = value;
                RaisePropertyChanged(() => AlternativeThreeField);
            }
        }

        private LinearGradientBrush _highScoreBackGround;
        public LinearGradientBrush HighScoreBackGround
        {
            get
            {
                return _highScoreBackGround;
            }
            set
            {
                _highScoreBackGround = value;
                RaisePropertyChanged(() => HighScoreBackGround);
            }
        }

        private LinearGradientBrush _alternativeOneBackground;
        public LinearGradientBrush AlternativeOneBackground
        {
            get
            {
                return _alternativeOneBackground;
            }
            set
            {
                _alternativeOneBackground = value;
                RaisePropertyChanged(() => AlternativeOneBackground);
            }
        }

        private LinearGradientBrush _alternativeTwoBackground;
        public LinearGradientBrush AlternativeTwoBackground
        {
            get
            {
                return _alternativeTwoBackground;
            }
            set
            {
                _alternativeTwoBackground = value;
                RaisePropertyChanged(() => AlternativeTwoBackground);
            }
        }

        private LinearGradientBrush _alternativeThreeBackground;
        public LinearGradientBrush AlternativeThreeBackground
        {
            get
            {
                return _alternativeThreeBackground;
            }
            set
            {
                _alternativeThreeBackground = value;
                RaisePropertyChanged(() => AlternativeThreeBackground);
            }
        }

        private LinearGradientBrush _questionFieldBackground;
        public LinearGradientBrush QuestionFieldBackground
        {
            get
            {
                return _questionFieldBackground;
            }
            set
            {
                _questionFieldBackground = value;
                RaisePropertyChanged(() => QuestionFieldBackground);
            }
        }
        #endregion


        private RelayCommand _buttonOneCommand;
        public RelayCommand ButtonOneCommand
        {
            get
            {
                if (_buttonOneCommand == null)
                {
                    _buttonOneCommand = new RelayCommand(() => SetInitialsOption1());
                }
                return _buttonOneCommand;
            }
        }

        private RelayCommand _buttonTwoCommand;
        public RelayCommand ButtonTwoCommand
        {
            get
            {
                if (_buttonTwoCommand == null)
                {
                    _buttonTwoCommand = new RelayCommand(() => SetInitialsOption2());
                }
                return _buttonTwoCommand;
            }
        }

        private RelayCommand _buttonThreeCommand;
        public RelayCommand ButtonThreeCommand
        {
            get
            {
                if (_buttonThreeCommand == null)
                {
                    _buttonThreeCommand = new RelayCommand(() => SetInitialsOption3());
                }
                return _buttonThreeCommand;
            }
        }

        public QuizViewModel(INavigationService navigationService)
        {
            DispatcherHelper.Initialize();
            NavigationService = navigationService;
            HighScoreList = new ObservableCollection<string>();
            handler = new QuizHandler();
            scoreList = new List<bool>();
            QuizRound = new List<QuestionData>();
            handler.QuizFinished += Handler_QuizFinished;
            handler.SeeHighScore += Handler_SeeHighScore;
            handler.SetupCompleted += Handler_SetupCompleted;
            var DefaultColor = new LinearGradientBrush();
            DefaultColor.StartPoint = new Point(0.5, 0);
            DefaultColor.EndPoint = new Point(0.5, 1);
            DefaultColor.GradientStops = new GradientStopCollection();
            DefaultColor.GradientStops.Add(new GradientStop() { Color = Windows.UI.Colors.Gray, Offset = 0 });
            DefaultColor.GradientStops.Add(new GradientStop() { Color = Windows.UI.Colors.Brown, Offset = 1 });
            AlternativeOneBackground = DefaultColor;
            AlternativeTwoBackground = DefaultColor;
            AlternativeThreeBackground = DefaultColor;
        }

        private void Handler_SetupCompleted(object sender, EventArgs e)
        {
            ExternalButtons = new GpioButtonService();
            if (ExternalButtons != null)
            {
                ExternalButtons.ButtonOnePressed += ExternalButtons_ButtonOnePressed;
                ExternalButtons.ButtonTwoPressed += ExternalButtons_ButtonTwoPressed;
                ExternalButtons.ButtonThreePressed += ExternalButtons_ButtonThreePressed;
            }
            HeaderField = handler.GetHeaderText();
            BackgroundImage = handler.GetBackgroundPicture();
            Loading = false;

            var DefaultColor = new LinearGradientBrush();
            DefaultColor.StartPoint = new Point(0.5, 0);
            DefaultColor.EndPoint = new Point(0.5, 1);
            DefaultColor.GradientStops = new GradientStopCollection();
            DefaultColor.GradientStops.Add(new GradientStop() { Color = Windows.UI.Colors.Gray, Offset = 0 });
            DefaultColor.GradientStops.Add(new GradientStop() { Color = Windows.UI.Colors.Brown, Offset = 1 });
            QuestionFieldBackground = DefaultColor;
            QuestionField = messageSetYourInitialz;
        }

        private async void ExternalButtons_ButtonOnePressed(object sender, EventArgs e)
        {
            await DispatcherHelper.RunAsync(() => SetInitialsOption1());
        }

        private async void ExternalButtons_ButtonTwoPressed(object sender, EventArgs e)
        {
            await DispatcherHelper.RunAsync(() => SetInitialsOption2());
        }

        private async void ExternalButtons_ButtonThreePressed(object sender, EventArgs e)
        {
            await DispatcherHelper.RunAsync(() => SetInitialsOption3());
        }

        private void SetInitialsOption1()
        {
            #region SetInitialz
            if (initialOneSetted == false)
            {
                if (initialOne == charsToInitialz[0])
                {
                    initialOne = charsToInitialz[charsToInitialz.Length - 1];
                }
                else
                {
                    for (int i = 0; i < charsToInitialz.Length; i++)
                    {
                        if (initialOne == charsToInitialz[i])
                        {
                            initialOne = charsToInitialz[i - 1];
                        }
                    }
                }
                QuestionField = messageSetYourInitialz + initialOne;
            }
            else if (initialOneSetted == true && initialTwoSetted == false)
            {
                if (initialTwo == charsToInitialz[0])
                {
                    initialTwo = charsToInitialz[charsToInitialz.Length - 1];
                }
                else
                {
                    for (int i = 0; i < charsToInitialz.Length; i++)
                    {
                        if (initialTwo == charsToInitialz[i])
                        {
                            initialTwo = charsToInitialz[i - 1];
                        }
                    }
                }
                QuestionField = messageSetYourInitialz + initialOne + initialTwo;
            }
            #endregion
        }

        private void SetInitialsOption2()
        {
            #region CheckAndSetField
            if (initialOneSetted == false && string.IsNullOrEmpty(initialOne.ToString()) == false)
            {
                initialOneSetted = true;
            }
            else if (initialOneSetted == true && initialTwoSetted == false && string.IsNullOrEmpty(initialTwo.ToString()) == false)
            {
                initialTwoSetted = true;
                ChangeButtonEventHandlers();
            }
            #endregion
        }

        private void SetInitialsOption3()
        {
            #region SetInitialz
            if (initialOneSetted == false)
            {
                if (initialOne == charsToInitialz[charsToInitialz.Length - 1])
                {
                    initialOne = charsToInitialz[0];
                }
                else
                {
                    for (int i = 0; i < charsToInitialz.Length - 1; i++)
                    {
                        if (initialOne == charsToInitialz[i])
                        {
                            initialOne = charsToInitialz[i + 1];
                            break;
                        }
                    }
                }
                QuestionField = messageSetYourInitialz + initialOne;
            }
            else if (initialOneSetted == true && initialTwoSetted == false)
            {
                if (initialTwo == charsToInitialz[charsToInitialz.Length - 1])
                {
                    initialTwo = charsToInitialz[0];
                }
                else
                {
                    for (int i = 0; i < charsToInitialz.Length; i++)
                    {
                        if (initialTwo == charsToInitialz[i])
                        {
                            initialTwo = charsToInitialz[i + 1];
                            break;
                        }
                    }
                }
                QuestionField = messageSetYourInitialz + initialOne + initialTwo;
            }
            #endregion
        }

        private void ChangeButtonEventHandlers()
        {
            QuizRound = handler.GetQuestionsAnswers();
            ExternalButtons.ButtonOnePressed += AlternativeOnePressed;
            ExternalButtons.ButtonTwoPressed += AlternativeTwoPressed;
            ExternalButtons.ButtonThreePressed += AlternativeThreePressed;
            ExternalButtons.ButtonOnePressed -= ExternalButtons_ButtonOnePressed;
            ExternalButtons.ButtonTwoPressed -= ExternalButtons_ButtonTwoPressed;
            ExternalButtons.ButtonThreePressed -= ExternalButtons_ButtonThreePressed;
            QuestionField = "Tryck på valfri knapp för att börja Quizen...";
        }

        private async void AlternativeOnePressed(object sender, EventArgs e)
        {
            if (LoadNextQuestion == false)
            {
                await DispatcherHelper.RunAsync(() => CheckAnswer(1));
                LoadNextQuestion = true;
            }
            else
            {
                QuestionIndex++;
                if (handler.CheckIfHaveMoreQuestion(QuestionIndex))
                {
                    await DispatcherHelper.RunAsync(() => SetupNextQuestion());
                    LoadNextQuestion = false;
                }
                else
                {
                    DetachQuizHandlers();
                }
            }
        }

        private async void AlternativeTwoPressed(object sender, EventArgs e)
        {

            if (LoadNextQuestion == false)
            {
                await DispatcherHelper.RunAsync(() => CheckAnswer(2));
                LoadNextQuestion = true;
            }
            else
            {
                QuestionIndex++;
                if (handler.CheckIfHaveMoreQuestion(QuestionIndex))
                {
                    await DispatcherHelper.RunAsync(() => SetupNextQuestion());
                    LoadNextQuestion = false;
                }
                else
                {
                    DetachQuizHandlers();
                }
            }
        }

        private async void AlternativeThreePressed(object sender, EventArgs e)
        {
            if (LoadNextQuestion == false)
            {
                await DispatcherHelper.RunAsync(() => CheckAnswer(3));
                LoadNextQuestion = true;
            }
            else
            {
                QuestionIndex++;
                if (handler.CheckIfHaveMoreQuestion(QuestionIndex))
                {
                    await DispatcherHelper.RunAsync(() => SetupNextQuestion());
                    LoadNextQuestion = false;
                }
                else
                {
                    DetachQuizHandlers();
                }
            }
        }

        private void CheckAnswer(int choice)
        {
            var GreenColor = new LinearGradientBrush();
            GreenColor.StartPoint = new Point(0.5, 0);
            GreenColor.EndPoint = new Point(0.5, 1);
            GreenColor.GradientStops = new GradientStopCollection();
            GreenColor.GradientStops.Add(new GradientStop() { Color = Windows.UI.Colors.Green, Offset = 0 });
            GreenColor.GradientStops.Add(new GradientStop() { Color = Windows.UI.Colors.DarkSeaGreen, Offset = 1 });

            var RedColor = new LinearGradientBrush();
            RedColor.StartPoint = new Point(0.5, 0);
            RedColor.EndPoint = new Point(0.5, 1);
            RedColor.GradientStops = new GradientStopCollection();
            RedColor.GradientStops.Add(new GradientStop() { Color = Windows.UI.Colors.Red, Offset = 0 });
            RedColor.GradientStops.Add(new GradientStop() { Color = Windows.UI.Colors.DarkRed, Offset = 1 });

            switch (choice)
            {
                #region checkIfAnswerOneOrTwo
                case 1:
                    {
                        if (QuizRound[QuestionIndex].AlternativeOne == QuizRound[QuestionIndex].Answer)
                        {
                            scoreList.Add(true);
                            QuestionFieldBackground = GreenColor;
                            QuestionField = "RÄTT! Tryck på valfri knapp för att fortsätta.";
                            ShowResult(1);
                        }
                        else
                        {
                            scoreList.Add(false);
                            if (QuizRound[QuestionIndex].AlternativeTwo == QuizRound[QuestionIndex].Answer)
                            {
                                QuestionFieldBackground = RedColor;
                                QuestionField = "FEL! Tryck på valfri knapp för att fortsätta.";
                                ShowResult(2);
                            }
                            else
                            {
                                QuestionFieldBackground = RedColor;
                                QuestionField = "FEL! Tryck på valfri knapp för att fortsätta.";
                                ShowResult(3);
                            }
                        }
                        break;
                    }

                case 2:
                    {
                        if (QuizRound[QuestionIndex].AlternativeTwo == QuizRound[QuestionIndex].Answer)
                        {
                            scoreList.Add(true);
                            QuestionFieldBackground = GreenColor;
                            QuestionField = "RÄTT! Tryck på valfri knapp för att fortsätta.";
                            ShowResult(2);
                        }
                        else
                        {
                            scoreList.Add(false);
                            if (QuizRound[QuestionIndex].AlternativeOne == QuizRound[QuestionIndex].Answer)
                            {
                                QuestionFieldBackground = RedColor;
                                QuestionField = "FEL! Tryck på valfri knapp för att fortsätta.";
                                ShowResult(1);
                            }
                            else
                            {
                                QuestionFieldBackground = RedColor;
                                QuestionField = "FEL! Tryck på valfri knapp för att fortsätta.";
                                ShowResult(3);
                            }
                        }
                        break;
                    }

                case 3:
                    {
                        if (QuizRound[QuestionIndex].AlternativeThree == QuizRound[QuestionIndex].Answer)
                        {
                            scoreList.Add(true);
                            QuestionFieldBackground = GreenColor;
                            QuestionField = "RÄTT! Tryck på valfri knapp för att fortsätta.";
                            ShowResult(3);
                        }
                        else
                        {
                            scoreList.Add(false);
                            if (QuizRound[QuestionIndex].AlternativeTwo == QuizRound[QuestionIndex].Answer)
                            {
                                QuestionFieldBackground = RedColor;
                                QuestionField = "FEL! Tryck på valfri knapp för att fortsätta.";
                                ShowResult(2);
                            }
                            else
                            {
                                QuestionFieldBackground = RedColor;
                                QuestionField = "FEL! Tryck på valfri knapp för att fortsätta.";
                                ShowResult(1);
                            }
                        }
                        break;
                    }
                    #endregion
            }
        }

        private void SetupNextQuestion()
        {
            var DefaultColor = new LinearGradientBrush();
            DefaultColor.StartPoint = new Point(0.5, 0);
            DefaultColor.EndPoint = new Point(0.5, 1);
            DefaultColor.GradientStops = new GradientStopCollection();
            DefaultColor.GradientStops.Add(new GradientStop() { Color = Windows.UI.Colors.Gray, Offset = 0 });
            DefaultColor.GradientStops.Add(new GradientStop() { Color = Windows.UI.Colors.Brown, Offset = 1 });

            QuestionImage = handler.GetPicture(QuestionIndex);
            QuestionFieldBackground = DefaultColor;
            QuestionField = QuizRound[QuestionIndex].Question;
            AlternativeOneBackground = DefaultColor;
            AlternativeOneField = QuizRound[QuestionIndex].AlternativeOne;
            AlternativeTwoBackground = DefaultColor;
            AlternativeTwoField = QuizRound[QuestionIndex].AlternativeTwo;
            AlternativeThreeBackground = DefaultColor;
            AlternativeThreeField = QuizRound[QuestionIndex].AlternativeThree;
        }

        private void ShowResult(int correctAnswer)
        {

            var GreenColor = new LinearGradientBrush();
            GreenColor.StartPoint = new Point(0.5, 0);
            GreenColor.EndPoint = new Point(0.5, 1);
            GreenColor.GradientStops = new GradientStopCollection();
            GreenColor.GradientStops.Add(new GradientStop() { Color = Windows.UI.Colors.Green, Offset = 0 });
            GreenColor.GradientStops.Add(new GradientStop() { Color = Windows.UI.Colors.DarkSeaGreen, Offset = 1 });

            var RedColor = new LinearGradientBrush();
            RedColor.StartPoint = new Point(0.5, 0);
            RedColor.EndPoint = new Point(0.5, 1);
            RedColor.GradientStops = new GradientStopCollection();
            RedColor.GradientStops.Add(new GradientStop() { Color = Windows.UI.Colors.Red, Offset = 0 });
            RedColor.GradientStops.Add(new GradientStop() { Color = Windows.UI.Colors.DarkRed, Offset = 1 });

            switch (correctAnswer)
            {
                case 1:
                    {
                        AlternativeOneBackground = GreenColor;
                        AlternativeTwoBackground = RedColor;
                        AlternativeThreeBackground = RedColor;
                        break;
                    }
                case 2:
                    {
                        AlternativeOneBackground = RedColor;
                        AlternativeTwoBackground = GreenColor;
                        AlternativeThreeBackground = RedColor;
                        break;
                    }
                case 3:
                    {
                        AlternativeOneBackground = RedColor;
                        AlternativeTwoBackground = RedColor;
                        AlternativeThreeBackground = GreenColor;
                        break;
                    }
            }
        }

        private async void Handler_SeeHighScore(object sender, EventArgs e)
        {
            await DispatcherHelper.RunAsync(() =>
            {
                var DefaultColor = new LinearGradientBrush();
                DefaultColor.StartPoint = new Point(0.5, 0);
                DefaultColor.EndPoint = new Point(0.5, 1);
                DefaultColor.GradientStops = new GradientStopCollection();
                DefaultColor.GradientStops.Add(new GradientStop() { Color = Windows.UI.Colors.Gray, Offset = 0 });
                DefaultColor.GradientStops.Add(new GradientStop() { Color = Windows.UI.Colors.Brown, Offset = 1 });
                HighScoreBackGround = DefaultColor;
                QuestionFieldBackground = DefaultColor;
                QuestionField = "Tryck på knappen i mitten för att gå till Startsidan.";
                AlternativeOneBackground = DefaultColor;
                AlternativeOneField = "";
                AlternativeTwoBackground = DefaultColor;
                ExternalButtons.ButtonTwoPressed += ButtonTwoGoHomePressed;
                AlternativeTwoField = "Gå till startsida";
                AlternativeThreeBackground = DefaultColor;
                AlternativeThreeField = "";
                var index = 1;
                foreach (var person in handler.GetHighscores())
                {
                    HighScoreList.Add("Plats " + index + ":" + "\t" + person.ToString() + " av " + scoreList.Count() + " möjliga.");
                    index++;
                }
            });
        }

        private async void ButtonTwoGoHomePressed(object sender, EventArgs e)
        {
            ViewModelLocator.ResetViewModels();
            await ExternalButtons.Dispose();
            await DispatcherHelper.RunAsync(() => NavigationService.NavigateTo("StartPage"));
        }

        private async void Handler_QuizFinished(object sender, EventArgs e)
        {
            string initialz = initialOne.ToString() + initialTwo.ToString();
            int totalScore = 0;
            foreach (var score in scoreList)
            {
                if (score)
                {
                    totalScore++;
                }
            }
            await handler.AddNewScore(initialz, totalScore);
        }

        private void DetachQuizHandlers()
        {
            ExternalButtons.ButtonOnePressed -= AlternativeOnePressed;
            ExternalButtons.ButtonTwoPressed -= AlternativeTwoPressed;
            ExternalButtons.ButtonThreePressed -= AlternativeThreePressed;
        }
    }
}
