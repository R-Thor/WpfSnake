using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using WpfSnake.Common;

namespace WpfSnake
{
    class Snake
    {
        internal protected event EventHandler SnakeLoaded;
        internal Location Head { get; set; }
        internal Location Tail { get; set; }
        private Vector _Vector = new Vector();
        internal Vector Vector { get { return(_Vector);} }
        private List<Location> _Body = new List<Location>();
        private List<Key> Moves = new List<Key>();
        internal Guid GUID = new Guid();
        private int _Age = 0;

        public int Age
        {
            get { return _Age; }
            //set { _Age = value; }
        }

        //internal event EventHandler ReadyToRenderEvent;
        //delegate ReadyToRender(object sender, EventArgs e);// MoveReady();

        internal List<Location> Body
        {
            get { return _Body; }
            set { _Body = value; }
        }
        internal bool Alive { get; set; }

        internal Snake(Vector v, Location initialHeadLocation, int initialLength, double GrowthRatePerTurn)
        {
            this.GUID = Guid.NewGuid();
            this.Alive = true;
            Location WorkingLocation = new Location();
            _Vector = v;
            Head = initialHeadLocation;
            WorkingLocation = Head;

            for (int x = 1; x < initialLength-1; x++)
            {
                Location BodySegment = new Location();
                switch (v.Direction)
                {
                    case Key.Up:
                        BodySegment = new Location(WorkingLocation.Row + 1, WorkingLocation.Column);
                        break;
                    case Key.Down:
                        BodySegment = new Location(WorkingLocation.Row - 1, WorkingLocation.Column);
                        break;
                    case Key.Left:
                        BodySegment = new Location(WorkingLocation.Row, WorkingLocation.Column - 1);
                        break;
                    case Key.Right:
                        BodySegment = new Location(WorkingLocation.Row, WorkingLocation.Column + 1);
                        break;
                }
               
                WorkingLocation = BodySegment;
                _Body.Add(BodySegment);
                if (SnakeLoaded != null)
                {
                    SnakeLoaded(this, new EventArgs());
                }
            }
            
            switch (v.Direction)
            {
                case Key.Up:
                    Tail = new Location(WorkingLocation.Row + 1, WorkingLocation.Column);
                    break;
                case Key.Down:
                    Tail = new Location(WorkingLocation.Row - 1, WorkingLocation.Column);
                    break;
                case Key.Left:
                    Tail = new Location(WorkingLocation.Row, WorkingLocation.Column - 1);
                    break;
                case Key.Right:
                    Tail = new Location(WorkingLocation.Row, WorkingLocation.Column + 1);
                    break;
            }
        }

        internal void ChangeDirection(Key k)
        {
            //if
            //    (
            //        (this._Vector.Direction == Key.Up && k == Key.Down)
            //        ||
            //        (this._Vector.Direction == Key.Down && k == Key.Up)
            //        ||
            //        (this._Vector.Direction == Key.Left && k == Key.Right)
            //        ||
            //        (this._Vector.Direction == Key.Right && k == Key.Left)
            //    )
            //{
            //    return;
            //}
            //this._Vector.Direction = k;
            Moves.Add(k);
        }
        internal void Move()
        {
            _Age++;
            Location workinglocation = this.Head;
            if (Moves.Count > 0)
            {
                for (int i = 0; i < Moves.Count; i++)
                {
                    if (IsMoveOption(Moves[0]))
                    {
                        this.Vector.Direction = Moves[0];
                        Moves.Remove(Moves[0]);
                        break;
                    }
                    else
                    {
                        Moves.Remove(Moves[0]);
                    }
                }
            }

            this.Head = VectorMove(this.Head, this.Vector.Direction);
            //Complex r1 = Complex.Reciprocal(value);
            //System.Math.re
            if (_Age % Properties.Settings.Default.GrowthTurnsPerOneUnit== 0)
            {
                Grow(workinglocation);
            }
            else
            {
                for (int x = 0; x < _Body.Count; x++)
                {
                    Location LastLocation = this._Body[x];
                    this._Body[x] = workinglocation;
                    workinglocation = LastLocation;
                }
                this.Tail = workinglocation;
            }
            
        }

        private bool IsMoveOption(Key key)
        {
            if
            (
                (this._Vector.Direction == Key.Up && key == Key.Down)
                ||
                (this._Vector.Direction == Key.Down && key == Key.Up)
                ||
                (this._Vector.Direction == Key.Left && key == Key.Right)
                ||
                (this._Vector.Direction == Key.Right && key == Key.Left)
                || key == this._Vector.Direction
            )
            {
                return (false);
            }
            return (true);
        }

        private void Grow(Location workinglocation)
        {
            _Body.Insert(0, workinglocation);
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
