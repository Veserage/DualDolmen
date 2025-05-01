using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace DualDolmen
{
    public partial class Reg : Page
    {
        public Reg()
        {
            InitializeComponent();
            LoadImage();
            InitializePlaceholders();
            this.PreviewMouseDown += Page_PreviewMouseDown;
        }

        private void LoadImage()
        {
            var uri = new Uri("/Images/books.png", UriKind.Relative);
            RightImage.Source = new BitmapImage(uri);
        }

        private void InitializePlaceholders()
        {
            // Устанавливаем Tag для всех полей
            LoginTextBox.Tag = "Введите логин";
            PasswordBox.Tag = "Введите пароль";
            RepeatPasswordBox.Tag = "Повторите пароль";

            // Инициализируем плейсхолдеры
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
            if (string.IsNullOrEmpty(PasswordBox.Password))
            {
                PasswordPlaceholder.Visibility = Visibility.Visible;
                PasswordBox.PasswordChar = '\0'; // Показываем текст плейсхолдера
                PasswordBox.Foreground = Brushes.Gray;
            }
            else
            {
                PasswordPlaceholder.Visibility = Visibility.Hidden;
                PasswordBox.PasswordChar = '•'; // Включаем символы пароля
                PasswordBox.Foreground = Brushes.Black;
            }
        }

        private void UpdateRepeatPasswordPlaceholder()
        {
            if (string.IsNullOrEmpty(RepeatPasswordBox.Password))
            {
                RepeatPasswordPlaceholder.Visibility = Visibility.Visible;
                RepeatPasswordBox.PasswordChar = '\0';
                RepeatPasswordBox.Foreground = Brushes.Gray;
            }
            else
            {
                RepeatPasswordPlaceholder.Visibility = Visibility.Hidden;
                RepeatPasswordBox.PasswordChar = '•';
                RepeatPasswordBox.Foreground = Brushes.Black;
            }
        }

        private void Page_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            // Проверяем, было ли нажатие вне элементов управления
            if (!(e.OriginalSource is TextBox) && !(e.OriginalSource is PasswordBox))
            {
                // Восстанавливаем плейсхолдеры, если поля пустые
                if (string.IsNullOrWhiteSpace(LoginTextBox.Text))
                {
                    UpdateLoginPlaceholder();
                }

                if (string.IsNullOrWhiteSpace(PasswordBox.Password))
                {
                    UpdatePasswordPlaceholder();
                }

                if (string.IsNullOrWhiteSpace(RepeatPasswordBox.Password))
                {
                    UpdateRepeatPasswordPlaceholder();
                }
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
            PasswordBox.PasswordChar = '•'; // Включаем символы пароля при фокусе
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
            NavigationService?.GoBack();
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            // Проверка заполнения полей (исключаем плейсхолдеры)
            bool isLoginEmpty = string.IsNullOrWhiteSpace(LoginTextBox.Text) || LoginTextBox.Text == (string)LoginTextBox.Tag;
            bool isPasswordEmpty = string.IsNullOrWhiteSpace(PasswordBox.Password) || PasswordBox.Password == (string)PasswordBox.Tag;
            bool isRepeatPasswordEmpty = string.IsNullOrWhiteSpace(RepeatPasswordBox.Password) || RepeatPasswordBox.Password == (string)RepeatPasswordBox.Tag;

            if (isLoginEmpty || isPasswordEmpty || isRepeatPasswordEmpty)
            {
                MessageBox.Show("Заполните все поля!");
                return;
            }

            if (PasswordBox.Password != RepeatPasswordBox.Password)
            {
                MessageBox.Show("Пароли не совпадают!");
                return;
            }

            // Логика регистрации
            MessageBox.Show("Регистрация успешна!");
        }
    }
}