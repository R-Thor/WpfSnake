using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WpfSnake.Common;

namespace WpfSnake
{
    class World
    {
        private int _Columns;

        internal int Columns
        {
            get { return _Columns; }
            set { _Columns = value; }
        }
        private int _Rows;

        internal int Rows
        {
            get { return _Rows; }
            set { _Rows = value; }
        }
        private int _Size;

        internal int Size
        {
            get { return _Size; }
            set { _Size = value; }
        }

        List<Location> _PopulatedLocations = new List<Location>();

        internal List<Location> PopulatedLocations
        {
            get { return _PopulatedLocations; }
            set { _PopulatedLocations = value; }
        }

        internal World(int Columns, int Rows, int Size)
        {
            //_Coordinates = new int[Columns, Rows];
            this.Columns = Columns;
            this.Rows = Rows;
            this.Size = Size;
        }
        
        //int[] 
    }
}
