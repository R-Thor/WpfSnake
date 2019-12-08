using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows.Input;
using System.Windows.Threading;
using WpfSnake.NetCore.Common;

//using WpfSnake.Properties;

namespace WpfSnake.NetCore.Business
{
    internal class Game
    {
        public Game()
        {
            Scores.DeserializeFromXml(ref this.GameScores);
        }

        public bool IsPaused
        {
            get;
            set;
        }

        protected internal World GameWorld
        {
            get;
            set;
        }

        protected internal void Start()
        {
            var gameThread = new Thread(this.Loop) {Name = @"GAME_THREAD", IsBackground = true};
            gameThread.Start();
        }
        private void Loop()
        {
            this.ResetGame();
            while (this.Snake.Alive)
            {
                if (!this.IsPaused)
                {
                    this._dispatcher.Invoke(new Action(this.Move), DispatcherPriority.Send, null);
                    Thread.Sleep(Settings.Default.TurnTime);
                }
                else
                {
                    while (this.IsPaused)
                    {
                    }
                }
            }

            //SnakeDeadEvent
            this.SnakeDied?.Invoke(this, new EventArgs());
        }

        private void ResetGame()
        {
            this.Snake = new Snake(new Vector(.5, Key.Up), new Location(7, 7), 5, .1);
            if (this.SnakeGenerated != null)
            {
                this.SnakeGenerated(this, new SnakeGeneratedEventArgs(this.Snake.Guid));
            }
            this.Bombs = new List<Bomb>();
        }
        private void CheckScore()
        {
            if (this.Snake.Alive)
            {
                if (this.Snake.Age > (this.GameScores.Count > 0
                        ? this.GameScores[0]
                            .PlayerScore
                        : 0))
                {
                    this.HighScoreAchieved(this, new EventArgs());
                }
            }
            else
            {
                var score = new Score("Player 1", this.Snake.Age, this.Snake.Guid);
                var scores = (from Score s in this.GameScores
                    where score.PlayerScore > s.PlayerScore
                    select s).ToList();
                if (scores.Count > 0 || this.GameScores.Count < 10)
                {
                    this.RecordScoreAchieved(this, new EventArgs());
                }
            }
        }
        private void Move()
        {
            this.GameWorld.PopulatedLocations.Clear();
            this.Snake.Move();
            this.PersistSnakeLocation();
            this.RemoveBomb(this.Snake.Head);
            this.PokeBombs();
            this.PersistBombLocations();
            this.ManageBomb();
            this.PokeSnake();
            this.CheckScore();
            if (this.Snake.Alive)
            {
                this.MoveCompleted(this, new EventArgs());
            }
        }

        private void PersistSnakeLocation()
        {
            this.GameWorld.PopulatedLocations.Add(this.Snake.Head);
            this.GameWorld.PopulatedLocations.Add(this.Snake.Tail);

            for (var x = 0; x < this.Snake.Body.Count; x++)
            {
                this.GameWorld.PopulatedLocations.Add(this.Snake.Body[x]);
            }
        }

        private void PersistBombLocations()
        {
            for (var x = 0; x < this.Bombs.Count; x++)
            {
                this.GameWorld.PopulatedLocations.Add
                (
                    this.Bombs[x].Location
                );
            }
        }

        private void PokeSnake()
        {
            if (this.Snake.Head.Column >= this.GameWorld.Columns)
            {
                this.Snake.Alive = false;
            }

            if (this.Snake.Head.Row >= this.GameWorld.Rows)
            {
                this.Snake.Alive = false;
            }

            if (this.Snake.Head.Row < 0)
            {
                this.Snake.Alive = false;
            }

            if (this.Snake.Head.Column < 0)
            {
                this.Snake.Alive = false;
            }

            var hitBodyParts = (from Location t in this.Snake.Body
                where t.Column == this.Snake.Head.Column && t.Row == this.Snake.Head.Row
                select t).ToList();

            if (hitBodyParts.Count > 0)
            {
                this.Snake.Alive = false;
            }

            if (this.Snake.Head.Row == this.Snake.Tail.Row && this.Snake.Head.Column == this.Snake.Tail.Column)
            {
                this.Snake.Alive = false;
            }
        }

        private void RemoveBomb(Location location)
        {
            var tempBombs = (from b in this.Bombs
                where b.Location.Column == location.Column && b.Location.Row == location.Row
                select b).ToList();

            for (var i = 0; i < tempBombs.Count; i++)
            {
                this.Bombs.Remove(tempBombs[i]);
            }
        }

        private void ManageBomb()
        {
            if (this.Snake.Age % Settings.Default.BombGenerationRateMax == 0)
            {
                for (var x = 0; x < Settings.Default.BombsLaidRate; x++)
                {
                    var newBomb = new Bomb(this.Snake.Age, Settings.Default.BombTimer);
                    newBomb.Location = this.FindEmptyLocation();
                    Console.WriteLine($@"C: {newBomb.Location.Column} R: {newBomb.Location.Column}");
                    this.Bombs.Add(newBomb);
                }
            }
        }

        private Location FindEmptyLocation()
        {
            var location = new Location();
            var randomNumber = this._random.Next(this.GameWorld.Columns * this.GameWorld.Rows);
            location.Column = randomNumber / this.GameWorld.Rows;
            location.Row = randomNumber % this.GameWorld.Rows;

            var hitCounter = (from Location l in this.GameWorld.PopulatedLocations
                where location.Column == l.Column && location.Row == l.Row
                select l).Count();

            if (hitCounter > 0)
            {
                location = this.FindEmptyLocation();
            }

            return location;
        }

        private void PokeBombs()
        {
            for (var i = 0; i < this.Bombs.Count; i++)
            {
                if (this.Bombs[i]
                    .IsBoomBoomTime(this.Snake.Age))
                {
                    this.Snake.Alive = false;
                    this.Boom(this, new EventArgs());
                    break;
                }
            }
        }

        #region Types

        #region EventHandlers/Args

        protected internal class SnakeGeneratedEventArgs : EventArgs
        {
            public SnakeGeneratedEventArgs(Guid snakeGuid)
            {
                this.SnakeGuid = snakeGuid;
            }
            public Guid SnakeGuid
            {
                get;
            }
        }
        protected internal delegate void SnakeGeneratedEventHandler(object sender, SnakeGeneratedEventArgs e);

        #endregion EventHandlers/Args

        #endregion Types

        #region Fields

        protected internal Snake Snake;
        protected internal readonly Scores GameScores = new Scores();
        private readonly Dispatcher _dispatcher = Dispatcher.CurrentDispatcher;
        protected internal List<Bomb> Bombs = new List<Bomb>();
        private readonly Random _random = new Random(DateTime.Now.Millisecond);

        #endregion Fields

        #region Properties

        #endregion Properties

        #region Events

        protected internal event EventHandler SnakeDied;
        protected internal event EventHandler Boom;
        protected internal event EventHandler HighScoreAchieved;
        protected internal event EventHandler RecordScoreAchieved;
        protected internal event EventHandler MoveCompleted;
        protected internal event SnakeGeneratedEventHandler SnakeGenerated;

        #endregion Events

    }
}