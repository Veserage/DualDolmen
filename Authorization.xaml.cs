using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DualDolmen
{
    public partial class Authorization : Page
    {
        private bool _isNavigationInProgress = false; // Флаг для блокировки множественных нажатий
        public Authorization()
        {
            InitializeComponent();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            if (_isNavigationInProgress) return;
            _isNavigationInProgress = true;

            // Создаем новую страницу входа
            Enter enterPage = new Enter();

            // Получаем NavigationService
            var navigationService = NavigationService.GetNavigationService(this) ?? this.NavigationService;
            if (navigationService == null)
            {
                _isNavigationInProgress = false;
                return;
            }

            // Настраиваем анимацию исчезновения текущей страницы
            var fadeOutStoryboard = new Storyboard();

            // Анимация прозрачности
            var fadeOutAnimation = new DoubleAnimation
            {
                From = 1.0,
                To = 0.0,
                Duration = TimeSpan.FromSeconds(0.3),
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseIn }
            };
            Storyboard.SetTargetProperty(fadeOutAnimation, new PropertyPath(OpacityProperty));
            fadeOutStoryboard.Children.Add(fadeOutAnimation);

            // Анимация сдвига влево
            var slideOutAnimation = new DoubleAnimation
            {
                From = 0,
                To = -this.ActualWidth * 0.3,
                Duration = TimeSpan.FromSeconds(0.3),
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseIn }
            };
            this.RenderTransform = new TranslateTransform();
            Storyboard.SetTargetProperty(slideOutAnimation, new PropertyPath("RenderTransform.X"));
            fadeOutStoryboard.Children.Add(slideOutAnimation);

            // Обработчик завершения анимации
            fadeOutStoryboard.Completed += (s, args) =>
            {
                // Настраиваем анимацию появления новой страницы
                enterPage.Opacity = 0;
                enterPage.RenderTransform = new TranslateTransform { X = this.ActualWidth * 0.3 };

                // Выполняем навигацию
                navigationService.Navigate(enterPage);

                // Анимация появления новой страницы
                var fadeInStoryboard = new Storyboard();

                var fadeInAnimation = new DoubleAnimation
                {
                    From = 0.0,
                    To = 1.0,
                    Duration = TimeSpan.FromSeconds(0.3),
                    EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut }
                };
                Storyboard.SetTargetProperty(fadeInAnimation, new PropertyPath(OpacityProperty));

                var slideInAnimation = new DoubleAnimation
                {
                    From = this.ActualWidth * 0.3,
                    To = 0,
                    Duration = TimeSpan.FromSeconds(0.3),
                    EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut }
                };
                Storyboard.SetTargetProperty(slideInAnimation, new PropertyPath("RenderTransform.X"));

                fadeInStoryboard.Children.Add(fadeInAnimation);
                fadeInStoryboard.Children.Add(slideInAnimation);
                fadeInStoryboard.Begin(enterPage);

                _isNavigationInProgress = false;
            };

            // Запускаем анимацию исчезновения
            fadeOutStoryboard.Begin(this);
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            if (_isNavigationInProgress) return;
            _isNavigationInProgress = true;

            // Создаем новую страницу регистрации
            Reg registrationPage = new Reg();

            // Получаем NavigationService
            var navigationService = NavigationService.GetNavigationService(this) ?? this.NavigationService;
            if (navigationService == null)
            {
                _isNavigationInProgress = false;
                return;
            }

            // Настраиваем анимацию исчезновения текущей страницы
            var fadeOutStoryboard = new Storyboard();

            // Анимация прозрачности
            var fadeOutAnimation = new DoubleAnimation
            {
                From = 1.0,
                To = 0.0,
                Duration = TimeSpan.FromSeconds(0.3),
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseIn }
            };
            Storyboard.SetTargetProperty(fadeOutAnimation, new PropertyPath(OpacityProperty));
            fadeOutStoryboard.Children.Add(fadeOutAnimation);

            // Анимация сдвига влево (опционально)
            var slideOutAnimation = new DoubleAnimation
            {
                From = 0,
                To = -this.ActualWidth * 0.3,
                Duration = TimeSpan.FromSeconds(0.3),
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseIn }
            };
            this.RenderTransform = new TranslateTransform();
            Storyboard.SetTargetProperty(slideOutAnimation, new PropertyPath("RenderTransform.X"));
            fadeOutStoryboard.Children.Add(slideOutAnimation);

            // Обработчик завершения анимации
            fadeOutStoryboard.Completed += (s, args) =>
            {
                // Настраиваем анимацию появления новой страницы
                registrationPage.Opacity = 0;
                registrationPage.RenderTransform = new TranslateTransform { X = this.ActualWidth * 0.3 };

                // Выполняем навигацию
                navigationService.Navigate(registrationPage);

                // Анимация появления новой страницы
                var fadeInStoryboard = new Storyboard();

                var fadeInAnimation = new DoubleAnimation
                {
                    From = 0.0,
                    To = 1.0,
                    Duration = TimeSpan.FromSeconds(0.3),
                    EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut }
                };
                Storyboard.SetTargetProperty(fadeInAnimation, new PropertyPath(OpacityProperty));

                var slideInAnimation = new DoubleAnimation
                {
                    From = this.ActualWidth * 0.3,
                    To = 0,
                    Duration = TimeSpan.FromSeconds(0.3),
                    EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut }
                };
                Storyboard.SetTargetProperty(slideInAnimation, new PropertyPath("RenderTransform.X"));

                fadeInStoryboard.Children.Add(fadeInAnimation);
                fadeInStoryboard.Children.Add(slideInAnimation);
                fadeInStoryboard.Begin(registrationPage);

                _isNavigationInProgress = false;
            };

            // Запускаем анимацию исчезновения
            fadeOutStoryboard.Begin(this);
        }
    }
}