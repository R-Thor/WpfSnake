using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace WpfSnake.Common
{
    internal class Vector
    {
        private double _cellsPerSecond;

        public double CellsPerSecond
        {
            get { return this._cellsPerSecond; }
            set { this._cellsPerSecond = value; }
        }
        private Key _direction;

        public Key Direction
        {
            get { return this._direction; }
            set { this._direction = value; }
        }
        public Vector()
        {

        }
        public Vector(double cellsPerSecond,Key direction)
        {
            this._cellsPerSecond = cellsPerSecond;
            this._direction = direction;
        }
    }
}
