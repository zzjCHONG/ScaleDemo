using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ScaleBarControls.ViewModels
{
    /// <summary>
    /// 比例尺控件的共享ViewModel，用于显示控件和设置控件之间的数据绑定
    /// </summary>
    public partial class ScaleBarViewModel : ObservableObject
    {
        #region 图像相关属性

        [ObservableProperty]
        private ImageSource? _imageSource;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsImageLoaded))]
        private double _imagePanelScale = -1;

        public bool IsImageLoaded => ImageSource != null;

        #endregion

        #region 缩放和平移设置

        [ObservableProperty]
        private double _minScale = 0.5;

        [ObservableProperty]
        private double _maxScale = 10.0;

        [ObservableProperty]
        private double _panSensitivity = 1.3;

        #endregion

        #region 比例尺显示设置

        [ObservableProperty]
        private bool _isScaleBarVisible = true;

        [ObservableProperty]
        private int _scaleLengthWidth = 50;

        [ObservableProperty]
        private int _scaleLengthHeight = 50;

        [ObservableProperty]
        private int _fontSize = 14;

        [ObservableProperty]
        private int _lineWidth = 4;

        [ObservableProperty]
        private Brush _textBrush = Brushes.White;

        [ObservableProperty]
        private Brush _backgroundBrush = Brushes.Gray;

        [ObservableProperty]
        private FontWeight _fontWeight = FontWeights.Bold;

        [ObservableProperty]
        private Visibility _fontVisibility = Visibility.Visible;

        [ObservableProperty]
        private string _fontFamily = "Arial";

        #endregion

        #region 比例尺位置和模式

        [ObservableProperty]
        private string _position = "左上";

        [ObservableProperty]
        private string _drawMode = "水平";

        [ObservableProperty]
        private bool _isSetScaleMode = true;

        #endregion

        #region 比例尺数值设置

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

        #endregion

        #region 命令

        [RelayCommand]
        private void LoadImage(string? imagePath)
        {
            if (string.IsNullOrEmpty(imagePath)) return;

            try
            {
                var bitmap = new BitmapImage(new Uri(imagePath));
                ImageSource = bitmap;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"加载图片失败: {ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        [RelayCommand]
        private void ResetScaleSettings()
        {
            IsScaleBarVisible = true;
            Position = "左上";
            DrawMode = "水平";
            FontSize = 14;
            LineWidth = 4;
            TextBrush = Brushes.White;
            BackgroundBrush = Brushes.Gray;
            FontWeight = FontWeights.Bold;
            FontVisibility = Visibility.Visible;
            FontFamily = "Arial";

            if (IsSetScaleMode)
            {
                WidthinUnit = 2.0;
                HeightinUnit = 2.0;
                Scale = 50.0;
                PixelAspectRatio = 1.0;
                Unit = "μm";
            }
            else
            {
                ScaleLengthWidth = 50;
                ScaleLengthHeight = 50;
            }
        }

        [RelayCommand]
        private void ToggleScaleMode()
        {
            IsSetScaleMode = !IsSetScaleMode;
        }

        #endregion

        #region 预设配置

        /// <summary>
        /// 应用预设配置
        /// </summary>
        [RelayCommand]
        private void ApplyPreset(string presetName)
        {
            switch (presetName)
            {
                case "Microscopy":
                    ApplyMicroscopyPreset();
                    break;
                case "Photography":
                    ApplyPhotographyPreset();
                    break;
                case "Engineering":
                    ApplyEngineeringPreset();
                    break;
                default:
                    ResetScaleSettings();
                    break;
            }
        }

        private void ApplyMicroscopyPreset()
        {
            IsSetScaleMode = true;
            Unit = "μm";
            Scale = 100.0;
            WidthinUnit = 10.0;
            HeightinUnit = 10.0;
            Position = "右下";
            DrawMode = "水平";
            FontSize = 12;
            TextBrush = Brushes.White;
            BackgroundBrush = new SolidColorBrush(Color.FromArgb(180, 0, 0, 0));
        }

        private void ApplyPhotographyPreset()
        {
            IsSetScaleMode = false;
            ScaleLengthWidth = 100;
            ScaleLengthHeight = 100;
            Position = "左下";
            DrawMode = "水平";
            FontSize = 16;
            TextBrush = Brushes.Yellow;
            BackgroundBrush = new SolidColorBrush(Color.FromArgb(120, 0, 0, 0));
        }

        private void ApplyEngineeringPreset()
        {
            IsSetScaleMode = true;
            Unit = "mm";
            Scale = 50.0;
            WidthinUnit = 5.0;
            HeightinUnit = 5.0;
            Position = "左上";
            DrawMode = "共存";
            FontSize = 14;
            TextBrush = Brushes.Black;
            BackgroundBrush = Brushes.White;
        }

        #endregion
    }

}
