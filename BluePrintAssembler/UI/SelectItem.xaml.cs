using System.Windows;

namespace BluePrintAssembler.UI
{
    /// <summary>
    /// Interaction logic for SelectItem.xaml
    /// </summary>
    public partial class SelectItem : Window
    {
        public SelectItem()
        {
            InitializeComponent();
        }

        private void SelectItem_OnLoaded(object sender, RoutedEventArgs e)
        {
            DataContext = new VM.SelectItem();
        }

        private void CommitSelection(object sender, RoutedEventArgs e)
        {
            if (((VM.SelectItem) DataContext).SelectedItem != null)
            {
                DialogResult = true;
                Close();
            }
        }
    }
}
