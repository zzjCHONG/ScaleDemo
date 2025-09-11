using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace WpfDemoApp.Converters
{
    public class BrushToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is SolidColorBrush brush && parameter is SolidColorBrush paramBrush)
            {
                return brush.Color == paramBrush.Color;
            }
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // This converter is primarily for one-way binding to set IsSelected.
            // If you need two-way, you'd need more complex logic to map back.
            // For now, we'll return the parameter brush if value is true.
            if (value is bool isSelected && isSelected && parameter is SolidColorBrush paramBrush)
            {
                return paramBrush;
            }
            return DependencyProperty.UnsetValue; // Indicate no change
        }
    }
}
