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
using System.Threading;

namespace WpfSnake
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        Random d1 = new Random(DateTime.Now.Millisecond);
        Random d2 = new Random(DateTime.UtcNow.Millisecond * DateTime.UtcNow.Hour * DateTime.UtcNow.Minute);
        int Tracking = 0;
        int Rolls = 0;
        public Window1()
        {
            InitializeComponent();
            Thread t = new Thread(new ThreadStart(Start)) { IsBackground = true };
            t.Start();
        }
        private void Start()
        {
            for (int i = 0; i < 400; i++)
            {
                this.Dispatcher.Invoke(new Action(GenerateNumber), null);
                Thread.Sleep(1000);                
            }

        }
        private void GenerateNumber()
        {
            //outputTextBlock.Text += Environment.NewLine + r.Next(400).ToString();
            int Di1 = 1+ (d1.Next(400) % 6);
            int Di2 = 1 + (d2.Next(400) % 6);
            Tracking += (Di1 + Di2);
            Rolls++;
            string message = Di1.ToString() + ":" + Di2.ToString() + "(" + (Di1 + Di2).ToString() + "){Avg:" + ((float)Tracking/Rolls).ToString() + "}";
            
            if (Di1 + Di2 == 7)
            {
                message += ": SEVEN"; 
            }
            if (Di1 + Di2 == 11)
            {
                message += ": ELEVEN";
            }
            if (Di1 + Di2 == 12)
            {
                message += ":BOX CARS!!!";
            }
            if (Di1 + Di2 == 2)
            {
                message += ":SNAKE EYES!!!";
            }
            if (Di1 + Di2 == 3)
            {
                message += ":CRAPS!!!";
            }
                        

            outputTextBlock.Text = message + Environment.NewLine + outputTextBlock.Text;
        }
    }
}
