using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows.Threading;

namespace WpfSnake
{
    class GamePresenter
    {
        MainWindow4 MainWindow = new MainWindow4();
        ScreenMode GameScreenMode = new ScreenMode();
        private Game GAME = new Game();
        private Grid GameGrid = new Grid();
        private Dispatcher Dispatcher = Dispatcher.CurrentDispatcher;
        private delegate void KeyDownEventHandler(object sender, KeyEventArgs e);
        private KeyDownEventHandler New_MainWindow_KeyDown;// = KeyInput_StartMode;
        
        public GamePresenter()
        {


            #region DelegateEvents
            
            this.MainWindow.ScoresDataGrid.ItemsSource = this.GAME.GameScores;
            this.MainWindow.CloseHighScore += new EventHandler(MainWindow_CloseHighScore);
            this.MainWindow.ScoreContainer.CellEditEnding += new EventHandler<DataGridCellEditEndingEventArgs>(ScoreContainer_CellEditEnding);
            this.MainWindow.ScoreContainer.GridLinesVisibility = DataGridGridLinesVisibility.Horizontal;
            this.MainWindow.KeyDown += new KeyEventHandler(MainWindow_KeyDown);

            this.GAME.SnakeDied += new EventHandler(GAME_SnakeDied);
            this.GAME.BOOM += new EventHandler(GAME_BOOM);
            this.GAME.HighScoreAchieved += new EventHandler(GAME_HighScoreAchieved);
            this.GAME.RecordScoreAchieved += new EventHandler(GAME_RecordScoreAchieved);
            this.GAME.MoveCompleted += new EventHandler(GAME_MoveCompleted);
            this.GAME.SnakeGenerated += new Game.SnakeGeneratedEventHandler(GAME_SnakeGenerated);
            //this.
            //this.GAME.SnakeLoaded += new EventHandler(Snake_SnakeLoaded);

            this.GameScreenMode.ModeChanged += new EventHandler(GameScreenMode_ModeChanged);

            #endregion DelegateEvents
            
            #region Initializations

            LoadScores();
            GenerateWorld(Properties.Settings.Default.WorldColumns, Properties.Settings.Default.WorldRows, Properties.Settings.Default.WorldCellSize);
            GameScreenMode.GameScreenModeType = ScreenMode.ScreenModeType.HIGHSCORES;
            MainWindow.Show();

            #endregion Initializations

        }

        void GAME_SnakeGenerated(object sender, Game.SnakeGeneratedEventArgs e)
        {
            this.Dispatcher.Invoke(new Action(delegate { this.MainWindow.CurrentSnakeGUID = e.SnakeGUID; }), DispatcherPriority.Send, null);
            //this.MainWindow.CurrentSnakeGUID = e.SnakeGUID;
            //string x = this.MainWindow.CurrentSnakeGUIDString;
        }

        void GAME_BOOM(object sender, EventArgs e)
        {
            DrawBombs();
        }



        private void StartRestartGame()
        {
            if (GAME.IsPaused)
            {
                GAME.IsPaused = false;
            }
            else
            {
                GAME.Start();
            }
            
        }

        private void GenerateWorld(int Columns, int Rows, int Size)
        {
            GAME.GameWorld = new World(Columns, Rows, Size);

            for (int iColumns = 0; iColumns < Columns; iColumns++)
            {
                this.GameGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(Size) });
            }
            for (int iRows = 0; iRows < Rows; iRows++)
            {
                this.GameGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(Size) });
            }
            //this.MainWindow.GameCanvas.Children.Remove(
            this.MainWindow.TargetGrid.Children.Add(this.GameGrid);
        }

        #region GameModeControl
        /*Notes:
         * Game Modes: HighscoreShow, RecordHighScore, PlayGame, Pause
         */

        void GameScreenMode_ModeChanged(object sender, EventArgs e)
        {
            switch (GameScreenMode.GameScreenModeType)
            {
                case ScreenMode.ScreenModeType.START:
                    ConfigueSTARTMode();
                    break;
                case ScreenMode.ScreenModeType.PLAY:

                    ConfiguePLAYMode();
                    break;
                case ScreenMode.ScreenModeType.HIGHSCORES:
                    ConfigueHIGHSCORESMode();
                    //New_MainWindow_KeyDown = this.KeyInput_ScoreMode;
                    break;
                case ScreenMode.ScreenModeType.RECORDHIGHSCORE:
                    ConfigueRECORDHIGHSCOREMode();
                    //New_MainWindow_KeyDown = this.KeyInput_ScoreMode;
                    break;
            }

        }
        private void ConfigueSTARTMode()
        {
            New_MainWindow_KeyDown = this.KeyInput_StartMode;
            this.Dispatcher.Invoke(new Action(delegate { this.MainWindow.ScoresDataGrid.IsEnabled = false; }), DispatcherPriority.Send, null);
            this.Dispatcher.Invoke(new Action(delegate { this.GameGrid.Visibility = Visibility.Visible; }), DispatcherPriority.Send, null);
            this.Dispatcher.Invoke(new Action(delegate { this.MainWindow.HighScoreScreen.Visibility = Visibility.Hidden; }), DispatcherPriority.Send, null);
            this.Dispatcher.Invoke(new Action(delegate { this.MainWindow.PauseIndicator.Visibility = Visibility.Visible; }), DispatcherPriority.Send, null);
        }
        void MainWindow_CloseHighScore(object sender, EventArgs e)
        {
            GameScreenMode.GameScreenModeType = ScreenMode.ScreenModeType.START;
        }
        void GAME_RecordScoreAchieved(object sender, EventArgs e)
        {
            GameScreenMode.GameScreenModeType = ScreenMode.ScreenModeType.RECORDHIGHSCORE;
        }
        private void ConfigueRECORDHIGHSCOREMode()
        {
            New_MainWindow_KeyDown = this.KeyInput_ScoreMode;
            //this.MainWindow.ScoresDataGrid.IsEnabled = true;
            PersistScore(new Score("Player", this.GAME.Snake.Age, this.GAME.Snake.GUID));

            this.Dispatcher.Invoke(new Action(delegate { this.MainWindow.ScoresDataGrid.IsEnabled = true; }), DispatcherPriority.Send, null);
            this.Dispatcher.Invoke(new Action(delegate { this.MainWindow.ScoresDataGrid.IsReadOnly = true; }), DispatcherPriority.Send, null);
            this.Dispatcher.Invoke(new Action(delegate { this.GameGrid.Visibility = Visibility.Hidden; }), DispatcherPriority.Send, null);
            this.Dispatcher.Invoke(new Action(delegate { this.MainWindow.HighScoreScreen.Visibility = Visibility.Visible; }), DispatcherPriority.Send, null);
            this.Dispatcher.Invoke(new Action(delegate { this.MainWindow.PauseIndicator.Visibility = Visibility.Hidden; }), DispatcherPriority.Send, null);

        }

        private void ConfigueHIGHSCORESMode()
        {
            New_MainWindow_KeyDown = this.KeyInput_StartMode;
            this.Dispatcher.Invoke(new Action(delegate { this.MainWindow.ScoresDataGrid.IsEnabled = true; }), DispatcherPriority.Send, null);
            this.Dispatcher.Invoke(new Action(delegate { this.MainWindow.ScoresDataGrid.IsReadOnly = true; }), DispatcherPriority.Send, null);
            this.Dispatcher.Invoke(new Action(delegate { this.GameGrid.Visibility = Visibility.Hidden; }), DispatcherPriority.Send, null);
            this.Dispatcher.Invoke(new Action(delegate { this.MainWindow.HighScoreScreen.Visibility = Visibility.Visible; }), DispatcherPriority.Send, null);
            this.Dispatcher.Invoke(new Action(delegate { this.MainWindow.PauseIndicator.Visibility = Visibility.Hidden; }), DispatcherPriority.Send, null);
        }

        //private void ConfigueRECORDSCOREMode()
        //{
        //}

        private void ConfiguePLAYMode()
        {
            New_MainWindow_KeyDown = this.KeyInput_PlayMode;
            this.GameGrid.Visibility = Visibility.Visible;
            this.GameGrid.Background = Brushes.White;
            //this.MainWindow.PlayerName.Visibility = Visibility.Hidden;
            this.MainWindow.PauseIndicator.Visibility = Visibility.Hidden;
        }



        #endregion GameModeControl

        #region InputControl

        void KeyInput_StartMode(object sender, KeyEventArgs e)
        {
            GameScreenMode.GameScreenModeType = ScreenMode.ScreenModeType.PLAY;
            StartRestartGame();

        }
        void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            New_MainWindow_KeyDown(sender, e);
        }
        void KeyInput_PlayMode(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Up:
                case Key.Down:
                case Key.Left:
                case Key.Right:
                    GAME.Snake.ChangeDirection(e.Key);
                    break;
                case Key.P:
                    GAME.Snake.ChangeDirection(e.Key);
                    break;
                //default:
                //    GAME.Unpause();
                //break;
            }
        }

        void KeyInput_ScoreMode(object sender, KeyEventArgs e) { }

        #endregion InputControl

        #region GAMEEventHandleing

        void Snake_SnakeLoaded(object sender, EventArgs e)
        {
            this.MainWindow.CurrentSnakeGUID = ((Snake)sender).GUID;
        }

        void GAME_HighScoreAchieved(object sender, EventArgs e)
        {
            //GameScreenMode.GameScreenModeType = ScreenMode.ScreenModeType.HIGHSCORES;
            this.Dispatcher.Invoke(new Action(delegate { MainWindow.HighScoreLabel.Content = this.GAME.Snake.Age.ToString().PadLeft(7, "0".ToCharArray()[0]); }), DispatcherPriority.Send, null);

        }


        void GAME_SnakeDied(object sender, EventArgs e)
        {
            this.Dispatcher.Invoke(new Action(delegate { this.GameGrid.Background = Brushes.Red; }), DispatcherPriority.Send, null);
            if (this.GAME.Snake.Age > this.GAME.GameScores[this.GAME.GameScores.Count - 1].PlayerScore)
            {
                GameScreenMode.GameScreenModeType = ScreenMode.ScreenModeType.RECORDHIGHSCORE;
            }
            else
            {
                GameScreenMode.GameScreenModeType = ScreenMode.ScreenModeType.HIGHSCORES;
            }
            //
            //throw new NotImplementedException();
        }

        void GAME_MoveCompleted(object sender, EventArgs e)
        {
            this.GameGrid.Children.Clear();
            DrawSnake();
            DrawBombs();
            UpdateScore();
            //throw new NotImplementedException();
        }

        #endregion GAMEEventHandleing

        #region ScoreManagement

        void ScoreContainer_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            Scores.SerializeData(this.GAME.GameScores);
            //this.MainWindow.ScoreContainer.
        }
        private void UpdateScore()
        {
            MainWindow.CurrentScore.Content = this.GAME.Snake.Age.ToString().PadLeft(7, "0".ToCharArray()[0]);
        }
        internal void LoadScores()
        {
            this.Dispatcher.Invoke(new Action(delegate { this.MainWindow.ScoresDataGrid.Items.Refresh(); }), DispatcherPriority.Send, null);
            
            MainWindow.HighScoreLabel.Content = (this.GAME.GameScores.Count > 0 ? this.GAME.GameScores[0].PlayerScore : 0).ToString().PadLeft(7, "0".ToCharArray()[0]);
        }
        private void PersistScore(Score score)
        {
            this.GAME.GameScores.Add(score);
            Scores.SerializeData(this.GAME.GameScores);
            this.LoadScores();
        }
        //void ScoresDataGrid_LoadingRow(object sender, DataGridRowEventArgs e)
        //{

        //}

        #endregion ScoreManagement

        #region Drawing

        private void DrawBombs()
        {
            for (int i = 0; i < this.GAME.Bombs.Count; i++)
            {
                Image BombImage = new Image();
                if (!this.GAME.Bombs[i].IsBoomBoomTime(this.GAME.Snake.Age))
                {
                    if (this.GAME.Snake.Age % 3 == 0)
                    {
                        BombImage.Source = new BitmapImage(new Uri(Properties.Settings.Default.BombLeftSparkImageURI, UriKind.Relative));
                        //BombImage.Source = new BitmapImage(new Uri(@".\Resources\BombLeftSpark.png", UriKind.Relative));
                        //BombImage.Source = BitmapSources["BombLeftSpark"];
                        //BombImage.Source = new BitmapImage(new Uri(@"C:\Users\BruteF0rce\Documents\Visual Studio 2010\Projects\WpfSnake\WpfSnake\Resources\BombLeftSpark.png", UriKind.Absolute));
                    }
                    else
                    {
                        BombImage.Source = new BitmapImage(new Uri(Properties.Settings.Default.BombRightSparkImageURI, UriKind.Relative));
                        //BombImage.Source = new BitmapImage(new Uri(@".\Resources\BombRightSpark.png", UriKind.Relative));
                        //BombImage.Source = BitmapSources["BombRightSpark"];

                        //BombImage.Source = new BitmapImage(new Uri(@"C:\Users\BruteF0rce\Documents\Visual Studio 2010\Projects\WpfSnake\WpfSnake\Resources\BombRightSpark.png", UriKind.Absolute));
                    }
                }
                else
                {
                    BombImage.Source = new BitmapImage(new Uri(Properties.Settings.Default.BOOMImageURI, UriKind.Relative));
                    //BombImage.Source = new BitmapImage(new Uri(@".\Resources\BOOM.png", UriKind.Relative));
                    //BombImage.Source = BitmapSources["BOOM"];
                    //BombImage.Source = new BitmapImage(new Uri(@"C:\Users\BruteF0rce\Documents\Visual Studio 2010\Projects\WpfSnake\WpfSnake\Resources\BOOM.png", UriKind.Absolute));
                }

                BombImage.Width = BombImage.Source.Width;
                BombImage.Height = BombImage.Source.Height;
                this.GameGrid.Children.Add(BombImage);

                Grid.SetColumn(BombImage, this.GAME.Bombs[i].Location.Column);
                Grid.SetRow(BombImage, this.GAME.Bombs[i].Location.Row);
            }
        }

        private void DrawSnake()
        {
            Image HeadImage = new Image();
            HeadImage.Source = new BitmapImage(new Uri(Properties.Settings.Default.HeadImageURI, UriKind.Relative));
            HeadImage.Width = HeadImage.Source.Width;
            HeadImage.Height = HeadImage.Source.Height;
            GameGrid.Children.Add(HeadImage);
            switch (GAME.Snake.Vector.Direction)
            {
                case Key.Up:
                    HeadImage.LayoutTransform = new RotateTransform(0);
                    break;

                case Key.Right:
                    HeadImage.LayoutTransform = new RotateTransform(90);
                    break;

                case Key.Down:
                    HeadImage.LayoutTransform = new RotateTransform(180);
                    break;

                case Key.Left:
                    HeadImage.LayoutTransform = new RotateTransform(270);
                    break;

            }
            Grid.SetColumn(HeadImage, GAME.Snake.Head.Column);
            Grid.SetRow(HeadImage, GAME.Snake.Head.Row);

            for (int iBody = 0; iBody < GAME.Snake.Body.Count; iBody++)
            {
                Image BodyImage = new Image();
                //BodyImage.Source = new BitmapImage(new Uri(@"C:\Users\BruteF0rce\Documents\Visual Studio 2010\Projects\WpfSnake\WpfSnake\Resources\Body.png", UriKind.Absolute));
                //BodyImage.Source = BitmapSources["Body"];
                BodyImage.Source = new BitmapImage(new Uri(Properties.Settings.Default.BodyImageURI, UriKind.Relative));
                BodyImage.Width = BodyImage.Source.Width;
                BodyImage.Height = BodyImage.Source.Height;
                GameGrid.Children.Add(BodyImage);
                Grid.SetColumn(BodyImage, GAME.Snake.Body[iBody].Column);
                Grid.SetRow(BodyImage, GAME.Snake.Body[iBody].Row);

            }
            Image TailImage = new Image();
            if (GAME.Snake.Age % 2 == 0)
            {
                //TailImage.Source = new BitmapImage(new Uri(@"C:\Users\BruteF0rce\Documents\Visual Studio 2010\Projects\WpfSnake\WpfSnake\Resources\TailZig.png", UriKind.Absolute));
                //TailImage.Source = BitmapSources["TailZig"];
                TailImage.Source = new BitmapImage(new Uri(Properties.Settings.Default.TailZigImageURI, UriKind.Relative));
            }
            else
            {
                //TailImage.Source = BitmapSources["TailZag"];
                //TailImage.Source = new BitmapImage(new Uri(@"C:\Users\BruteF0rce\Documents\Visual Studio 2010\Projects\WpfSnake\WpfSnake\Resources\TailZag.png", UriKind.Absolute));
                TailImage.Source = new BitmapImage(new Uri(Properties.Settings.Default.TailZagImageURI, UriKind.Relative));
            }


            TailImage.Width = TailImage.Source.Width;
            TailImage.Height = TailImage.Source.Height;
            GameGrid.Children.Add(TailImage);
            Grid.SetColumn(TailImage, GAME.Snake.Tail.Column);
            Grid.SetRow(TailImage, GAME.Snake.Tail.Row);

            Key lastBodyPartVector = new Key();

            if (GAME.Snake.Body[GAME.Snake.Body.Count - 1].Column > GAME.Snake.Tail.Column)
            {
                lastBodyPartVector = Key.Right;
            }

            if (GAME.Snake.Body[GAME.Snake.Body.Count - 1].Row > GAME.Snake.Tail.Row)
            {
                lastBodyPartVector = Key.Down;
            }

            if (GAME.Snake.Body[GAME.Snake.Body.Count - 1].Row < GAME.Snake.Tail.Row)
            {
                lastBodyPartVector = Key.Up;
            }

            if (GAME.Snake.Body[GAME.Snake.Body.Count - 1].Column < GAME.Snake.Tail.Column)
            {
                lastBodyPartVector = Key.Left;
            }

            {//Transform Tail
                switch (lastBodyPartVector)
                {
                    case Key.Up:
                        TailImage.LayoutTransform = new RotateTransform(0);
                        break;

                    case Key.Right:
                        TailImage.LayoutTransform = new RotateTransform(90);
                        break;

                    case Key.Down:
                        TailImage.LayoutTransform = new RotateTransform(180);
                        break;

                    case Key.Left:
                        TailImage.LayoutTransform = new RotateTransform(270);
                        break;

                }

            }

        }

        #endregion Drawing



       

       

        //internal void PlayerInput(Key KeyInput)
        //{
        //    switch (KeyInput)
        //    {
        //        case Key.Up:
        //        case Key.Down:
        //        case Key.Left:
        //        case Key.Right:
        //            Game.Snake.ChangeDirection(KeyInput);
        //            break;
        //        case Key.P:
        //            Game.Snake.ChangeDirection(KeyInput);
        //            break;
        //        default:
        //            Game.Unpause();
        //            break;
        //    }
        //}
    }
}
