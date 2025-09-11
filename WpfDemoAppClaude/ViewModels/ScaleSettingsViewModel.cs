using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace WpfDemoAppClaude.ViewModels
{
    /// <summary>
    /// 比例尺设置ViewModel
    /// </summary>
    public partial class ScaleSettingsViewModel : ObservableObject
    {
        #region 比例尺显示属性

        [ObservableProperty]
        private bool _isScaleBarVisible = true;

        [ObservableProperty]
        private string _position = "左上";

        [ObservableProperty]
        private string _drawMode = "水平";

        #endregion

        #region 比例尺模式属性

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

        [ObservableProperty]
        private string _unit = "μm";

        [ObservableProperty]
        private int _scaleLengthWidth = 50;

        [ObservableProperty]
        private int _scaleLengthHeight = 50;

        #endregion

        #region 字体属性

        [ObservableProperty]
        private bool _isFontVisible = true;

        [ObservableProperty]
        private int _fontSize = 14;

        [ObservableProperty]
        private string _fontFamily = "Arial";

        [ObservableProperty]
        private FontWeight _fontWeight = FontWeights.Bold;

        [ObservableProperty]
        private ComboBoxItem? _selectedTextColor;

        [ObservableProperty]
        private Brush _textBrush = Brushes.White;

        #endregion

        #region 背景和线条属性

        [ObservableProperty]
        private ComboBoxItem? _selectedBackgroundColor;

        [ObservableProperty]
        private Brush _backgroundBrush = Brushes.Gray;

        [ObservableProperty]
        private int _lineWidth = 4;

        #endregion

        #region 缩放控制属性

        [ObservableProperty]
        private double _minScale = 0.5;

        [ObservableProperty]
        private double _maxScale = 10.0;

        [ObservableProperty]
        private double _panSensitivity = 1.3;

        #endregion

        #region 字体可见性属性

        public Visibility FontVisibility => IsFontVisible ? Visibility.Visible : Visibility.Collapsed;

        partial void OnIsFontVisibleChanged(bool value)
        {
            OnPropertyChanged(nameof(FontVisibility));
        }

        #endregion

        #region 颜色转换方法

        partial void OnSelectedTextColorChanged(ComboBoxItem? value)
        {
            if (value?.Tag is string colorName)
            {
                TextBrush = GetBrushFromColorName(colorName);
            }
        }

        partial void OnSelectedBackgroundColorChanged(ComboBoxItem? value)
        {
            if (value?.Tag is string colorName)
            {
                BackgroundBrush = GetBrushFromColorName(colorName);
            }
        }

        private Brush GetBrushFromColorName(string colorName)
        {
            return colorName switch
            {
                "White" => Brushes.White,
                "Black" => Brushes.Black,
                "Red" => Brushes.Red,
                "Green" => Brushes.Green,
                "Blue" => Brushes.Blue,
                "Yellow" => Brushes.Yellow,
                "Orange" => Brushes.Orange,
                "Gray" => Brushes.Gray,
                "DarkGray" => Brushes.DarkGray,
                "LightGray" => Brushes.LightGray,
                "Transparent" => Brushes.Transparent,
                _ => Brushes.Gray
            };
        }

        #endregion

        #region 命令

        [RelayCommand]
        private void ResetToDefault()
        {
            IsScaleBarVisible = true;
            Position = "左上";
            DrawMode = "水平";
            IsSetScaleMode = true;
            WidthinUnit = 2.0;
            HeightinUnit = 2.0;
            Scale = 50.0;
            PixelAspectRatio = 1.0;
            Unit = "μm";
            ScaleLengthWidth = 50;
            ScaleLengthHeight = 50;
            IsFontVisible = true;
            FontSize = 14;
            FontFamily = "Arial";
            FontWeight = FontWeights.Bold;
            TextBrush = Brushes.White;
            BackgroundBrush = Brushes.Gray;
            LineWidth = 4;
            MinScale = 0.5;
            MaxScale = 10.0;
            PanSensitivity = 1.3;
        }

        [RelayCommand]
        private void ApplySettings()
        {
            // 这里可以实现应用设置的逻辑
            // 比如保存到配置文件或触发相关事件
            SettingsApplied?.Invoke();
        }

        #endregion

        #region 事件

        public event Action? SettingsApplied;

        #endregion

        #region 构造函数

        public ScaleSettingsViewModel()
        {
            // 初始化默认的选中颜色项
            InitializeColorSelections();
        }

        private void InitializeColorSelections()
        {
            // 创建默认的颜色选择项
            var whiteTextItem = new ComboBoxItem { Content = "白色", Tag = "White" };
            var grayBackgroundItem = new ComboBoxItem { Content = "灰色", Tag = "Gray" };

            SelectedTextColor = whiteTextItem;
            SelectedBackgroundColor = grayBackgroundItem;
        }

        #endregion
    }
}
