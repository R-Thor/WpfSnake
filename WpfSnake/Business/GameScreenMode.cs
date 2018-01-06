using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WpfSnake
{
    class ScreenMode
    {
        internal protected enum ScreenModeType
        {
            PLAY,
            START,
            HIGHSCORES,
            RECORDHIGHSCORE
        }
        internal protected event EventHandler ModeChanged;
        //internal protected delegate void GameScreenModeChanged(object sender, EventArgs e);
        private ScreenModeType _GameScreenModeType;

        internal ScreenModeType GameScreenModeType
        {
            get { return (_GameScreenModeType); } 
            set 
            {
                ScreenModeType tempGameScreenModeType = _GameScreenModeType;

                _GameScreenModeType = value;
                
                if (_GameScreenModeType != tempGameScreenModeType)
                {
                    if (ModeChanged != null)
                    {
                        ModeChanged(this, new EventArgs());
                    }
                }

            } 
        }
    }
}
