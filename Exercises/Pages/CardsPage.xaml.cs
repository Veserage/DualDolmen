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

        private void Card_Click(object sender, RoutedEventArgs e)
        {
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
                // Второй клик
                if (firstSelectedItem.PairId == card.PairId)
                {
                    // Совпадение
                    button.Background = Brushes.LightGreen;
                    firstSelectedCard.Background = Brushes.LightGreen;

                    button.Visibility = Visibility.Hidden;
                    firstSelectedCard.Visibility = Visibility.Hidden;
                    pairCount++;
                }
                else
                {
                    // Ошибка
                    button.Background = Brushes.IndianRed;
                    firstSelectedCard.Background = Brushes.IndianRed;
                    //gameManager.RegisterFailure(gameManager.CurrentExerciseIndex);
                }

                // Очистка выбора
                Task.Delay(500).ContinueWith(_ =>
                {
                    Dispatcher.Invoke(() =>
                    {
                        button.Background = Brushes.White;

                        if (firstSelectedCard != null)
                            firstSelectedCard.Background = Brushes.White; // Card - это на самом деле Button

                        firstSelectedCard = null;
                        firstSelectedItem = null;

                        CheckIfCompleted();
                    });
                });
            }
        }

        private void CheckIfCompleted()
        {
            if (pairCount == totalPairCount)
            {
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

    public class CardItem
    {
        public string Text { get; set; }
        public string PairId { get; set; } // идентификатор пары
    }
}
