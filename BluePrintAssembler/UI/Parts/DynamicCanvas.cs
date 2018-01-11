﻿using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using BluePrintAssembler.Utils;

namespace BluePrintAssembler.UI.Parts
{
// ReSharper disable ClassWithVirtualMembersNeverInherited.Global
        public class DynamicCanvas : Panel
// ReSharper restore ClassWithVirtualMembersNeverInherited.Global
        {
            #region public bool SizeWidthToContent
            /// <summary>
            /// Gets or sets a value indicating whether the dynamic canvas should
            /// size its width to its content.
            /// </summary>
            public bool SizeWidthToContent
            {
                get { return (bool)GetValue(SizeWidthToContentProperty); }
                set { SetValue(SizeWidthToContentProperty, value); }
            }

            /// <summary>
            /// Identifies the SizeWidthToContent dependency property.
            /// </summary>
            public static readonly DependencyProperty SizeWidthToContentProperty =
                DependencyProperty.Register(
                    "SizeWidthToContent",
                    typeof(bool),
                    typeof(DynamicCanvas),
                    new PropertyMetadata(false, OnSizeWidthToContentPropertyChanged));

            /// <summary>
            /// SizeWidthToContentProperty property changed handler.
            /// </summary>
            /// <param name="d">DynamicCanvas that changed its SizeWidthToContent.</param>
            /// <param name="e">Event arguments.</param>
            private static void OnSizeWidthToContentPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
            {
                var source = (DynamicCanvas)d;
                var oldValue = (bool)e.OldValue;
                var newValue = (bool)e.NewValue;
                source.OnSizeWidthToContentPropertyChanged(oldValue, newValue);
            }

            /// <summary>
            /// SizeWidthToContentProperty property changed handler.
            /// </summary>
            /// <param name="oldValue">Old value.</param>
            /// <param name="newValue">New value.</param>        
// ReSharper disable UnusedParameter.Global
            protected virtual void OnSizeWidthToContentPropertyChanged(bool oldValue, bool newValue)
// ReSharper restore UnusedParameter.Global
            {
                Invalidate();
            }
            #endregion public bool SizeWidthToContent

            #region public bool SizeHeightToContent
            /// <summary>
            /// Gets or sets a value indicating whether the canvas should size its
            /// height to its content.
            /// </summary>
            public bool SizeHeightToContent
            {
                get { return (bool)GetValue(SizeHeightToContentProperty); }
                set { SetValue(SizeHeightToContentProperty, value); }
            }

            /// <summary>
            /// Identifies the SizeHeightToContent dependency property.
            /// </summary>
            public static readonly DependencyProperty SizeHeightToContentProperty =
                DependencyProperty.Register(
                    "SizeHeightToContent",
                    typeof(bool),
                    typeof(DynamicCanvas),
                    new PropertyMetadata(false, OnSizeHeightToContentPropertyChanged));

            /// <summary>
            /// SizeHeightToContentProperty property changed handler.
            /// </summary>
            /// <param name="d">DynamicCanvas that changed its SizeHeightToContent.</param>
            /// <param name="e">Event arguments.</param>
            private static void OnSizeHeightToContentPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
            {
                var source = (DynamicCanvas)d;
                var oldValue = (bool)e.OldValue;
                var newValue = (bool)e.NewValue;
                source.OnSizeHeightToContentPropertyChanged(oldValue, newValue);
            }

            /// <summary>
            /// SizeHeightToContentProperty property changed handler.
            /// </summary>
            /// <param name="oldValue">Old value.</param>
            /// <param name="newValue">New value.</param>        
// ReSharper disable UnusedParameter.Global
            protected virtual void OnSizeHeightToContentPropertyChanged(bool oldValue, bool newValue)
// ReSharper restore UnusedParameter.Global
            {
                Invalidate();
            }
            #endregion public bool SizeHeightToContent

            #region public attached double Bottom
            /// <summary>
            /// Gets the value of the Bottom attached property for a specified UIElement.
            /// </summary>
            /// <param name="element">The UIElement from which the property value is read.</param>
            /// <returns>The Bottom property value for the UIElement.</returns>
            public static double GetBottom(UIElement element)
            {
                if (element == null)
                {
                    throw new ArgumentNullException("element");
                }
                return (double)element.GetValue(BottomProperty);
            }

            /// <summary>
            /// Sets the value of the Bottom attached property to a specified UIElement.
            /// </summary>
            /// <param name="element">The UIElement to which the attached property is written.</param>
            /// <param name="value">The needed Bottom value.</param>
            public static void SetBottom(UIElement element, double value)
            {
                if (element == null)
                {
                    throw new ArgumentNullException("element");
                }
                element.SetValue(BottomProperty, value);
            }

            /// <summary>
            /// Identifies the Bottom dependency property.
            /// </summary>
            public static readonly DependencyProperty BottomProperty =
                DependencyProperty.RegisterAttached(
                    "Bottom",
                    typeof(double),
                    typeof(DynamicCanvas),
                    new PropertyMetadata(double.NaN, OnBottomPropertyChanged));

            /// <summary>
            /// BottomProperty property changed handler.
            /// </summary>
            /// <param name="dependencyObject">UIElement that changed its Bottom.</param>
            /// <param name="eventArgs">Event arguments.</param>
            public static void OnBottomPropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs eventArgs)
            {
                var source = dependencyObject as UIElement;
                if (source == null)
                {
                    throw new ArgumentException("dependencyObject");
                }
                var parent = VisualTreeHelper.GetParent(source) as DynamicCanvas;
                if (parent != null)
                {
                    parent.Invalidate();
                }
            }
            #endregion public attached double Bottom

            #region public attached double Left
            /// <summary>
            /// Gets the value of the Left attached property for a specified UIElement.
            /// </summary>
            /// <param name="element">The UIElement from which the property value is read.</param>
            /// <returns>The Left property value for the UIElement.</returns>
            public static double GetLeft(UIElement element)
            {
                if (element == null)
                {
                    throw new ArgumentNullException("element");
                }
                return (double)element.GetValue(LeftProperty);
            }

            /// <summary>
            /// Sets the value of the Left attached property to a specified UIElement.
            /// </summary>
            /// <param name="element">The UIElement to which the attached property is written.</param>
            /// <param name="value">The needed Left value.</param>
            public static void SetLeft(UIElement element, double value)
            {
                if (element == null)
                {
                    throw new ArgumentNullException("element");
                }
                element.SetValue(LeftProperty, value);
            }

            /// <summary>
            /// Identifies the Left dependency property.
            /// </summary>
            public static readonly DependencyProperty LeftProperty =
                DependencyProperty.RegisterAttached(
                    "Left",
                    typeof(double),
                    typeof(DynamicCanvas),
                    new PropertyMetadata(double.NaN, OnLeftPropertyChanged));

            /// <summary>
            /// LeftProperty property changed handler.
            /// </summary>
            /// <param name="dependencyObject">UIElement that changed its Left.</param>
            /// <param name="eventArgs">Event arguments.</param>
            public static void OnLeftPropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs eventArgs)
            {
                var source = dependencyObject as UIElement;
                if (source == null)
                {
                    throw new ArgumentException("dependencyObject");
                }
                var parent = VisualTreeHelper.GetParent(source) as DynamicCanvas;
                if (parent != null)
                {
                    parent.Invalidate();
                }
            }
            #endregion public attached double Left

            #region public attached double Right
            /// <summary>
            /// Gets the value of the Right attached property for a specified UIElement.
            /// </summary>
            /// <param name="element">The UIElement from which the property value is read.</param>
            /// <returns>The Right property value for the UIElement.</returns>
            public static double GetRight(UIElement element)
            {
                if (element == null)
                {
                    throw new ArgumentNullException("element");
                }
                return (double)element.GetValue(RightProperty);
            }

            /// <summary>
            /// Sets the value of the Right attached property to a specified UIElement.
            /// </summary>
            /// <param name="element">The UIElement to which the attached property is written.</param>
            /// <param name="value">The needed Right value.</param>
            public static void SetRight(UIElement element, double value)
            {
                if (element == null)
                {
                    throw new ArgumentNullException("element");
                }
                element.SetValue(RightProperty, value);
            }

            /// <summary>
            /// Identifies the Right dependency property.
            /// </summary>
            public static readonly DependencyProperty RightProperty =
                DependencyProperty.RegisterAttached(
                    "Right",
                    typeof(double),
                    typeof(DynamicCanvas),
                    new PropertyMetadata(double.NaN, OnRightPropertyChanged));

            /// <summary>
            /// RightProperty property changed handler.
            /// </summary>
            /// <param name="dependencyObject">UIElement that changed its Right.</param>
            /// <param name="eventArgs">Event arguments.</param>
            public static void OnRightPropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs eventArgs)
            {
                var source = dependencyObject as UIElement;
                if (source == null)
                {
                    throw new ArgumentException("dependencyObject");
                }
                var parent = VisualTreeHelper.GetParent(source) as DynamicCanvas;
                if (parent != null)
                {
                    parent.Invalidate();
                }
            }
            #endregion public attached double Right

            #region public attached double Top
            /// <summary>
            /// Gets the value of the Top attached property for a specified UIElement.
            /// </summary>
            /// <param name="element">The UIElement from which the property value is read.</param>
            /// <returns>The Top property value for the UIElement.</returns>
            public static double GetTop(UIElement element)
            {
                if (element == null)
                {
                    throw new ArgumentNullException("element");
                }
                return (double)element.GetValue(TopProperty);
            }

            /// <summary>
            /// Sets the value of the Top attached property to a specified UIElement.
            /// </summary>
            /// <param name="element">The UIElement to which the attached property is written.</param>
            /// <param name="value">The needed Top value.</param>
            public static void SetTop(UIElement element, double value)
            {
                if (element == null)
                {
                    throw new ArgumentNullException("element");
                }
                element.SetValue(TopProperty, value);
            }

            /// <summary>
            /// Identifies the Top dependency property.
            /// </summary>
            public static readonly DependencyProperty TopProperty =
                DependencyProperty.RegisterAttached(
                    "Top",
                    typeof(double),
                    typeof(DynamicCanvas),
                    new PropertyMetadata(double.NaN, OnTopPropertyChanged));

            /// <summary>
            /// TopProperty property changed handler.
            /// </summary>
            /// <param name="dependencyObject">UIElement that changed its Top.</param>
            /// <param name="eventArgs">Event arguments.</param>
            public static void OnTopPropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs eventArgs)
            {
                var source = dependencyObject as UIElement;
                if (source == null)
                {
                    throw new ArgumentException("dependencyObject");
                }
                var parent = VisualTreeHelper.GetParent(source) as DynamicCanvas;
                if (parent != null)
                {
                    parent.Invalidate();
                }
            }
            #endregion public attached double Top

            #region public attached double CenterBottom
            /// <summary>
            /// Gets the value of the CenterBottom attached property for a specified UIElement.
            /// </summary>
            /// <param name="element">The UIElement from which the property value is read.</param>
            /// <returns>The CenterBottom property value for the UIElement.</returns>
            public static double GetCenterBottom(UIElement element)
            {
                if (element == null)
                {
                    throw new ArgumentNullException("element");
                }
                return (double)element.GetValue(CenterBottomProperty);
            }

            /// <summary>
            /// Sets the value of the CenterBottom attached property to a specified UIElement.
            /// </summary>
            /// <param name="element">The UIElement to which the attached property is written.</param>
            /// <param name="value">The needed CenterBottom value.</param>
            public static void SetCenterBottom(UIElement element, double value)
            {
                if (element == null)
                {
                    throw new ArgumentNullException("element");
                }
                element.SetValue(CenterBottomProperty, value);
            }

            /// <summary>
            /// Identifies the CenterBottom dependency property.
            /// </summary>
            public static readonly DependencyProperty CenterBottomProperty =
                DependencyProperty.RegisterAttached(
                    "CenterBottom",
                    typeof(double),
                    typeof(DynamicCanvas),
                    new PropertyMetadata(double.NaN, OnCenterBottomPropertyChanged));

            /// <summary>
            /// CenterBottomProperty property changed handler.
            /// </summary>
            /// <param name="dependencyObject">UIElement that changed its CenterBottom.</param>
            /// <param name="eventArgs">Event arguments.</param>
            public static void OnCenterBottomPropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs eventArgs)
            {
                var source = dependencyObject as UIElement;
                if (source == null)
                {
                    throw new ArgumentException("dependencyObject");
                }
                var parent = VisualTreeHelper.GetParent(source) as DynamicCanvas;
                if (parent != null)
                {
                    parent.Invalidate();
                }
            }
            #endregion public attached double CenterBottom

            #region public attached double CenterLeft
            /// <summary>
            /// Gets the value of the CenterLeft attached property for a specified UIElement.
            /// </summary>
            /// <param name="element">The UIElement from which the property value is read.</param>
            /// <returns>The CenterLeft property value for the UIElement.</returns>
            public static double GetCenterLeft(UIElement element)
            {
                if (element == null)
                {
                    throw new ArgumentNullException("element");
                }
                return (double)element.GetValue(CenterLeftProperty);
            }

            /// <summary>
            /// Sets the value of the CenterLeft attached property to a specified UIElement.
            /// </summary>
            /// <param name="element">The UIElement to which the attached property is written.</param>
            /// <param name="value">The needed CenterLeft value.</param>
            public static void SetCenterLeft(UIElement element, double value)
            {
                if (element == null)
                {
                    throw new ArgumentNullException("element");
                }
                element.SetValue(CenterLeftProperty, value);
            }

            /// <summary>
            /// Identifies the CenterLeft dependency property.
            /// </summary>
            public static readonly DependencyProperty CenterLeftProperty =
                DependencyProperty.RegisterAttached(
                    "CenterLeft",
                    typeof(double),
                    typeof(DynamicCanvas),
                    new PropertyMetadata(double.NaN, OnCenterLeftPropertyChanged));

            /// <summary>
            /// CenterLeftProperty property changed handler.
            /// </summary>
            /// <param name="dependencyObject">UIElement that changed its CenterLeft.</param>
            /// <param name="eventArgs">Event arguments.</param>
            public static void OnCenterLeftPropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs eventArgs)
            {
                var source = dependencyObject as UIElement;
                if (source == null)
                {
                    throw new ArgumentException("dependencyObject");
                }
                var parent = VisualTreeHelper.GetParent(source) as DynamicCanvas;
                if (parent != null)
                {
                    parent.Invalidate();
                }
            }
            #endregion public attached double CenterLeft

            #region public attached double CenterRight
            /// <summary>
            /// Gets the value of the CenterRight attached property for a specified UIElement.
            /// </summary>
            /// <param name="element">The UIElement from which the property value is read.</param>
            /// <returns>The CenterRight property value for the UIElement.</returns>
            public static double GetCenterRight(UIElement element)
            {
                if (element == null)
                {
                    throw new ArgumentNullException("element");
                }
                return (double)element.GetValue(CenterRightProperty);
            }

            /// <summary>
            /// Sets the value of the CenterRight attached property to a specified UIElement.
            /// </summary>
            /// <param name="element">The UIElement to which the attached property is written.</param>
            /// <param name="value">The needed CenterRight value.</param>
            public static void SetCenterRight(UIElement element, double value)
            {
                if (element == null)
                {
                    throw new ArgumentNullException("element");
                }
                element.SetValue(CenterRightProperty, value);
            }

            /// <summary>
            /// Identifies the CenterRight dependency property.
            /// </summary>
            public static readonly DependencyProperty CenterRightProperty =
                DependencyProperty.RegisterAttached(
                    "CenterRight",
                    typeof(double),
                    typeof(DynamicCanvas),
                    new PropertyMetadata(double.NaN, OnCenterRightPropertyChanged));

            /// <summary>
            /// CenterRightProperty property changed handler.
            /// </summary>
            /// <param name="dependencyObject">UIElement that changed its CenterRight.</param>
            /// <param name="eventArgs">Event arguments.</param>
            public static void OnCenterRightPropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs eventArgs)
            {
                var source = dependencyObject as UIElement;
                if (source == null)
                {
                    throw new ArgumentException("dependencyObject");
                }
                var parent = VisualTreeHelper.GetParent(source) as DynamicCanvas;
                if (parent != null)
                {
                    parent.Invalidate();
                }
            }
            #endregion public attached double CenterRight

            #region public attached double CenterTop
            /// <summary>
            /// Gets the value of the CenterTop attached property for a specified UIElement.
            /// </summary>
            /// <param name="element">The UIElement from which the property value is read.</param>
            /// <returns>The CenterTop property value for the UIElement.</returns>
            public static double GetCenterTop(UIElement element)
            {
                if (element == null)
                {
                    throw new ArgumentNullException("element");
                }
                return (double)element.GetValue(CenterTopProperty);
            }

            /// <summary>
            /// Sets the value of the CenterTop attached property to a specified UIElement.
            /// </summary>
            /// <param name="element">The UIElement to which the attached property is written.</param>
            /// <param name="value">The needed CenterTop value.</param>
            public static void SetCenterTop(UIElement element, double value)
            {
                if (element == null)
                {
                    throw new ArgumentNullException("element");
                }
                element.SetValue(CenterTopProperty, value);
            }

            /// <summary>
            /// Identifies the CenterTop dependency property.
            /// </summary>
            public static readonly DependencyProperty CenterTopProperty =
                DependencyProperty.RegisterAttached(
                    "CenterTop",
                    typeof(double),
                    typeof(DynamicCanvas),
                    new PropertyMetadata(double.NaN, OnCenterTopPropertyChanged));

            /// <summary>
            /// CenterTopProperty property changed handler.
            /// </summary>
            /// <param name="dependencyObject">UIElement that changed its CenterTop.</param>
            /// <param name="eventArgs">Event arguments.</param>
            public static void OnCenterTopPropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs eventArgs)
            {
                var source = dependencyObject as UIElement;
                if (source == null)
                {
                    throw new ArgumentException("dependencyObject");
                }
                var parent = VisualTreeHelper.GetParent(source) as DynamicCanvas;
                if (parent != null)
                {
                    parent.Invalidate();
                }
            }
            #endregion public attached double CenterTop
            /// <summary>
            /// Invalidates the position of child elements.
            /// </summary>
            private void Invalidate()
            {
                if (SizeHeightToContent || SizeWidthToContent)
                {
                    InvalidateMeasure();
                }
                else
                {
                    InvalidateArrange();
                }
            }

            /// <summary>
            /// Measures all the children and returns their size.
            /// </summary>
            /// <param name="constraint">The available size.</param>
            /// <returns>The desired size.</returns>
            protected override Size MeasureOverride(Size constraint)
            {
                var availableSize = new Size(double.PositiveInfinity, double.PositiveInfinity);
                if (SizeHeightToContent || SizeWidthToContent)
                {
                    foreach (UIElement child in Children)
                    {
                        child.Measure(availableSize);
                    }

                    double maxWidth = 0;
                    if (SizeWidthToContent)
                    {

                        maxWidth =
                            Children
                                .Cast<UIElement>()
                                .Where(child => !double.IsNaN(GetLeft(child)))
                                .Select(child => GetLeft(child) + child.DesiredSize.Width)
                                .Concat(
                                    Children
                                        .Cast<UIElement>()
                                        .Where(child => !double.IsNaN(GetCenterLeft(child)))
                                        .Select(child => GetCenterLeft(child) + (child.DesiredSize.Width / 2))).MaxOrDefault() ?? 0.0;


                        var width = maxWidth;
                        var maxRightOffset =
                            Children
                                .Cast<UIElement>()
                                .Where(child => !double.IsNaN(GetRight(child)))
                                .Select(child => (width - GetRight(child)) - child.DesiredSize.Width)
                                .Concat(
                                    Children
                                        .Cast<UIElement>()
                                        .Where(child => !double.IsNaN(GetCenterRight(child)))
                                        .Select(child => (width - GetCenterRight(child)) - (child.DesiredSize.Width / 2))).MinOrDefault() ?? 0.0;


                        if (maxRightOffset < 0.0)
                        {
                            maxWidth += Math.Abs(maxRightOffset);
                        }
                    }

                    double maxHeight = 0;
                    if (SizeHeightToContent)
                    {
                        maxHeight =
                            Children
                                .Cast<UIElement>()
                                .Where(child => !double.IsNaN(GetTop(child)))
                                .Select(child => GetTop(child) + child.DesiredSize.Height)
                                .Concat(
                                    Children
                                        .Cast<UIElement>()
                                        .Where(child => !double.IsNaN(GetCenterTop(child)))
                                        .Select(child => GetCenterTop(child) + (child.DesiredSize.Height / 2))).MaxOrDefault() ?? 0.0;


                        var height = maxHeight;
                        var maxBottomOffset =
                            Children
                                .Cast<UIElement>()
                                .Where(child => !double.IsNaN(GetBottom(child)))
                                .Select(child => (height - GetBottom(child)) - child.DesiredSize.Height)
                                .Concat(
                                    Children
                                        .Cast<UIElement>()
                                        .Where(child => !double.IsNaN(GetCenterBottom(child)))
                                        .Select(child => (height - GetCenterBottom(child)) - (child.DesiredSize.Height / 2))).MinOrDefault() ?? 0.0;

                        if (maxBottomOffset < 0.0)
                        {
                            maxHeight += Math.Abs(maxBottomOffset);
                        }
                    }

                    return new Size(maxWidth, maxHeight);
                }
                foreach (UIElement element in Children)
                {
                    if (element != null)
                    {
                        element.Measure(availableSize);
                    }
                }
                return Size.Empty;
            }

            /// <summary>
            /// Arranges all children in the correct position.
            /// </summary>
            /// <param name="arrangeSize">The size to arrange element's within.
            /// </param>
            /// <returns>The size that element's were arranged in.</returns>
            protected override Size ArrangeOverride(Size arrangeSize)
            {
                foreach (UIElement element in Children)
                {
                    if (element == null)
                    {
                        continue;
                    }
                    var x = 0.0;
                    var y = 0.0;
                    var left = GetLeft(element);
                    var centerLeft = GetCenterLeft(element);
                    var halfWidth = (element.DesiredSize.Width / 2.0);
                    if (!double.IsNaN(left))
                    {
                        x = left;
                    }
                    else if (!double.IsNaN(centerLeft))
                    {
                        x = centerLeft - halfWidth;
                    }
                    else
                    {
                        var right = GetRight(element);
                        if (!double.IsNaN(right))
                        {
                            x = (arrangeSize.Width - element.DesiredSize.Width) - right;
                        }
                        else
                        {
                            var centerRight = GetCenterRight(element);
                            if (!double.IsNaN(centerRight))
                            {
                                x = (arrangeSize.Width - halfWidth) - centerRight;
                            }
                        }
                    }
                    var top = GetTop(element);
                    var centerTop = GetCenterTop(element);
                    var halfHeight = (element.DesiredSize.Height / 2.0);
                    if (!double.IsNaN(top))
                    {
                        y = top;
                    }
                    else if (!double.IsNaN(centerTop))
                    {
                        y = centerTop - halfHeight;
                    }
                    else
                    {
                        var bottom = GetBottom(element);
                        if (!double.IsNaN(bottom))
                        {
                            y = (arrangeSize.Height - element.DesiredSize.Height) - bottom;
                        }
                        else
                        {
                            var centerBottom = GetCenterBottom(element);
                            if (!double.IsNaN(centerBottom))
                            {
                                y = (arrangeSize.Height - halfHeight) - centerBottom;
                            }
                        }
                    }
                    element.Arrange(new Rect(new Point(x, y), element.DesiredSize));
                }
                return arrangeSize;
            }
        }
}
