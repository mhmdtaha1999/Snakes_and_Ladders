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
using System.Windows.Shapes;
using System.Windows.Threading;

namespace WPFTest
{
    /// <summary>
    /// Interaction logic for DiceForm.xaml
    /// </summary>
    public partial class DiceForm : Window
    {

        public Gameplay gamePlay;
        private int DiceValue = 0;
        private int Counter = 0;

        DispatcherTimer FadeOutTimer, dispatcherTimer, FadeTimer;

        public DiceForm()
        {
            InitializeComponent();

            dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Tick += dispatcherTimer_Tick;
            dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, 100);            

            FadeTimer = new DispatcherTimer();
            FadeTimer.Tick += FadeTimer_Tick;
            FadeTimer.Interval = new TimeSpan(0, 0, 0, 0, 10);            

            FadeOutTimer = new DispatcherTimer();
            FadeOutTimer.Tick += FadeOut_Tick;
            FadeOutTimer.Interval = new TimeSpan(0, 0, 0, 0, 10);            

        }

        private void FadeTimer_Tick(object sender, EventArgs e)
        {
            if (this.Opacity < 0.85)
                this.Opacity += 0.1;
            else
                FadeTimer.Stop();                   
        }

        private void FadeOut_Tick(object sender, EventArgs e)
        {
            this.Opacity -= 0.1;
            if (Opacity <= 0)
                this.Close();
        }

        private void btnRoll_Click(object sender, RoutedEventArgs e)
        {
            btnRoll.IsEnabled = false;
            dispatcherTimer.Start();
        }

        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            float WaitTime = 15;            
            if (Counter < WaitTime)
            {
                Random rnd = new Random();
                int RandomValue = rnd.Next(1, 7);
                while (RandomValue == Convert.ToInt32(lblDice.Text))
                    RandomValue = rnd.Next(1, 7);
                lblDice.Text = RandomValue.ToString();                
            }

            Counter++;
            if (Counter >= WaitTime)
            {
                DiceValue = Convert.ToInt32(lblDice.Text);
                if (DiceValue == 6)
                {
                    lblDice.Foreground = Brushes.LawnGreen;
                    if (Counter % 2 == 0)
                        lblDice.Visibility = Visibility.Hidden;                        

                    else
                        lblDice.Visibility = Visibility.Visible;

                    if (Counter == WaitTime + 10)
                    {
                        dispatcherTimer.Stop();
                        lblDice.Visibility = Visibility.Visible;
                        gamePlay.Dice = DiceValue;
                        gamePlay.Sync = true;
                        FadeOutTimer.Start();
                    }
                }
                else if (Counter == WaitTime + 5)
                {
                    dispatcherTimer.Stop();
                    gamePlay.Dice = DiceValue;
                    gamePlay.Sync = true;
                    FadeOutTimer.Start();
                }
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            DiceValue = 0;
            FadeTimer.Start();
            if (gamePlay.Turn == 1)
            {
                lblMsg.Text = "Red Player's Turn";
                lblMsg.Foreground = Brushes.Red;
            }
            else
            {
                lblMsg.Text = "Blue Player's Turn";
                lblMsg.Foreground = Brushes.DodgerBlue;
            }
        }
    }
}
