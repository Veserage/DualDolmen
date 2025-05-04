using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using Newtonsoft.Json;

namespace DualDolmen
{
    public partial class Enter : Page
    {
        private bool _isNavigationInProgress = false;
        private Dictionary<string, string> _users = new Dictionary<string, string>();

        public Enter()
        {
            InitializeComponent();
            LoadUsers();
        }
        // TODO: заебошить enter

        private void LoadUsers()
        {
            try
            {
                string appDirectory = AppDomain.CurrentDomain.BaseDirectory;
                string usersFilePath = Path.Combine(appDirectory, "users.json");

                if (File.Exists(usersFilePath))
                {
                    string json = File.ReadAllText(usersFilePath);
                    _users = JsonConvert.DeserializeObject<Dictionary<string, string>>(json)
                             ?? new Dictionary<string, string>();
                }
            }
            catch (Exception ex)
            {
                ShowErrorMessage($"Ошибка загрузки пользователей: {ex.Message}");
            }
        }

        private async void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string login = LoginTextBox.Text;
            string password = PasswordBox.Password;

            if (string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(password))
            {
                ShowErrorMessage("Все поля должны быть заполнены!");
                return;
            }

            if (!_users.ContainsKey(login))
            {
                ShowErrorMessage("Пользователь не найден!");
                return;
            }

            if (_users[login] != password)
            {
                ShowErrorMessage("Неверный пароль!");
                return;
            }

            // Успешная авторизация
            await AnimateTransitionToMenuExercises(login);
        }

        private async Task AnimateTransitionToMenuExercises(string username)
        {
            if (_isNavigationInProgress || NavigationService == null)
                return;

            _isNavigationInProgress = true;

            // Анимация исчезновения текущей страницы
            var fadeOut = new DoubleAnimation
            {
                From = 1,
                To = 0,
                Duration = TimeSpan.FromSeconds(0.3),
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseIn }
            };

            this.BeginAnimation(OpacityProperty, fadeOut);
            await Task.Delay(300); // Ждем завершения анимации

            // Переход на целевую страницу
            NavigationService.Navigate(new MenuExercises(username));
            _isNavigationInProgress = false;
        }

        private void ShowErrorMessage(string message)
        {
            InfoLabel.Content = message;
            InfoLabel.Visibility = Visibility.Visible;

            // Автоматическое скрытие сообщения через 1 секунду
            var timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
            timer.Tick += (s, args) =>
            {
                InfoLabel.Visibility = Visibility.Collapsed;
                timer.Stop();
            };
            timer.Start();
        }

        // Обработчики для плейсхолдеров
        private void LoginTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            LoginPlaceholder.Visibility = Visibility.Collapsed;
        }

        private void LoginTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(LoginTextBox.Text))
                LoginPlaceholder.Visibility = Visibility.Visible;
        }

        private void PasswordBox_GotFocus(object sender, RoutedEventArgs e)
        {
            PasswordPlaceholder.Visibility = Visibility.Collapsed;
        }

        private void PasswordBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(PasswordBox.Password))
                PasswordPlaceholder.Visibility = Visibility.Visible;
        }

        private void BackText_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (_isNavigationInProgress || NavigationService == null)
                return;

            _isNavigationInProgress = true;

            // Анимация исчезновения текущей страницы
            var exitStoryboard = new Storyboard();

            var slideOutAnimation = new DoubleAnimation
            {
                From = 0,
                To = this.ActualWidth,
                Duration = TimeSpan.FromSeconds(0.3),
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseIn }
            };
            Storyboard.SetTargetProperty(slideOutAnimation, new PropertyPath("RenderTransform.X"));

            var fadeOutAnimation = new DoubleAnimation
            {
                From = 1,
                To = 0,
                Duration = TimeSpan.FromSeconds(0.25)
            };
            Storyboard.SetTargetProperty(fadeOutAnimation, new PropertyPath("Opacity"));

            exitStoryboard.Children.Add(slideOutAnimation);
            exitStoryboard.Children.Add(fadeOutAnimation);

            this.RenderTransform = new TranslateTransform();

            exitStoryboard.Completed += (s, _) =>
            {
                // Создаем страницу авторизации
                var authPage = new Authorization();

                // Настраиваем анимацию появления
                authPage.RenderTransform = new TranslateTransform { X = -authPage.ActualWidth };
                authPage.Opacity = 0;

                // Выполняем навигацию
                NavigationService.Navigate(authPage);

                // Анимация появления новой страницы
                var enterStoryboard = new Storyboard();

                var slideInAnimation = new DoubleAnimation
                {
                    From = -authPage.ActualWidth,
                    To = 0,
                    Duration = TimeSpan.FromSeconds(0.3),
                    EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut }
                };
                Storyboard.SetTargetProperty(slideInAnimation, new PropertyPath("RenderTransform.X"));

                var fadeInAnimation = new DoubleAnimation
                {
                    From = 0,
                    To = 1,
                    Duration = TimeSpan.FromSeconds(0.2)
                };
                Storyboard.SetTargetProperty(fadeInAnimation, new PropertyPath("Opacity"));

                enterStoryboard.Children.Add(slideInAnimation);
                enterStoryboard.Children.Add(fadeInAnimation);
                enterStoryboard.Begin(authPage);

                _isNavigationInProgress = false;
            };

            exitStoryboard.Begin(this);
        }
    }
}