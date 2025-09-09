using CommunityToolkit.Mvvm.ComponentModel;
using System.Windows;
using System.Windows.Media;

namespace ScaleBarMvvm
{
    public partial class ScaleBarExViewModel : ObservableObject
    {
        /// <summary>
        /// 图像输入
        /// </summary>
        [ObservableProperty]
        private ImageSource? _imageSource;

        /// <summary>
        /// 比例尺是否可见
        /// </summary>
        [ObservableProperty]
        private bool _isScaleBarVisible = true;

        /// <summary>
        /// 比例尺横向长度（像素）
        /// </summary>
        [ObservableProperty]
        private int _scaleLengthWidth = 50;

        /// <summary>
        /// 比例尺纵向长度（像素）
        /// </summary>
        [ObservableProperty]
        private int _scaleLengthHeight = 50;

        /// <summary>
        /// 字体大小
        /// </summary>
        [ObservableProperty]
        private int _fontSize = 14;

        /// <summary>
        /// 线条宽度
        /// </summary>
        [ObservableProperty]
        private int _lineWidth = 4;

        /// <summary>
        /// 字体颜色
        /// </summary>
        [ObservableProperty]
        private Brush _textBrush = Brushes.White;

        /// <summary>
        /// 背景颜色
        /// </summary>
        [ObservableProperty]
        private Brush _backgroundBrush = Brushes.Gray;

        /// <summary>
        /// 字体样式
        /// </summary>
        [ObservableProperty]
        private FontWeight _fontWeight = FontWeights.Bold;

        /// <summary>
        /// 字体是否可见
        /// </summary>
        [ObservableProperty]
        private Visibility _fontVisibility = Visibility.Visible;

        /// <summary>
        /// 字体类型
        /// </summary>
        [ObservableProperty]
        private string _fontFamily = "Arial";

        /// <summary>
        /// 摆放位置-四角
        /// </summary>
        [ObservableProperty]
        private string _position = "左上";

        /// <summary>
        /// 绘制模式（水平/垂直/共存）
        /// </summary>
        [ObservableProperty]
        private string _drawMode = "水平";

        /// <summary>
        /// 是否已设置比例尺模式（显示形式有区别）
        /// </summary>
        [ObservableProperty]
        private bool _isSetScaleMode = true;

        /// <summary>
        /// 横向数值显示
        /// </summary>
        [ObservableProperty]
        private double _widthinUnit = 2.0;

        /// <summary>
        /// 纵向数值显示
        /// </summary>
        [ObservableProperty]
        private double _heightinUnit = 2.0;

        /// <summary>
        /// 比例尺（pixel/unit）
        /// </summary>
        [ObservableProperty]
        private double _scale = 50.0;

        /// <summary>
        /// 宽高比
        /// </summary>
        [ObservableProperty]
        private double _pixelAspectRatio = 1.0;

        /// <summary>
        /// 单位
        /// </summary>
        [ObservableProperty]
        private string _unit = "μm";
    }
}
