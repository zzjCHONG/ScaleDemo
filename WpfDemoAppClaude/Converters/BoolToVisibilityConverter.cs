using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace WpfDemoAppClaude.Converters
{
    /// <summary>
    /// 布尔值到可见性转换器
    /// </summary>
    public class BoolToVisibilityConverter : IValueConverter
    {
        public static readonly BoolToVisibilityConverter Instance = new();
        public static readonly BoolToVisibilityConverter InverseInstance = new() { IsInverse = true };

        public bool IsInverse { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool boolValue = value is bool b && b;
            if (IsInverse) boolValue = !boolValue;
            return boolValue ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool isVisible = value is Visibility v && v == Visibility.Visible;
            if (IsInverse) isVisible = !isVisible;
            return isVisible;
        }
    }

    /// <summary>
    /// 布尔值状态转换器
    /// </summary>
    public class BoolToStatusConverter : IValueConverter
    {
        public static readonly BoolToStatusConverter Instance = new();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is bool b && b ? "活跃" : "非活跃";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// 空值到可见性转换器
    /// </summary>
    public class NullToVisibilityConverter : IValueConverter
    {
        public static readonly NullToVisibilityConverter Instance = new();
        public static readonly NullToVisibilityConverter InverseInstance = new() { IsInverse = true };

        public bool IsInverse { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool isNull = value == null;
            if (IsInverse) isNull = !isNull;
            return isNull ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
