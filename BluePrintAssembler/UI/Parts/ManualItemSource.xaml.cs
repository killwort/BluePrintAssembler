using System.Windows;
using System.Windows.Controls;

namespace BluePrintAssembler.UI.Parts
{
    /// <summary>
    /// Interaction logic for ManualItemSource.xaml
    /// </summary>
    public partial class ManualItemSource : UserControl
    {
        public ManualItemSource()
        {
            InitializeComponent();
        }

        private void AddToFactory(object sender, RoutedEventArgs e)
        {
            ((VM.ManualItemSource) DataContext).AddToFactory();
        }
    }
}
