using System;
using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using BluePrintAssembler.Domain;
using BluePrintAssembler.Utils;
using Brush = System.Drawing.Brush;

namespace BluePrintAssembler.UI.Parts
{
    /// <summary>
    /// Interaction logic for IconWithText.xaml
    /// </summary>
    public partial class IconWithText : UserControl
    {
        private static ImageBrush _bg;

        public static ImageBrush BackgroundBrush => _bg ?? (_bg = new ImageBrush(Imaging.CreateBitmapSourceFromHBitmap(Configuration.Instance.StdIconSlot.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions())));

        public IconWithText()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty IconProperty = DependencyProperty.Register(
            "Icon", typeof(NotifyTaskCompletion<Bitmap>), typeof(IconWithText), new PropertyMetadata(default(NotifyTaskCompletion<Bitmap>)));

        public NotifyTaskCompletion<Bitmap> Icon
        {
            get { return (NotifyTaskCompletion<Bitmap>) GetValue(IconProperty); }
            set { SetValue(IconProperty, value); }
        }

        public static readonly DependencyProperty AmountProperty = DependencyProperty.Register(
            "Amount", typeof(float?), typeof(IconWithText), new PropertyMetadata(default(float?)));

        public float? Amount
        {
            get { return (float?) GetValue(AmountProperty); }
            set { SetValue(AmountProperty, value); }
        }

        public static readonly DependencyProperty NamedObjectProperty = DependencyProperty.Register(
            "NamedObject", typeof(INamed), typeof(IconWithText), new PropertyMetadata(default(INamed)));

        public INamed NamedObject
        {
            get { return (INamed) GetValue(NamedObjectProperty); }
            set { SetValue(NamedObjectProperty, value); }
        }

        public static readonly DependencyProperty AmountEditableProperty = DependencyProperty.Register(
            "AmountEditable", typeof(bool), typeof(IconWithText), new PropertyMetadata(default(bool)));

        public bool AmountEditable
        {
            get { return (bool) GetValue(AmountEditableProperty); }
            set { SetValue(AmountEditableProperty, value); }
        }

        public static readonly DependencyProperty IsEditedProperty = DependencyProperty.Register(
            "IsEdited", typeof(bool), typeof(IconWithText), new PropertyMetadata(default(bool)));

        public bool IsEdited
        {
            get { return (bool) GetValue(IsEditedProperty); }
            set { SetValue(IsEditedProperty, value); }
        }
        public bool IsNotEdited
        {
            get { return !(bool)GetValue(IsEditedProperty); }
            set { SetValue(IsEditedProperty, !value); }
        }

        private void UIElement_OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (AmountEditable)
            {
                IsEdited = true;
                AmountTextBox.Focus();
            }
        }

        private void AmountTextBox_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                //AmountTextBox.BindingGroup.UpdateSources();
                IsEdited = false;
            }
        }

        private void AmountTextBox_OnLostFocus(object sender, RoutedEventArgs e)
        {
            //AmountTextBox.BindingGroup.UpdateSources();
            IsEdited = false;
        }
    }
}
