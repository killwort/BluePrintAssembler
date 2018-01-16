using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using BluePrintAssembler.UI.Parts;
using BluePrintAssembler.UI.VM;
using BluePrintAssembler.Utils;
using GraphSharp;
using GraphSharp.Algorithms.Layout.Simple.Hierarchical;
using Microsoft.Win32;
using Newtonsoft.Json;

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
        private object _updaterLock = new object();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ((UI.VM.MainWindow) DataContext).CurrentWorkspace.TestAddItem();
        }

        private void DrawingAreaPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var vsender = MyVisualTreeHelper.GetParent(((FrameworkElement) e.OriginalSource), x => x.Focusable, x => x.DataContext is ISelectableElement);
            if (vsender == null)
            {
                SelectElement(null);
                return;
            }

            if (vsender.Focusable) return;
            var canvas = MyVisualTreeHelper.GetParent<ItemsControl>((FrameworkElement) e.Source);
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
            //var ui =(UIElement) MyVisualTreeHelper.GetParentWithDataContext<DraggableElement>(_draggedDeviceVisual);
            var ui = (UIElement) MyVisualTreeHelper.GetParent<ContentPresenter>(_draggedDeviceVisual);
            //var container = MyVisualTreeHelper.GetParent<DynamicCanvas>(_draggedDeviceVisual);
            _dragStartLeft = DynamicCanvas.GetLeft(ui);
            //ui.Left;
            _dragStartTop = DynamicCanvas.GetTop(ui);
            //ui.Top;

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
                        //var ui = (UIElement)MyVisualTreeHelper.GetParentWithDataContext<DraggableElement>(_draggedDeviceVisual);
                        var ui = (UIElement) MyVisualTreeHelper.GetParent<ContentPresenter>(_draggedDeviceVisual);
                        DynamicCanvas.SetTop(ui, _dragStartTop + _overlayElement.TopOffset);
                        DynamicCanvas.SetLeft(ui, _dragStartLeft + _overlayElement.LeftOffset);
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
            var vsender = MyVisualTreeHelper.GetParent(((FrameworkElement) e.OriginalSource), x => x.Focusable, x => x.DataContext is ISelectableElement);
            if (vsender.Focusable && !_isDragging) return;

            if (_isMouseDown)
                _isMouseDown = false;
            if (_isDragging)
            {
                DragFinished(false);
                e.Handled = true;
            } /*else
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
                var canvas = MyVisualTreeHelper.GetParent<ItemsControl>((FrameworkElement) e.Source);
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


        private void AutoLayout_Click(object sender, RoutedEventArgs e)
        {
            var w = ((UI.VM.MainWindow) DataContext).CurrentWorkspace;
            if (w.VertexCount < 3)
            {
                var off = new Point(0, 0);
                foreach (var vertex in w.Vertices)
                {
                    var ui = (UIElement) DrawingArea.ItemContainerGenerator.ContainerFromItem(vertex);
                    DynamicCanvas.SetLeft(ui, off.X);
                    DynamicCanvas.SetTop(ui, off.Y);
                    off.X += ui.DesiredSize.Width + 100;
                    off.Y += ui.DesiredSize.Height + 100;
                }
            }
            else
            {
                var sizes = w.Vertices.ToDictionary(x => x, x => ((UIElement) DrawingArea.ItemContainerGenerator.ContainerFromItem(x))?.DesiredSize ?? new Size(0, 0));
                //var alg = new EfficientSugiyamaLayoutAlgorithm<IGraphNode, UI.VM.ProducibleItem, Workspace>(w, new EfficientSugiyamaLayoutParameters{VertexDistance = 100,PositionMode = 1}, new Dictionary<IGraphNode, Point>(), sizes);
                var alg = new SugiyamaLayoutAlgorithm<IGraphNode, UI.VM.ProducibleItem, Workspace>(w, sizes, new Dictionary<IGraphNode, Point>(), new SugiyamaLayoutParameters {HorizontalGap = 50, VerticalGap = 50}, edge => EdgeTypes.Hierarchical);
                alg.Compute();
                var offX = 0.0;
                var offY = 0.0;
                foreach (var p in alg.VertexPositions)
                {
                    if (p.Value.X < offX) offX = p.Value.X;
                    if (p.Value.Y < offY) offY = p.Value.Y;
                }

                foreach (var p in alg.VertexPositions)
                {
                    var ui = (UIElement) DrawingArea.ItemContainerGenerator.ContainerFromItem(p.Key);
                    DynamicCanvas.SetLeft(ui, p.Value.X - offX);
                    DynamicCanvas.SetTop(ui, p.Value.Y - offY);
                }
            }

            //MyVisualTreeHelper.GetChild<DynamicCanvas>(DrawingArea).AutoLayout();
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            var jss=new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                TypeNameHandling = TypeNameHandling.Auto
            };
            var dialog=new SaveFileDialog();
            dialog.Filter = BluePrintAssembler.Resources.MainWindow.FileFilter;
            if (dialog.ShowDialog(this) ?? false)
                File.WriteAllText(dialog.FileName, JsonConvert.SerializeObject(DataContext,jss));
        }
        private void Load_Click(object sender, RoutedEventArgs e)
        {
            var jss = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                TypeNameHandling = TypeNameHandling.Auto
            };
            var dialog=new OpenFileDialog();
            dialog.Filter = BluePrintAssembler.Resources.MainWindow.FileFilter;
            if (dialog.ShowDialog(this) ?? false)
                DataContext = JsonConvert.DeserializeObject<UI.VM.MainWindow>(File.ReadAllText(dialog.FileName), jss);
        }

        private void AddResult_Click(object sender, RoutedEventArgs e)
        {
            var dialog=new UI.SelectItem();
            if (dialog.ShowDialog() ?? false)
                ((UI.VM.MainWindow) DataContext).CurrentWorkspace.WantedResults.Add(((UI.VM.SelectItem) dialog.DataContext).SelectedItem);
        }

        private void AddSource_Click(object sender, RoutedEventArgs e)
        {
            var dialog=new UI.SelectItem();
            if (dialog.ShowDialog() ?? false)
                ((UI.VM.MainWindow) DataContext).CurrentWorkspace.ExistingSources.Add(((UI.VM.SelectItem) dialog.DataContext).SelectedItem);
        }
    }
}