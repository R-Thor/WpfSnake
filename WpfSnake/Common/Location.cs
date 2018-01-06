using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WpfSnake.Common
{
    internal class Location
    {
        internal int Row { get; set; }
        internal int Column { get; set; }
        internal Location()
        {

        }
        internal Location(int row, int col)
        {
            Row = row;
            Column = col;
        }
    }
}
