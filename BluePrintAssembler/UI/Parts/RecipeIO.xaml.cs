using System;
using System.Windows;
using System.Windows.Controls;
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
