using System;
using System.IO;
using System.Windows;

namespace DualDolmen
{
    public partial class MainWindow : Window
    {
        public static readonly string AppDataPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "DualDolmen");

        public MainWindow()
        {
            InitializeComponent();

            // Создание директории, где будет храниться пользовательский прогресс
            if (!Directory.Exists(AppDataPath))
            {
                Directory.CreateDirectory(AppDataPath);
            }

            MainFrame.Navigate(new Authorization());
        }
    }
}
