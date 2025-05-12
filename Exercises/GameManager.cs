using DualDolmen.Exercises.Pages;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;

namespace DualDolmen.Exercises
{
    public class UserData
    {
        public List<int> FinishedLevels { get; set; }
        public string TimePassed { get; set; }
        public int WordsLearnedCount { get; set; }
    }

    public class GameManager
    {
        public List<Exercise> exercises;
        public int currentExerciseIndex = 0;
        public string currentUsername;
        public UserData userData;
        public int levelNumber;
        public DateTime levelStartTime;
        private int completedWordsCountForLevel = 0;

        private static string AppDataPath =>
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "DualDolmen");

        private string UserDataFilePath =>
            Path.Combine(AppDataPath, "UsersData", $"{currentUsername}_data.json");

        public GameManager(int levelNumber, string CurrentUsername)
        {
            this.levelNumber = levelNumber;

            if (!File.Exists($"Levels/Level{levelNumber}.json"))
            {
                MessageBox.Show($"Ошибка - не найден файл Level{levelNumber}.json, загрузка уровня не возможна", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                Application.Current.Shutdown();
                return;
            }

            var jsonLevel = File.ReadAllText($"Levels/Level{levelNumber}.json");
            var levelData = System.Text.Json.JsonSerializer.Deserialize<LevelData>(jsonLevel);

            exercises = levelData.Exercises;
            completedWordsCountForLevel = levelData.CompletedWordsCount;
            this.currentUsername = CurrentUsername;

            string userDataDir = Path.GetDirectoryName(UserDataFilePath);
            if (!Directory.Exists(userDataDir))
                Directory.CreateDirectory(userDataDir);

            if (File.Exists(UserDataFilePath))
            {
                string jsonUserData = File.ReadAllText(UserDataFilePath);
                userData = System.Text.Json.JsonSerializer.Deserialize<UserData>(jsonUserData);
            }
            else
            {
                userData = new UserData
                {
                    FinishedLevels = new List<int>(),
                    TimePassed = "00:00:00",
                    WordsLearnedCount = 0
                };
            }

            this.levelStartTime = DateTime.Now;
        }

        public Page GetCurrentExercisePage()
        {
            var exercise = exercises[currentExerciseIndex];
            return exercise.Type switch
            {
                "Cards" => new CardsPage(exercise, this),
                "OddOne" => new OddOnePage(exercise, this),
                "InsertWord" => new InsertWordPage(exercise, this),
                _ => throw new Exception($"Неизвестный тип упражнения: {exercise.Type}")
            };
        }

        public void AdvanceExercise() => currentExerciseIndex++;

        public bool HasMoreExercises => currentExerciseIndex < exercises.Count - 1;

        public void MarkLevelAsCompleted()
        {
            if (!userData.FinishedLevels.Contains(levelNumber))
            {
                userData.WordsLearnedCount += completedWordsCountForLevel;
                userData.FinishedLevels.Add(levelNumber);
            }

            TimeSpan sessionDuration = DateTime.Now - levelStartTime;
            TimeSpan totalTimePassed = TimeSpan.TryParse(userData.TimePassed, out var parsed)
                ? parsed + sessionDuration
                : sessionDuration;

            userData.TimePassed = totalTimePassed.ToString(@"hh\:mm\:ss");

            string jsonUserData = JsonConvert.SerializeObject(userData, Formatting.Indented);
            File.WriteAllText(UserDataFilePath, jsonUserData);
        }
    }
}
