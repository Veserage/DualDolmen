using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace DualDolmen
{
    public partial class Reg : Page
    {
        private bool _isBackNavigationInProgress = false; // Флаг для блокировки множественных нажатий
        public Reg()
        {
            InitializeComponent();
            InitializePlaceholders();
            this.PreviewMouseDown += Page_PreviewMouseDown;
        }

        private void InitializePlaceholders()
        {
            LoginTextBox.Tag = "Введите логин";
            PasswordBox.Tag = "Введите пароль";
            RepeatPasswordBox.Tag = "Повторите пароль";

            UpdateLoginPlaceholder();
            UpdatePasswordPlaceholder();
            UpdateRepeatPasswordPlaceholder();
        }

        private void UpdateLoginPlaceholder()
        {
            LoginPlaceholder.Visibility = string.IsNullOrEmpty(LoginTextBox.Text) ?
                Visibility.Visible : Visibility.Hidden;
        }

        private void UpdatePasswordPlaceholder()
        {
            PasswordPlaceholder.Visibility = string.IsNullOrEmpty(PasswordBox.Password) ?
                Visibility.Visible : Visibility.Hidden;

            // Всегда показывать звездочки, если есть текст
            PasswordBox.PasswordChar = string.IsNullOrEmpty(PasswordBox.Password) ? '\0' : '•';
            PasswordBox.Foreground = Brushes.Black;
        }

        private void UpdateRepeatPasswordPlaceholder()
        {
            RepeatPasswordPlaceholder.Visibility = string.IsNullOrEmpty(RepeatPasswordBox.Password) ?
                Visibility.Visible : Visibility.Hidden;

            RepeatPasswordBox.PasswordChar = string.IsNullOrEmpty(RepeatPasswordBox.Password) ? '\0' : '•';
            RepeatPasswordBox.Foreground = Brushes.Black;
        }

        private void Page_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            // Убираем фокус с текущего элемента, если клик вне полей ввода
            if (!(e.OriginalSource is TextBox) && !(e.OriginalSource is PasswordBox))
            {
                Keyboard.ClearFocus();
            }
        }

        private void LoginTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            LoginPlaceholder.Visibility = Visibility.Hidden;
        }

        private void LoginTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            UpdateLoginPlaceholder();
        }

        private void PasswordBox_GotFocus(object sender, RoutedEventArgs e)
        {
            PasswordPlaceholder.Visibility = Visibility.Hidden;
            PasswordBox.PasswordChar = '•';
            PasswordBox.Foreground = Brushes.Black;
        }

        private void PasswordBox_LostFocus(object sender, RoutedEventArgs e)
        {
            UpdatePasswordPlaceholder();
        }

        private void RepeatPasswordBox_GotFocus(object sender, RoutedEventArgs e)
        {
            RepeatPasswordPlaceholder.Visibility = Visibility.Hidden;
            RepeatPasswordBox.PasswordChar = '•';
            RepeatPasswordBox.Foreground = Brushes.Black;
        }

        private void RepeatPasswordBox_LostFocus(object sender, RoutedEventArgs e)
        {
            UpdateRepeatPasswordPlaceholder();
        }

        private void LoginTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateLoginPlaceholder();
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            UpdatePasswordPlaceholder();
        }

        private void RepeatPasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            UpdateRepeatPasswordPlaceholder();
        }

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                RegisterButton_Click(sender, e);
                e.Handled = true;
            }
        }

        private void PasswordBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                RegisterButton_Click(sender, e);
                e.Handled = true;
            }
        }

        private void BackText_MouseDown(object sender, MouseButtonEventArgs e)
        {
            // Если навигация уже выполняется - игнорируем новое нажатие
            if (_isBackNavigationInProgress || NavigationService == null)
                return;

            _isBackNavigationInProgress = true;

            // Создаем новую страницу авторизации
            var authorizationPage = new Authorization();

            // Настраиваем анимацию исчезновения текущей страницы
            var exitStoryboard = new Storyboard();

            // Анимация сдвига вправо
            var slideOutAnimation = new DoubleAnimation
            {
                From = 0,
                To = this.ActualWidth,
                Duration = TimeSpan.FromSeconds(0.3),
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseIn }
            };
            Storyboard.SetTargetProperty(slideOutAnimation, new PropertyPath("RenderTransform.X"));

            // Анимация исчезновения
            var fadeOutAnimation = new DoubleAnimation
            {
                From = 1.0,
                To = 0.0,
                Duration = TimeSpan.FromSeconds(0.25)
            };
            Storyboard.SetTargetProperty(fadeOutAnimation, new PropertyPath("Opacity"));

            exitStoryboard.Children.Add(slideOutAnimation);
            exitStoryboard.Children.Add(fadeOutAnimation);

            // Инициализируем трансформацию
            this.RenderTransform = new TranslateTransform();

            // Настраиваем анимацию появления новой страницы
            authorizationPage.RenderTransform = new TranslateTransform { X = -authorizationPage.ActualWidth };
            authorizationPage.Opacity = 0;

            // Обработчик завершения анимации исчезновения
            exitStoryboard.Completed += (s, args) =>
            {
                // Выполняем навигацию
                NavigationService.Navigate(authorizationPage);

                // Анимация появления новой страницы
                var enterStoryboard = new Storyboard();

                var slideInAnimation = new DoubleAnimation
                {
                    From = -authorizationPage.ActualWidth,
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
                enterStoryboard.Begin(authorizationPage);

                _isBackNavigationInProgress = false; // Сбрасываем флаг после завершения
            };

            // Запускаем анимацию исчезновения
            exitStoryboard.Begin(this);
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(LoginTextBox.Text) ||
                string.IsNullOrWhiteSpace(PasswordBox.Password) ||
                string.IsNullOrWhiteSpace(RepeatPasswordBox.Password))
            {
                MessageBox.Show("Заполните все поля!");
                return;
            }

            if (PasswordBox.Password != RepeatPasswordBox.Password)
            {
                MessageBox.Show("Пароли не совпадают!");
                return;
            }

            MessageBox.Show("Регистрация успешна!");
        }
    }
}