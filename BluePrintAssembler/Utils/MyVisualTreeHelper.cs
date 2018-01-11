using System;
using System.Windows;
using System.Windows.Media;

namespace BluePrintAssembler.Utils
{
    internal static class MyVisualTreeHelper
    {
        public static T GetParentDataContext<T>(DependencyObject d)
        {
            DependencyObject lastGood = null;
            while (d != null)
            {
                if ((d as FrameworkElement)?.DataContext is T)
                    lastGood = d;
                else if (lastGood != null)
                    break;
                d = VisualTreeHelper.GetParent(d);
            }
            //while (d != null && !((d as FrameworkElement)?.DataContext is T)) d = VisualTreeHelper.GetParent(d);
            return (T) ((FrameworkElement) lastGood)?.DataContext;
        }
        public static DependencyObject GetParentWithDataContext<T>(DependencyObject d)
        {
            DependencyObject lastGood = null;
            while (d != null)
            {
                if ((d as FrameworkElement)?.DataContext is T)
                    lastGood = d;
                else if (lastGood != null)
                    break;
                d = VisualTreeHelper.GetParent(d);
            }
            return lastGood;
        }
        public static T GetParent<T>(DependencyObject d) where T : DependencyObject
        {
            while (d != null && !(d is T)) d = VisualTreeHelper.GetParent(d);
            return (T)d;
        }

        public static T GetChild<T>(DependencyObject depObj) where T : DependencyObject
        {
            if (depObj != null)
            {
                for (var i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
                {
                    var child = VisualTreeHelper.GetChild(depObj, i);
                    if (child != null && child is T)
                    {
                        return (T)child;
                    }

                    var childItem = GetChild<T>(child);
                    if (childItem != null) return childItem;
                }
            }
            return null;
        }
        public static T GetChild<T>(DependencyObject depObj,Func<T,bool> predicate) where T : DependencyObject
        {
            if (depObj != null)
            {
                for (var i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
                {
                    var child = VisualTreeHelper.GetChild(depObj, i);
                    if (child != null && child is T && predicate((T)child))
                    {
                        return (T)child;
                    }

                    var childItem = GetChild<T>(child);
                    if (childItem != null) return childItem;
                }
            }
            return null;
        }

    }
}