using System;
using System.Collections.Generic;
using System.Windows.Input;
using WpfSnake.NetCore.Common;

namespace WpfSnake.NetCore.Business
{
    class Snake
    {
        internal protected event EventHandler SnakeLoaded;
        internal Location Head { get; set; }
        internal Location Tail { get; set; }
        private readonly Vector _vector = new Vector();
        internal Vector Vector { get { return(this._vector);} }
        private List<Location> _body = new List<Location>();
        private readonly List<Key> _moves = new List<Key>();
        internal Guid Guid = new Guid();
        private int _age = 0;

        public int Age
        {
            get { return this._age; }
        }

        internal List<Location> Body
        {
            get { return this._body; }
            set { this._body = value; }
        }
        internal bool Alive { get; set; }

        internal Snake(Vector v, Location initialHeadLocation, int initialLength, double growthRatePerTurn)
        {
            this.Guid = Guid.NewGuid();
            this.Alive = true;
            Location workingLocation = new Location();
            this._vector = v;
            this.Head = initialHeadLocation;
            workingLocation = this.Head;

            for (int x = 1; x < initialLength-1; x++)
            {
                Location bodySegment = new Location();
                switch (v.Direction)
                {
                    case Key.Up:
                        bodySegment = new Location(workingLocation.Row + 1, workingLocation.Column);
                        break;
                    case Key.Down:
                        bodySegment = new Location(workingLocation.Row - 1, workingLocation.Column);
                        break;
                    case Key.Left:
                        bodySegment = new Location(workingLocation.Row, workingLocation.Column - 1);
                        break;
                    case Key.Right:
                        bodySegment = new Location(workingLocation.Row, workingLocation.Column + 1);
                        break;
                }
               
                workingLocation = bodySegment;
                this._body.Add(bodySegment);
                if (this.SnakeLoaded != null)
                {
                    this.SnakeLoaded(this, new EventArgs());
                }
            }
            
            switch (v.Direction)
            {
                case Key.Up:
                    this.Tail = new Location(workingLocation.Row + 1, workingLocation.Column);
                    break;
                case Key.Down:
                    this.Tail = new Location(workingLocation.Row - 1, workingLocation.Column);
                    break;
                case Key.Left:
                    this.Tail = new Location(workingLocation.Row, workingLocation.Column - 1);
                    break;
                case Key.Right:
                    this.Tail = new Location(workingLocation.Row, workingLocation.Column + 1);
                    break;
            }
        }

        internal void ChangeDirection(Key k)
        {
            this._moves.Add(k);
        }

        internal void Move()
        {
            this._age++;
            Location workinglocation = this.Head;
            if (this._moves.Count > 0)
            {
                for (int i = 0; i < this._moves.Count; i++)
                {
                    if (this.IsMoveOption(this._moves[0]))
                    {
                        this.Vector.Direction = this._moves[0];
                        this._moves.Remove(this._moves[0]);
                        break;
                    }
                    else
                    {
                        this._moves.Remove(this._moves[0]);
                    }
                }
            }

            this.Head = this.VectorMove(this.Head, this.Vector.Direction);
            if (this._age % Settings.Default.GrowthTurnsPerOneUnit== 0)
            {
                this.Grow(workinglocation);
            }
            else
            {
                for (int x = 0; x < this._body.Count; x++)
                {
                    Location lastLocation = this._body[x];
                    this._body[x] = workinglocation;
                    workinglocation = lastLocation;
                }
                this.Tail = workinglocation;
            }
            
        }

        private bool IsMoveOption(Key key)
        {
            if
            (
                (this._vector.Direction == Key.Up && key == Key.Down)
                ||
                (this._vector.Direction == Key.Down && key == Key.Up)
                ||
                (this._vector.Direction == Key.Left && key == Key.Right)
                ||
                (this._vector.Direction == Key.Right && key == Key.Left)
                || key == this._vector.Direction
            )
            {
                return (false);
            }
            return (true);
        }

        private void Grow(Location workinglocation)
        {
            this._body.Insert(0, workinglocation);
        }

        private Location VectorMove(Location location, Key key)
        {
            switch (key)
            {
                case Key.Up:
                    location = new Location(location.Row - 1, location.Column);
                    break;
                case Key.Down:
                    location = new Location(location.Row + 1, location.Column);
                    break;
                case Key.Left:
                    location = new Location(location.Row, location.Column - 1);
                    break;
                case Key.Right:
                    location = new Location(location.Row, location.Column + 1);
                    break;
            }
            return (location);
        }
    }
}
