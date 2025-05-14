using DualDolmen.Exercises;
using Newtonsoft.Json;
using System.Windows;
using System.IO;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Text;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;

namespace DualDolmen
{
    public partial class MenuExercises : Page
    {
        static MenuExercises()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        }

        // Активный пользователь
        public string CurrentUsername { get; set; }
        UserData userData = new UserData();
        UserManager userManager = new UserManager();

        public MenuExercises(string username)
        {
            InitializeComponent();
            CurrentUsername = username;
            DataContext = this;

            string jsonUserData = string.Empty;

            try
            {
                // Формируем путь для файла в AppData
                string userDataFolder = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "DualDolmen", "UsersData");
                Directory.CreateDirectory(userDataFolder); // Создаём папку, если её нет

                string userDataFile = System.IO.Path.Combine(userDataFolder, $"{username}_data.json");

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

        private void GenerateReportButton_Click(object sender, RoutedEventArgs e)
        {
            ContentBorder.Child = GetGenerateReportPanel();
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
                Text = $"{CurrentUsername} - Добро пожаловать!",
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

                if (userData != null && userData.FinishedLevels.Contains(levels.IndexOf(level) + 1)) // +1 т.к. уровни с единицы, а индексы - с нуля
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

        private UIElement GetGenerateReportPanel()
        {
            var panel = new StackPanel
            {
                Margin = new Thickness(30),
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center
            };

            panel.Children.Add(new TextBlock
            {
                Text = "Вы действительно хотите сгенерировать отчёт о пользователе?",
                FontSize = 28,
                FontWeight = FontWeights.SemiBold,
                TextWrapping = TextWrapping.Wrap,
                TextAlignment = TextAlignment.Center,
                Margin = new Thickness(0, 0, 0, 30)
            });

            var buttonsPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(0, 25, 0, 0)
            };

            var yesButton = new Button
            {
                Content = "Да",
                FontSize = 20,
                Background = Brushes.ForestGreen,
                Foreground = Brushes.Black,
                Padding = new Thickness(32, 16, 32, 16),
                Margin = new Thickness(0, 0, 30, 0),
                FontWeight = FontWeights.SemiBold,
                MinWidth = 180,
                BorderBrush = Brushes.ForestGreen,
                BorderThickness = new Thickness(2),
                Cursor = Cursors.Hand, // Ладошка при наведении
                VerticalContentAlignment = VerticalAlignment.Center,
                HorizontalContentAlignment = HorizontalAlignment.Center,
                Effect = new DropShadowEffect
                {
                    Color = Colors.Black,
                    Direction = 320,
                    ShadowDepth = 3,
                    Opacity = 0.3
                }
            };

            var noButton = new Button
            {
                Content = "Нет",
                Background = Brushes.IndianRed,
                Foreground = Brushes.Black,
                Padding = new Thickness(32, 16, 32, 16),
                Margin = new Thickness(0, 0, 30, 0),
                FontSize = 20,
                FontWeight = FontWeights.SemiBold,
                MinWidth = 180,
                BorderBrush = Brushes.IndianRed,
                BorderThickness = new Thickness(2),
                Cursor = Cursors.Hand, // Ладошка при наведении
                VerticalContentAlignment = VerticalAlignment.Center,
                HorizontalContentAlignment = HorizontalAlignment.Center,
                Effect = new DropShadowEffect
                {
                    Color = Colors.Black,
                    Direction = 320,
                    ShadowDepth = 3,
                    Opacity = 0.3
                }
            };

            yesButton.Click += (s, e) =>
            {
                try
                {
                    Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

                    string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                    string filePath = System.IO.Path.Combine(desktopPath, "DualDolmen_Report.pdf");

                    // Шрифты
                    BaseFont baseFont = BaseFont.CreateFont(@"C:\Windows\Fonts\times.ttf", BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
                    Font font = new Font(baseFont, 14);
                    Font headerFont = new Font(baseFont, 14, Font.BOLD);
                    Font titleFont = new Font(baseFont, 18, Font.BOLD);
                    Font infoFont = new Font(baseFont, 14);

                    // Собираем все данные
                    List<string[]> allRows = new List<string[]>();

                    // Чтение существующих данных
                    if (File.Exists(filePath))
                    {
                        using (PdfReader reader = new PdfReader(filePath))
                        {
                            for (int i = 1; i <= reader.NumberOfPages; i++)
                            {
                                string pageText = PdfTextExtractor.GetTextFromPage(reader, i);
                                // Более точный парсинг данных из таблицы
                                var lines = pageText.Split('\n')
                                                  .Where(line => line.Trim().Length > 0)
                                                  .ToList();

                                // Пропускаем заголовки и описания
                                int dataStartIndex = lines.FindIndex(line => line.Contains("Имя") && line.Contains("Пароль")) + 1;
                                if (dataStartIndex > 0)
                                {
                                    for (int j = dataStartIndex; j < lines.Count; j++)
                                    {
                                        var row = lines[j].Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                                        if (row.Length >= 5) // Проверяем, что это строка с данными
                                        {
                                            allRows.Add(row);
                                        }
                                    }
                                }
                            }
                        }
                    }

                    // Добавляем новые данные
                    allRows.Add(new string[] {
                        CurrentUsername,
                        userManager.GetPassword(CurrentUsername),
                        userData.FinishedLevels.Count.ToString(),
                        userData.TimePassed.ToString(),
                        userData.WordsLearnedCount.ToString()
                    });

                    // Создаем новый PDF
                    using (FileStream fs = new FileStream(filePath, FileMode.Create))
                    {
                        Document document = new Document(PageSize.A4, 50, 50, 30, 30);
                        PdfWriter writer = PdfWriter.GetInstance(document, fs);
                        document.Open();

                        // Добавляем заголовок
                        Paragraph title = new Paragraph("Отчёт о данных пользователей приложения DualDolmen", titleFont)
                        {
                            Alignment = Element.ALIGN_CENTER,
                            SpacingAfter = 15f
                        };
                        document.Add(title);

                        // Добавляем информационный блок
                        Paragraph info = new Paragraph
                        {
                            SpacingAfter = 20f
                        };

                        // Дата генерации
                        Chunk dateLabel = new Chunk("Дата последней генерации отчёта: ", infoFont);
                        Chunk dateValue = new Chunk(DateTime.Now.ToString("dd.MM.yyyy HH:mm"), font);
                        info.Add(dateLabel);
                        info.Add(dateValue);

                        // Перенос строки
                        info.Add(Chunk.NEWLINE);

                        // Количество записей
                        Chunk countLabel = new Chunk("Всего записей: ", infoFont);
                        Chunk countValue = new Chunk(allRows.Count.ToString(), font);
                        info.Add(countLabel);
                        info.Add(countValue);

                        document.Add(info);

                        // Создаем таблицу
                        PdfPTable table = new PdfPTable(5)
                        {
                            WidthPercentage = 95,
                            HorizontalAlignment = Element.ALIGN_CENTER,
                            SpacingBefore = 10f,
                            SpacingAfter = 20f
                        };

                        // Устанавливаем ширину столбцов (в процентах)
                        float[] columnWidths = { 20f, 20f, 20f, 20f, 20f };
                        table.SetWidths(columnWidths);

                        // Заголовки таблицы
                        string[] headers = { "Имя", "Пароль", "Пройдено уровней", "Общее время в уровнях", "Изученные слова" };
                        foreach (string header in headers)
                        {
                            PdfPCell cell = new PdfPCell(new Phrase(header, headerFont))
                            {
                                HorizontalAlignment = Element.ALIGN_CENTER,
                                VerticalAlignment = Element.ALIGN_MIDDLE,
                                BackgroundColor = new BaseColor(230, 230, 230),
                                Padding = 8f,
                                BorderWidth = 0.75f
                            };
                            table.AddCell(cell);
                        }

                        // Данные таблицы
                        foreach (var row in allRows)
                        {
                            foreach (string item in row)
                            {
                                PdfPCell cell = new PdfPCell(new Phrase(item, font))
                                {
                                    HorizontalAlignment = Element.ALIGN_CENTER,
                                    VerticalAlignment = Element.ALIGN_MIDDLE,
                                    Padding = 6f,
                                    BorderWidth = 0.5f
                                };
                                table.AddCell(cell);
                            }
                        }

                        document.Add(table);
                        document.Close();
                    }

                    MessageBox.Show("PDF-отчёт успешно сохранён на рабочем столе!");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при генерации отчёта:\n{ex.Message}\n\n" +
                                   "Убедитесь, что файл не открыт в другой программе.");
                }
            };

            noButton.Click += (s, e) =>
            {
                ContentBorder.Child = GetAccountPanel();
            };

            buttonsPanel.Children.Add(yesButton);
            buttonsPanel.Children.Add(noButton);
            panel.Children.Add(buttonsPanel);

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
                FontSize = 28,
                FontWeight = FontWeights.SemiBold,
                Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#2a2b29")),
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
                Padding = new Thickness(24, 12, 24, 12),
                FontSize = 20,
                FontWeight = FontWeights.SemiBold,
                Margin = new Thickness(0, 0, 30, 0),
                MinWidth = 120,
                BorderThickness = new Thickness(0),
                Cursor = Cursors.Hand,
                VerticalContentAlignment = VerticalAlignment.Center,
                HorizontalContentAlignment = HorizontalAlignment.Center,
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
                Padding = new Thickness(24, 12, 24, 12),
                FontSize = 20,
                FontWeight = FontWeights.SemiBold,
                Margin = new Thickness(30, 0, 0, 0),
                MinWidth = 180,
                BorderThickness = new Thickness(0),
                Cursor = Cursors.Hand,
                VerticalContentAlignment = VerticalAlignment.Center,
                HorizontalContentAlignment = HorizontalAlignment.Center,
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