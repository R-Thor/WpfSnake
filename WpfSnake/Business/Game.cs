using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Threading;

namespace WpfSnake
{
    internal class Game
    {
        #region Types

        #region EventHandlers/Args
        internal protected class SnakeGeneratedEventArgs : EventArgs
        {
            public Guid SnakeGUID { get; set; }
            public SnakeGeneratedEventArgs(Guid SnakeGUID)
            {
                this.SnakeGUID = SnakeGUID;
            }
        }
        internal protected delegate void SnakeGeneratedEventHandler(object sender, SnakeGeneratedEventArgs e);
        #endregion EventHandlers/Args
        
        #endregion Types

        #region Fields
        private World _GameWorld;// = new World();
        internal protected Snake Snake;
        internal protected Scores GameScores = new Scores();
        private bool _IsPaused = false;
        private Dispatcher Dispatcher = Dispatcher.CurrentDispatcher;
        internal protected List<Bomb> Bombs = new List<Bomb>();
        Random r = new Random(DateTime.Now.Millisecond);
        
        //internal protected event EventHandler SnakeLoaded;

        //internal Guid GameGuid = new Guid();
        #endregion Fields

        #region Properties

        #endregion Properties

        


        #region Events

        //internal protected event EventHandler ModeChanged;
        internal protected event EventHandler SnakeDied;
        internal protected event EventHandler BOOM;
        internal protected event EventHandler HighScoreAchieved;
        internal protected event EventHandler RecordScoreAchieved;
        internal protected event EventHandler MoveCompleted;
        internal protected event SnakeGeneratedEventHandler SnakeGenerated;

        #endregion Events
        

        public bool IsPaused
        {
            get { return _IsPaused; }
            set { _IsPaused = value; }
        }
        //public bool IsPaused { get; set; }
        internal protected World GameWorld
        {
            get { return _GameWorld; }
            set { _GameWorld = value; }
        }
        
        public Game()
        {
            //this.
            Scores.DeserializeFromXML(ref this.GameScores);
        }


        //internal void Unpause()
        //{
        //    this.IsPaused = false;
        //}

        internal protected void Start()
        {
            System.Threading.Thread GAMEThread = new System.Threading.Thread(new System.Threading.ThreadStart(loop)) { Name="GAME_THREAD", IsBackground = true };
            GAMEThread.Start();
        }
        private void loop()
        {
            ResetGame();
            
            while (Snake.Alive)
            {
                if (!IsPaused)
                {
                    this.Dispatcher.Invoke(new Action(this.Move), DispatcherPriority.Send, null);
                    System.Threading.Thread.Sleep(Properties.Settings.Default.TurnTime);
                }
                else
                {
                    while (IsPaused){}
                }
            }
            //SnakeDeadEvent
            SnakeDied(this, new EventArgs());
            CheckScore();
            //this.Dispatcher.Invoke(new Action(delegate { this.WorldGrid.Background = Brushes.Red; }), DispatcherPriority.Send, null);
            //CheckScore();
        }

        private void ResetGame()
        {
            Snake = new Snake(new Common.Vector(.5, Key.Up), new Common.Location(7, 7), 5, .1);
            if (SnakeGenerated != null)
            {
                SnakeGenerated(this, new SnakeGeneratedEventArgs(this.Snake.GUID));
            }

            Bombs = new List<Bomb>();
        }
        private void CheckScore()
        {

            if (this.Snake.Alive)
            {
                if (this.Snake.Age > (this.GameScores.Count > 0 ? this.GameScores[0].PlayerScore : 0))
                {
                    HighScoreAchieved(this, new EventArgs());
                }
            }
            else
            {
                Score _Score = new Score("Player 1", this.Snake.Age, this.Snake.GUID);
                List<Score> _Scores = (from Score s in this.GameScores where _Score.PlayerScore > s.PlayerScore select s).ToList<Score>();
                if (_Scores.Count > 0 || this.GameScores.Count < 10)
                {
                    RecordScoreAchieved(this, new EventArgs());
                    return;
                }
            }
        }
        private void Move()
        {
            this._GameWorld.PopulatedLocations.Clear();

            this.Snake.Move();

            PersistSnakeLocation();

            RemoveBomb(this.Snake.Head);
            PokeBombs();
            PersistBombLocations();
            ManageBomb();
            //PersistBombLocations();
            
            PokeSnake();
            CheckScore();
            if (Snake.Alive)
            {
                MoveCompleted(this, new EventArgs());
            }
        }

        private void PersistSnakeLocation()
        {
            this.GameWorld.PopulatedLocations.Add(this.Snake.Head);
            this.GameWorld.PopulatedLocations.Add(this.Snake.Tail);
            for (int x = 0; x < this.Snake.Body.Count; x++)
            {
                this.GameWorld.PopulatedLocations.Add(this.Snake.Body[x]);
            }
        }
        private void PersistBombLocations()
        {
            for (int x = 0; x < this.Bombs.Count; x++)
            {
                this.GameWorld.PopulatedLocations.Add(this.Bombs[x].Location);
            }
        }
        private void PokeSnake()
        {
            if (Snake.Head.Column >= this._GameWorld.Columns)
            {
                Snake.Alive = false;
            }
            if (Snake.Head.Row >= this._GameWorld.Rows)
            {
                Snake.Alive = false;
            }
            if (Snake.Head.Row < 0)
            {
                Snake.Alive = false;
            }
            if (Snake.Head.Column < 0)
            {
                Snake.Alive = false;
            }
            List<WpfSnake.Common.Location> HitBodyParts = (from WpfSnake.Common.Location t in Snake.Body where t.Column == Snake.Head.Column && t.Row == Snake.Head.Row select t).ToList<Common.Location>();
            if (HitBodyParts.Count > 0)
            {
                Snake.Alive = false;
            }
            if (Snake.Head.Row == Snake.Tail.Row && Snake.Head.Column == Snake.Tail.Column)
            {
                Snake.Alive = false;
            }
        }
        

        
        private void RemoveBomb(Common.Location location)
        {
            List<Bomb> TempBombs = (from b in this.Bombs where b.Location.Column == location.Column && b.Location.Row == location.Row select b).ToList<Bomb>();
            for (int i = 0; i < TempBombs.Count; i++)
            {
                this.Bombs.Remove(TempBombs[i]);
            }
        }

        private void ManageBomb()
        {
           
            //int BombGenerationRate = BombGenerationRateMax;// (r.Next(BombGenerationRateMax - BombGenerationRateMin)) + BombGenerationRateMin;
            //Console.WriteLine("BombGenerationRate:" + BombGenerationRate.ToString());
            if (Snake.Age % Properties.Settings.Default.BombGenerationRateMax == 0)
            {
                for (int x = 0; x < Properties.Settings.Default.BombsLaidRate; x++)
                {
                    Bomb newBomb = new Bomb(Snake.Age, Properties.Settings.Default.BombTimer);
                    newBomb.Location = FindEmptyLocation();
                    Console.WriteLine("C:" + newBomb.Location.Column.ToString() + "R:" + newBomb.Location.Column.ToString());
                    Bombs.Add(newBomb);
                }
            }
        }

        private Common.Location FindEmptyLocation()
        {
            Common.Location __Location = new Common.Location();
            int RandomNumber = r.Next((this._GameWorld.Columns * this._GameWorld.Rows));
            __Location.Column = (RandomNumber / this._GameWorld.Rows);
            __Location.Row = (RandomNumber % this._GameWorld.Rows);

            int HitCounter = (from Common.Location l in this._GameWorld.PopulatedLocations where __Location.Column == l.Column && __Location.Row == l.Row select l).Count();

            if (HitCounter > 0)
            {
                __Location = FindEmptyLocation();
            }
            return (__Location);
        }


        private void PokeBombs()
        {
            for (int i = 0; i < Bombs.Count; i++)
            {
                if (Bombs[i].IsBoomBoomTime(Snake.Age))
                {
                    this.Snake.Alive = false;
                    BOOM(this, new EventArgs());
                    break;
                }
            }
        }
        //private void PokeBomb(Bomb bomb)
        //{
        //    if (bomb.IsBoomBoomTime(Snake.Age))
        //    {
        //        bomb.BOOM = true;
        //        this.Snake.Alive = false;
        //    }
        //}
    }
}
//    internal class Game
//    {
//        internal Grid WorldGrid = null;
//        internal Snake.Snake Snake = new Snake.Snake(new Common.Vector(.5, Key.Up), new Common.Location(7, 7), 5, .1);
//        internal List<Bomb> Bombs = new List<Bomb>();
//        internal int BombGenerationRateMax = 40;
//        //internal Dictionary<string, BitmapSource> BitmapSources = new Dictionary<string, BitmapSource>();
//        //internal int BombGenerationRateMin = 25;
//        //List<Move> MoveQueue = new List<Move>();
//        internal int BombTimer = 100;
//        Random r = new Random(DateTime.Now.Millisecond);
//        private Scores GameScores = new Scores();
//        private int WaitCount = 3;
//        private bool IsPaused = false;
//        private World GameWorld = new World();


//        public Game()
//        {
//            Scores.DeserializeFromXML(ref GameScores);
//            this.LoadScores();

//            GenerateWorld(20, 20, 15);
//        }
//        private void GenerateWorld(int rows, int cols, int size)
//        {
//            this.WorldGrid = new Grid();
//            //this.GameBorder.Child = this.WorldGrid;
//            this.WorldGrid.RowDefinitions.Clear();
//            this.WorldGrid.ColumnDefinitions.Clear();
//            for (int iColumns = 0; iColumns < cols; iColumns++)
//            {
//                this.WorldGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(size) });
//            }
//            for (int iRows = 0; iRows < rows; iRows++)
//            {
//                this.WorldGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(size) });
//            }
//            this.WorldGrid.ShowGridLines = false;
//            DrawSnake();


//        }

//        private void DrawSnake()
//        {
//            Image HeadImage = new Image();
//            //HeadImage.Source = BitmapSources["Head"];
//            //HeadImage.Source = new BitmapImage(new Uri(@"C:\Users\BruteF0rce\Documents\Visual Studio 2010\Projects\WpfSnake\WpfSnake\Resources\Head.png", UriKind.Absolute));
//            HeadImage.Source = new BitmapImage(new Uri(@".\Resources\Head.png", UriKind.Relative));
//            HeadImage.Width = HeadImage.Source.Width;
//            HeadImage.Height = HeadImage.Source.Height;
//            WorldGrid.Children.Add(HeadImage);
//            switch (_snake.Vector.Direction)
//            {
//                case Key.Up:
//                    HeadImage.LayoutTransform = new RotateTransform(0);
//                    break;

//                case Key.Right:
//                    HeadImage.LayoutTransform = new RotateTransform(90);
//                    break;

//                case Key.Down:
//                    HeadImage.LayoutTransform = new RotateTransform(180);
//                    break;

//                case Key.Left:
//                    HeadImage.LayoutTransform = new RotateTransform(270);
//                    break;

//            }
//            Grid.SetColumn(HeadImage, _snake.Head.Column);
//            Grid.SetRow(HeadImage, _snake.Head.Row);

//            for (int iBody = 0; iBody < _snake.Body.Count; iBody++)
//            {
//                Image BodyImage = new Image();
//                //BodyImage.Source = new BitmapImage(new Uri(@"C:\Users\BruteF0rce\Documents\Visual Studio 2010\Projects\WpfSnake\WpfSnake\Resources\Body.png", UriKind.Absolute));
//                //BodyImage.Source = BitmapSources["Body"];
//                BodyImage.Source = new BitmapImage(new Uri(@".\Resources\Body.png", UriKind.Relative));
//                BodyImage.Width = BodyImage.Source.Width;
//                BodyImage.Height = BodyImage.Source.Height;
//                WorldGrid.Children.Add(BodyImage);
//                Grid.SetColumn(BodyImage, _snake.Body[iBody].Column);
//                Grid.SetRow(BodyImage, _snake.Body[iBody].Row);

//            }
//            Image TailImage = new Image();
//            if (_snake.Age % 2 == 0)
//            {
//                //TailImage.Source = new BitmapImage(new Uri(@"C:\Users\BruteF0rce\Documents\Visual Studio 2010\Projects\WpfSnake\WpfSnake\Resources\TailZig.png", UriKind.Absolute));
//                //TailImage.Source = BitmapSources["TailZig"];
//                TailImage.Source = new BitmapImage(new Uri(@".\Resources\TailZig.png", UriKind.Relative));
//            }
//            else
//            {
//                //TailImage.Source = BitmapSources["TailZag"];
//                //TailImage.Source = new BitmapImage(new Uri(@"C:\Users\BruteF0rce\Documents\Visual Studio 2010\Projects\WpfSnake\WpfSnake\Resources\TailZag.png", UriKind.Absolute));
//                TailImage.Source = new BitmapImage(new Uri(@".\Resources\TailZag.png", UriKind.Relative));
//            }


//            TailImage.Width = TailImage.Source.Width;
//            TailImage.Height = TailImage.Source.Height;
//            WorldGrid.Children.Add(TailImage);
//            Grid.SetColumn(TailImage, _snake.Tail.Column);
//            Grid.SetRow(TailImage, _snake.Tail.Row);

//            Key lastBodyPartVector = new Key();

//            if (_snake.Body[_snake.Body.Count - 1].Column > _snake.Tail.Column)
//            {
//                lastBodyPartVector = Key.Right;
//            }

//            if (_snake.Body[_snake.Body.Count - 1].Row > _snake.Tail.Row)
//            {
//                lastBodyPartVector = Key.Down;
//            }

//            if (_snake.Body[_snake.Body.Count - 1].Row < _snake.Tail.Row)
//            {
//                lastBodyPartVector = Key.Up;
//            }

//            if (_snake.Body[_snake.Body.Count - 1].Column < _snake.Tail.Column)
//            {
//                lastBodyPartVector = Key.Left;
//            }

//            {//Transform Tail
//                switch (lastBodyPartVector)
//                {
//                    case Key.Up:
//                        TailImage.LayoutTransform = new RotateTransform(0);
//                        break;

//                    case Key.Right:
//                        TailImage.LayoutTransform = new RotateTransform(90);
//                        break;

//                    case Key.Down:
//                        TailImage.LayoutTransform = new RotateTransform(180);
//                        break;

//                    case Key.Left:
//                        TailImage.LayoutTransform = new RotateTransform(270);
//                        break;

//                }

//            }

//        }

//        private void DrawBombs()
//        {
//            for (int i = 0; i < Bombs.Count; i++)
//            {
//                Image BombImage = new Image();
//                if (!Bombs[i].IsBoomBoomTime(_snake.Age))
//                {
//                    if (_snake.Age % 3 == 0)
//                    {
//                        BombImage.Source = new BitmapImage(new Uri(@".\Resources\BombLeftSpark.png", UriKind.Relative));
//                        //BombImage.Source = BitmapSources["BombLeftSpark"];
//                        //BombImage.Source = new BitmapImage(new Uri(@"C:\Users\BruteF0rce\Documents\Visual Studio 2010\Projects\WpfSnake\WpfSnake\Resources\BombLeftSpark.png", UriKind.Absolute));
//                    }
//                    else
//                    {
//                        BombImage.Source = new BitmapImage(new Uri(@".\Resources\BombRightSpark.png", UriKind.Relative));
//                        //BombImage.Source = BitmapSources["BombRightSpark"];

//                        //BombImage.Source = new BitmapImage(new Uri(@"C:\Users\BruteF0rce\Documents\Visual Studio 2010\Projects\WpfSnake\WpfSnake\Resources\BombRightSpark.png", UriKind.Absolute));
//                    }
//                }
//                else
//                {
//                    BombImage.Source = new BitmapImage(new Uri(@".\Resources\BOOM.png", UriKind.Relative));
//                    //BombImage.Source = BitmapSources["BOOM"];
//                    //BombImage.Source = new BitmapImage(new Uri(@"C:\Users\BruteF0rce\Documents\Visual Studio 2010\Projects\WpfSnake\WpfSnake\Resources\BOOM.png", UriKind.Absolute));
//                }

//                BombImage.Width = BombImage.Source.Width;
//                BombImage.Height = BombImage.Source.Height;
//                WorldGrid.Children.Add(BombImage);

//                Grid.SetColumn(BombImage, Bombs[i].Location.Column);
//                Grid.SetRow(BombImage, Bombs[i].Location.Row);
//            }
//        }

//        private void RenderMove()
//        {
//            this._snake.Move();
//            this.ScoreLabel.Content = _snake.Age.ToString().PadLeft(7, "0".ToCharArray()[0]);
//            ManageBomb(this._snake.Head);
//            PokeSnake();
//            if (_snake.Alive)
//            {
//                this.WorldGrid.Children.Clear();
//                DrawBombs();
//                DrawSnake();

//            }
//        }

//        private void ManageBomb(Common.Location location)
//        {
//            List<Bomb> TempBombs = (from b in this.Bombs where b.Location.Column == location.Column && b.Location.Row == location.Row select b).ToList<Bomb>();
//            for (int i = 0; i < TempBombs.Count; i++)
//            {
//                this.Bombs.Remove(TempBombs[i]);
//            }

//            ManageBomb();
//        }

//        private void ManageBomb()
//        {
//            CheckBombs();


//            int BombGenerationRate = BombGenerationRateMax;// (r.Next(BombGenerationRateMax - BombGenerationRateMin)) + BombGenerationRateMin;
//            //Console.WriteLine("BombGenerationRate:" + BombGenerationRate.ToString());
//            if (_snake.Age % BombGenerationRate == 0)
//            {
//                Bomb newBomb = new Bomb(_snake.Age, BombTimer);
//                newBomb.Location = FindEmptyLocation();
//                Console.WriteLine("C:" + newBomb.Location.Column.ToString() + "R:" + newBomb.Location.Column.ToString());
//                Bombs.Add(newBomb);
//            }
//        }

//        private Common.Location FindEmptyLocation()
//        {
//            Common.Location __Location = new Common.Location();
//            int RandomNumber = r.Next((this.WorldGrid.ColumnDefinitions.Count * this.WorldGrid.RowDefinitions.Count));
//            __Location.Column = (RandomNumber / this.WorldGrid.RowDefinitions.Count);
//            __Location.Row = (RandomNumber % this.WorldGrid.RowDefinitions.Count);

//            for (int i = 0; i < this.WorldGrid.Children.Count; i++)
//            {
//                if (__Location.Column == Grid.GetColumn(this.WorldGrid.Children[i]) && __Location.Row == Grid.GetRow(this.WorldGrid.Children[i]))
//                {
//                    __Location = FindEmptyLocation();
//                }
//            }


//            return (__Location);
//        }

//        private void CheckBombs()
//        {
//            for (int i = 0; i < Bombs.Count; i++)
//            {
//                PokeBomb(Bombs[i]);
//            }
//        }

//        private void PokeBomb(Bomb bomb)
//        {
//            if (bomb.IsBoomBoomTime(_snake.Age))
//            {
//                this._snake.Alive = false;
//                DrawBombs();
//            }
//        }

//        private void PokeSnake()
//        {
//            if (_snake.Head.Column >= this.WorldGrid.ColumnDefinitions.Count)
//            {
//                _snake.Alive = false;
//            }
//            if (_snake.Head.Row >= this.WorldGrid.RowDefinitions.Count)
//            {
//                _snake.Alive = false;
//            }
//            if (_snake.Head.Row < 0)
//            {
//                _snake.Alive = false;
//            }
//            if (_snake.Head.Column < 0)
//            {
//                _snake.Alive = false;
//            }
//            List<WpfSnake.Common.Location> HitBodyParts = (from WpfSnake.Common.Location t in _snake.Body where t.Column == _snake.Head.Column && t.Row == _snake.Head.Row select t).ToList<Common.Location>();
//            if (HitBodyParts.Count > 0)
//            {
//                _snake.Alive = false;
//            }
//            if (_snake.Head.Row == _snake.Tail.Row && _snake.Head.Column == _snake.Tail.Column)
//            {
//                _snake.Alive = false;
//            }
//        }

//        private void Start()
//        {
//            System.Threading.Thread GAME = new System.Threading.Thread(new System.Threading.ThreadStart(loop)) { IsBackground = true };
//            GAME.Start();
//        }

//        private void loop()
//        {
//            while (_snake.Alive)
//            {
//                if (!IsPaused)
//                {
//                    this.Dispatcher.Invoke(new Action(this.RenderMove), DispatcherPriority.Send, null);
//                    System.Threading.Thread.Sleep(100);
//                }
//                else
//                {
//                    while (IsPaused)
//                    {

//                    }
//                }
//            }
//            this.Dispatcher.Invoke(new Action(delegate { this.WorldGrid.Background = Brushes.Red; }), DispatcherPriority.Send, null);
//            CheckScore();
//        }

//        private void CheckScore()
//        {
//            Score _Score = new Score("Player 1", this._snake.Age);

//            List<Score> _Scores = (from Score s in this.GameScores where _Score.PlayerScore > s.PlayerScore select s).ToList<Score>();
//            if (_Scores.Count > 0 || this.GameScores.Count < 10)
//            {
//                this.Dispatcher.Invoke(new Action(GetPlayerName), null);
//                return;
//            }
//            //PersistScore(_Score);
//        }

//        private void GetPlayerName()
//        {
//            this.GameViewbox.Visibility = System.Windows.Visibility.Hidden;
//            this.PlayerNameViewbox.Visibility = System.Windows.Visibility.Visible;
//        }

//        private void PersistScore(Score _Score)
//        {
//            this.GameScores.Add(_Score);
//            Scores.SerializeData(this.GameScores);
//            this.LoadScores();
//        }

//        private void StartButton_Click(object sender, RoutedEventArgs e)
//        {
//            _snake = new Snake.Snake(new Common.Vector(.5, Key.Up), new Common.Location(7, 7), 5, .1);
//            Bombs = new List<Bomb>();
//            GenerateWorld(20, 20, 15);
//            Thread CountDownThread = new Thread(new ThreadStart(StartCountDown));
//            CountDownThread.Start();
//            //Start();
//        }

//        private void StartCountDown()
//        {
//            this.Dispatcher.Invoke(DispatcherPriority.Send, new Action(delegate { this.WorldGrid.Visibility = System.Windows.Visibility.Hidden; }));
//            this.Dispatcher.Invoke(DispatcherPriority.Send, new Action(delegate { this.PauseLabel.Visibility = System.Windows.Visibility.Hidden; }));
//            this.Dispatcher.Invoke(DispatcherPriority.Send, new Action(delegate { this.StartLabel.Visibility = System.Windows.Visibility.Visible; }));
//            for (int i = this.WaitCount; i > 0; i--)
//            {
//                this.Dispatcher.Invoke(DispatcherPriority.Send, new Action(delegate { this.StartLabel.Content = i.ToString(); }));
//                Thread.Sleep(1000);
//            }
//            this.Dispatcher.Invoke(DispatcherPriority.Send, new Action(delegate { this.WorldGrid.Visibility = System.Windows.Visibility.Visible; }));
//            this.Dispatcher.Invoke(DispatcherPriority.Send, new Action(delegate { this.PauseLabel.Visibility = System.Windows.Visibility.Hidden; }));
//            this.Dispatcher.Invoke(DispatcherPriority.Send, new Action(delegate { this.StartLabel.Visibility = System.Windows.Visibility.Hidden; }));
//            this.Dispatcher.Invoke(DispatcherPriority.Send, new Action(delegate { Start(); }));
//        }
//        //private void ScoresButton_Click(object sender, RoutedEventArgs e)
//        //{
//        //    //ScoreWindow = new HighScoreWindow();
//        //    ScoreWindow.LoadScores(this.GameScores);
//        //    //if(ScoreWindow.
//        //    ScoreWindow.Show();
//        //}



//        internal void Unpause()
//        {
//            this.PauseLabel.Visibility = System.Windows.Visibility.Hidden;
//            this.WorldGrid.Visibility = System.Windows.Visibility.Visible;

//            this.IsPaused = false;
//        }

//        private void TogglePauseGame()
//        {
//            if (this.IsPaused)
//            {
//                Unpause();
//            }
//            else
//            {
//                Pause();
//            }
//        }

//        private void Pause()
//        {
//            this.IsPaused = true;
//            this.WorldGrid.Visibility = System.Windows.Visibility.Hidden;
//            this.PauseLabel.Visibility = System.Windows.Visibility.Visible;
//        }

//        //private void Window_Loaded(object sender, RoutedEventArgs e)
//        //{
//        //    ScoreWindow = new HighScoreWindow() { Owner = this };
//        //    this.ScoreWindow.Show();
//        //    this.ScoreWindow.LoadScores(this.GameScores);
//        //}

//        private void Button_Click(object sender, RoutedEventArgs e)
//        {
//            this.GameViewbox.Visibility = System.Windows.Visibility.Visible;
//            this.PlayerNameViewbox.Visibility = System.Windows.Visibility.Hidden;
//            Score _Score = new Score(this.PlayerNameTextBox.Text, this._snake.Age);
//            PersistScore(_Score);
//        }

//        internal void LoadScores()
//        {
//            this.ScoresDataGrid.ItemsSource = this.GameScores;
//            //this.Dispatcher.Invoke(new Action(RefreshScores));
//            RefreshScores();
//        }

//        private void RefreshScores()
//        {
//            this.ScoresDataGrid.Items.Refresh();

//        }
//        private void ScoresDataGrid_LoadingRow(object sender, DataGridRowEventArgs e)
//        {
//            if (((Score)(e.Row.Item)).IsLastScoreMade)
//            {
//                e.Row.Background = Brushes.Black;
//                e.Row.Foreground = Brushes.White;
//            }
//        }

//    }
//}
