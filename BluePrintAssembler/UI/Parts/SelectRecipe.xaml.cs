using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

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

        public static readonly DependencyProperty AddToFactoryProperty = DependencyProperty.Register(
            "AddToFactory", typeof(ICommand), typeof(SelectRecipe), new PropertyMetadata(default(ICommand)));

        public ICommand AddToFactory
        {
            get { return (ICommand) GetValue(AddToFactoryProperty); }
            set { SetValue(AddToFactoryProperty, value); }
        }

        public static readonly DependencyProperty UseRecipeProperty = DependencyProperty.Register(
            "UseRecipe", typeof(ICommand), typeof(SelectRecipe), new PropertyMetadata(default(ICommand)));

        public ICommand UseRecipe
        {
            get { return (ICommand) GetValue(UseRecipeProperty); }
            set { SetValue(UseRecipeProperty, value); }
        }
    }
}
