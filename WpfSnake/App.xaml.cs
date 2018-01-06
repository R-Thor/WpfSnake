using System.Windows;
using WpfSnake.Presenter;

namespace WpfSnake
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        void App_Startup(object sender, StartupEventArgs e)
        {
            new GamePresenter();
        }
    }
}
