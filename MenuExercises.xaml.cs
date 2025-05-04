using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;

namespace DualDolmen
{
    public partial class MenuExercises : Page
    {
        public string CurrentUsername { get; set; }

        public MenuExercises(string username)
        {
            InitializeComponent();
            CurrentUsername = username;
            DataContext = this;
        }

        private void AccountButton_Click(object sender, RoutedEventArgs e)
        {
            ContentBorder.Child = GetAccountPanel();
        }

        private void ExercisesButton_Click(object sender, RoutedEventArgs e)
        {
            ContentBorder.Child = GetExercisesPanel();
        }

        private void LearnedWordsButton_Click(object sender, RoutedEventArgs e)
        {
            ContentBorder.Child = GetLearnedWordsPanel();
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            ContentBorder.Child = GetLogoutConfirmationPanel();
        }

        // Мой аккаунт
        private UIElement GetAccountPanel() 
        {
            var panel = new StackPanel();
            panel.Children.Add(new TextBlock { Text = $"{ CurrentUsername }", Style = (Style)FindResource("HeaderTextStyle") });
            panel.Children.Add(new TextBlock { Text = "Уровней пройдено:", Style = (Style)FindResource("InfoTextStyle") });
            panel.Children.Add(new TextBlock { Text = "Точность ответов:", Style = (Style)FindResource("InfoTextStyle") });
            panel.Children.Add(new TextBlock { Text = "Времени в упражнениях:", Style = (Style)FindResource("InfoTextStyle") });
            panel.Children.Add(new TextBlock { Text = "Всего слов изучено:", Style = (Style)FindResource("InfoTextStyle") });
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

            // Цвета
            var categoryColor = Color.FromRgb(6, 101, 123);     // #06657B
            var categoryHoverColor = Color.FromRgb(4, 77, 95);  // #044D5F
            var levelColor = Color.FromRgb(8, 129, 155);        // #08819B
            var levelHoverColor = Color.FromRgb(6, 101, 123);   // #06657B

            // Стиль для кнопок категорий (увеличенные размеры)
            var categoryButtonStyle = new Style(typeof(Button))
            {
                Setters =
                    {
                        new Setter(Button.BackgroundProperty, new SolidColorBrush(categoryColor)),
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
                                new Setter(Button.BackgroundProperty, new SolidColorBrush(categoryHoverColor)),
                                new Setter(Button.CursorProperty, Cursors.Hand),
                                new Setter(Button.ForegroundProperty, Brushes.Black)
                            }
                        }
                    }
            };

            // Стиль для кнопок уровней (увеличенные размеры)
            var levelButtonStyle = new Style(typeof(Button))
            {
                Setters =
                    {
                        new Setter(Button.BackgroundProperty, new SolidColorBrush(levelColor)),
                        new Setter(Button.ForegroundProperty, Brushes.White),
                        new Setter(Button.FontSizeProperty, 18.0), // Увеличенный шрифт
                        new Setter(Button.FontWeightProperty, FontWeights.SemiBold),
                        new Setter(Button.BorderThicknessProperty, new Thickness(0)),
                        new Setter(Button.PaddingProperty, new Thickness(20, 12, 20, 12)), // Больше отступы
                        new Setter(Button.MarginProperty, new Thickness(0, 5, 0, 5)), // Больше отступы
                        new Setter(Button.HorizontalContentAlignmentProperty, HorizontalAlignment.Left),
                        new Setter(Button.MinHeightProperty, 50.0) // Минимальная высота
                    },
                Triggers =
                    {
                        new Trigger
                        {
                            Property = Button.IsMouseOverProperty,
                            Value = true,
                            Setters =
                            {
                                new Setter(Button.BackgroundProperty, new SolidColorBrush(levelHoverColor)),
                                new Setter(Button.CursorProperty, Cursors.Hand),
                                new Setter(Button.ForegroundProperty, Brushes.Black)
                            }
                        }
                    }
            };

            // Словарь категорий и их уровней
            var categories = new Dictionary<string, List<string>>
            {
                { "Общение", new List<string> { "Приветствие и прощание", "Знакомство", "Поздравление", "Обращение к человеку", "Встреча" } },
                { "Городская черта", new List<string> { "Почта, телеграф", "Транспорт", "Внука", "В бассейне", "Развлечения" } },
                { "Путешествия", new List<string> { "На самолёте", "На поезде", "На теплоходе", "На автобусе", "В такси" } },
                { "Магазин, покупки", new List<string> { "В супермаркете", "На рынке", "Фрукты", "Овощи", "Ягоды" } },
                { "Прибытие и отбытие", new List<string> { "Паспортный контроль", "Таможня", "На вокзале", "В аэропорту", "В отеле" } }
            };

            foreach (var category in categories)
            {
                var categoryContainer = new StackPanel { Orientation = Orientation.Vertical };

                var categoryButton = new Button
                {
                    Content = category.Key,
                    Style = categoryButtonStyle,
                    Tag = category.Value
                };

                var levelsPanel = new StackPanel
                {
                    Orientation = Orientation.Vertical,
                    Visibility = Visibility.Collapsed,
                    Margin = new Thickness(25, 0, 0, 10) // Увеличенный отступ
                };

                for (int i = 0; i < category.Value.Count; i++)
                {
                    var levelButton = new Button
                    {
                        Content = $"Уровень {i + 1}. {category.Value[i]}",
                        Style = levelButtonStyle,
                        Tag = $"{category.Key}|{i + 1}"
                    };

                    levelButton.Click += (s, e) =>
                    {
                        var parts = ((string)levelButton.Tag).Split('|');
                        MessageBox.Show($"Выбрана категория: {parts[0]}, Уровень: {parts[1]}");
                    };

                    levelsPanel.Children.Add(levelButton);
                }

                categoryButton.Click += (s, e) =>
                {
                    levelsPanel.Visibility = levelsPanel.Visibility == Visibility.Visible
                        ? Visibility.Collapsed
                        : Visibility.Visible;
                };

                categoryContainer.Children.Add(categoryButton);
                categoryContainer.Children.Add(levelsPanel);
                mainPanel.Children.Add(categoryContainer);
            }

            return scrollViewer;
        }

        // Изученные слова
        private UIElement GetLearnedWordsPanel() 
        {
            var panel = new StackPanel { Margin = new Thickness(0, -10, 0, 0) };

            string[] categories = { "Существительные", "Глаголы", "Прилагательные", "Числительные", "Местоимения", "Наречия" };

            foreach (var category in categories)
            {
                var textBlock = new TextBlock
                {
                    Text = $"> {category}",
                    FontSize = 20,
                    FontWeight = FontWeights.Bold,
                    Foreground = Brushes.White,
                    Margin = new Thickness(0, 30, 0, 0),
                    TextDecorations = TextDecorations.Underline,
                    Cursor = System.Windows.Input.Cursors.Hand // Курсор в виде руки при наведении
                };
                
                // Добавляем тень
                textBlock.Effect = new DropShadowEffect
                {
                    Color = Colors.Black,
                    Direction = 315,
                    ShadowDepth = 2,
                    Opacity = 0.5,
                    BlurRadius = 2
                };

                // Делаем TextBlock кликабельным
                textBlock.MouseLeftButtonDown += (sender, e) =>
                {
                    MessageBox.Show($"Вы выбрали категорию: {category}", "Категория", MessageBoxButton.OK, MessageBoxImage.Information);
                };

                panel.Children.Add(textBlock);
            }

            return panel;
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
                Content = "Нет, я хотел бы остаться",
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
                    var usersApp = new UsersApp("users.json");

                    bool isDeleted = usersApp.DeleteUser(CurrentUsername);

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