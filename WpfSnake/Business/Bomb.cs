using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WpfSnake.Common;

namespace WpfSnake
{
    class Bomb
    {
        //public bool BOOM { get; set; }
        private int Timer = 0;
        private int StartTime;
        public Location Location { get; set; }
        //private bool _BOOM = false;

        //public bool BOOM
        //{
        //    get { return _BOOM; }
        //    set { _BOOM = value; }
        //}

        public Bomb(int Time, int Timer)
        {
            this.StartTime = Time;
            this.Timer = Timer;
        }
        internal bool IsBoomBoomTime(int Time)
        {
            if(Time >= (Timer+StartTime))
            {
                return(true);
            }
            return(false);
        }
    }
}
