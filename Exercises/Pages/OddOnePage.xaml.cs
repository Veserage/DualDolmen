using Newtonsoft.Json;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Input;
using System.Windows.Media.Animation;

namespace DualDolmen.Exercises.Pages
{
    /// <summary>
    /// Interaction logic for OddOnePage.xaml
    /// </summary>
    public partial class OddOnePage : Page
    {
        private GameManager gameManager;
        private Exercise exercise;

        private List<string> Words;
        private string Answer;

        private bool _isExiting = false; // Флаг для блокировки повторных нажатий привыходе из упражнения
        private bool _isProcessing = false; // Флаг для блокировки повторных нажатий при след. упражнении

        public OddOnePage(Exercise exercise, GameManager gameManager)
        {
            InitializeComponent();
            this.exercise = exercise;
            this.gameManager = gameManager;

            InitWords();
        }

        private void InitWords()
        {
            Words = exercise.Content.GetProperty("Words").EnumerateArray().Select(x => x.GetString()).ToList();
            Answer = exercise.Content.GetProperty("Answer").ToString();

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
            // Если уже обрабатывается нажатие - игнорируем
            if (_isProcessing) return;

            _isProcessing = true; // Устанавливаем флаг обработки
            var btn = sender as Button;

            try
            {
                if (btn == null) return;

                if (btn.Content.ToString() == Answer)
                {
                    // 1. Подсвечиваем правильный ответ зелёным
                    btn.Background = Brushes.LightGreen;

                    // 2. Ждём 1 секунду, чтобы пользователь увидел подсветку
                    await Task.Delay(800);

                    // 3. Анимация сдвига влево текущей страницы
                    var slideOut = new ThicknessAnimation
                    {
                        From = new Thickness(0),
                        To = new Thickness(-this.ActualWidth, 0, this.ActualWidth, 0),
                        Duration = TimeSpan.FromSeconds(0.8),
                        EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
                    };

                    var mainContainer = this.Content as FrameworkElement;
                    if (mainContainer != null)
                    {
                        mainContainer.BeginAnimation(MarginProperty, slideOut);
                    }

                    await Task.Delay(500);

                    if (!gameManager.HasMoreExercises)
                    {
                        gameManager.MarkLevelAsCompleted();
                        NavigationService.Navigate(new MenuExercises(gameManager.currentUsername));
                        return;
                    }

                    gameManager.AdvanceExercise();
                    var nextPage = gameManager.GetCurrentExercisePage();

                    // Подготовка новой страницы
                    nextPage.Opacity = 1;
                    var nextPageContainer = nextPage.Content as FrameworkElement;
                    if (nextPageContainer != null)
                    {
                        nextPageContainer.Margin = new Thickness(this.ActualWidth, 0, -this.ActualWidth, 0);
                    }

                    NavigationService.Navigate(nextPage);

                    // Анимация "въезда" новой страницы
                    if (nextPageContainer != null)
                    {
                        var slideIn = new ThicknessAnimation
                        {
                            To = new Thickness(0),
                            Duration = TimeSpan.FromSeconds(0.8),
                            EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
                        };
                        nextPageContainer.BeginAnimation(MarginProperty, slideIn);
                    }
                }
                else
                {
                    // Обработка неправильного ответа
                    btn.Background = Brushes.IndianRed;
                    await Task.Delay(500);
                    btn.Background = Brushes.Snow;
                }
            }
            finally
            {
                _isProcessing = false; // Всегда сбрасываем флаг, даже если возникла ошибка
            }
        }

        private async void Exit_Click(object sender, RoutedEventArgs e)
        {
            // Если уже выходим - игнорируем повторное нажатие
            if (_isExiting) return;

            _isExiting = true; // Устанавливаем флаг

            try
            {
                // Плавное затемнение текущей страницы
                var fadeOut = new DoubleAnimation
                {
                    From = 1.0,
                    To = 0.0,
                    Duration = TimeSpan.FromSeconds(0.5),
                    EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
                };

                this.BeginAnimation(OpacityProperty, fadeOut);
                await Task.Delay(500); // Ждём завершения анимации

                // Подсчёт времени
                TimeSpan sessionDuration = DateTime.Now - gameManager.levelStartTime;

                TimeSpan totalTimePassed = TimeSpan.Zero;
                if (TimeSpan.TryParse(gameManager.userData.TimePassed, out var parsed))
                    totalTimePassed = parsed;

                // Обновляем время
                totalTimePassed += sessionDuration;
                gameManager.userData.TimePassed = totalTimePassed.ToString(@"hh\:mm\:ss");

                // Сохранение данных
                string appDataPath = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    "DualDolmen",
                    "UsersData"
                );

                Directory.CreateDirectory(appDataPath); // Создаём директорию если её нет

                string userFilePath = Path.Combine(appDataPath, $"{gameManager.currentUsername}_data.json");
                string newTimeUserData = JsonConvert.SerializeObject(gameManager.userData, Formatting.Indented);
                await File.WriteAllTextAsync(userFilePath, newTimeUserData);

                // Плавное появление новой страницы
                var newPage = new MenuExercises(gameManager.currentUsername);
                newPage.Opacity = 0;
                NavigationService.Navigate(newPage);

                var fadeIn = new DoubleAnimation
                {
                    From = 0.0,
                    To = 1.0,
                    Duration = TimeSpan.FromSeconds(0.5),
                };
                newPage.BeginAnimation(OpacityProperty, fadeIn);
            }
            finally
            {
                _isExiting = false; // Сбрасываем флаг даже в случае ошибки
            }
        }

        // Загрузка данных пользователя из AppData
        public void LoadUserData(string username)
        {
            // Определяем путь к AppData для текущего пользователя
            string appDataPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "DualDolmen", // Название вашей программы или директории для приложения
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
