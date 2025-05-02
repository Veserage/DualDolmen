using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace DualDolmen
{
    public partial class Enter : Page
    {
        private bool _isNavigationInProgress = false;

        public Enter()
        {
            InitializeComponent();
            this.Loaded += (s, e) => AnimatePageEntrance();
        }

        private void AnimatePageEntrance()
        {
            if (this.RenderTransform == null)
            {
                this.RenderTransform = new TranslateTransform();
                var storyboard = new Storyboard();

                var slideAnimation = new DoubleAnimation
                {
                    From = this.ActualWidth,
                    To = 0,
                    Duration = TimeSpan.FromSeconds(0.3),
                    EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut }
                };
                Storyboard.SetTargetProperty(slideAnimation, new PropertyPath("RenderTransform.X"));

                var fadeAnimation = new DoubleAnimation
                {
                    From = 0,
                    To = 1,
                    Duration = TimeSpan.FromSeconds(0.25)
                };
                Storyboard.SetTargetProperty(fadeAnimation, new PropertyPath("Opacity"));

                storyboard.Children.Add(slideAnimation);
                storyboard.Children.Add(fadeAnimation);
                storyboard.Begin(this);
            }
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

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            // Проверка введенных данных
            if (string.IsNullOrEmpty(LoginTextBox.Text) || string.IsNullOrEmpty(PasswordBox.Password))
            {
                MessageBox.Show("Пожалуйста, введите логин и пароль");
                return;
            }

            NavigateToMenuExercises();
        }

        private void NavigateToMenuExercises()
        {
            if (_isNavigationInProgress) return;
            _isNavigationInProgress = true;

            // Создаем главную страницу
            var mainPage = new MenuExercises();

            // Анимация исчезновения текущей страницы
            var exitStoryboard = new Storyboard();

            var slideOutAnimation = new DoubleAnimation
            {
                From = 0,
                To = -this.ActualWidth,
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
                NavigationService?.Navigate(mainPage);
                _isNavigationInProgress = false;
            };

            exitStoryboard.Begin(this);
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