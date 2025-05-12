using Newtonsoft.Json;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Input;

namespace DualDolmen.Exercises.Pages
{
    /// <summary>
    /// Interaction logic for InsertWordPage.xaml
    /// </summary>
    public partial class InsertWordPage : Page
    {
        private GameManager gameManager;
        private Exercise exercise;

        private string Sentence;
        private List<string> Words;
        private string Answer;

        public InsertWordPage(Exercise exercise, GameManager gameManager)
        {
            InitializeComponent();
            this.exercise = exercise;
            this.gameManager = gameManager;

            SentenceTextBlock.Text = exercise.Content.GetProperty("Text").ToString();
            this.Answer = exercise.Content.GetProperty("Answer").ToString();

            InitWords();
        }

        private void InitWords()
        {
            Words = exercise.Content.GetProperty("Words").EnumerateArray().Select(x => x.GetString()).ToList();

            // Расположение кнопок со словами
            foreach (string word in Words)
            {
                var btn = new Button
                {
                    Content = word,
                    FontSize = 30,
                    Padding = new Thickness(12, 8, 12, 8),
                    Margin = new Thickness(20),
                    Background = Brushes.Snow,
                    Cursor = Cursors.Hand
                };

                btn.Click += Word_Click;
                WordsPanel.Children.Add(btn);
            }

            // Обновление номера упражнения в левом верхнем углу
            ProgressTextBlock.Text = $"{gameManager.currentExerciseIndex + 1}/{gameManager.exercises.Count}";
        }

        private async void Word_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            if (btn == null) return;

            if (btn.Content.ToString() == Answer)
            {
                btn.Background = Brushes.LightGreen;
                await Task.Delay(1000);

                // Если упражнений больше не осталось
                if (!gameManager.HasMoreExercises)
                {
                    gameManager.MarkLevelAsCompleted();
                    NavigationService.Navigate(new MenuExercises(gameManager.currentUsername));
                    return;
                }

                gameManager.AdvanceExercise();
                NavigationService.Navigate(gameManager.GetCurrentExercisePage());
            }
            else
            {
                btn.Background = Brushes.IndianRed;
                await Task.Delay(1000);
                btn.Background = Brushes.Snow;
            }
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            // Подсчёт времени
            TimeSpan sessionDuration = DateTime.Now - gameManager.levelStartTime;

            TimeSpan totalTimePassed = TimeSpan.Zero; // по умолчанию прошло 0 секунд
            if (TimeSpan.TryParse(gameManager.userData.TimePassed, out var parsed)) // Если не 0, то столько, сколько в файле прогресса пользователя
                totalTimePassed = parsed;

            // Обновляем время в gameManager
            totalTimePassed += sessionDuration;
            gameManager.userData.TimePassed = totalTimePassed.ToString(@"hh\:mm\:ss");

            // Определяем путь к AppData для текущего пользователя
            string appDataPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "DualDolmen", 
                "UsersData"
            );

            // Создаём директорию, если её нет
            if (!Directory.Exists(appDataPath))
            {
                Directory.CreateDirectory(appDataPath);
            }

            // Сохраняем userdata с новым временем в файл в AppData
            string newTimeUserData = JsonConvert.SerializeObject(gameManager.userData, Formatting.Indented);
            string userFilePath = Path.Combine(appDataPath, $"{gameManager.currentUsername}_data.json");
            File.WriteAllText(userFilePath, newTimeUserData);

            // Переход к меню
            NavigationService.Navigate(new MenuExercises(gameManager.currentUsername));
        }

        // Загрузка данных пользователя из AppData
        public void LoadUserData(string username)
        {
            // Определяем путь к AppData для текущего пользователя
            string appDataPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "DualDolmen", 
                "UsersData"
            );

            string userFilePath = Path.Combine(appDataPath, $"{username}_data.json");

            if (File.Exists(userFilePath))
            {
                // Загрузка данных о пользователе
                string userDataJson = File.ReadAllText(userFilePath);
                gameManager.userData = JsonConvert.DeserializeObject<UserData>(userDataJson);
            }
            else
            {
                // Если файл не найден, инициализируем пустые данные
                gameManager.userData = new UserData();
            }
        }
    }
}
