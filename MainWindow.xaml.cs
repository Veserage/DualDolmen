using System.Windows;

namespace DualDolmen
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            MainFrame.Navigate(new Authorization());

            //
        }
    }
}