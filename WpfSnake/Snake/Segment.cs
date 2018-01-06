using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using WpfSnake.Common;
namespace WpfSnake.Snake
{
    internal class Segment
    {
        private Location _Location;

        internal Location Location
        {
            get { return _Location; }
            set { _Location = value; }
        }
        private Vector _Vector;

        internal Vector Vector
        {
            get { return _Vector; }
            set { _Vector = value; }
        }
        private Image _Image;

        public Image Image
        {
            get { return _Image; }
            set { _Image = value; }
        }
    }
}
