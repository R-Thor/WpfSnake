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
            Play,
            Start,
            Highscores,
            Recordhighscore
        }
        internal protected event EventHandler ModeChanged;
        //internal protected delegate void GameScreenModeChanged(object sender, EventArgs e);
        private ScreenModeType _gameScreenModeType;

        internal ScreenModeType GameScreenModeType
        {
            get { return (this._gameScreenModeType); } 
            set 
            {
                ScreenModeType tempGameScreenModeType = this._gameScreenModeType;

                this._gameScreenModeType = value;
                
                if (this._gameScreenModeType != tempGameScreenModeType)
                {
                    if (this.ModeChanged != null)
                    {
                        this.ModeChanged(this, new EventArgs());
                    }
                }

            } 
        }
    }
}
