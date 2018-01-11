using System;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace BluePrintAssembler.UI.Parts
{
    public class SelectionAdorner : Adorner
    {
        public bool IsDragging {
			get { return (bool)GetValue(IsDraggingProperty); }
			set{
				SetValue(IsDraggingProperty,value);
				m_child.Fill=value?m_brush:null;
			}
		}

		// Using a DependencyProperty as the backing store for IsDragging.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty IsDraggingProperty=
			DependencyProperty.Register("IsDragging",typeof(bool),typeof(SelectionAdorner),new UIPropertyMetadata(false));

    	private readonly VisualBrush m_brush;
        // Be sure to call the base class constructor.
        public SelectionAdorner(UIElement adornedElement)
            : base(adornedElement)
        {
             m_brush= new VisualBrush(adornedElement);
             ((FrameworkElement)adornedElement).SizeChanged += AdornedElementSizeChanged;
            m_child = new Rectangle
                         {
                             Width = adornedElement.RenderSize.Width,
                             Height = adornedElement.RenderSize.Height,
                             Stroke = Brushes.Red,
                             StrokeThickness = 2,
                             StrokeDashCap = PenLineCap.Round,
                             StrokeDashArray = new DoubleCollection(new double[] {1, 3}),
                         };

            var animation = new DoubleAnimation(0.3, 0.6, new Duration(TimeSpan.FromSeconds(0.5)))
                                            {
                                                AutoReverse = true,
                                                RepeatBehavior = RepeatBehavior.Forever
                                            };
            m_brush.BeginAnimation(Brush.OpacityProperty, animation);

            //_child.Fill = _brush;
        }

        void AdornedElementSizeChanged(object sender, SizeChangedEventArgs e)
        {
            m_child.Width = e.NewSize.Width;
            m_child.Height= e.NewSize.Height;
        }
		

        // A common way to implement an adorner's rendering behavior is to override the OnRender
        // method, which is called by the layout subsystem as part of a rendering pass.
        protected override void OnRender(DrawingContext drawingContext)
        {
            // Get a rectangle that represents the desired size of the rendered element
            // after the rendering pass.  This will be used to draw at the corners of the 
            // adorned element.
           /* Rect adornedElementRect = new Rect(this.AdornedElement.DesiredSize);

            // Some arbitrary drawing implements.
            SolidColorBrush renderBrush = new SolidColorBrush(Colors.Green);
            renderBrush.Opacity = 0.2;
            Pen renderPen = new Pen(new SolidColorBrush(Colors.Navy), 1.5);
            double renderRadius = 5.0;

            // Just draw a circle at each corner.
            drawingContext.DrawRectangle(renderBrush, renderPen, adornedElementRect);
            */
            /*drawingContext.DrawEllipse(renderBrush, renderPen, adornedElementRect.TopLeft, renderRadius, renderRadius);
            drawingContext.DrawEllipse(renderBrush, renderPen, adornedElementRect.TopRight, renderRadius, renderRadius);
            drawingContext.DrawEllipse(renderBrush, renderPen, adornedElementRect.BottomLeft, renderRadius, renderRadius);
            drawingContext.DrawEllipse(renderBrush, renderPen, adornedElementRect.BottomRight, renderRadius, renderRadius);*/
        }

        protected override Size MeasureOverride(Size constraint)
        {
            m_child.Measure(constraint);
            return m_child.DesiredSize;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            m_child.Arrange(new Rect(finalSize));
            return finalSize;
        }

        protected override Visual GetVisualChild(int index)
        {
            return m_child;
        }

        protected override int VisualChildrenCount
        {
            get
            {
                return 1;
            }
        }

        public double LeftOffset
        {
            get
            {
                return m_leftOffset;
            }
            set
            {
                m_leftOffset = value;
                UpdatePosition();
            }
        }

        public double TopOffset
        {
            get
            {
                return m_topOffset;
            }
            set
            {
                m_topOffset = value;
                UpdatePosition();

            }
        }

        private void UpdatePosition()
        {
            var adornerLayer = Parent as AdornerLayer;
            if (adornerLayer != null)
            {
                adornerLayer.Update(AdornedElement);
            }
        }

        public override GeneralTransform GetDesiredTransform(GeneralTransform transform)
        {
            var result = new GeneralTransformGroup();
            result.Children.Add(base.GetDesiredTransform(transform));
            result.Children.Add(new TranslateTransform(m_leftOffset, m_topOffset));
            return result;
        }

        private readonly Rectangle m_child;
        private double m_leftOffset;
        private double m_topOffset;
    }
}
