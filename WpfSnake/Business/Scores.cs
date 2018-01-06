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
    public class Scores : List<Score>
    {
        public Scores()
        {
            
        }
        public static Scores Clone(Scores Scores)
        {
            Scores CloneScores = new Scores();
            foreach (Score s in Scores)
            {
                CloneScores.Add(s);
            }
            return (CloneScores);
        }
        public new void Add(Score Score)
        {
            //Score.IsLastScoreMade = true;
            base.Add(Score);

            this.Sort(new Comparison<Score>(Scores.Compare));

            foreach (Score s in this)
            {
                if (s.IsLastScoreMade)
                {
                    s.IsLastScoreMade = false;
                    //break;
                }
            }

            this
            [
                this.IndexOf(
                (
                    from Score i in this
                    where i.Date ==
                        (from Score i2 in this select i2.Date).Max()
                    select i
                ).ToList<Score>()[0])
            ].IsLastScoreMade = true;
            
        }
        internal static Scores SortByDate(Scores Scores)
        {
            Scores.Sort(CompareDate);
            return(Scores);
        }
        internal static Scores SortByDateDesc(Scores Scores)
        {
            Scores.Sort(CompareDate);
            Scores.Reverse();
            return (Scores);
        }
        internal static int CompareDate(Score A, Score B)
        {
            if (A.Date == new DateTime())
            {
                if (B.Date == new DateTime())
                {
                    return 0;
                }
                else
                {
                    return 1;
                }
            }
            else
            {
                if (B.Date == new DateTime())
                {
                    return -1;
                }
                else
                {
                    if (A.Date > B.Date)
                    {
                        return (-1);
                    }
                    else
                    {
                        if (A.Date < B.Date)
                        {
                            return (1);
                        }
                    }
                }
            }
            return (0);
        }
        internal static int Compare(Score A, Score B)
        {
            if (A == null)
            {
                if (B == null)
                {
                    return 0;
                }
                else
                {
                    return 1;
                }
            }
            else
            {
                if (B == null)
                {
                    return -1;
                }
                else
                {
                    if (A.PlayerScore > B.PlayerScore)
                    {
                        return (-1);
                    }
                    else
                    {
                        if (A.PlayerScore < B.PlayerScore)
                        {
                            return (1);
                        }
                    }
                }
            }
            if (A.PlayerScore == B.PlayerScore)
            {
                if (A.Date < B.Date)
                {
                    return (-1);
                }
                if (A.Date > B.Date)
                {
                    return (1);
                }
            }
            return (0);
        }
        //static public void SerializeData(Scores _Scores)
        //{
        //    if (_Scores.Count > 10)
        //    {
        //        _Scores.RemoveRange(10, _Scores.Count-10);
        //    }
        //    XmlSerializer mySerializer = new XmlSerializer(typeof(Scores));
        //    StreamWriter myWriter = new StreamWriter(Properties.Settings.Default.ScoresFile);
        //    mySerializer.Serialize(myWriter, _Scores);
        //    myWriter.Close();
        //}
        //static public void DeserializeFromXML(ref Scores _Scores)
        //{
        //    XmlSerializer mySerializer = new XmlSerializer(typeof(Scores));
        //    FileStream myFileStream = new FileStream(Properties.Settings.Default.ScoresFile, FileMode.Open);
        //    _Scores = (Scores)mySerializer.Deserialize(myFileStream);
        //    myFileStream.Close();
        //}
        static public void SerializeData(Scores _Scores)
        {
            if (_Scores.Count > 10)
            {
                _Scores.RemoveRange(10, _Scores.Count - 10);
            }
            XmlSerializer mySerializer = new XmlSerializer(typeof(Scores));
            //StreamWriter myWriter = new StreamWriter(Properties.Settings.Default.ScoresFile);
            MemoryStream mem = new MemoryStream();
            mySerializer.Serialize(mem, _Scores);
            Properties.Settings.Default.HighScores = Encoding.ASCII.GetString(mem.ToArray());
            Properties.Settings.Default.Save();
            //mySerializer.Serialize(myWriter, _Scores);
            //myWriter.Close();
        }
        static public void DeserializeFromXML(ref Scores _Scores)
        {
            XmlSerializer mySerializer = new XmlSerializer(typeof(Scores));
            //FileStream myFileStream = new FileStream(Properties.Settings.Default.ScoresFile, FileMode.Open);
            MemoryStream mem = new MemoryStream(Encoding.ASCII.GetBytes(Properties.Settings.Default.HighScores.ToCharArray()));
            _Scores = (Scores)mySerializer.Deserialize(mem);
        }
    }
	
}