using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;

namespace DualDolmen.Exercises.Pages
{
    /// <summary>
    /// Interaction logic for CardsPage.xaml
    /// </summary>
    public partial class CardsPage : Page
    {
        private List<CardItem> cards = new();
        private GameManager gameManager;
        private Exercise exercise;

        private Button firstSelectedCard;
        private CardItem firstSelectedItem;

        private int totalPairCount = 0; // Количество пар всего
        private int pairCount = 0; // Количество пар выбрано

        private bool _isExiting = false; // Флаг для блокировки повторных нажатий при выходе из упражнения
        private bool _isProcessing = false; // Флаг для блокировки повторных нажатий при след. упражнении

        public CardsPage(Exercise exercise, GameManager gameManager)
        {
            InitializeComponent();
            this.exercise = exercise;
            this.gameManager = gameManager;
            InitCards();
        }

        private void InitCards()
        {
            // Заполнение карточек из данных упражнения
            foreach (var pairElement in exercise.Content.EnumerateArray())
            {
                string term = pairElement.GetProperty("Term").GetString();
                string match = pairElement.GetProperty("Match").GetString();

                // term связывает слово и его перевод, из 3 строк json получаем 6 карточек, из 4 строк - 8 карточек и т.д.
                cards.Add(new CardItem { Text = term, PairId = term });
                cards.Add(new CardItem { Text = match, PairId = term });

                totalPairCount++;
            }

            Shuffle(cards);

            foreach (var card in cards)
            {
                var button = new Button
                {
                    Content = card.Text,
                    Tag = card,
                    Margin = new Thickness(15),
                    FontSize = 20,
                    Background = new SolidColorBrush(Colors.Snow),
                    Cursor = Cursors.Hand
                };
                button.Click += Card_Click;
                CardGrid.Children.Add(button);
            }

            // Обновление номера упражнения в левом верхнем углу
            ProgressTextBlock.Text = $"{gameManager.currentExerciseIndex + 1}/{gameManager.exercises.Count}";
        }

        private async void Card_Click(object sender, RoutedEventArgs e)
        {
            if (_isProcessing) return; // Блокировка кликов при завершении

            var button = sender as Button;
            var card = (CardItem)button.Tag;

            if (firstSelectedCard == null)
            {
                firstSelectedCard = button;
                firstSelectedItem = card;
                button.Background = Brushes.LightBlue;
            }
            else if (button != firstSelectedCard)
            {
                _isProcessing = true; // блокируем до завершения текущей пары

                // Подсветка второго выбора
                button.Background = Brushes.LightBlue;

                // Подождём немного, чтобы пользователь увидел выбор
                await Task.Delay(250);

                if (firstSelectedItem.PairId == card.PairId)
                {
                    // Совпадение
                    button.Background = Brushes.LightGreen;
                    firstSelectedCard.Background = Brushes.LightGreen;

                    await Task.Delay(250); // небольшая задержка перед скрытием

                    button.Visibility = Visibility.Hidden;
                    firstSelectedCard.Visibility = Visibility.Hidden;
                    pairCount++;
                }
                else
                {
                    // Ошибка — показываем красный после небольшой паузы
                    button.Background = Brushes.IndianRed;
                    firstSelectedCard.Background = Brushes.IndianRed;

                    await Task.Delay(500); // время показа ошибки

                    button.Background = Brushes.White;
                    firstSelectedCard.Background = Brushes.White;
                }

                // Очистка
                firstSelectedCard = null;
                firstSelectedItem = null;

                _isProcessing = false;

                // Только здесь вызываем проверку, чтобы исключить ранний переход
                CheckIfCompleted();
            }
        }

        // Переход на страницу MenuExercises в случае, если все упражнения кончились на уровне
        private async void CheckIfCompleted()
        {
            if (_isProcessing) return;

            if (pairCount == totalPairCount)
            {
                _isProcessing = true;

                if (!gameManager.HasMoreExercises)
                {
                    gameManager.MarkLevelAsCompleted();

                    // Плавное затухание перед переходом на MenuExercises
                    var fadeOut = new DoubleAnimation
                    {
                        From = 1.0,
                        To = 0.0,
                        Duration = TimeSpan.FromSeconds(0.5),
                        EasingFunction = new CubicEase { EasingMode = EasingMode.EaseIn }
                    };

                    this.BeginAnimation(OpacityProperty, fadeOut);
                    await Task.Delay(500);

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
                else
                {
                    gameManager.AdvanceExercise();

                    var fadeOut = new DoubleAnimation
                    {
                        From = 1.0,
                        To = 0.0,
                        Duration = TimeSpan.FromSeconds(0.5),
                        EasingFunction = new CubicEase { EasingMode = EasingMode.EaseIn }
                    };

                    this.BeginAnimation(OpacityProperty, fadeOut);
                    await Task.Delay(500);

                    var nextPage = gameManager.GetCurrentExercisePage();
                    nextPage.Opacity = 0;
                    NavigationService.Navigate(nextPage);

                    var fadeIn = new DoubleAnimation
                    {
                        From = 0.0,
                        To = 1.0,
                        Duration = TimeSpan.FromSeconds(0.5),
                    };
                    nextPage.BeginAnimation(OpacityProperty, fadeIn);
                }
            }
        }

        // Перетасовка карточек
        private void Shuffle<T>(IList<T> list)
        {
            var rng = new Random();
            for (int i = 0; i < list.Count; i++)
            {
                int j = rng.Next(i, list.Count);
                (list[i], list[j]) = (list[j], list[i]);
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

    public class CardItem
    {
        public string Text { get; set; }
        public string PairId { get; set; } // идентификатор пары
    }
}
