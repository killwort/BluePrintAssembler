﻿using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using BluePrintAssembler.Domain;
using BluePrintAssembler.Utils;

namespace BluePrintAssembler.UI.Parts
{
    /// <summary>
    /// Interaction logic for IconWithText.xaml
    /// </summary>
    public partial class IconWithText : UserControl
    {
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
    }
}