using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace ScaleBarControls.Converters
{
    /// <summary>
    /// 布尔值到可见性转换器
    /// </summary>
    public class BooleanToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue)
            {
                // 支持反转参数
                bool invert = parameter?.ToString() == "Invert";
                bool result = invert ? !boolValue : boolValue;
                return result ? Visibility.Visible : Visibility.Collapsed;
            }
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Visibility visibility)
            {
                bool result = visibility == Visibility.Visible;
                bool invert = parameter?.ToString() == "Invert";
                return invert ? !result : result;
            }
            return false;
        }
    }

    /// <summary>
    /// 布尔值到厚度转换器
    /// </summary>
    public class BoolToThicknessConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue && boolValue)
            {
                return new Thickness(1);
            }
            return new Thickness(0);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// 字符串到可见性转换器
    /// </summary>
    public class StringToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Visibility visibility)
            {
                return visibility.ToString();
            }
            return "Visible";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string stringValue && Enum.TryParse<Visibility>(stringValue, out var result))
            {
                return result;
            }
            return Visibility.Visible;
        }
    }

    /// <summary>
    /// FontWeight转换器
    /// </summary>
    public class FontWeightConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is FontWeight fontWeight)
            {
                return fontWeight == FontWeights.Bold;
            }
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isBold && isBold)
            {
                return FontWeights.Bold;
            }
            return FontWeights.Normal;
        }
    }

    /// <summary>
    /// 数值范围限制转换器
    /// </summary>
    public class NumericRangeConverter : IValueConverter
    {
        public double MinValue { get; set; } = double.MinValue;
        public double MaxValue { get; set; } = double.MaxValue;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string stringValue && double.TryParse(stringValue, out var doubleValue))
            {
                return Math.Max(MinValue, Math.Min(MaxValue, doubleValue));
            }
            return value;
        }
    }
}
