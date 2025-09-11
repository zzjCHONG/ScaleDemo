﻿using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace WpfDemoApp.Converters
{
    public class VisibilityToBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is Visibility visibility && visibility == Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is bool isVisible && isVisible ? Visibility.Visible : Visibility.Collapsed;
        }
    }
}
