using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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
    }
}
