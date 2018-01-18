using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace BluePrintAssembler.UI.Parts
{

    public class AddableToFactoryUserControl:UserControl{

    }
    public partial class ManualItemSource : UserControl
    {
        public ManualItemSource()
        {
            InitializeComponent();
        }
        
        public static readonly DependencyProperty AddToFactoryProperty = DependencyProperty.Register("AddToFactory", typeof(ICommand), typeof(ManualItemSource), new PropertyMetadata(default(ICommand)));

        public ICommand AddToFactory
        {
            get { return (ICommand) GetValue(AddToFactoryProperty); }
            set { SetValue(AddToFactoryProperty, value); }
        }
    }
}
