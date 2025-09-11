using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace WpfDemoOPUS.ViewModels
{
    /// <summary>
    /// 比例尺设置的ViewModel
    /// </summary>
    public partial class ScaleBarSettingsViewModel : ObservableObject
    {
        [ObservableProperty]
        private bool isScaleBarVisible = true;

        [ObservableProperty]
        private bool isSetScaleMode = true;

        [ObservableProperty]
        private bool showFont = true;

        [ObservableProperty]
        private string position = "位置:左上";

        [ObservableProperty]
        private string drawMode = "模式:水平";

        [ObservableProperty]
        private int scaleLengthWidth = 50;

        [ObservableProperty]
        private int scaleLengthHeight = 50;

        [ObservableProperty]
        private double widthinUnit = 2.0;

        [ObservableProperty]
        private double heightinUnit = 2.0;

        [ObservableProperty]
        private double scale = 50.0;

        [ObservableProperty]
        private double pixelAspectRatio = 1.0;

        [ObservableProperty]
        private string unit = "μm";

        [ObservableProperty]
        private int fontSize = 14;

        [ObservableProperty]
        private int lineWidth = 4;

        [ObservableProperty]
        private string fontFamily = "Arial";

        [ObservableProperty]
        private string textColor = "White";

        [ObservableProperty]
        private string backgroundColor = "Gray";

        [ObservableProperty]
        private double minScale = 0.5;

        [ObservableProperty]
        private double maxScale = 10.0;

        [ObservableProperty]
        private double panSensitivity = 1.3;

        /// <summary>
        /// 获取文字颜色的Brush对象
        /// </summary>
        //public Brush GetTextBrush()
        //{
        //    return TextColor switch
        //    {
        //        "White" => Brushes.White,
        //        "Black" => Brushes.Black,
        //        "Red" => Brushes.Red,
        //        "Yellow" => Brushes.Yellow,
        //        "Green" => Brushes.Green,
        //        "Blue" => Brushes.Blue,
        //        _ => Brushes.White
        //    };
        //}

        ///// <summary>
        ///// 获取背景颜色的Brush对象
        ///// </summary>
        //public Brush GetBackgroundBrush()
        //{
        //    return BackgroundColor switch
        //    {
        //        "Gray" => Brushes.Gray,
        //        "Black" => Brushes.Black,
        //        "White" => Brushes.White,
        //        "Transparent" => Brushes.Transparent,
        //        _ => Brushes.Gray
        //    };
        //}

        ///// <summary>
        ///// 获取字体可见性
        ///// </summary>
        //public Visibility GetFontVisibility()
        //{
        //    return ShowFont ? Visibility.Visible : Visibility.Collapsed;
        //}
    }
}
