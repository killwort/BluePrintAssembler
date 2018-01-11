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
using BluePrintAssembler.Utils;

namespace BluePrintAssembler.UI.Parts
{
    /// <summary>
    /// Interaction logic for RecipeIO.xaml
    /// </summary>
    public partial class RecipeIO : UserControl
    {
        public RecipeIO()
        {
            InitializeComponent();
        }

        private void UserControl_LayoutUpdated(object sender, EventArgs e)
        {
            var pt = ConnectionPoint;
            if (pt.HasValue)
                ((VM.RecipeIO) DataContext).ConnectionPoint = pt.Value;
        }
        public Point? ConnectionPoint
        {
            get
            {
                var canvas = MyVisualTreeHelper.GetParent<DynamicCanvas>(this);
                if (canvas != null)
                {
                    var connectionPoint = new Point(ActualWidth / 2, ActualHeight / 2);
                    connectionPoint = TransformToAncestor(canvas).Transform(connectionPoint);

                    return connectionPoint;
                }
                return null;
            }
        }

    }
}
