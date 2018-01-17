using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Brushes = System.Windows.Media.Brushes;
using Point = System.Windows.Point;

namespace BluePrintAssembler.UI.Parts
{
    public class ProducibleItem : Control
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

        public VM.ProducibleItem Connection
        {
            get => (VM.ProducibleItem) GetValue(ConnectionProperty);
            set => SetValue(ConnectionProperty, value);
        }

        public Point StartPoint
        {
            get { return (Point) GetValue(StartPointProperty); }
            set { SetValue(StartPointProperty, value); }
        }

        public Point EndPoint
        {
            get { return (Point) GetValue(EndPointProperty); }
            set { SetValue(EndPointProperty, value); }
        }

        public static readonly DependencyProperty ColorProperty = DependencyProperty.Register(
            "Color", typeof(Color), typeof(ProducibleItem), new FrameworkPropertyMetadata(new Color(), FrameworkPropertyMetadataOptions.AffectsRender));

        public Color Color
        {
            get { return (Color) GetValue(ColorProperty); }
            set { SetValue(ColorProperty, value); }
        }
        protected override void OnRender(DrawingContext drawingContext)
        {
            var w = EndPoint.Y - StartPoint.Y;
            if (Math.Abs(w) < 100.0) w = Math.Sign(w) * 100.0;
            var startTangent = new Point(StartPoint.X, w > 0 ? (w / 2.0 + StartPoint.Y) : StartPoint.Y - w / 2.0);
            var endTangent = new Point(EndPoint.X, w > 0 ? (-w / 2.0 + EndPoint.Y) : EndPoint.Y + w / 2.0);

            var geom = new PathGeometry(new[]
            {
                new PathFigure(StartPoint,
                    new PathSegment[]
                        {new BezierSegment(startTangent, endTangent, EndPoint, true)}, false)
            });

            var brush = new SolidColorBrush(Color);
            drawingContext.DrawGeometry(null, new Pen(brush, 2), /*ConnectionsPen.PenFromType(((ConnectionInConfiguration)DataContext).Source.Type), */geom);
        }
    }
}