using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Security.Permissions;
using System.Windows.Threading;
using WpfSnake.Input;
using System.Windows.Controls.Primitives;


namespace WpfSnake
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>

    public partial class MainWindow : Window
    {
        internal Grid WorldGrid = null;
        internal Snake.Snake _snake = new Snake.Snake(new Common.Vector(.5, Key.Up), new Common.Location(7, 7), 5, .1);
        internal List<Bomb> Bombs = new List<Bomb>();
        internal int BombGenerationRateMax = 40;
        //internal int BombGenerationRateMin = 25;
        List<Move> MoveQueue = new List<Move>();
        internal int BombTimer = 100;
        Random r = new Random(DateTime.Now.Millisecond);
        private Scores GameScores = new Scores();

        internal HighScoreWindow ScoreWindow;

        public MainWindow()
        {
            InitializeComponent();
            //ScoreWindow = new HighScoreWindow() { Owner = this };
            Scores.DeserializeFromXML(ref GameScores);
            GenerateWorld(20,20,15);

        }

        private void GenerateWorld(int rows,int cols,int size)
        {
            this.WorldGrid = new Grid();
            this.GameBorder.Child = this.WorldGrid;
            this.WorldGrid.RowDefinitions.Clear();
            this.WorldGrid.ColumnDefinitions.Clear();
            for (int iColumns = 0; iColumns < cols; iColumns++)
            {
                this.WorldGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(size) });
            }
            for (int iRows = 0; iRows < rows; iRows++)
            {
                this.WorldGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(size) });
            }
            this.WorldGrid.ShowGridLines = false;
            DrawSnake();

        }

        private void DrawSnake()
        {
            Image HeadImage = new Image();
            HeadImage.Source = new BitmapImage(new Uri(@"C:\Users\BruteF0rce\Documents\Visual Studio 2010\Projects\WpfSnake\WpfSnake\Resources\Head.png", UriKind.Absolute));
            HeadImage.Width = HeadImage.Source.Width;
            HeadImage.Height = HeadImage.Source.Height;
            WorldGrid.Children.Add(HeadImage);
            switch (_snake.Vector.Direction)
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
            Grid.SetColumn(HeadImage, _snake.Head.Column);
            Grid.SetRow(HeadImage, _snake.Head.Row);

            for (int iBody = 0; iBody < _snake.Body.Count; iBody++)
            {
                Image BodyImage = new Image();
                BodyImage.Source = new BitmapImage(new Uri(@"C:\Users\BruteF0rce\Documents\Visual Studio 2010\Projects\WpfSnake\WpfSnake\Resources\Body.png", UriKind.Absolute));
                BodyImage.Width = BodyImage.Source.Width;
                BodyImage.Height = BodyImage.Source.Height;
                WorldGrid.Children.Add(BodyImage);
                Grid.SetColumn(BodyImage, _snake.Body[iBody].Column);
                Grid.SetRow(BodyImage, _snake.Body[iBody].Row);

            }
            Image TailImage = new Image();
            if (_snake.Age % 2 == 0)
            {
                TailImage.Source = new BitmapImage(new Uri(@"C:\Users\BruteF0rce\Documents\Visual Studio 2010\Projects\WpfSnake\WpfSnake\Resources\TailZig.png", UriKind.Absolute));
            }
            else
            {
                TailImage.Source = new BitmapImage(new Uri(@"C:\Users\BruteF0rce\Documents\Visual Studio 2010\Projects\WpfSnake\WpfSnake\Resources\TailZag.png", UriKind.Absolute));
            }


            TailImage.Width = TailImage.Source.Width;
            TailImage.Height = TailImage.Source.Height;
            WorldGrid.Children.Add(TailImage);
            Grid.SetColumn(TailImage, _snake.Tail.Column);
            Grid.SetRow(TailImage, _snake.Tail.Row);

            Key lastBodyPartVector = new Key();

            if(_snake.Body[_snake.Body.Count-1].Column > _snake.Tail.Column)
            {
                lastBodyPartVector = Key.Right;
            }

            if(_snake.Body[_snake.Body.Count-1].Row > _snake.Tail.Row)
            {
                lastBodyPartVector = Key.Down;
            }

            if(_snake.Body[_snake.Body.Count-1].Row < _snake.Tail.Row)
            {
                lastBodyPartVector = Key.Up;
            }

            if (_snake.Body[_snake.Body.Count - 1].Column < _snake.Tail.Column)
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

        private void DrawBombs()
        {
            for (int i = 0; i < Bombs.Count; i++)
            {
                Image BombImage = new Image();
                if (!Bombs[i].IsBoomBoomTime(_snake.Age))
                {
                    if (_snake.Age % 3 == 0)
                    {
                        BombImage.Source = new BitmapImage(new Uri(@"C:\Users\BruteF0rce\Documents\Visual Studio 2010\Projects\WpfSnake\WpfSnake\Resources\BombLeftSpark.png", UriKind.Absolute));
                    }
                    else
                    {
                        BombImage.Source = new BitmapImage(new Uri(@"C:\Users\BruteF0rce\Documents\Visual Studio 2010\Projects\WpfSnake\WpfSnake\Resources\BombRightSpark.png", UriKind.Absolute));
                    }
                }
                else
                {
                    BombImage.Source = new BitmapImage(new Uri(@"C:\Users\BruteF0rce\Documents\Visual Studio 2010\Projects\WpfSnake\WpfSnake\Resources\BOOM.png", UriKind.Absolute));
                }

                BombImage.Width = BombImage.Source.Width;
                BombImage.Height = BombImage.Source.Height;
                WorldGrid.Children.Add(BombImage);

                Grid.SetColumn(BombImage, Bombs[i].Location.Column);
                Grid.SetRow(BombImage, Bombs[i].Location.Row);               
            }
        }

        private void RenderMove()
        {
            this._snake.Move();
            this.ScoreLabel.Content = _snake.Age.ToString().PadLeft(7,"0".ToCharArray()[0]); 
            ManageBomb(this._snake.Head);
            PokeSnake();
            if (_snake.Alive)
            {
                this.WorldGrid.Children.Clear();
                DrawBombs();
                DrawSnake();
               
            }
        }

        private void ManageBomb(Common.Location location)
        {
            List<Bomb> TempBombs = (from b in this.Bombs where b.Location.Column == location.Column && b.Location.Row == location.Row select b).ToList<Bomb>();
            for (int i = 0; i < TempBombs.Count; i++)
            {
                this.Bombs.Remove(TempBombs[i]);
            }

            ManageBomb();
        }

        private void ManageBomb()
        {
            CheckBombs();


            int BombGenerationRate = BombGenerationRateMax;// (r.Next(BombGenerationRateMax - BombGenerationRateMin)) + BombGenerationRateMin;
            //Console.WriteLine("BombGenerationRate:" + BombGenerationRate.ToString());
            if(_snake.Age % BombGenerationRate == 0)
            {
                Bomb newBomb = new Bomb(_snake.Age, BombTimer);
                newBomb.Location = FindEmptyLocation();
                Console.WriteLine("C:" + newBomb.Location.Column.ToString() + "R:" + newBomb.Location.Column.ToString());
                Bombs.Add(newBomb);
            }
        }

        private Common.Location FindEmptyLocation()
        {
            Common.Location __Location = new Common.Location();
            int RandomNumber = r.Next((this.WorldGrid.ColumnDefinitions.Count * this.WorldGrid.RowDefinitions.Count));
            __Location.Column = (RandomNumber / this.WorldGrid.RowDefinitions.Count);
            __Location.Row = (RandomNumber % this.WorldGrid.RowDefinitions.Count);

            for (int i = 0; i < this.WorldGrid.Children.Count; i++)
			{
                if(__Location.Column == Grid.GetColumn(this.WorldGrid.Children[i]) && __Location.Row == Grid.GetRow(this.WorldGrid.Children[i]))
                {
                    __Location = FindEmptyLocation();
                }
			}
                

            return (__Location);
        }

        private void CheckBombs()
        {
            for (int i = 0; i < Bombs.Count; i++)
            {
                PokeBomb(Bombs[i]);
            }
        }

        private void PokeBomb(Bomb bomb)
        {
            if (bomb.IsBoomBoomTime(_snake.Age))
            {
                this._snake.Alive = false;
                DrawBombs();
            }
        }

        private void PokeSnake()
        {
            if (_snake.Head.Column >= this.WorldGrid.ColumnDefinitions.Count)
            {
                _snake.Alive = false;
            }
            if (_snake.Head.Row >= this.WorldGrid.RowDefinitions.Count)
            {
                _snake.Alive = false;
            }
            if (_snake.Head.Row < 0)
            {
                _snake.Alive = false;
            }
            if (_snake.Head.Column < 0)
            {
                _snake.Alive = false;
            }
            List<WpfSnake.Common.Location> HitBodyParts = (from WpfSnake.Common.Location t in _snake.Body where t.Column == _snake.Head.Column && t.Row == _snake.Head.Row select t).ToList<Common.Location>();
            if (HitBodyParts.Count > 0)
            {
                _snake.Alive = false;
            }
            if (_snake.Head.Row == _snake.Tail.Row && _snake.Head.Column == _snake.Tail.Column)
            {
                _snake.Alive = false;
            }
        }

        private void Start()
        {
            //for (; ; )
            //{
                System.Threading.Thread GAME = new System.Threading.Thread(new System.Threading.ThreadStart(loop)) { IsBackground = true };
                GAME.Start();
                //GAME.Join();
                //_snake = new Snake.Snake(new Snake.Vector(.5, Key.Up), new Snake.Location(7, 7), 5, .1);
                //GenerateWorld(20, 20, 15);
            //}

        }

        private void loop()
        {
            while (_snake.Alive)
            {
                this.Dispatcher.Invoke(new Action(this.RenderMove), DispatcherPriority.Send, null);
                System.Threading.Thread.Sleep(100);
            }
            this.Dispatcher.Invoke(new Action(delegate {this.WorldGrid.Background = Brushes.Red;}), DispatcherPriority.Send, null);
            CheckScore();
        }

        private void CheckScore()
        {
            Score _Score = new Score("Player 1", this._snake.Age);
            
            List<Score> _Scores = (from Score s in this.GameScores where _Score.PlayerScore > s.PlayerScore select s).ToList<Score>();
            if (_Scores.Count > 0)
            {
                this.Dispatcher.Invoke(new Action(GetPlayerName),null);
                return;
            }
            //PersistScore(_Score);
        }

        private void GetPlayerName()
        {
            this.GameViewbox.Visibility = System.Windows.Visibility.Hidden;
            this.PlayerNameViewbox.Visibility = System.Windows.Visibility.Visible;
        }

        private void PersistScore(Score _Score)
        {
            this.GameScores.Add(_Score);
            Scores.SerializeData(this.GameScores);
            this.ScoreWindow.LoadScores(this.GameScores);
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            _snake = new Snake.Snake(new Common.Vector(.5, Key.Up), new Common.Location(7, 7), 5, .1);
            Bombs = new List<Bomb>();
            GenerateWorld(20, 20, 15);
            Start();
        }
        private void ScoresButton_Click(object sender, RoutedEventArgs e)
        {
            //ScoreWindow = new HighScoreWindow();
            ScoreWindow.LoadScores(this.GameScores);
            //if(ScoreWindow.
            ScoreWindow.Show();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Up:
                case Key.Down:
                case Key.Left:
                case Key.Right:
                    _snake.ChangeDirection(e.Key);
                    break;
                case Key.P:
                    TogglePauseGame();
                    break;
                default:
                    Unpause();
                    break;
            }
        }

        private void Unpause()
        {
            this.PauseLabel.Visibility = System.Windows.Visibility.Hidden;
            this.WorldGrid.Visibility = System.Windows.Visibility.Visible;
            
        }

        private void TogglePauseGame()
        {
            if (this.PauseLabel.IsVisible)
            {
                Unpause();
            }
            else
            {
                Pause();
            }
        }

        private void Pause()
        {
            this.WorldGrid.Visibility = System.Windows.Visibility.Hidden;
            this.PauseLabel.Visibility = System.Windows.Visibility.Visible;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ScoreWindow = new HighScoreWindow() { Owner = this };
            this.ScoreWindow.Show();
            this.ScoreWindow.LoadScores(this.GameScores);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.GameViewbox.Visibility = System.Windows.Visibility.Visible;
            this.PlayerNameViewbox.Visibility = System.Windows.Visibility.Hidden;
            Score _Score = new Score(this.PlayerNameTextBox.Text, this._snake.Age);
            PersistScore(_Score);
        }
    }
}
