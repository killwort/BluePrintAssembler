using System;
using System.Runtime.Versioning;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using BluePrintAssembler.UI.Parts;
using BluePrintAssembler.UI.VM;
using BluePrintAssembler.Utils;

namespace BluePrintAssembler
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool _isMouseDown;
        private Point _dragStartPoint;
        private FrameworkElement _draggedDeviceVisual;
        private bool _isDragging;
        private double _dragStartLeft;
        private double _dragStartTop;
        private SelectionAdorner _overlayElement;
        private object _updaterLock=new object();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ((UI.VM.MainWindow)DataContext).CurrentWorkspace.Test();
        }

        private void DrawingAreaPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var vsender = (FrameworkElement)MyVisualTreeHelper.GetParentWithDataContext<ISelectableElement>((FrameworkElement) e.OriginalSource);
            if (vsender == null)
            {
                SelectElement(null);
                return;
            }
            /*if (vsender == null)
            {
                var psender = MyVisualTreeHelper.GetParent<ParameterVisual>((FrameworkElement)e.OriginalSource);
                if (psender == null)
                {
                    SelectElement(null);
                    return;
                }
                if(m_connectOrigin!=null)
                {
                    Connect(psender,m_connectOrigin);
                    SelectElement(null);
                    return;
                }
                psender.IsSelected = true;
                e.Handled = true;
                SelectElement(null);
                m_connectOrigin = psender;
                return;
            }*/

            var canvas = MyVisualTreeHelper.GetParent<ItemsControl>((FrameworkElement)e.Source);
            _isMouseDown = true;
            _dragStartPoint = e.GetPosition(canvas);
            _draggedDeviceVisual = vsender;
            SelectElement(vsender);
            canvas.CaptureMouse();
            e.Handled = true;

        }

        private void SelectElement(FrameworkElement element)
        {
            if (_overlayElement != null)
            {
                var l = AdornerLayer.GetAdornerLayer(_overlayElement.AdornedElement);
                if (l != null)
                    l.Remove(_overlayElement);
            }
            if (element != null)
            {
                //ParametersPanel.DataContext = element.DataContext;
                _overlayElement = new SelectionAdorner(element);
                _overlayElement.MouseLeftButtonDown += OverlayElementMouseLeftButtonDown;
                var layer = AdornerLayer.GetAdornerLayer(element);
                if (layer != null)
                    layer.Add(_overlayElement);
            }
            else
            {
                //ParametersPanel.DataContext = null;
                _overlayElement = null;
                /*if (m_connectOrigin != null)
                    m_connectOrigin.IsSelected = false;
                m_connectOrigin = null;*/
            }

        }

        private void OverlayElementMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _isMouseDown = true;
            //m_originalElement = m_overlayElement.AdornedElement;
            var canvas = MyVisualTreeHelper.GetParent<Canvas>(_draggedDeviceVisual);
            _dragStartPoint = e.GetPosition(canvas);
            canvas.CaptureMouse();
            e.Handled = true;
        }

        private void DragMoved(ItemsControl canvas)
        {
            var currentPosition = Mouse.GetPosition(canvas);

            _overlayElement.LeftOffset = currentPosition.X - _dragStartPoint.X;
            _overlayElement.TopOffset = currentPosition.Y - _dragStartPoint.Y;
        }

        private void DragStarted()
        {
            _isDragging = true;
            var ui = MyVisualTreeHelper.GetParentDataContext<DraggableElement>(_draggedDeviceVisual);
            _dragStartLeft = ui.Left;
            _dragStartTop = ui.Top;

            SelectElement(_draggedDeviceVisual);
            _overlayElement.IsDragging = true;

        }
        
        private void DragFinished(bool cancelled)
        {
            Mouse.Capture(null);
            lock (_updaterLock)
            {
                if (_isDragging)
                {
                    if (cancelled == false)
                    {
                        var ui = MyVisualTreeHelper.GetParentDataContext<DraggableElement>(_draggedDeviceVisual);
                        ui.Top = _dragStartTop + _overlayElement.TopOffset;
                        ui.Left= _dragStartLeft+ _overlayElement.LeftOffset;
                    }
                    SelectElement(_draggedDeviceVisual);
                    _overlayElement.IsDragging = false;
                    //ThreadPool.QueueUserWorkItem(UpdateConnections);
                }
                _isDragging = false;
                _isMouseDown = false;
            }

        }
        private void DrawingAreaPreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
                        if (_isMouseDown)
                _isMouseDown = false;
            if (_isDragging)
            {
                DragFinished(false);
                e.Handled = true;
            }/*else
            {
                if(CreateDevice(e.GetPosition((FrameworkElement)e.Source)))
                {
                    e.Handled = true;
                    SelectionTool.IsChecked = true;
                }
            }*/
            Mouse.Capture(null);

        }


        private void DrawingAreaPreviewMouseMove(object sender, MouseEventArgs e)
        {
                        if (_isMouseDown)
            {
                var canvas = MyVisualTreeHelper.GetParent<ItemsControl>((FrameworkElement)e.Source);
                if ((_isDragging == false) && ((Math.Abs(e.GetPosition(canvas).X - _dragStartPoint.X) > SystemParameters.MinimumHorizontalDragDistance) ||
                    (Math.Abs(e.GetPosition(canvas).Y - _dragStartPoint.Y) > SystemParameters.MinimumVerticalDragDistance)))
                {
                    DragStarted();
                }
                if (_isDragging)
                {
                    DragMoved(canvas);
                }
            }

        }


    }
}
