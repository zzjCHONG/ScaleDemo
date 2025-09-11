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

namespace WpfDemoAppClaude.Controls
{
    /// <summary>
    /// ScaleSettinigsControl.xaml 的交互逻辑
    /// </summary>
    public partial class ScaleSettinigsControl : UserControl
    {
        public ScaleSettinigsControl()
        {
            InitializeComponent();
        }

        #region 依赖属性

        /// <summary>
        /// 比例尺是否可见
        /// </summary>
        public bool IsScaleBarVisible
        {
            get { return (bool)GetValue(IsScaleBarVisibleProperty); }
            set { SetValue(IsScaleBarVisibleProperty, value); }
        }

        public static readonly DependencyProperty IsScaleBarVisibleProperty =
            DependencyProperty.Register("IsScaleBarVisible", typeof(bool), typeof(ScaleSettinigsControl),
                new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        /// <summary>
        /// 位置
        /// </summary>
        public string Position
        {
            get { return (string)GetValue(PositionProperty); }
            set { SetValue(PositionProperty, value); }
        }

        public static readonly DependencyProperty PositionProperty =
            DependencyProperty.Register("Position", typeof(string), typeof(ScaleSettinigsControl),
                new FrameworkPropertyMetadata("左上", FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        /// <summary>
        /// 绘制模式
        /// </summary>
        public string DrawMode
        {
            get { return (string)GetValue(DrawModeProperty); }
            set { SetValue(DrawModeProperty, value); }
        }

        public static readonly DependencyProperty DrawModeProperty =
            DependencyProperty.Register("DrawMode", typeof(string), typeof(ScaleSettinigsControl),
                new FrameworkPropertyMetadata("水平", FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        /// <summary>
        /// 是否已设置比例尺模式
        /// </summary>
        public bool IsSetScaleMode
        {
            get { return (bool)GetValue(IsSetScaleModeProperty); }
            set { SetValue(IsSetScaleModeProperty, value); }
        }

        public static readonly DependencyProperty IsSetScaleModeProperty =
            DependencyProperty.Register("IsSetScaleMode", typeof(bool), typeof(ScaleSettinigsControl),
                new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        /// <summary>
        /// 横向数值
        /// </summary>
        public double WidthinUnit
        {
            get { return (double)GetValue(WidthinUnitProperty); }
            set { SetValue(WidthinUnitProperty, value); }
        }

        public static readonly DependencyProperty WidthinUnitProperty =
            DependencyProperty.Register("WidthinUnit", typeof(double), typeof(ScaleSettinigsControl),
                new FrameworkPropertyMetadata(2.0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        /// <summary>
        /// 纵向数值
        /// </summary>
        public double HeightinUnit
        {
            get { return (double)GetValue(HeightinUnitProperty); }
            set { SetValue(HeightinUnitProperty, value); }
        }

        public static readonly DependencyProperty HeightinUnitProperty =
            DependencyProperty.Register("HeightinUnit", typeof(double), typeof(ScaleSettinigsControl),
                new FrameworkPropertyMetadata(2.0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        /// <summary>
        /// 比例尺
        /// </summary>
        public double Scale
        {
            get { return (double)GetValue(ScaleProperty); }
            set { SetValue(ScaleProperty, value); }
        }

        public static readonly DependencyProperty ScaleProperty =
            DependencyProperty.Register("Scale", typeof(double), typeof(ScaleSettinigsControl),
                new FrameworkPropertyMetadata(50.0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        /// <summary>
        /// 像素宽高比
        /// </summary>
        public double PixelAspectRatio
        {
            get { return (double)GetValue(PixelAspectRatioProperty); }
            set { SetValue(PixelAspectRatioProperty, value); }
        }

        public static readonly DependencyProperty PixelAspectRatioProperty =
            DependencyProperty.Register("PixelAspectRatio", typeof(double), typeof(ScaleSettinigsControl),
                new FrameworkPropertyMetadata(1.0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        /// <summary>
        /// 单位
        /// </summary>
        public string Unit
        {
            get { return (string)GetValue(UnitProperty); }
            set { SetValue(UnitProperty, value); }
        }

        public static readonly DependencyProperty UnitProperty =
            DependencyProperty.Register("Unit", typeof(string), typeof(ScaleSettinigsControl),
                new FrameworkPropertyMetadata("μm", FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        /// <summary>
        /// 横向长度（像素）
        /// </summary>
        public int ScaleLengthWidth
        {
            get { return (int)GetValue(ScaleLengthWidthProperty); }
            set { SetValue(ScaleLengthWidthProperty, value); }
        }

        public static readonly DependencyProperty ScaleLengthWidthProperty =
            DependencyProperty.Register("ScaleLengthWidth", typeof(int), typeof(ScaleSettinigsControl),
                new FrameworkPropertyMetadata(50, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        /// <summary>
        /// 纵向长度（像素）
        /// </summary>
        public int ScaleLengthHeight
        {
            get { return (int)GetValue(ScaleLengthHeightProperty); }
            set { SetValue(ScaleLengthHeightProperty, value); }
        }

        public static readonly DependencyProperty ScaleLengthHeightProperty =
            DependencyProperty.Register("ScaleLengthHeight", typeof(int), typeof(ScaleSettinigsControl),
                new FrameworkPropertyMetadata(50, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        /// <summary>
        /// 字体是否可见
        /// </summary>
        public bool IsFontVisible
        {
            get { return (bool)GetValue(IsFontVisibleProperty); }
            set { SetValue(IsFontVisibleProperty, value); }
        }

        public static readonly DependencyProperty IsFontVisibleProperty =
            DependencyProperty.Register("IsFontVisible", typeof(bool), typeof(ScaleSettinigsControl),
                new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        /// <summary>
        /// 字体大小
        /// </summary>
        public int FontSize
        {
            get { return (int)GetValue(FontSizeProperty); }
            set { SetValue(FontSizeProperty, value); }
        }

        public static readonly DependencyProperty FontSizeProperty =
            DependencyProperty.Register("FontSize", typeof(int), typeof(ScaleSettinigsControl),
                new FrameworkPropertyMetadata(14, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        /// <summary>
        /// 字体族
        /// </summary>
        public string FontFamily
        {
            get { return (string)GetValue(FontFamilyProperty); }
            set { SetValue(FontFamilyProperty, value); }
        }

        public static readonly DependencyProperty FontFamilyProperty =
            DependencyProperty.Register("FontFamily", typeof(string), typeof(ScaleSettinigsControl),
                new FrameworkPropertyMetadata("Arial", FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        /// <summary>
        /// 字体粗细
        /// </summary>
        public FontWeight FontWeight
        {
            get { return (FontWeight)GetValue(FontWeightProperty); }
            set { SetValue(FontWeightProperty, value); }
        }

        public static readonly DependencyProperty FontWeightProperty =
            DependencyProperty.Register("FontWeight", typeof(FontWeight), typeof(ScaleSettinigsControl),
                new FrameworkPropertyMetadata(FontWeights.Bold, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        /// <summary>
        /// 选中的文本颜色
        /// </summary>
        public ComboBoxItem SelectedTextColor
        {
            get { return (ComboBoxItem)GetValue(SelectedTextColorProperty); }
            set { SetValue(SelectedTextColorProperty, value); }
        }

        public static readonly DependencyProperty SelectedTextColorProperty =
            DependencyProperty.Register("SelectedTextColor", typeof(ComboBoxItem), typeof(ScaleSettinigsControl),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        /// <summary>
        /// 选中的背景颜色
        /// </summary>
        public ComboBoxItem SelectedBackgroundColor
        {
            get { return (ComboBoxItem)GetValue(SelectedBackgroundColorProperty); }
            set { SetValue(SelectedBackgroundColorProperty, value); }
        }

        public static readonly DependencyProperty SelectedBackgroundColorProperty =
            DependencyProperty.Register("SelectedBackgroundColor", typeof(ComboBoxItem), typeof(ScaleSettinigsControl),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        /// <summary>
        /// 文本画刷
        /// </summary>
        public Brush TextBrush
        {
            get { return (Brush)GetValue(TextBrushProperty); }
            set { SetValue(TextBrushProperty, value); }
        }

        public static readonly DependencyProperty TextBrushProperty =
            DependencyProperty.Register("TextBrush", typeof(Brush), typeof(ScaleSettinigsControl),
                new FrameworkPropertyMetadata(Brushes.White, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        /// <summary>
        /// 背景画刷
        /// </summary>
        public Brush BackgroundBrush
        {
            get { return (Brush)GetValue(BackgroundBrushProperty); }
            set { SetValue(BackgroundBrushProperty, value); }
        }

        public static readonly DependencyProperty BackgroundBrushProperty =
            DependencyProperty.Register("BackgroundBrush", typeof(Brush), typeof(ScaleSettinigsControl),
                new FrameworkPropertyMetadata(Brushes.Gray, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        /// <summary>
        /// 线条宽度
        /// </summary>
        public int LineWidth
        {
            get { return (int)GetValue(LineWidthProperty); }
            set { SetValue(LineWidthProperty, value); }
        }

        public static readonly DependencyProperty LineWidthProperty =
            DependencyProperty.Register("LineWidth", typeof(int), typeof(ScaleSettinigsControl),
                new FrameworkPropertyMetadata(4, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        /// <summary>
        /// 最小缩放比例
        /// </summary>
        public double MinScale
        {
            get { return (double)GetValue(MinScaleProperty); }
            set { SetValue(MinScaleProperty, value); }
        }

        public static readonly DependencyProperty MinScaleProperty =
            DependencyProperty.Register("MinScale", typeof(double), typeof(ScaleSettinigsControl),
                new FrameworkPropertyMetadata(0.5, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        /// <summary>
        /// 最大缩放比例
        /// </summary>
        public double MaxScale
        {
            get { return (double)GetValue(MaxScaleProperty); }
            set { SetValue(MaxScaleProperty, value); }
        }

        public static readonly DependencyProperty MaxScaleProperty =
            DependencyProperty.Register("MaxScale", typeof(double), typeof(ScaleSettinigsControl),
                new FrameworkPropertyMetadata(10.0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        /// <summary>
        /// 拖拽灵敏度
        /// </summary>
        public double PanSensitivity
        {
            get { return (double)GetValue(PanSensitivityProperty); }
            set { SetValue(PanSensitivityProperty, value); }
        }

        public static readonly DependencyProperty PanSensitivityProperty =
            DependencyProperty.Register("PanSensitivity", typeof(double), typeof(ScaleSettinigsControl),
                new FrameworkPropertyMetadata(1.3, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        #endregion
    }
}
