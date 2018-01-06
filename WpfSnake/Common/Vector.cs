using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace WpfSnake.Common
{
    internal class Vector
    {
        private double _CellsPerSecond;

        public double CellsPerSecond
        {
            get { return _CellsPerSecond; }
            set { _CellsPerSecond = value; }
        }
        private Key _Direction;

        public Key Direction
        {
            get { return _Direction; }
            set { _Direction = value; }
        }
        public Vector()
        {

        }
        public Vector(double cellsPerSecond,Key direction)
        {
            _CellsPerSecond = cellsPerSecond;
            _Direction = direction;
        }
    }
}
