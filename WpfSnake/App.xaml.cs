using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;

namespace WpfSnake
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        void App_Startup(object sender, StartupEventArgs e)
        {
            // Application is running
            // Process command line args
            //bool startMinimized = false;
            //for (int i = 0; i != e.Args.Length; ++i)
            //{
            //    if (e.Args[i] == "/StartMinimized")
            //    {
            //        startMinimized = true;
            //    }
            //}

            // Create main application window, starting minimized if specified
            //MainWindow4 mainWindow = new MainWindow4();
            //if (startMinimized)
            //{
            //    mainWindow.WindowState = WindowState.Minimized;
            //}
            //mainWindow.Show();
            GamePresenter GamePresenter = new GamePresenter();
        }
    }
}
