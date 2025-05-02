using System.Windows;
using System.Windows.Controls;

namespace DualDolmen
{
    public partial class MenuExercises : Page
    {
        public MenuExercises()
        {
            InitializeComponent();
        }

        private void FullScreenButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Привет, ёпта!");
        }
    }
}