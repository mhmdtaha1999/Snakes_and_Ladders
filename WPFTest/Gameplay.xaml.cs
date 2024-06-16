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
    /// Interaction logic for Gameplay.xaml
    /// </summary>
    public partial class Gameplay : Window
    {
        public int SnakesCount = 0;
        public int LaddersCount =0;
        public int Dice = 0;
        public int Turn = 1;
        public bool Sync = false;
        private bool PresentRoll = false;
        private bool MovePawn = false;
        private Point TargetLocation;
        private bool ShowTMsg = false;
        private bool IsGameFinished = false;
        private double TMsgOp = 0;

        private int[] SnakeHouses = new int[30] { -10, -10, -10, -10, -10, -10, -10, -10, -10, -10, -10, -10, -10, -10, -10, -10, -10, -10, -10, -10, -10, -10, -10, -10, -10, -10, -10, -10, -10, -10 };
        private int[] LadderHouses = new int[30] { -10, -10, -10, -10, -10, -10, -10, -10, -10, -10, -10, -10, -10, -10, -10, -10, -10, -10, -10, -10, -10, -10, -10, -10, -10, -10, -10, -10, -10, -10 };

        private int[,] Snakes = new int[15, 4] { { 300, 300, -4, 3 }, { 300, 260, -4, 3}, { 440, 470, -6, 6 }, { 300, 190, -4, 2 }, { 250, 250, 3, 3 }
            ,{ 95, 300, -1, 4 }, { 230, 80, -3, 1 }, { 100, 100, -1, 1 }, { 180, 150, -2, 2 }, { 180,150, -2,2 }, { 170, 190, 2, 2 }, { 250, 140, 3, 2 }
        ,{ 180, 360, 2, 5 }, { 310, 210, 4, 3 }, { 210, 160, 3, 2 } };

        private int[,] Ladders = new int[5, 4] { { 100, 200, -1, 3 }, { 120, 240, 1, 3 }, { 50, 150, 0, 2 }, { 350, 150, -2, 2 }, { 350, 150, 2, 2 } };

        private bool IsBlueIn = false;
        private bool IsRedIn = false;

        public Gameplay()
        {
            InitializeComponent();
        }

        private void GameplayWindow_Loaded(object sender, RoutedEventArgs e)
        {

            DiceForm DF = new DiceForm();
            DF.gamePlay = this;
            DF.Show();

            DispatcherTimer TickTimer = new DispatcherTimer();
            TickTimer.Tick += TickTimer_Tick;
            TickTimer.Interval = new TimeSpan(0, 0, 0, 0, 10);
            TickTimer.Start();


            // SNAKES

            int[] SpawnedSnakes = new int[15] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

            for (int i = 1; i <= SnakesCount; i++)
            {
                bool IsSnakeValid = false;
                int TheSnake = 0;
                Random rnd = new Random();

                while (!IsSnakeValid)
                {
                    rnd = new Random();
                    TheSnake = rnd.Next(1, 16);
                    int j = 0;
                    for (int k = 0; k < i; k++)
                    {
                        if (SpawnedSnakes[k] == TheSnake)
                            j++;
                    }

                    if (j == 0)
                    {
                        IsSnakeValid = true;
                        SpawnedSnakes[i - 1] = TheSnake;
                    }
                }

                Image SnakeImage = new Image();
                SnakeImage.Width = Snakes[TheSnake - 1, 0];
                SnakeImage.Height = Snakes[TheSnake - 1, 1];
                SnakeImage.VerticalAlignment = VerticalAlignment.Bottom;
                SnakeImage.HorizontalAlignment = HorizontalAlignment.Left;
                Panel.SetZIndex(SnakeImage, 1);
                SnakeImage.Source = new BitmapImage(
        new Uri(@"/WPFTest;component/Images/Snake" + TheSnake + ".png", UriKind.Relative));


                bool IsSpawnValid = false;
                double Row = 0, Col = 0;
                while (!IsSpawnValid)
                {
                    rnd = new Random();
                    int SnakeHouseX = Snakes[TheSnake - 1, 2], SnakeHouseY = Snakes[TheSnake - 1, 3];

                    int RandomHouse = rnd.Next(10, 100);

                    Row = Math.Floor(Convert.ToDouble((RandomHouse - 1) / 10)) + 1;
                    Col = RandomHouse - ((Row - 1) * 10);

                    if (Row % 2 == 0)
                    {
                        SnakeHouseX *= -1;
                    }

                    int Occupied = 0;
                    for (int z = 0; z < i * 2; z++)
                    {
                        double SRow, SCol;
                        SRow = Math.Floor(Convert.ToDouble((SnakeHouses[z] - 1) / 10)) + 1;
                        SCol = SnakeHouses[z] - ((SRow - 1) * 10);

                        if ((SRow == Row && SCol == Col) || (SRow + 1 == Row && SCol == Col) || (SRow - 1 == Row && SCol == Col) || (SRow == Row && SCol + 1 == Col) || (SRow == Row && SCol - 1 == Col))
                            Occupied++;

                    }

                    if (Col + SnakeHouseX <= 10 && Col + SnakeHouseX >= 1 && Row - (SnakeHouseY + 1) >= 1 && Row - (SnakeHouseY + 1) <= 10 && Occupied == 0)
                    {
                        IsSpawnValid = true;
                        SnakeHouses[2 * (i - 1)] = RandomHouse;
                        if ((Row - SnakeHouseY) % 2 == Row % 2)
                        {
                            SnakeHouses[(2 * i) - 1] = Convert.ToInt32(RandomHouse - (10 * (SnakeHouseY)) + SnakeHouseX);
                        }
                        else
                        {
                            double LastDigit = RandomHouse - (Math.Floor(Convert.ToDouble(RandomHouse / 10)) * 10);
                            double MaxRowNum = Math.Ceiling(Convert.ToDouble(RandomHouse - (10 * SnakeHouseY)) / 10) * 10;
                            SnakeHouses[(2 * i) - 1] = Convert.ToInt32(MaxRowNum + 1 - LastDigit - SnakeHouseX);
                        }
                    }

                }
                float SnakeWidth = Snakes[TheSnake - 1, 0];
                float SnakeHeight = Snakes[TheSnake - 1, 1];
                if (Row % 2 == 0)
                {
                    Col = 10 - Col;
                    if (Snakes[TheSnake - 1, 2] > 0)
                        SnakeImage.Margin = new Thickness((Col * 73) + 40, 0, 0, (Row * 73) - (SnakeHeight) - 10);
                    else
                        SnakeImage.Margin = new Thickness((Col * 73) - (SnakeWidth) + 40, 0, 0, (Row * 73) - (SnakeHeight) - 10);
                }
                else
                {
                    if (Snakes[TheSnake - 1, 2] > 0)
                        SnakeImage.Margin = new Thickness((Col * 73) - 40, 0, 0, (Row * 73) - (SnakeHeight) - 10);
                    else
                        SnakeImage.Margin = new Thickness((Col * 73) - (SnakeWidth) - 40, 0, 0, (Row * 73) - (SnakeHeight) - 10);
                }

                MyGrid.Children.Add(SnakeImage);
            }



            // LADDERS

            for (int i = 1; i <= LaddersCount; i++)
            {
                int TheLadder = 0;
                Random rnd = new Random();
                rnd = new Random();
                TheLadder = rnd.Next(1, 6);

                Image LadderImage = new Image();
                LadderImage.Width = Ladders[TheLadder - 1, 0];
                LadderImage.Height = Ladders[TheLadder - 1, 1];
                LadderImage.VerticalAlignment = VerticalAlignment.Bottom;
                LadderImage.HorizontalAlignment = HorizontalAlignment.Left;
                Panel.SetZIndex(LadderImage, 2);
                LadderImage.Source = new BitmapImage(
        new Uri(@"/WPFTest;component/Images/Ladder" + TheLadder + ".png", UriKind.Relative));


                bool IsSpawnValid = false;
                double Row = 0, Col = 0;
                while (!IsSpawnValid)
                {
                    rnd = new Random();
                    int LadderHouseX = Ladders[TheLadder - 1, 2], LadderHouseY = Ladders[TheLadder - 1, 3];

                    int RandomHouse = rnd.Next(2, 80);

                    Row = Math.Floor(Convert.ToDouble((RandomHouse - 1) / 10)) + 1;
                    Col = RandomHouse - ((Row - 1) * 10);

                    if (Row % 2 != 0)
                    {
                        LadderHouseX *= -1;
                    }

                    int Occupied = 0;
                    for (int z = 0; z < i * 2; z++)
                    {
                        double SRow, SCol;
                        SRow = Math.Floor(Convert.ToDouble((LadderHouses[z] - 1) / 10)) + 1;
                        SCol = LadderHouses[z] - ((SRow - 1) * 10);

                        if ((SRow == Row && SCol == Col) || (SRow + 1 == Row && SCol == Col) || (SRow - 1 == Row && SCol == Col) || (SRow == Row && SCol + 1 == Col) || (SRow == Row && SCol - 1 == Col))
                            Occupied++;

                    }

                    for (int z2 = 0; z2 < SnakesCount * 2; z2++)
                    {
                        if (SnakeHouses[z2] != -10)
                        {
                            double SRow, SCol;
                            SRow = Math.Floor(Convert.ToDouble((SnakeHouses[z2] - 1) / 10)) + 1;
                            SCol = SnakeHouses[z2] - ((SRow - 1) * 10);

                            if ((SRow == Row && SCol == Col))
                                Occupied++;
                        }
                    }

                    if (Col + LadderHouseX <= 10 && Col + LadderHouseX >= 1 && Row + (LadderHouseY + 1) >= 1 && Row + (LadderHouseY + 1) <= 10 && Occupied == 0)
                    {
                        IsSpawnValid = true;
                        LadderHouses[2 * (i - 1)] = RandomHouse;
                        if ((Row + LadderHouseY) % 2 == Row % 2)
                        {
                            LadderHouses[(2 * i) - 1] = Convert.ToInt32(RandomHouse + (10 * (LadderHouseY)) + LadderHouseX);
                        }
                        else
                        {
                            double LastDigit = RandomHouse - (Math.Floor(Convert.ToDouble(RandomHouse / 10)) * 10);
                            double MaxRowNum = Math.Ceiling(Convert.ToDouble(RandomHouse + (10 * LadderHouseY)) / 10) * 10;
                            LadderHouses[(2 * i) - 1] = Convert.ToInt32(MaxRowNum + 1 - LastDigit - LadderHouseX);
                        }
                    }

                }
                float LadderWidth = Ladders[TheLadder - 1, 0];
                float LadderHeight = Ladders[TheLadder - 1, 1];

                int LOffset = 0;

                if (Row % 2 == 0)
                {
                    if (Ladders[TheLadder - 1, 2] > 0)
                    {
                        if (TheLadder == 5)
                            LOffset = 10;
                        LadderImage.Margin = new Thickness(680 - ((Col - 1) * 73) - (LadderWidth / 2) + LOffset, 0, 0, ((Row - 1) * 73) + 27);
                    }
                    else
                        LadderImage.Margin = new Thickness(680 - ((Col - 1) * 73), 0, 0, ((Row - 1) * 73) + 27);
                }
                else
                {
                    if (Ladders[TheLadder - 1, 2] > 0)
                    {
                        if (TheLadder == 5)
                            LOffset = 40;
                        LadderImage.Margin = new Thickness(((Col - 1) * 73) - (LadderWidth / 2) + LOffset, 0, 0, ((Row - 1) * 73) + 27);
                    }
                    else
                        LadderImage.Margin = new Thickness(22 + ((Col - 1) * 73), 0, 0, ((Row - 1) * 73) + 27);
                }

                MyGrid.Children.Add(LadderImage);
            }

        }

        //RMV LATER
        private void NewMethod()
        {
            Image SnakeImage = new Image();
            SnakeImage.Width = Snakes[8, 0];
            SnakeImage.Height = Snakes[8, 1];
            SnakeImage.VerticalAlignment = VerticalAlignment.Bottom;
            SnakeImage.HorizontalAlignment = HorizontalAlignment.Left;
            Panel.SetZIndex(SnakeImage, 1);
            SnakeImage.Source = new BitmapImage(
    new Uri(@"/WPFTest;component/Images/Snake8.png", UriKind.Relative));


            double Row = 0, Col = 0;
            int SnakeHouseX = Snakes[8, 2], SnakeHouseY = Snakes[8, 3];

            int RandomHouse = 25;

            Row = Math.Floor(Convert.ToDouble((RandomHouse - 1) / 10)) + 1;
            Col = RandomHouse - ((Row - 1) * 10);

            if (Row % 2 == 0)
            {
                SnakeHouseX *= -1;
            }

            SnakeHouses[0] = RandomHouse;
            SnakeHouses[1] = Convert.ToInt32(RandomHouse - (10 * (SnakeHouseY)) + SnakeHouseX);

            float SnakeWidth = Snakes[8, 0];
            float SnakeHeight = Snakes[8, 1];
            if (Row % 2 == 0)
            {
                Col = 10 - Col;
                if (Snakes[8, 2] > 0)
                    SnakeImage.Margin = new Thickness((Col * 73) + 40, 0, 0, (Row * 73) - (SnakeHeight) - 10);
                else
                    SnakeImage.Margin = new Thickness((Col * 73) - (SnakeWidth) + 40, 0, 0, (Row * 73) - (SnakeHeight) - 10);
            }
            else
            {
                if (Snakes[8, 2] > 0)
                    SnakeImage.Margin = new Thickness((Col * 73) - 40, 0, 0, (Row * 73) - (SnakeHeight) - 10);
                else
                    SnakeImage.Margin = new Thickness((Col * 73) - (SnakeWidth) - 40, 0, 0, (Row * 73) - (SnakeHeight) - 10);
            }

            MyGrid.Children.Add(SnakeImage);
        }

        private void GameplayWindow_Closed(object sender, EventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }

        private void TickTimer_Tick(object sender, EventArgs e)
        {
            if (ShowTMsg)
            {
                TMsgOp += 0.2f;
                if (TMsgOp >= 100)
                {
                    ShowTMsg = false;
                }
                else
                {
                    lblTMsg.Opacity = TMsgOp;
                }
            }
            else
            {
                if (TMsgOp > 0)
                {
                    TMsgOp -= 0.5f;
                    lblTMsg.Opacity = TMsgOp;
                }
                else
                {
                    if (lblTMsg.Content.ToString() == "Red Player Won!" || lblTMsg.Content.ToString() == "Blue Player Won!")
                        System.Windows.Application.Current.Shutdown();
                }
            }

            if (IsGameFinished) return;
            double RedRow = Math.Floor((RedPawn.Margin.Bottom - 10) / 70) + 1;
            if (RedRow == 10 && RedPawn.Margin.Left < 30)
            {
                IsGameFinished = true;
                lblTMsg.Content = "Red Player Won!";
                ShowTMsg = true;
            }
            double BlueRow = Math.Floor((BluePawn.Margin.Bottom - 10) / 70) + 1;
            if (BlueRow == 10 && BluePawn.Margin.Left < 30)
            {
                IsGameFinished = true;
                lblTMsg.Content = "Blue Player Won!";
                ShowTMsg = true;
            }

            if (Sync)
            {
                if (Dice == 6)
                    PresentRoll = true;
                else
                    PresentRoll = false;

                Sync = false;
                if (Turn == 1)
                {
                    Panel.SetZIndex(RedPawn, 10);
                    Panel.SetZIndex(BluePawn, 9);

                    if (IsRedIn)
                    {
                        int UpMove = 0;
                        double Row = Math.Floor((RedPawn.Margin.Bottom - 10) / 70) + 1;

                        if ((Dice * 73) + RedPawn.Margin.Left > 700 && Row % 2 != 0 && Row != 10)
                        {
                            UpMove = 1;
                            Dice = Convert.ToInt32(Math.Floor(Convert.ToDouble(((Dice * 73) + RedPawn.Margin.Left - 700) / 73)));
                        }
                        else if (RedPawn.Margin.Left - (Dice * 73) < 5 && Row % 2 == 0 && Row != 10)
                        {

                            UpMove = 1;
                            Dice = Convert.ToInt32(Math.Floor(Convert.ToDouble(Math.Abs(RedPawn.Margin.Left - (Dice * 73)) / 73)));
                        }


                        if (Row % 2 == 0)
                        {
                            if (UpMove != 0)
                                TargetLocation = new Point(22 + (Dice * 73), RedPawn.Margin.Bottom + (UpMove * 73));
                            else
                                TargetLocation = new Point(RedPawn.Margin.Left - (Dice * 73), RedPawn.Margin.Bottom);
                        }
                        else
                        {
                            if (UpMove != 0)
                                TargetLocation = new Point(680 - (Dice * 73), RedPawn.Margin.Bottom + (UpMove * 73));
                            else
                                TargetLocation = new Point((Dice * 73) + RedPawn.Margin.Left, RedPawn.Margin.Bottom);
                        }

                        MovePawn = true;
                    }
                    else
                    {
                        if (Dice == 6)
                        {
                            IsRedIn = true;
                            RedPawn.Visibility = Visibility.Visible;
                            Dice = 0;
                            DiceForm DF = new DiceForm();
                            DF.gamePlay = this;
                            DF.Show();
                        }
                        else
                        {
                            Turn = 2;
                            Dice = 0;
                            DiceForm DF = new DiceForm();
                            DF.gamePlay = this;
                            DF.Show();
                        }
                    }
                }
                else
                {
                    Panel.SetZIndex(RedPawn, 9);
                    Panel.SetZIndex(BluePawn, 10);

                    if (IsBlueIn)
                    {
                        int UpMove = 0;
                        double Row = Math.Floor((BluePawn.Margin.Bottom - 10) / 70) + 1;

                        if ((Dice * 73) + BluePawn.Margin.Left > 700 && Row % 2 != 0 && Row != 10)
                        {
                            UpMove = 1;
                            Dice = Convert.ToInt32(Math.Floor(Convert.ToDouble(((Dice * 73) + BluePawn.Margin.Left - 710) / 73)));
                        }
                        else if (BluePawn.Margin.Left - (Dice * 73) < 5 && Row % 2 == 0 && Row != 10)
                        {
                            UpMove = 1;
                            Dice = Convert.ToInt32(Math.Floor(Convert.ToDouble(Math.Abs(BluePawn.Margin.Left - (Dice * 73)) / 73)));
                        }


                        if (Row % 2 == 0)
                        {
                            if (UpMove != 0)
                                TargetLocation = new Point(22 + (Dice * 73), BluePawn.Margin.Bottom + (UpMove * 73));
                            else
                                TargetLocation = new Point(BluePawn.Margin.Left - (Dice * 73), BluePawn.Margin.Bottom);
                        }
                        else
                        {
                            if (UpMove != 0)
                                TargetLocation = new Point(680 - (Dice * 73), BluePawn.Margin.Bottom + (UpMove * 73));
                            else
                                TargetLocation = new Point((Dice * 73) + BluePawn.Margin.Left, BluePawn.Margin.Bottom);
                        }

                        MovePawn = true;
                    }
                    else
                    {
                        if (Dice == 6)
                        {
                            IsBlueIn = true;
                            BluePawn.Visibility = Visibility.Visible;
                            Dice = 0;
                            DiceForm DF = new DiceForm();
                            DF.gamePlay = this;
                            DF.Show();
                        }
                        else
                        {
                            Turn = 1;
                            Dice = 0;
                            DiceForm DF = new DiceForm();
                            DF.gamePlay = this;
                            DF.Show();
                        }
                    }
                }
            }

            if (MovePawn)
            {
                if (Turn == 1)
                {
                    if (RedPawn.Margin.Left != TargetLocation.X || RedPawn.Margin.Bottom != TargetLocation.Y)
                    {
                        if (RedPawn.Margin.Left < TargetLocation.X)
                            RedPawn.Margin = new Thickness(RedPawn.Margin.Left + 1, RedPawn.Margin.Top, RedPawn.Margin.Right, RedPawn.Margin.Bottom);
                        else if (RedPawn.Margin.Left > TargetLocation.X)
                            RedPawn.Margin = new Thickness(RedPawn.Margin.Left - 1, RedPawn.Margin.Top, RedPawn.Margin.Right, RedPawn.Margin.Bottom);

                        if (RedPawn.Margin.Bottom < TargetLocation.Y)
                            RedPawn.Margin = new Thickness(RedPawn.Margin.Left, RedPawn.Margin.Top, RedPawn.Margin.Right, RedPawn.Margin.Bottom + 1);
                        else if (RedPawn.Margin.Bottom > TargetLocation.Y)
                            RedPawn.Margin = new Thickness(RedPawn.Margin.Left, RedPawn.Margin.Top, RedPawn.Margin.Right, RedPawn.Margin.Bottom - 1);
                    }
                    else
                    {
                        MovePawn = false;

                        double Row = Math.Floor((RedPawn.Margin.Bottom - 10) / 70) + 1;
                        double Col = Math.Floor((RedPawn.Margin.Left - 10) / 70) + 1;
                        if (Row % 2 == 0)
                            Col = 11 - Col;

                        for (int z = 0; z < SnakesCount * 2; z += 2)
                        {
                            if (SnakeHouses[z] != -10)
                            {
                                double SRow, SCol;
                                SRow = Math.Floor(Convert.ToDouble((SnakeHouses[z] - 1) / 10)) + 1;
                                SCol = SnakeHouses[z] - ((SRow - 1) * 10);

                                if (Row == SRow && Col == SCol)
                                {
                                    SRow = Math.Floor(Convert.ToDouble((SnakeHouses[z + 1] - 1) / 10)) + 1;
                                    SCol = SnakeHouses[z + 1] - ((SRow - 1) * 10);

                                    if (SRow % 2 == 0)
                                        TargetLocation = new Point(680 - ((SCol - 1) * 73), 14 + ((SRow - 1) * 73));
                                    else
                                        TargetLocation = new Point(22 + ((SCol - 1) * 73), 14 + ((SRow - 1) * 73));

                                    MovePawn = true;
                                }
                            }
                        }


                        for (int z = 0; z < LaddersCount * 2; z += 2)
                        {
                            if (LadderHouses[z] != -10)
                            {
                                double SRow, SCol;
                                SRow = Math.Floor(Convert.ToDouble((LadderHouses[z] - 1) / 10)) + 1;
                                SCol = LadderHouses[z] - ((SRow - 1) * 10);

                                if (Row == SRow && Col == SCol)
                                {
                                    SRow = Math.Floor(Convert.ToDouble((LadderHouses[z + 1] - 1) / 10)) + 1;
                                    SCol = LadderHouses[z + 1] - ((SRow - 1) * 10);

                                    if (SRow % 2 == 0)
                                        TargetLocation = new Point(680 - ((SCol - 1) * 73), 14 + ((SRow - 1) * 73));
                                    else
                                        TargetLocation = new Point(22 + ((SCol - 1) * 73), 14 + ((SRow - 1) * 73));

                                    MovePawn = true;
                                }
                            }
                        }

                        if (!MovePawn)
                        {
                            if (RedPawn.Margin == BluePawn.Margin)
                            {
                                if (Turn == 1)
                                {
                                    lblTMsg.Content = "Blue Player Terminated";
                                    BluePawn.Visibility = Visibility.Hidden;
                                    BluePawn.Margin = new Thickness(22, 0, 0, 14);
                                    IsBlueIn = false;
                                }
                                else
                                {
                                    lblTMsg.Content = "Red Player Terminated";
                                    RedPawn.Visibility = Visibility.Hidden;
                                    RedPawn.Margin = new Thickness(22, 0, 0, 14);
                                    IsRedIn = false;
                                }
                                ShowTMsg = true;
                            }

                            if (!PresentRoll)
                                Turn = 2;
                            Dice = 0;
                            DiceForm DF = new DiceForm();
                            DF.gamePlay = this;
                            DF.Show();
                        }
                    }
                }
                else
                {
                    if (BluePawn.Margin.Left != TargetLocation.X || BluePawn.Margin.Bottom != TargetLocation.Y)
                    {
                        if (BluePawn.Margin.Left < TargetLocation.X)
                            BluePawn.Margin = new Thickness(BluePawn.Margin.Left + 1, BluePawn.Margin.Top, BluePawn.Margin.Right, BluePawn.Margin.Bottom);
                        else if (BluePawn.Margin.Left > TargetLocation.X)
                            BluePawn.Margin = new Thickness(BluePawn.Margin.Left - 1, BluePawn.Margin.Top, BluePawn.Margin.Right, BluePawn.Margin.Bottom);

                        if (BluePawn.Margin.Bottom < TargetLocation.Y)
                            BluePawn.Margin = new Thickness(BluePawn.Margin.Left, BluePawn.Margin.Top, BluePawn.Margin.Right, BluePawn.Margin.Bottom + 1);
                        else if (BluePawn.Margin.Bottom > TargetLocation.Y)
                            BluePawn.Margin = new Thickness(BluePawn.Margin.Left, BluePawn.Margin.Top, BluePawn.Margin.Right, BluePawn.Margin.Bottom - 1);
                    }
                    else
                    {

                        MovePawn = false;

                        double Row = Math.Floor((BluePawn.Margin.Bottom - 10) / 70) + 1;
                        double Col = Math.Floor((BluePawn.Margin.Left - 10) / 70) + 1;
                        if (Row % 2 == 0)
                            Col = 11 - Col;

                        for (int z = 0; z < SnakesCount * 2; z += 2)
                        {
                            if (SnakeHouses[z] != -10)
                            {
                                double SRow, SCol;
                                SRow = Math.Floor(Convert.ToDouble((SnakeHouses[z] - 1) / 10)) + 1;
                                SCol = SnakeHouses[z] - ((SRow - 1) * 10);

                                if (Row == SRow && Col == SCol)
                                {
                                    SRow = Math.Floor(Convert.ToDouble((SnakeHouses[z + 1] - 1) / 10)) + 1;
                                    SCol = SnakeHouses[z + 1] - ((SRow - 1) * 10);

                                    if (SRow % 2 == 0)
                                        TargetLocation = new Point(680 - ((SCol - 1) * 73), 14 + ((SRow - 1) * 73));
                                    else
                                        TargetLocation = new Point(22 + ((SCol - 1) * 73), 14 + ((SRow - 1) * 73));

                                    MovePawn = true;
                                }
                            }
                        }

                        for (int z = 0; z < LaddersCount * 2; z += 2)
                        {
                            if (LadderHouses[z] != -10)
                            {
                                double SRow, SCol;
                                SRow = Math.Floor(Convert.ToDouble((LadderHouses[z] - 1) / 10)) + 1;
                                SCol = LadderHouses[z] - ((SRow - 1) * 10);

                                if (Row == SRow && Col == SCol)
                                {
                                    SRow = Math.Floor(Convert.ToDouble((LadderHouses[z + 1] - 1) / 10)) + 1;
                                    SCol = LadderHouses[z + 1] - ((SRow - 1) * 10);

                                    if (SRow % 2 == 0)
                                        TargetLocation = new Point(680 - ((SCol - 1) * 73), 14 + ((SRow - 1) * 73));
                                    else
                                        TargetLocation = new Point(22 + ((SCol - 1) * 73), 14 + ((SRow - 1) * 73));

                                    MovePawn = true;
                                }
                            }
                        }


                        if (!MovePawn)
                        {
                            MovePawn = false;

                            if (RedPawn.Margin == BluePawn.Margin)
                            {
                                if (Turn == 1)
                                {
                                    lblTMsg.Content = "Blue Player Terminated";
                                    BluePawn.Visibility = Visibility.Hidden;
                                    BluePawn.Margin = new Thickness(22, 0, 0, 14);
                                    IsBlueIn = false;
                                }
                                else
                                {
                                    lblTMsg.Content = "Red Player Terminated";
                                    RedPawn.Visibility = Visibility.Hidden;
                                    RedPawn.Margin = new Thickness(22, 0, 0, 14);
                                    IsRedIn = false;
                                }
                                ShowTMsg = true;
                            }

                            if (!PresentRoll)
                                Turn = 1;
                            Dice = 0;
                            DiceForm DF = new DiceForm();
                            DF.gamePlay = this;
                            DF.Show();
                        }
                    }
                }
            }

        }

    }
}
