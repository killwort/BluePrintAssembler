using System;
using System.Windows;
using BluePrintAssembler.Domain;

namespace BluePrintAssembler
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private Splash _splash;
        private void App_OnStartup(object sender, StartupEventArgs e)
        {
            Configuration.Instance.Loaded += ConfigurationLoaded;
            _splash=new Splash();
            _splash.Show();
        }

        private void ConfigurationLoaded(object sender, EventArgs e)
        {
            var mainWindow = new MainWindow();
            mainWindow.Show();
            _splash.Close();
        }
    }
}
