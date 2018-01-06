using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WpfSnake
{
    /// <summary>
    /// Interaction logic for HighScoreWindow.xaml
    /// </summary>
    public partial class HighScoreWindow : Window
    {
        Scores _Scores = new Scores();
        public HighScoreWindow()
        {
            InitializeComponent();
        }

        internal void LoadScores(Scores _Scores)//(object sender, RoutedEventArgs e)
        {
            this._Scores = _Scores;
            this.Dispatcher.Invoke(new Action(RefreshScores));
        }

        private void RefreshScores()
        {
            this.ScoreGrid.Children.Clear();
            int LastScoreIndex = GetHightLastScoreIndex();

            for (int i = 0; i < (_Scores.Count < 10 ? _Scores.Count : 10); i++)
            {
                Label RankLabel = new Label() { Name = "RankLabel" + _Scores.Count.ToString().PadLeft(2, "0".ToArray()[0]) };
                RankLabel.Content = (i + 1).ToString();
                RankLabel.FontWeight = (LastScoreIndex == i?FontWeights.UltraBold:FontWeights.Normal);
                this.ScoreGrid.Children.Add(RankLabel);
                Grid.SetColumn(RankLabel, 0);
                Grid.SetRow(RankLabel, i);
                //Setter s = new Setter(

                Label NameLabel = new Label() { Name = "NameLabel" + _Scores.Count.ToString().PadLeft(2, "0".ToArray()[0]) };
                NameLabel.Content = _Scores[i].PlayerName;
                NameLabel.FontWeight = (LastScoreIndex == i ? FontWeights.UltraBold : FontWeights.Normal);
                this.ScoreGrid.Children.Add(NameLabel);
                Grid.SetColumn(NameLabel, 1);
                Grid.SetRow(NameLabel, i);

                Label DateLabel = new Label() { Name = "DateLabel" + _Scores.Count.ToString().PadLeft(2, "0".ToArray()[0]) };
                DateLabel.Content = _Scores[i].Date.ToShortDateString() + " " + _Scores[i].Date.ToShortTimeString();// + _Scores[i].Date.TimeOfDay ;
                DateLabel.FontWeight = (LastScoreIndex == i ? FontWeights.UltraBold : FontWeights.Normal);
                this.ScoreGrid.Children.Add(DateLabel);
                Grid.SetColumn(DateLabel, 2);
                Grid.SetRow(DateLabel, i);

                Label ScoreLabel = new Label() { Name = "ScoreLabel" + _Scores.Count.ToString().PadLeft(2, "0".ToArray()[0]) };
                ScoreLabel.Content = _Scores[i].PlayerScore.ToString();
                ScoreLabel.FontWeight = (LastScoreIndex == i ? FontWeights.UltraBold : FontWeights.Normal);
                this.ScoreGrid.Children.Add(ScoreLabel);
                Grid.SetColumn(ScoreLabel, 3);
                Grid.SetRow(ScoreLabel, i);
            }
            
        }

        private int GetHightLastScoreIndex()
        {
            Scores Temp;
            Temp = Scores.SortByDate(Scores.Clone(_Scores));
            Score _score = Temp[0];
            int index = _Scores.IndexOf(Temp[0]);
            //this.ScoreGrid.Background .RowDefinitions[row].Style
            //this.ScoreGrid.RowDefinitions.

            return (index);
        }
    }
}
