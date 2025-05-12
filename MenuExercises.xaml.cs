using DualDolmen.Exercises;
using Newtonsoft.Json;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json.Nodes;
using System.Windows;
using System.IO;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;

namespace DualDolmen
{
    public partial class MenuExercises : Page
    {
        // Активный пользователь
        public string CurrentUsername { get; set; }
		private UserData userData;

        public MenuExercises(string username)
        {
            InitializeComponent();
            CurrentUsername = username;
            DataContext = this;

            string jsonUserData = string.Empty;

            try
            {
                // Формируем путь для файла в AppData
                string userDataFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "DualDolmen", "UsersData");
                Directory.CreateDirectory(userDataFolder); // Создаём папку, если её нет

                string userDataFile = Path.Combine(userDataFolder, $"{username}_data.json");

                if (!File.Exists(userDataFile))
                {
                    // Если файл не существует, создаём новый
                    userData = new UserData
                    {
                        FinishedLevels = new List<int>(),
                        TimePassed = "00:00:00",
                        WordsLearnedCount = 0
                    };
                    jsonUserData = JsonConvert.SerializeObject(userData, Formatting.Indented);
                    File.WriteAllText(userDataFile, jsonUserData);
                }
                else
                {
                    jsonUserData = File.ReadAllText(userDataFile);
                    if (string.IsNullOrEmpty(jsonUserData))
                    {
                        MessageBox.Show($"Ошибка, файл {userDataFile} пуст", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        Application.Current.Shutdown();
                        return;
                    }
                    userData = JsonConvert.DeserializeObject<UserData>(jsonUserData);
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                Application.Current.Shutdown();
                return;
            }

            // Обновление информации о пользователе на панели "Мой аккаунт"
            ContentBorder.Child = GetAccountPanel();
        }


        private void AccountButton_Click(object sender, RoutedEventArgs e)
        {
            ContentBorder.Child = GetAccountPanel();
        }

        private void ExercisesButton_Click(object sender, RoutedEventArgs e)
        {
            ContentBorder.Child = GetExercisesPanel();
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            ContentBorder.Child = GetLogoutConfirmationPanel();
        }

		// Мой аккаунт
		private UIElement GetAccountPanel()
		{
			var panel = new StackPanel();

            // Никнейм пользователя
			var header = new TextBlock
			{
				Text = $"{CurrentUsername}",
				Style = (Style)FindResource("HeaderTextStyle")
			};

			var levelsCompleted = new TextBlock
			{
				Text = $"Уровней пройдено: {userData?.FinishedLevels?.Count ?? 0}",
				Style = (Style)FindResource("InfoTextStyle"),
				TextDecorations = TextDecorations.Underline
			};

			var timeSpent = new TextBlock
			{
				Text = $"Времени в упражнениях: {userData?.TimePassed ?? "00:00:00"}",
				Style = (Style)FindResource("InfoTextStyle"),
				TextDecorations = TextDecorations.Underline
			};

			var wordsLearned = new TextBlock
			{
				Text = $"Всего слов изучено: {userData?.WordsLearnedCount ?? 0}",
				Style = (Style)FindResource("InfoTextStyle"),
                TextDecorations = TextDecorations.Underline
			};

			panel.Children.Add(header);
			panel.Children.Add(levelsCompleted);
			panel.Children.Add(timeSpent);
			panel.Children.Add(wordsLearned);

			return panel;
		}


		// Упражнения
		private UIElement GetExercisesPanel() 
        {
            var scrollViewer = new ScrollViewer
            {
                VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
                HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled,
                Padding = new Thickness(5)
            };

            var mainPanel = new StackPanel { Orientation = Orientation.Vertical };
            scrollViewer.Content = mainPanel;

            // Цвета кнопок уровней
            var unfinishedColor = Color.FromRgb(6, 101, 123);     
            var unfinishedHoverColor = Color.FromRgb(4, 77, 95);  

            var finishedColor = Color.FromRgb(8, 155, 37);       
            var finishedHoverColor = Color.FromRgb(8, 106, 27);  

            // Стиль для кнопок незавершенных уровней
            var unfinishedLevelStyle = new Style(typeof(Button))
            {
                Setters =
                    {
                        new Setter(Button.BackgroundProperty, new SolidColorBrush(unfinishedColor)),
                        new Setter(Button.ForegroundProperty, Brushes.White),
                        new Setter(Button.FontSizeProperty, 20.0), // Увеличенный шрифт
                        new Setter(Button.FontWeightProperty, FontWeights.Bold), // Более жирный
                        new Setter(Button.BorderThicknessProperty, new Thickness(0)),
                        new Setter(Button.PaddingProperty, new Thickness(20, 15, 20, 15)), // Больше отступы
                        new Setter(Button.MarginProperty, new Thickness(0, 8, 0, 0)), // Больше отступ сверху
                        new Setter(Button.HorizontalContentAlignmentProperty, HorizontalAlignment.Left),
                        new Setter(Button.MinHeightProperty, 60.0) // Минимальная высота
                    },
                Triggers =
                    {
                        new Trigger
                        {
                            Property = Button.IsMouseOverProperty,
                            Value = true,
                            Setters =
                            {
                                new Setter(Button.BackgroundProperty, new SolidColorBrush(unfinishedHoverColor)),
                                new Setter(Button.CursorProperty, Cursors.Hand),
                                new Setter(Button.ForegroundProperty, Brushes.Black)
                            }
                        }
                    }
            };

            // Стиль для кнопок завершенных уровней
            var finishedLevelStyle = new Style(typeof(Button))
            {
                Setters =
                    {
                        new Setter(Button.BackgroundProperty, new SolidColorBrush(finishedColor)),
                        new Setter(Button.ForegroundProperty, Brushes.White),
                        new Setter(Button.FontSizeProperty, 20.0), // Увеличенный шрифт
                        new Setter(Button.FontWeightProperty, FontWeights.Bold), // Более жирный
                        new Setter(Button.BorderThicknessProperty, new Thickness(0)),
                        new Setter(Button.PaddingProperty, new Thickness(20, 15, 20, 15)), // Больше отступы
                        new Setter(Button.MarginProperty, new Thickness(0, 8, 0, 0)), // Больше отступ сверху
                        new Setter(Button.HorizontalContentAlignmentProperty, HorizontalAlignment.Left),
                        new Setter(Button.MinHeightProperty, 60.0) // Минимальная высота
                    },
                Triggers =
                    {
                        new Trigger
                        {
                            Property = Button.IsMouseOverProperty,
                            Value = true,
                            Setters =
                            {
                                new Setter(Button.BackgroundProperty, new SolidColorBrush(finishedHoverColor)),
                                new Setter(Button.CursorProperty, Cursors.Hand),
                                new Setter(Button.ForegroundProperty, Brushes.Black)
                            }
                        }
                    }
            };


            // Список уровней
            var levels = new List<string>
            {
	            { "Основы общения" },
	            { "Мой мир" },
	            { "Путешествия" },
	            { "В магазине" },
	            { "Хобби" }
            };

			foreach (var level in levels)
			{
                Button levelButton;

                // Если уровень оказался в списке пройденных
                
                if (userData != null && userData.FinishedLevels.Contains( levels.IndexOf(level) + 1)) // +1 т.к. уровни с единицы, а индексы - с нуля
                {
                    levelButton = new Button
                    {
                        Content = level,
                        Style = finishedLevelStyle,
                    };
                }
                else // Если уровень еще не был пройден 
                {
                    levelButton = new Button
                    {
                        Content = level,
                        Style = unfinishedLevelStyle,
                    };
                }


                // Переход на уровень
                levelButton.Click += (s, e) =>
                {
                    GameManager gameManager = new GameManager(levels.IndexOf(level) + 1, CurrentUsername); // Названия уровней в Levels идут с 1, а не с нуля.
                    NavigationService.Navigate(gameManager.GetCurrentExercisePage());
                };

				
				mainPanel.Children.Add(levelButton);
			}

			return scrollViewer;
		}

        // Выход из аккаунта
        private UIElement GetLogoutConfirmationPanel() 
        {
            var panel = new StackPanel
            {
                Margin = new Thickness(30),
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center
            };

            // Текст с увеличенным отступом сверху
            panel.Children.Add(new TextBlock
            {
                Text = "Вы действительно хотите выйти из аккаунта?",
                FontSize = 26,
                FontWeight = FontWeights.SemiBold,
                Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#E0E2D4")),
                TextWrapping = TextWrapping.Wrap,
                Margin = new Thickness(20, -10, 0, 25),
                TextAlignment = TextAlignment.Center
            });

            var buttonsPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(0, 30, 0, 0) 
            };

            var buttonTextColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#000000"));

            var stayButton = new Button
            {
                Content = "Нет",
                Background = Brushes.LightGray,
                Foreground = buttonTextColor,
                Padding = new Thickness(16, 8, 16, 8),
                FontSize = 16,
                FontWeight = FontWeights.Medium,
                Margin = new Thickness(0, 0, 15, 0),
                BorderThickness = new Thickness(0),
                BorderBrush = null,
                Cursor = Cursors.Hand
            };

            stayButton.Click += (s, e) =>
            {
                ContentBorder.Child = GetAccountPanel();
            };

            var leaveButton = new Button
            {
                Content = "Да, конечно",
                Background = Brushes.IndianRed,
                Foreground = buttonTextColor,
                Padding = new Thickness(16, 8, 16, 8),
                FontSize = 16,
                FontWeight = FontWeights.Medium,
                Width = 160,
                BorderThickness = new Thickness(0),
                BorderBrush = null,
                Cursor = Cursors.Hand
            };

            leaveButton.Click += async (s, e) =>
            {
                var fadeOut = new DoubleAnimation
                {
                    From = 1,
                    To = 0,
                    Duration = TimeSpan.FromSeconds(0.3),
                    EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseIn }
                };

                this.BeginAnimation(OpacityProperty, fadeOut);
                await Task.Delay(300);

                NavigationService?.Navigate(new Authorization());
            };

            buttonsPanel.Children.Add(stayButton);
            buttonsPanel.Children.Add(leaveButton);
            panel.Children.Add(buttonsPanel);

            return panel;
        }

        // Удаление пользователя
        private async void DeleteLabel_MouseDown(object sender, MouseButtonEventArgs e) 
        {
            if (string.IsNullOrEmpty(CurrentUsername))
            {
                MessageBox.Show("Ошибка: Пользователь не авторизован!", "Ошибка",
                                MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var result = MessageBox.Show(
                "Вы действительно хотите удалить аккаунт? Это действие нельзя отменить!",
                "Подтверждение удаления",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    var UserManager = new UserManager();

                    bool isDeleted = UserManager.DeleteUser(CurrentUsername);

                    if (isDeleted)
                    {
                        MessageBox.Show("Аккаунт успешно удалён!", "Успех",
                                        MessageBoxButton.OK, MessageBoxImage.Information);

                        // Анимация исчезновения текущей страницы
                        var fadeOut = new DoubleAnimation
                        {
                            From = 1,
                            To = 0,
                            Duration = TimeSpan.FromSeconds(0.3),
                            EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseIn }
                        };

                        this.BeginAnimation(OpacityProperty, fadeOut);
                        await Task.Delay(300);

                        // Переход на страницу авторизации
                        NavigationService?.Navigate(new Authorization());
                    }
                    else
                    {
                        MessageBox.Show("Не удалось удалить аккаунт.", "Ошибка",
                                        MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при удалении: {ex.Message}", "Ошибка",
                                    MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}