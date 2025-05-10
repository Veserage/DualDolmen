using System.IO;
using System.Windows;

namespace DualDolmen
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            // Создание папки с прогрессами пользователей
            if (!Directory.Exists("UsersData")) { Directory.CreateDirectory("UsersData"); }

            MainFrame.Navigate(new Authorization());

            
        }
    }
}