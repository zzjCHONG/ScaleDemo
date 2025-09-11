using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WpfCustomControlLibrary1;

namespace WpfDemoAppClaude.Controls
{
    /// <summary>
    /// ImageDisplayControl.xaml 的交互逻辑
    /// </summary>
    public partial class ImageDisplayControl : UserControl
    {
        public ImageDisplayControl()
        {
            InitializeComponent();
        }

        #region 图像源属性

        /// <summary>
        /// 图像源
        /// </summary>
        public ImageSource ImageSource
        {
            get { return (ImageSource)GetValue(ImageSourceProperty); }
            set { SetValue(ImageSourceProperty, value); }
        }

        public static readonly DependencyProperty ImageSourceProperty =
            DependencyProperty.Register("ImageSource", typeof(ImageSource), typeof(ImageDisplayControl),
                new FrameworkPropertyMetadata(null));

        #endregion

        #region 比例尺属性

        /// <summary>
        /// 比例尺是否可见
        /// </summary>
        public bool IsScaleBarVisible
        {
            get { return (bool)GetValue(IsScaleBarVisibleProperty); }
            set { SetValue(IsScaleBarVisibleProperty, value); }
        }

        public static readonly DependencyProperty IsScaleBarVisibleProperty =
            DependencyProperty.Register("IsScaleBarVisible", typeof(bool), typeof(ImageDisplayControl),
                new FrameworkPropertyMetadata(true));

        /// <summary>
        /// 比例尺横向长度（像素）
        /// </summary>
        public int ScaleLengthWidth
        {
            get { return (int)GetValue(ScaleLengthWidthProperty); }
            set { SetValue(ScaleLengthWidthProperty, value); }
        }

        public static readonly DependencyProperty ScaleLengthWidthProperty =
            DependencyProperty.Register("ScaleLengthWidth", typeof(int), typeof(ImageDisplayControl),
                new FrameworkPropertyMetadata(50));

        /// <summary>
        /// 比例尺纵向长度（像素）
        /// </summary>
        public int ScaleLengthHeight
        {
            get { return (int)GetValue(ScaleLengthHeightProperty); }
            set { SetValue(ScaleLengthHeightProperty, value); }
        }

        public static readonly DependencyProperty ScaleLengthHeightProperty =
            DependencyProperty.Register("ScaleLengthHeight", typeof(int), typeof(ImageDisplayControl),
                new FrameworkPropertyMetadata(50));

        /// <summary>
        /// 字体大小
        /// </summary>
        public int FontSize
        {
            get { return (int)GetValue(FontSizeProperty); }
            set { SetValue(FontSizeProperty, value); }
        }

        public static readonly DependencyProperty FontSizeProperty =
            DependencyProperty.Register("FontSize", typeof(int), typeof(ImageDisplayControl),
                new FrameworkPropertyMetadata(14));

        /// <summary>
        /// 线条宽度
        /// </summary>
        public int LineWidth
        {
            get { return (int)GetValue(LineWidthProperty); }
            set { SetValue(LineWidthProperty, value); }
        }

        public static readonly DependencyProperty LineWidthProperty =
            DependencyProperty.Register("LineWidth", typeof(int), typeof(ImageDisplayControl),
                new FrameworkPropertyMetadata(4));

        /// <summary>
        /// 字体颜色
        /// </summary>
        public Brush TextBrush
        {
            get { return (Brush)GetValue(TextBrushProperty); }
            set { SetValue(TextBrushProperty, value); }
        }

        public static readonly DependencyProperty TextBrushProperty =
            DependencyProperty.Register("TextBrush", typeof(Brush), typeof(ImageDisplayControl),
                new FrameworkPropertyMetadata(Brushes.White));

        /// <summary>
        /// 背景颜色
        /// </summary>
        public Brush BackgroundBrush
        {
            get { return (Brush)GetValue(BackgroundBrushProperty); }
            set { SetValue(BackgroundBrushProperty, value); }
        }

        public static readonly DependencyProperty BackgroundBrushProperty =
            DependencyProperty.Register("BackgroundBrush", typeof(Brush), typeof(ImageDisplayControl),
                new FrameworkPropertyMetadata(Brushes.Gray));

        /// <summary>
        /// 字体样式
        /// </summary>
        public FontWeight FontWeight
        {
            get { return (FontWeight)GetValue(FontWeightProperty); }
            set { SetValue(FontWeightProperty, value); }
        }

        public static readonly DependencyProperty FontWeightProperty =
            DependencyProperty.Register("FontWeight", typeof(FontWeight), typeof(ImageDisplayControl),
                new FrameworkPropertyMetadata(FontWeights.Bold));

        /// <summary>
        /// 字体是否可见
        /// </summary>
        public Visibility FontVisibility
        {
            get { return (Visibility)GetValue(FontVisibilityProperty); }
            set { SetValue(FontVisibilityProperty, value); }
        }

        public static readonly DependencyProperty FontVisibilityProperty =
            DependencyProperty.Register("FontVisibility", typeof(Visibility), typeof(ImageDisplayControl),
                new FrameworkPropertyMetadata(Visibility.Visible));

        /// <summary>
        /// 字体类型
        /// </summary>
        public string FontFamily
        {
            get { return (string)GetValue(FontFamilyProperty); }
            set { SetValue(FontFamilyProperty, value); }
        }

        public static readonly DependencyProperty FontFamilyProperty =
            DependencyProperty.Register("FontFamily", typeof(string), typeof(ImageDisplayControl),
                new FrameworkPropertyMetadata("Arial"));

        /// <summary>
        /// 摆放位置-四角
        /// </summary>
        public string Position
        {
            get { return (string)GetValue(PositionProperty); }
            set { SetValue(PositionProperty, value); }
        }

        public static readonly DependencyProperty PositionProperty =
            DependencyProperty.Register("Position", typeof(string), typeof(ImageDisplayControl),
                new FrameworkPropertyMetadata("左上"));

        /// <summary>
        /// 绘制模式（水平/垂直/共存）
        /// </summary>
        public string DrawMode
        {
            get { return (string)GetValue(DrawModeProperty); }
            set { SetValue(DrawModeProperty, value); }
        }

        public static readonly DependencyProperty DrawModeProperty =
            DependencyProperty.Register("DrawMode", typeof(string), typeof(ImageDisplayControl),
                new FrameworkPropertyMetadata("水平"));

        /// <summary>
        /// 是否已设置比例尺模式
        /// </summary>
        public bool IsSetScaleMode
        {
            get { return (bool)GetValue(IsSetScaleModeProperty); }
            set { SetValue(IsSetScaleModeProperty, value); }
        }

        public static readonly DependencyProperty IsSetScaleModeProperty =
            DependencyProperty.Register("IsSetScaleMode", typeof(bool), typeof(ImageDisplayControl),
                new FrameworkPropertyMetadata(true));

        /// <summary>
        /// 横向数值显示
        /// </summary>
        public double WidthinUnit
        {
            get { return (double)GetValue(WidthinUnitProperty); }
            set { SetValue(WidthinUnitProperty, value); }
        }

        public static readonly DependencyProperty WidthinUnitProperty =
            DependencyProperty.Register("WidthinUnit", typeof(double), typeof(ImageDisplayControl),
                new FrameworkPropertyMetadata(2.0));

        /// <summary>
        /// 纵向数值显示
        /// </summary>
        public double HeightinUnit
        {
            get { return (double)GetValue(HeightinUnitProperty); }
            set { SetValue(HeightinUnitProperty, value); }
        }

        public static readonly DependencyProperty HeightinUnitProperty =
            DependencyProperty.Register("HeightinUnit", typeof(double), typeof(ImageDisplayControl),
                new FrameworkPropertyMetadata(2.0));

        /// <summary>
        /// 比例尺（pixel/unit）
        /// </summary>
        public double Scale
        {
            get { return (double)GetValue(ScaleProperty); }
            set { SetValue(ScaleProperty, value); }
        }

        public static readonly DependencyProperty ScaleProperty =
            DependencyProperty.Register("Scale", typeof(double), typeof(ImageDisplayControl),
                new FrameworkPropertyMetadata(50.0));

        /// <summary>
        /// 宽高比
        /// </summary>
        public double PixelAspectRatio
        {
            get { return (double)GetValue(PixelAspectRatioProperty); }
            set { SetValue(PixelAspectRatioProperty, value); }
        }

        public static readonly DependencyProperty PixelAspectRatioProperty =
            DependencyProperty.Register("PixelAspectRatio", typeof(double), typeof(ImageDisplayControl),
                new FrameworkPropertyMetadata(1.0));

        /// <summary>
        /// 单位
        /// </summary>
        public string Unit
        {
            get { return (string)GetValue(UnitProperty); }
            set { SetValue(UnitProperty, value); }
        }

        public static readonly DependencyProperty UnitProperty =
            DependencyProperty.Register("Unit", typeof(string), typeof(ImageDisplayControl),
                new FrameworkPropertyMetadata("μm"));

        /// <summary>
        /// 最小缩放比例
        /// </summary>
        public double MinScale
        {
            get { return (double)GetValue(MinScaleProperty); }
            set { SetValue(MinScaleProperty, value); }
        }

        public static readonly DependencyProperty MinScaleProperty =
            DependencyProperty.Register("MinScale", typeof(double), typeof(ImageDisplayControl),
                new FrameworkPropertyMetadata(0.5));

        /// <summary>
        /// 最大缩放比例
        /// </summary>
        public double MaxScale
        {
            get { return (double)GetValue(MaxScaleProperty); }
            set { SetValue(MaxScaleProperty, value); }
        }

        public static readonly DependencyProperty MaxScaleProperty =
            DependencyProperty.Register("MaxScale", typeof(double), typeof(ImageDisplayControl),
                new FrameworkPropertyMetadata(10.0));

        /// <summary>
        /// 拖拽灵敏度
        /// </summary>
        public double PanSensitivity
        {
            get { return (double)GetValue(PanSensitivityProperty); }
            set { SetValue(PanSensitivityProperty, value); }
        }

        public static readonly DependencyProperty PanSensitivityProperty =
            DependencyProperty.Register("PanSensitivity", typeof(double), typeof(ImageDisplayControl),
                new FrameworkPropertyMetadata(1.3));

        #endregion

        #region 公开方法

        /// <summary>
        /// 捕获控件内容为图像
        /// </summary>
        public BitmapImage? CaptureContent(ScaleBarEx.ImageFormat format = ScaleBarEx.ImageFormat.Png)
        {
            return ScaleBarControl.CaptureGridContent(format);
        }

        #endregion
    }
}
