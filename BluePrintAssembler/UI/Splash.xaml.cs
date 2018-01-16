using System.Windows;
using BluePrintAssembler.Domain;

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
            Configuration.Instance.Load();
        }
    }
}
