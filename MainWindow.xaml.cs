﻿using System.Text;
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
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            
            InitializeComponent();
        }

        private void Gleb_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Глеб, када на МАЛАДЁШКУ?", "Увага", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}