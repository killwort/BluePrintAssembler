using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Brushes = System.Windows.Media.Brushes;
using Point = System.Windows.Point;

namespace BluePrintAssembler.UI.Parts
{
    public class  ProducibleItem : Control
    {
        public ProducibleItem()
        {
            Cursor = Cursors.Hand;
        }

        public static readonly DependencyProperty ConnectionProperty = DependencyProperty.Register("Connection", typeof(VM.ProducibleItem),
                                                                                           typeof(ProducibleItem));
        public static readonly DependencyProperty StartPointProperty = DependencyProperty.Register("StartPoint", typeof(Point),
                                                                                           typeof(ProducibleItem), new FrameworkPropertyMetadata(new Point(0, 0), FrameworkPropertyMetadataOptions.AffectsRender));
        public static readonly DependencyProperty EndPointProperty = DependencyProperty.Register("EndPoint", typeof(Point),
                                                                                           typeof(ProducibleItem), new FrameworkPropertyMetadata(new Point(0, 0), FrameworkPropertyMetadataOptions.AffectsRender));
        public static readonly DependencyProperty StartTangentProperty = DependencyProperty.Register("StartTangent", typeof(Point),
                                                                                           typeof(ProducibleItem), new FrameworkPropertyMetadata(new Point(0, 0), FrameworkPropertyMetadataOptions.AffectsRender));
        public static readonly DependencyProperty EndTangentProperty = DependencyProperty.Register("EndTangent", typeof(Point),

            typeof(ProducibleItem), new FrameworkPropertyMetadata(new Point(0, 0), FrameworkPropertyMetadataOptions.AffectsRender));
        public VM.ProducibleItem Connection
        {
            get => (VM.ProducibleItem)GetValue(ConnectionProperty);
            set => SetValue(ConnectionProperty, value);
        }
        public Point StartPoint
        {
            get { return (Point)GetValue(StartPointProperty); }
            set { SetValue(StartPointProperty, value); }
        }
        public Point EndPoint
        {
            get { return (Point)GetValue(EndPointProperty); }
            set { SetValue(EndPointProperty, value); }
        }
        public Point StartTangent
        {
            get { return (Point)GetValue(StartTangentProperty); }
            set { SetValue(StartTangentProperty, value); }
        }
        public Point EndTangent
        {
            get { return (Point)GetValue(EndTangentProperty); }
            set { SetValue(EndTangentProperty, value); }
        }
        protected override void OnRender(DrawingContext drawingContext)
        {
            var geom =
                new PathGeometry(new[]
                                     {
                                         new PathFigure(StartPoint,
                                                        new PathSegment[]
                                                            {
                                                                new BezierSegment(StartTangent, EndTangent, EndPoint, true)
                                                            }, false)
                                     });
            drawingContext.DrawGeometry(null, new System.Windows.Media.Pen (Brushes.Red,2),/*ConnectionsPen.PenFromType(((ConnectionInConfiguration)DataContext).Source.Type), */geom);
        }
    }
}
