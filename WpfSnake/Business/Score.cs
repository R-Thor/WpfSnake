using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
//using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Xml.Serialization;
using System.IO;

namespace WpfSnake
{
    public class Score
    {
        private string _playerName = string.Empty;
        public string PlayerName
        {
            get { return this._playerName; }
            set { this._playerName = value; }
        }
        private int _playerScore = 0;
        public int PlayerScore
        {
            get { return this._playerScore; }
            set { this._playerScore = value; }
        }
        private DateTime _date = new DateTime();
        public DateTime Date
        {
            get { return this._date; }
            set { this._date = value; }
        }
        public Score(){}
        public Score(string player, int score, Guid snakeGuid)
        {
            this._date = DateTime.Now;
            this._playerName = player;
            this._playerScore = score;
            this._snakeGuid = snakeGuid;
        }
        private bool _isLastScoreMade = false;
        public bool IsLastScoreMade
        {
            get { return this._isLastScoreMade; }
            set { this._isLastScoreMade = value; }
        }
        private Guid _snakeGuid = new Guid();
        public Guid SnakeGuid
        {
            get { return this._snakeGuid; }
            set { this._snakeGuid = value; }
        }
        //private bool _CurrentScore = false;
        //public bool CurrentScore
        //{
        //    get { return _IsLastScoreMade; }
        //    set { _IsLastScoreMade = value; }
        //}

    }

}
