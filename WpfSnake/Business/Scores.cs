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
        public static Scores Clone(Scores scores)
        {
            Scores cloneScores = new Scores();
            foreach (Score s in scores)
            {
                cloneScores.Add(s);
            }
            return (cloneScores);
        }
        public new void Add(Score score)
        {
            //Score.IsLastScoreMade = true;
            base.Add(score);

            this.Sort(Compare);

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
        internal static Scores SortByDate(Scores scores)
        {
            scores.Sort(CompareDate);
            return(scores);
        }
        internal static Scores SortByDateDesc(Scores scores)
        {
            scores.Sort(CompareDate);
            scores.Reverse();
            return (scores);
        }
        internal static int CompareDate(Score a, Score b)
        {
            if (a.Date == new DateTime())
            {
                if (b.Date == new DateTime())
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
                if (b.Date == new DateTime())
                {
                    return -1;
                }
                else
                {
                    if (a.Date > b.Date)
                    {
                        return (-1);
                    }
                    else
                    {
                        if (a.Date < b.Date)
                        {
                            return (1);
                        }
                    }
                }
            }
            return (0);
        }
        internal static int Compare(Score a, Score b)
        {
            if (a == null)
            {
                if (b == null)
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
                if (b == null)
                {
                    return -1;
                }
                else
                {
                    if (a.PlayerScore > b.PlayerScore)
                    {
                        return (-1);
                    }
                    else
                    {
                        if (a.PlayerScore < b.PlayerScore)
                        {
                            return (1);
                        }
                    }
                }
            }
            if (a.PlayerScore == b.PlayerScore)
            {
                if (a.Date < b.Date)
                {
                    return (-1);
                }
                if (a.Date > b.Date)
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
        static public void SerializeData(Scores scores)
        {
            if (scores.Count > 10)
            {
                scores.RemoveRange(10, scores.Count - 10);
            }
            XmlSerializer mySerializer = new XmlSerializer(typeof(Scores));
            //StreamWriter myWriter = new StreamWriter(Properties.Settings.Default.ScoresFile);
            MemoryStream mem = new MemoryStream();
            mySerializer.Serialize(mem, scores);
            Properties.Settings.Default.HighScores = Encoding.ASCII.GetString(mem.ToArray());
            Properties.Settings.Default.Save();
            //mySerializer.Serialize(myWriter, _Scores);
            //myWriter.Close();
        }
        static public void DeserializeFromXml(ref Scores scores)
        {
            XmlSerializer mySerializer = new XmlSerializer(typeof(Scores));
            //FileStream myFileStream = new FileStream(Properties.Settings.Default.ScoresFile, FileMode.Open);
            MemoryStream mem = new MemoryStream(Encoding.ASCII.GetBytes(Properties.Settings.Default.HighScores.ToCharArray()));
            scores = (Scores)mySerializer.Deserialize(mem);
        }
    }
	
}