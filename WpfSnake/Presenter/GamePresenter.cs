using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using WpfSnake.Business;

namespace WpfSnake.Presenter
{
    class GamePresenter
    {
        readonly MainWindow4 _mainWindow = new MainWindow4();
        readonly ScreenMode _gameScreenMode = new ScreenMode();
        private readonly Game _game = new Game();
        private readonly Grid _gameGrid = new Grid();
        private readonly Dispatcher _dispatcher = Dispatcher.CurrentDispatcher;
        private delegate void KeyDownEventHandler(object sender, KeyEventArgs e);
        private KeyDownEventHandler _newMainWindowKeyDown;
        
        public GamePresenter()
        {
            #region DelegateEvents
            
            this._mainWindow.ScoresDataGrid.ItemsSource = this._game.GameScores;
            this._mainWindow.CloseHighScore += this.MainWindow_CloseHighScore;
            this._mainWindow.ScoreContainer.CellEditEnding += this.ScoreContainer_CellEditEnding;
            this._mainWindow.ScoreContainer.GridLinesVisibility = DataGridGridLinesVisibility.Horizontal;
            this._mainWindow.KeyDown += this.MainWindow_KeyDown;
            this._game.SnakeDied += this.GAME_SnakeDied;
            this._game.Boom += this.GAME_BOOM;
            this._game.HighScoreAchieved += this.GAME_HighScoreAchieved;
            this._game.RecordScoreAchieved += this.GAME_RecordScoreAchieved;
            this._game.MoveCompleted += this.GAME_MoveCompleted;
            this._game.SnakeGenerated += this.GAME_SnakeGenerated;
            this._gameScreenMode.ModeChanged += this.GameScreenMode_ModeChanged;

            #endregion DelegateEvents
            
            #region Initializations

            this.LoadScores();
            this.GenerateWorld(Properties.Settings.Default.WorldColumns, Properties.Settings.Default.WorldRows, Properties.Settings.Default.WorldCellSize);
            this._gameScreenMode.GameScreenModeType = ScreenMode.ScreenModeType.Highscores;
            this._mainWindow.Show();

            #endregion Initializations
        }

        void GAME_SnakeGenerated(object sender, Game.SnakeGeneratedEventArgs e)
        {
            this._dispatcher.Invoke(new Action(delegate { this._mainWindow.CurrentSnakeGuid = e.SnakeGuid; }), DispatcherPriority.Send, null);
        }

        void GAME_BOOM(object sender, EventArgs e)
        {
            this.DrawBombs();
        }

        private void StartRestartGame()
        {
            if (this._game.IsPaused)
            {
                this._game.IsPaused = false;
            }
            else
            {
                this._game.Start();
            }
        }

        private void GenerateWorld(int columns, int rows, int size)
        {
            this._game.GameWorld = new World(columns, rows, size);

            for (int iColumns = 0; iColumns < columns; iColumns++)
            {
                this._gameGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(size) });
            }
            for (int iRows = 0; iRows < rows; iRows++)
            {
                this._gameGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(size) });
            }

            this._mainWindow.TargetGrid.Children.Add(this._gameGrid);
        }

        #region GameModeControl
        /*Notes:
         * Game Modes: HighscoreShow, RecordHighScore, PlayGame, Pause
         */

        void GameScreenMode_ModeChanged(object sender, EventArgs e)
        {
            switch (this._gameScreenMode.GameScreenModeType)
            {
                case ScreenMode.ScreenModeType.Start:
                    this.ConfigureStartMode();
                    break;
                case ScreenMode.ScreenModeType.Play:

                    this.ConfigurePlayMode();
                    break;
                case ScreenMode.ScreenModeType.Highscores:
                    this.ConfigureHighscoresMode();
                    break;
                case ScreenMode.ScreenModeType.Recordhighscore:
                    this.ConfigureRecordhighscoreMode();
                    break;
            }

        }

        private void ConfigureStartMode()
        {
            this._newMainWindowKeyDown = this.KeyInput_StartMode;
            this._dispatcher.Invoke(new Action(delegate { this._mainWindow.ScoresDataGrid.IsEnabled = false; }), DispatcherPriority.Send, null);
            this._dispatcher.Invoke(new Action(delegate { this._gameGrid.Visibility = Visibility.Visible; }), DispatcherPriority.Send, null);
            this._dispatcher.Invoke(new Action(delegate { this._mainWindow.HighScoreScreen.Visibility = Visibility.Hidden; }), DispatcherPriority.Send, null);
            this._dispatcher.Invoke(new Action(delegate { this._mainWindow.PauseIndicator.Visibility = Visibility.Visible; }), DispatcherPriority.Send, null);
        }
        void MainWindow_CloseHighScore(object sender, EventArgs e)
        {
            this._gameScreenMode.GameScreenModeType = ScreenMode.ScreenModeType.Start;
        }
        void GAME_RecordScoreAchieved(object sender, EventArgs e)
        {
            this._gameScreenMode.GameScreenModeType = ScreenMode.ScreenModeType.Recordhighscore;
        }
        private void ConfigureRecordhighscoreMode()
        {
            this._newMainWindowKeyDown = this.KeyInput_ScoreMode;
            this.PersistScore(new Score("Player", this._game.Snake.Age, this._game.Snake.Guid));
            this._dispatcher.Invoke(new Action(delegate { this._mainWindow.ScoresDataGrid.IsEnabled = true; }), DispatcherPriority.Send, null);
            this._dispatcher.Invoke(new Action(delegate { this._mainWindow.ScoresDataGrid.IsReadOnly = true; }), DispatcherPriority.Send, null);
            this._dispatcher.Invoke(new Action(delegate { this._gameGrid.Visibility = Visibility.Hidden; }), DispatcherPriority.Send, null);
            this._dispatcher.Invoke(new Action(delegate { this._mainWindow.HighScoreScreen.Visibility = Visibility.Visible; }), DispatcherPriority.Send, null);
            this._dispatcher.Invoke(new Action(delegate { this._mainWindow.PauseIndicator.Visibility = Visibility.Hidden; }), DispatcherPriority.Send, null);
        }

        private void ConfigureHighscoresMode()
        {
            this._newMainWindowKeyDown = this.KeyInput_StartMode;
            this._dispatcher.Invoke(new Action(delegate { this._mainWindow.ScoresDataGrid.IsEnabled = true; }), DispatcherPriority.Send, null);
            this._dispatcher.Invoke(new Action(delegate { this._mainWindow.ScoresDataGrid.IsReadOnly = true; }), DispatcherPriority.Send, null);
            this._dispatcher.Invoke(new Action(delegate { this._gameGrid.Visibility = Visibility.Hidden; }), DispatcherPriority.Send, null);
            this._dispatcher.Invoke(new Action(delegate { this._mainWindow.HighScoreScreen.Visibility = Visibility.Visible; }), DispatcherPriority.Send, null);
            this._dispatcher.Invoke(new Action(delegate { this._mainWindow.PauseIndicator.Visibility = Visibility.Hidden; }), DispatcherPriority.Send, null);
        }

        private void ConfigurePlayMode()
        {
            this._newMainWindowKeyDown = this.KeyInput_PlayMode;
            this._gameGrid.Visibility = Visibility.Visible;
            this._gameGrid.Background = Brushes.White;
            this._mainWindow.PauseIndicator.Visibility = Visibility.Hidden;
        }

        #endregion GameModeControl

        #region InputControl

        void KeyInput_StartMode(object sender, KeyEventArgs e)
        {
            this._gameScreenMode.GameScreenModeType = ScreenMode.ScreenModeType.Play;
            this.StartRestartGame();

        }
        void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            this._newMainWindowKeyDown(sender, e);
        }
        void KeyInput_PlayMode(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Up:
                case Key.Down:
                case Key.Left:
                case Key.Right:
                    this._game.Snake.ChangeDirection(e.Key);
                    break;
                case Key.P:
                    this._game.Snake.ChangeDirection(e.Key);
                    break;
            }
        }

        void KeyInput_ScoreMode(object sender, KeyEventArgs e) { }

        #endregion InputControl

        #region GAMEEventHandleing

        void GAME_HighScoreAchieved(object sender, EventArgs e)
        {
            this._dispatcher.Invoke(new Action(delegate { this._mainWindow.HighScoreLabel.Content = this._game.Snake.Age.ToString().PadLeft(7, "0".ToCharArray()[0]); }), DispatcherPriority.Send, null);
        }

        void GAME_SnakeDied(object sender, EventArgs e)
        {
            this._dispatcher.Invoke(new Action(delegate { this._gameGrid.Background = Brushes.Red; }), DispatcherPriority.Send, null);

            if (this._game.Snake.Age > this._game.GameScores[this._game.GameScores.Count - 1].PlayerScore)
            {
                this._gameScreenMode.GameScreenModeType = ScreenMode.ScreenModeType.Recordhighscore;
            }
            else
            {
                this._gameScreenMode.GameScreenModeType = ScreenMode.ScreenModeType.Highscores;
            }
        }

        void GAME_MoveCompleted(object sender, EventArgs e)
        {
            this._gameGrid.Children.Clear();
            this.DrawSnake();
            this.DrawBombs();
            this.UpdateScore();
        }

        #endregion GAMEEventHandleing

        #region ScoreManagement

        void ScoreContainer_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            Scores.SerializeData(this._game.GameScores);
        }

        private void UpdateScore()
        {
            this._mainWindow.CurrentScore.Content = this._game.Snake.Age.ToString().PadLeft(7, "0".ToCharArray()[0]);
        }

        private void LoadScores()
        {
            this._dispatcher.Invoke(new Action(delegate { this._mainWindow.ScoresDataGrid.Items.Refresh(); }), DispatcherPriority.Send, null);
            
            this._mainWindow.HighScoreLabel.Content = (this._game.GameScores.Count > 0 ? this._game.GameScores[0].PlayerScore : 0).ToString().PadLeft(7, "0".ToCharArray()[0]);
        }
        private void PersistScore(Score score)
        {
            this._game.GameScores.Add(score);
            Scores.SerializeData(this._game.GameScores);
            this.LoadScores();
        }

        #endregion ScoreManagement

        #region Drawing

        private void DrawBombs()
        {
            for (int i = 0; i < this._game.Bombs.Count; i++)
            {
                Image bombImage = new Image();
                if (!this._game.Bombs[i].IsBoomBoomTime(this._game.Snake.Age))
                {
                    if (this._game.Snake.Age % 3 == 0)
                    {
                        bombImage.Source = new BitmapImage(new Uri(Properties.Settings.Default.BombLeftSparkImageURI, UriKind.Relative));
                    }
                    else
                    {
                        bombImage.Source = new BitmapImage(new Uri(Properties.Settings.Default.BombRightSparkImageURI, UriKind.Relative));
                    }
                }
                else
                {
                    bombImage.Source = new BitmapImage(new Uri(Properties.Settings.Default.BOOMImageURI, UriKind.Relative));
                }

                bombImage.Width = bombImage.Source.Width;
                bombImage.Height = bombImage.Source.Height;
                this._gameGrid.Children.Add(bombImage);
                Grid.SetColumn(bombImage, this._game.Bombs[i].Location.Column);
                Grid.SetRow(bombImage, this._game.Bombs[i].Location.Row);
            }
        }

        private void DrawSnake()
        {
            Image headImage = new Image();
            headImage.Source = new BitmapImage(new Uri(Properties.Settings.Default.HeadImageURI, UriKind.Relative));
            headImage.Width = headImage.Source.Width;
            headImage.Height = headImage.Source.Height;
            this._gameGrid.Children.Add(headImage);

            switch (this._game.Snake.Vector.Direction)
            {
                case Key.Up:
                    headImage.LayoutTransform = new RotateTransform(0);
                    break;

                case Key.Right:
                    headImage.LayoutTransform = new RotateTransform(90);
                    break;

                case Key.Down:
                    headImage.LayoutTransform = new RotateTransform(180);
                    break;

                case Key.Left:
                    headImage.LayoutTransform = new RotateTransform(270);
                    break;
            }

            Grid.SetColumn(headImage, this._game.Snake.Head.Column);
            Grid.SetRow(headImage, this._game.Snake.Head.Row);

            for (int iBody = 0; iBody < this._game.Snake.Body.Count; iBody++)
            {
                Image bodyImage = new Image();
                bodyImage.Source = new BitmapImage(new Uri(Properties.Settings.Default.BodyImageURI, UriKind.Relative));
                bodyImage.Width = bodyImage.Source.Width;
                bodyImage.Height = bodyImage.Source.Height;
                this._gameGrid.Children.Add(bodyImage);
                Grid.SetColumn
                (
                    bodyImage, this._game.Snake.Body[iBody]
                        .Column);
                Grid.SetRow
                (
                    bodyImage, this._game.Snake.Body[iBody]
                        .Row);
            }

            Image tailImage = new Image();
            if (this._game.Snake.Age % 2 == 0)
            {
                tailImage.Source = new BitmapImage(new Uri(Properties.Settings.Default.TailZigImageURI, UriKind.Relative));
            }
            else
            {
                tailImage.Source = new BitmapImage(new Uri(Properties.Settings.Default.TailZagImageURI, UriKind.Relative));
            }

            tailImage.Width = tailImage.Source.Width;
            tailImage.Height = tailImage.Source.Height;
            this._gameGrid.Children.Add(tailImage);
            Grid.SetColumn(tailImage, this._game.Snake.Tail.Column);
            Grid.SetRow(tailImage, this._game.Snake.Tail.Row);
            Key lastBodyPartVector = new Key();

            if (this._game.Snake.Body[this._game.Snake.Body.Count - 1]
                    .Column > this._game.Snake.Tail.Column)
            {
                lastBodyPartVector = Key.Right;
            }

            if (this._game.Snake.Body[this._game.Snake.Body.Count - 1]
                    .Row > this._game.Snake.Tail.Row)
            {
                lastBodyPartVector = Key.Down;
            }

            if (this._game.Snake.Body[this._game.Snake.Body.Count - 1]
                    .Row < this._game.Snake.Tail.Row)
            {
                lastBodyPartVector = Key.Up;
            }

            if (this._game.Snake.Body[this._game.Snake.Body.Count - 1]
                    .Column < this._game.Snake.Tail.Column)
            {
                lastBodyPartVector = Key.Left;
            }

            //Transform Tail
            switch (lastBodyPartVector)
            {
                case Key.Up:
                    tailImage.LayoutTransform = new RotateTransform(0);
                    break;

                case Key.Right:
                    tailImage.LayoutTransform = new RotateTransform(90);
                    break;

                case Key.Down:
                    tailImage.LayoutTransform = new RotateTransform(180);
                    break;

                case Key.Left:
                    tailImage.LayoutTransform = new RotateTransform(270);
                    break;

            }
        }

        #endregion Drawing
    }
}
