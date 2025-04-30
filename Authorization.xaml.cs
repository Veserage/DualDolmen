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
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DualDolmen
{
    public partial class Authorization : Page
    {
        public Authorization()
        {
            InitializeComponent();
            LoadImage();
        }

        private void LoadImage()
        {
            var uri = new Uri("/Images/London.jpg", UriKind.Relative);
            MainImage.Source = new BitmapImage(uri);
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Завтра продолжим!");
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("А пока спать!");
        }
    }
}
