using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace DualDolmen
{
    public partial class Reg : Page
    {
        private readonly UsersApp _usersApp;
        private bool _isNavigationInProgress = false;

        public Reg()
        {
            InitializeComponent();

            // Путь к файлу users.json в папке bin
            string appDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string usersFilePath = Path.Combine(appDirectory, "users.json");

            _usersApp = new UsersApp(usersFilePath);
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

        private void RepeatPasswordBox_GotFocus(object sender, RoutedEventArgs e)
        {
            RepeatPasswordPlaceholder.Visibility = Visibility.Collapsed;
        }

        private void RepeatPasswordBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(RepeatPasswordBox.Password))
                RepeatPasswordPlaceholder.Visibility = Visibility.Visible;
        }

        private async void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            string login = LoginTextBox.Text;
            string password = PasswordBox.Password;
            string repeatPassword = RepeatPasswordBox.Password;

            try
            {
                // Проверка на пустые поля
                if (string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(repeatPassword))
                {
                    ShowErrorMessage("Все поля должны быть заполнены!");
                    return;
                }

                // Проверка совпадения паролей
                if (password != repeatPassword)
                {
                    ShowErrorMessage("Пароли не совпадают!");
                    return;
                }

                // Вызов метода регистрации
                _usersApp.RegisterUser(login, password, repeatPassword);

                // Анимация перехода
                await AnimateTransitionToMenuExercises(login);
            }
            catch (ArgumentException ex)
            {
                ShowErrorMessage(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                ShowErrorMessage(ex.Message);
            }
            catch (Exception ex)
            {
                ShowErrorMessage($"Произошла ошибка: {ex.Message}");
            }
        }

        private async Task AnimateTransitionToMenuExercises(string username)
        {
            if (_isNavigationInProgress || NavigationService == null)
                return;

            _isNavigationInProgress = true;

            // Анимация исчезновения текущей страницы
            var exitAnimation = new DoubleAnimation
            {
                From = 1,
                To = 0,
                Duration = TimeSpan.FromSeconds(0.3),
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseIn }
            };

            this.BeginAnimation(OpacityProperty, exitAnimation);

            await Task.Delay(300); // Ждем завершения анимации

            // Переход на страницу MenuExercises с передачей имени пользователя
            NavigationService.Navigate(new MenuExercises(username));

            _isNavigationInProgress = false;
        }


        private void ShowErrorMessage(string message)
        {
            InfoLabel.Content = message;
            InfoLabel.Visibility = Visibility.Visible;
            // Установка стиля ошибки (красный текст)
            InfoLabel.Foreground = new SolidColorBrush(Colors.Red);

            // Автоматическое скрытие сообщения через 1 секунду
            var timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
            timer.Tick += (sender, args) =>
            {
                InfoLabel.Visibility = Visibility.Collapsed;
                timer.Stop();
            };
            timer.Start();
        }
    }
}