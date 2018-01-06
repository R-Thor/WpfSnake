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
        private string _PlayerName = string.Empty;
        public string PlayerName
        {
            get { return _PlayerName; }
            set { _PlayerName = value; }
        }
        private int _PlayerScore = 0;
        public int PlayerScore
        {
            get { return _PlayerScore; }
            set { _PlayerScore = value; }
        }
        private DateTime _Date = new DateTime();
        public DateTime Date
        {
            get { return _Date; }
            set { _Date = value; }
        }
        public Score(){}
        public Score(string Player, int Score, Guid SnakeGuid)
        {
            this._Date = DateTime.Now;
            this._PlayerName = Player;
            this._PlayerScore = Score;
            this._SnakeGuid = SnakeGuid;
        }
        private bool _IsLastScoreMade = false;
        public bool IsLastScoreMade
        {
            get { return _IsLastScoreMade; }
            set { _IsLastScoreMade = value; }
        }
        private Guid _SnakeGuid = new Guid();
        public Guid SnakeGuid
        {
            get { return _SnakeGuid; }
            set { _SnakeGuid = value; }
        }
        //private bool _CurrentScore = false;
        //public bool CurrentScore
        //{
        //    get { return _IsLastScoreMade; }
        //    set { _IsLastScoreMade = value; }
        //}

    }

}
