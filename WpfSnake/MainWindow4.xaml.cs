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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Security.Permissions;
using System.Windows.Threading;
using System.Windows.Controls.Primitives;
using System.Threading;
//using System.Reflection;
//using System.Drawing;
//using System.IO;


namespace WpfSnake
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>

    public partial class MainWindow4 : Window
    {
        internal protected Grid GameContainer { get; set; }
        internal protected DataGrid ScoreContainer { get; set; }
        internal protected Viewbox PlayerName { get; set; }
        internal protected Label PauseIndicator { get; set; }
        internal protected Label CurrentScore { get; set; }
        internal protected Grid HighScoreScreen { get; set; }
        internal protected event EventHandler CloseHighScore;

        //internal protected Guid _CurrentSnakeGuid = new Guid();

        public static DependencyProperty CurrentSnakeGUIDProperty = DependencyProperty.Register("CurrentSnakeGUID", typeof(Guid), typeof(MainWindow4));

        public Guid CurrentSnakeGUID
        {
            get
            {
                return (Guid)GetValue(CurrentSnakeGUIDProperty);
            }
            set 
            {
                SetValue(CurrentSnakeGUIDProperty, value); 
            }
        }


        //internal protected string CurrentSnakeGUIDString
        //{
        //    get
        //    {
        //        return (CurrentSnakeGuid.ToString());
        //    }
        //}




        public MainWindow4()
        {
            CurrentSnakeGUID = new Guid();
            InitializeComponent();
            GameContainer = this.TargetGrid;
            ScoreContainer = this.ScoresDataGrid;
            HighScoreScreen = this.HighScoreGrid;
            PauseIndicator = this.PauseLabel;
            CurrentScore = this.ScoreLabel;
            //MessageBox.Show("Need to fix the Record User XAML");
            
        }

        private void BackToGameButton_Click(object sender, RoutedEventArgs e)
        {
            CloseHighScore(this, new EventArgs());
        }
    }
}
