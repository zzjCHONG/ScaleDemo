using CommunityToolkit.Mvvm.ComponentModel;
using System.Windows;
using System.Windows.Media;

namespace ScaleBarMvvm
{
    public partial class ScaleBarExViewModel : ObservableObject
    {

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(ImageInfo))] // 用于调试
        private ImageSource? _imageSource;

        [ObservableProperty]
        private bool _isScaleBarVisible = true;

        [ObservableProperty]
        private int _fontSize = 14;

        [ObservableProperty]
        private string _unit = "μm";

        [ObservableProperty]
        private string _position = "左上";

        [ObservableProperty]
        private string _drawMode = "水平";

        [ObservableProperty]
        private Brush _textBrush = Brushes.White;

        [ObservableProperty]
        private Brush _backgroundBrush = Brushes.Gray;

        // 其他属性...
        [ObservableProperty]
        private int _scaleLengthWidth = 50;

        [ObservableProperty]
        private int _scaleLengthHeight = 50;

        [ObservableProperty]
        private int _lineWidth = 4;

        [ObservableProperty]
        private FontWeight _fontWeight = FontWeights.Bold;

        [ObservableProperty]
        private Visibility _fontVisibility = Visibility.Visible;

        [ObservableProperty]
        private string _fontFamily = "Arial";

        [ObservableProperty]
        private bool _isSetScaleMode = true;

        [ObservableProperty]
        private double _widthinUnit = 2.0;

        [ObservableProperty]
        private double _heightinUnit = 2.0;

        [ObservableProperty]
        private double _scale = 50.0;

        [ObservableProperty]
        private double _pixelAspectRatio = 1.0;

        // 调试属性 - 用于显示图像信息
        public string ImageInfo => ImageSource?.ToString() ?? "无图像";

        // 手动属性变更通知测试
        partial void OnImageSourceChanged(ImageSource? value)
        {
            System.Diagnostics.Debug.WriteLine($"ViewModel: ImageSource changed to {value}");
            OnPropertyChanged(nameof(ImageInfo));
        }
    }
}
