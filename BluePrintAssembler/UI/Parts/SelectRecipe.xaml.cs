using System.Windows;
using System.Windows.Controls;

namespace BluePrintAssembler.UI.Parts
{
    /// <summary>
    /// Interaction logic for SelectRecipe.xaml
    /// </summary>
    public partial class SelectRecipe : UserControl
    {
        public SelectRecipe()
        {
            InitializeComponent();
        }

        private void DoUseRecipe(object sender, RoutedEventArgs e)
        {
            ((VM.SelectRecipe) DataContext).UseRecipe((VM.Recipe) ((FrameworkElement) e.Source).DataContext);
        }

        private void DoAddToFactory(object sender, RoutedEventArgs e)
        {
            ((VM.SelectRecipe) DataContext).AddToFactory();
        }
    }
}
