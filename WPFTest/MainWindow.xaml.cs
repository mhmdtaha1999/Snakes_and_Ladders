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
using System.Text.RegularExpressions;

namespace WPFTest
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

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");            
            e.Handled = regex.IsMatch(e.Text);
        }

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            Gameplay g = new Gameplay();
            g.SnakesCount = Convert.ToInt32(Snakes.Text);
            g.LaddersCount = Convert.ToInt32(Ladders.Text);
            g.Show();
            this.Hide();
        }
    }
}
