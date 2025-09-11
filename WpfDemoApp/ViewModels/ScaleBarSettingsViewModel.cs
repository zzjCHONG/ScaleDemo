using CommunityToolkit.Mvvm.ComponentModel;
using System.Windows;
using System.Windows.Media;

namespace WpfDemoApp.ViewModels
{
    public partial class ScaleBarSettingsViewModel : ObservableObject
    {
        [ObservableProperty] private bool _isScaleBarVisible = true;
        [ObservableProperty] private int _scaleLengthWidth = 50;
        [ObservableProperty] private int _scaleLengthHeight = 50;
        [ObservableProperty] private int _fontSize = 14;
        [ObservableProperty] private int _lineWidth = 4;
        [ObservableProperty] private Brush _textBrush = Brushes.White;
        [ObservableProperty] private Brush _backgroundBrush = Brushes.Gray;
        [ObservableProperty] private FontWeight _fontWeight = FontWeights.Bold;
        [ObservableProperty] private Visibility _fontVisibility = Visibility.Visible;
        [ObservableProperty] private string _fontFamily = "Arial";
        [ObservableProperty] private string _position = "左上"; // 对应 ScaleBarEx 的 Position 属性
        [ObservableProperty] private string _drawMode = "水平"; // 对应 ScaleBarEx 的 DrawMode 属性
        [ObservableProperty] private bool _isSetScaleMode = true;
        [ObservableProperty] private double _widthinUnit = 2.0;
        [ObservableProperty] private double _heightinUnit = 2.0;
        [ObservableProperty] private double _scale = 50.0;
        [ObservableProperty] private double _pixelAspectRatio = 1.0;
        [ObservableProperty] private string _unit = "μm";
    }
}
