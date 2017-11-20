using SchoolQuizMachine.Models.Poco;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;
using System.Diagnostics;

namespace SchoolQuizMachine.Models.Handlers
{
    public class QuizHandler
    {
        private List<QuestionData> QuestionList;
        private List<BitmapImage> PictureList;
        private List<Person> HighScoreList;
        private FileHandler FileHandler;
        private string _headerText;

        public event EventHandler SetupCompleted;
        public event EventHandler QuizFinished;
        public event EventHandler SeeHighScore;

        public QuizHandler()
        {
            QuestionList = new List<QuestionData>();
            PictureList = new List<BitmapImage>();
            HighScoreList = new List<Person>();
            FileHandler = new FileHandler();
            FileHandler.DevicesIsInitialized += FileHandler_DevicesIsInitialized;
        }

        private async void FileHandler_DevicesIsInitialized(object sender, EventArgs e)
        {
            await SetupItems();
        }

        public List<Person> GetHighscores()
        {
            return HighScoreList;
        }

        public bool CheckIfHaveMoreQuestion(int index)
        {
            if (index >= QuestionList.Count)
            {
                QuizFinished?.Invoke(this, EventArgs.Empty);
                return false;
            }
            else
            {
                return true;
            }
        }

        public BitmapImage GetBackgroundPicture()
        {
            return PictureList[PictureList.Count - 1];
        }

        public BitmapImage GetPicture(int index)
        {
                return PictureList[index];
        }

        private async Task SetupItems()
        {
            foreach (var file in FileHandler.GetItems())
            {
                if (file.Name.Contains(".jpg") || file.Name.Contains(".jpeg") || file.Name.Contains(".bmp") ||
                    file.Name.Contains(".png") || file.Name.Contains(".gif"))
                {
                    if (file.Name.Length >= 10)
                    {
                        if (file.Name.ToLower().Substring(0, 10) == "background")
                        {
                            var splitPattern = new char[]{'#'};
                            var nameArray = file.Name.Split(splitPattern);
                            _headerText = nameArray[1].Substring(0,nameArray[1].Length - 4);
                        }
                    }
                    BitmapImage bitmapImage = new BitmapImage();
                    var stream = (FileRandomAccessStream)await file.OpenAsync(FileAccessMode.Read);
                    bitmapImage.SetSource(stream);
                    PictureList.Add(bitmapImage);
                    stream.Dispose();
                }                

                else if (file.Name.ToLower().Contains("questions.json"))
                {
                    Stream stream = await file.OpenStreamForReadAsync();
                    using (StreamReader str = new StreamReader(stream))
                    {
                        string jsonString = await str.ReadToEndAsync();
                        QuestionList = JsonConvert.DeserializeObject<List<QuestionData>>(jsonString);
                    }
                    stream.Dispose();

                }
                else if (file.Name.ToLower().Contains("highscore.json"))
                {
                    Stream stream = await file.OpenStreamForReadAsync();
                    using (StreamReader str = new StreamReader(stream))
                    {
                        string jsonString = await str.ReadToEndAsync();
                        HighScoreList = JsonConvert.DeserializeObject<List<Person>>(jsonString);
                    }
                    stream.Dispose();
                }
            }
            SetupCompleted?.Invoke(this, EventArgs.Empty);
        }

        public List<QuestionData> GetQuestionsAnswers()
        {
            return QuestionList;
        }

        public async Task AddNewScore(string initialz, int score)
        {
            if (HighScoreList == null)
            {
                HighScoreList = new List<Person>();
            }

            HighScoreList.Sort((p1, p2) => p2.Score.CompareTo(p1.Score));

            switch (HighScoreList.Count())
            {
                case 10:
                    {
                        HighScoreList.Add(new Person(initialz, score));
                        HighScoreList.Sort((p1, p2) => p2.Score.CompareTo(p1.Score));
                        HighScoreList.RemoveAt(10);
                        await FileHandler.SaveHighScore(HighScoreList);
                        SeeHighScore?.Invoke(this, EventArgs.Empty);
                        break;
                    }
                default:
                    {
                        HighScoreList.Add(new Person(initialz, score));
                        HighScoreList.Sort((p1, p2) => p2.Score.CompareTo(p1.Score));
                        await FileHandler.SaveHighScore(HighScoreList);
                        SeeHighScore?.Invoke(this, EventArgs.Empty);
                        break;
                    }
            }
        }

        internal string GetHeaderText()
        {
            return _headerText;
        }
    }
}

