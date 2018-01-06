using WpfSnake.Common;

namespace WpfSnake.Business
{
    class Bomb
    {
        private readonly int _timer = 0;
        private readonly int _startTime;
        public Location Location { get; set; }

        public Bomb(int time, int timer)
        {
            this._startTime = time;
            this._timer = timer;
        }
        internal bool IsBoomBoomTime(int time)
        {
            if (time >= (this._timer + this._startTime))
            {
                return (true);
            }

            return(false);
        }
    }
}
