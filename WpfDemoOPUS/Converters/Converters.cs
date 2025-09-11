using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace WpfDemoOPUS.Converters
{
    /// <summary>
    /// 文本颜色字符串到 Brush 的转换器
    /// </summary>
    public class TextColorToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return Brushes.White;

            string colorName = value.ToString();

            // 如果是 ComboBoxItem.ToString()，提取冒号后面的部分
            if (colorName.Contains(":"))
            {
                colorName = colorName.Split(':').Last().Trim();
            }

            return colorName switch
            {
                "White" => Brushes.White,
                "Black" => Brushes.Black,
                "Red" => Brushes.Red,
                "Yellow" => Brushes.Yellow,
                "Green" => Brushes.Green,
                "Blue" => Brushes.Blue,
                _ => Brushes.White
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is SolidColorBrush brush)
            {
                if (brush.Color == Colors.White) return "White";
                if (brush.Color == Colors.Black) return "Black";
                if (brush.Color == Colors.Red) return "Red";
                if (brush.Color == Colors.Yellow) return "Yellow";
                if (brush.Color == Colors.Green) return "Green";
                if (brush.Color == Colors.Blue) return "Blue";
            }
            return "White";
        }
    }

    /// <summary>
    /// 背景颜色字符串到 Brush 的转换器
    /// </summary>
    public class BackgroundColorToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return Brushes.Gray;

            string colorName = value.ToString();

            // 如果是 ComboBoxItem.ToString()，提取冒号后面的部分
            if (colorName.Contains(":"))
            {
                colorName = colorName.Split(':').Last().Trim();
            }

            return colorName switch
            {
                "Gray" => Brushes.Gray,
                "Black" => Brushes.Black,
                "White" => Brushes.White,
                "Transparent" => Brushes.Transparent,
                _ => Brushes.Gray
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is SolidColorBrush brush)
            {
                if (brush.Color == Colors.Gray) return "Gray";
                if (brush.Color == Colors.Black) return "Black";
                if (brush.Color == Colors.White) return "White";
                if (brush.Color == Colors.Transparent) return "Transparent";
            }
            return "Gray";
        }
    }

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

}
