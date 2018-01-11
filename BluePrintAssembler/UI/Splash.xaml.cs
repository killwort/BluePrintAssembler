using System.Linq;
using System.Windows;
using BluePrintAssembler.Domain;
using BluePrintAssembler.Steam;

namespace BluePrintAssembler
{
    /// <summary>
    /// Interaction logic for Splash.xaml
    /// </summary>
    public partial class Splash : Window
    {
        public Splash()
        {
            InitializeComponent();
            DataContext=Configuration.Instance;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var tsk=Configuration.Instance.Load();
            tsk.ContinueWith(x => { Configuration.Instance.Test(); });
        }
    }
}
