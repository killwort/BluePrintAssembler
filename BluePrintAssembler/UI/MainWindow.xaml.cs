using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using BluePrintAssembler.UI.Parts;
using BluePrintAssembler.UI.VM;
using BluePrintAssembler.Utils;
using GraphSharp;
using GraphSharp.Algorithms.Layout.Simple.Hierarchical;
using GraphSharp.Algorithms.Layout.Simple.Tree;
using Microsoft.Win32;
using Newtonsoft.Json;
using ProducibleItem = BluePrintAssembler.UI.VM.ProducibleItem;

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
        private readonly object _updaterLock = new object();

        public MainWindow()
        {
            InitializeComponent();
            ((UI.VM.MainWindow)DataContext).CurrentWorkspace.FlowChanged += Relayout;
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
                AdornerLayer.GetAdornerLayer(_overlayElement.AdornedElement)?.Remove(_overlayElement);

            if (element != null)
            {
                _overlayElement = new SelectionAdorner(element);
                _overlayElement.MouseLeftButtonDown += OverlayElementMouseLeftButtonDown;
                AdornerLayer.GetAdornerLayer(element)?.Add(_overlayElement);
            }
            else
            {
                _overlayElement = null;
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
            var ui = (UIElement) MyVisualTreeHelper.GetParent<ContentPresenter>(_draggedDeviceVisual);
            _dragStartLeft = DynamicCanvas.GetLeft(ui);
            _dragStartTop = DynamicCanvas.GetTop(ui);
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
                        var ui = (UIElement) MyVisualTreeHelper.GetParent<ContentPresenter>(_draggedDeviceVisual);
                        DynamicCanvas.SetTop(ui, _dragStartTop + _overlayElement.TopOffset);
                        DynamicCanvas.SetLeft(ui, _dragStartLeft + _overlayElement.LeftOffset);
                    }

                    SelectElement(_draggedDeviceVisual);
                    _overlayElement.IsDragging = false;
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
            } 
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


        private void Relayout(object sender, EventArgs e)
        {
            Task.Run(() =>
            {
                Task.Delay(500).Wait();
                Dispatcher.Invoke(() =>
                {
                    var w = ((UI.VM.MainWindow) DataContext).CurrentWorkspace;

                    var sizes = w.Vertices.ToDictionary(x => x, x => ((UIElement) DrawingArea.ItemContainerGenerator.ContainerFromItem(x))?.DesiredSize ?? new Size(0, 0));
                    GraphSharp.Algorithms.Layout.ILayoutAlgorithm<IGraphNode, ProducibleItem, Workspace> alg;
                    if (w.VertexCount < 3)
                    {
                        alg = new SimpleTreeLayoutAlgorithm<IGraphNode, ProducibleItem, Workspace>(w, new Dictionary<IGraphNode, Point>(), sizes, new SimpleTreeLayoutParameters {VertexGap = 50});
                    }
                    else
                    {
                        alg = new SugiyamaLayoutAlgorithm<IGraphNode, UI.VM.ProducibleItem, Workspace>(w, sizes, new Dictionary<IGraphNode, Point>(), new SugiyamaLayoutParameters {HorizontalGap = 50, VerticalGap = 50}, edge => EdgeTypes.Hierarchical);
                    }

                    //var alg = new EfficientSugiyamaLayoutAlgorithm<IGraphNode, UI.VM.ProducibleItem, Workspace>(w, new EfficientSugiyamaLayoutParameters{VertexDistance = 100,PositionMode = 1}, new Dictionary<IGraphNode, Point>(), sizes);
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
                        var n = p.Key as BaseFlowNode;
                        if (n == null) continue;
                        n.LayoutLeft = p.Value.X - offX;
                        n.LayoutTop = p.Value.Y - offY;
                        /*var ui = (UIElement) DrawingArea.ItemContainerGenerator.ContainerFromItem(p.Key);
                        DynamicCanvas.SetLeft(ui, p.Value.X - offX);
                        DynamicCanvas.SetTop(ui, p.Value.Y - offY);*/

                    }
                });
            });
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            var jss=new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                TypeNameHandling = TypeNameHandling.Auto
            };
            var dialog = new SaveFileDialog {Filter = BluePrintAssembler.Resources.MainWindow.FileFilter};
            if ((bool) dialog.ShowDialog(this))
                File.WriteAllText(dialog.FileName, JsonConvert.SerializeObject(DataContext,jss));
        }
        private void Load_Click(object sender, RoutedEventArgs e)
        {
            var jss = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                TypeNameHandling = TypeNameHandling.Auto
            };
            var dialog = new OpenFileDialog {Filter = BluePrintAssembler.Resources.MainWindow.FileFilter};
            if ((bool) dialog.ShowDialog(this))
            {
                DataContext = JsonConvert.DeserializeObject<UI.VM.MainWindow>(File.ReadAllText(dialog.FileName), jss);
                ((UI.VM.MainWindow)DataContext).CurrentWorkspace.FlowChanged += Relayout;
            }
        }

        private void AddResult_Click(object sender, RoutedEventArgs e)
        {
            var dialog=new UI.SelectItem();
            if (dialog.ShowDialog() ?? false)
                ((UI.VM.MainWindow) DataContext).CurrentWorkspace.WantedResults.Add(((SelectItem) dialog.DataContext).SelectedItem);
        }

        private void AddSource_Click(object sender, RoutedEventArgs e)
        {
            var dialog=new UI.SelectItem();
            if (dialog.ShowDialog() ?? false)
                ((UI.VM.MainWindow) DataContext).CurrentWorkspace.ExistingSources.Add(((SelectItem) dialog.DataContext).SelectedItem);
        }
    }
}