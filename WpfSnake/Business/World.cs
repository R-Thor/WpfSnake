using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WpfSnake.Common;

namespace WpfSnake
{
    class World
    {
        private int _columns;

        internal int Columns
        {
            get { return this._columns; }
            set { this._columns = value; }
        }
        private int _rows;

        internal int Rows
        {
            get { return this._rows; }
            set { this._rows = value; }
        }
        private int _size;

        internal int Size
        {
            get { return this._size; }
            set { this._size = value; }
        }

        List<Location> _populatedLocations = new List<Location>();

        internal List<Location> PopulatedLocations
        {
            get { return this._populatedLocations; }
            set { this._populatedLocations = value; }
        }

        internal World(int columns, int rows, int size)
        {
            //_Coordinates = new int[Columns, Rows];
            this.Columns = columns;
            this.Rows = rows;
            this.Size = size;
        }
        
        //int[] 
    }
}
