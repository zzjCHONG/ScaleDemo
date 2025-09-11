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

namespace WpfDemoOPUS.Controls
{
    /// <summary>
    /// ImageDisplayControl.xaml 的交互逻辑
    /// 这个控件包装了ScaleBarEx，提供了类似Image控件的使用体验
    /// </summary>
    public partial class ImageDisplayControl : UserControl
    {
        public ImageDisplayControl()
        {
            InitializeComponent();
            // 设置默认背景为黑色
            Background = Brushes.Black;
        }

        #region 图像源属性 - 最重要的属性，类似Image控件的Source

        public static readonly DependencyProperty SourceProperty =
            DependencyProperty.Register(nameof(Source), typeof(ImageSource), typeof(ImageDisplayControl),
                new PropertyMetadata(null));

        /// <summary>
        /// 图像源，可以像Image控件一样使用
        /// </summary>
        public ImageSource Source
        {
            get => (ImageSource)GetValue(SourceProperty);
            set => SetValue(SourceProperty, value);
        }

        #endregion

        #region 比例尺显示相关属性

        public static readonly DependencyProperty IsScaleBarVisibleProperty =
            DependencyProperty.Register(nameof(IsScaleBarVisible), typeof(bool), typeof(ImageDisplayControl),
                new PropertyMetadata(true));

        public bool IsScaleBarVisible
        {
            get => (bool)GetValue(IsScaleBarVisibleProperty);
            set => SetValue(IsScaleBarVisibleProperty, value);
        }

        public static readonly DependencyProperty ScaleLengthWidthProperty =
            DependencyProperty.Register(nameof(ScaleLengthWidth), typeof(int), typeof(ImageDisplayControl),
                new PropertyMetadata(50));

        public int ScaleLengthWidth
        {
            get => (int)GetValue(ScaleLengthWidthProperty);
            set => SetValue(ScaleLengthWidthProperty, value);
        }

        public static readonly DependencyProperty ScaleLengthHeightProperty =
            DependencyProperty.Register(nameof(ScaleLengthHeight), typeof(int), typeof(ImageDisplayControl),
                new PropertyMetadata(50));

        public int ScaleLengthHeight
        {
            get => (int)GetValue(ScaleLengthHeightProperty);
            set => SetValue(ScaleLengthHeightProperty, value);
        }

        public static readonly DependencyProperty ScaleFontSizeProperty =
            DependencyProperty.Register(nameof(ScaleFontSize), typeof(int), typeof(ImageDisplayControl),
                new PropertyMetadata(14));

        public int ScaleFontSize
        {
            get => (int)GetValue(ScaleFontSizeProperty);
            set => SetValue(ScaleFontSizeProperty, value);
        }

        public static readonly DependencyProperty LineWidthProperty =
            DependencyProperty.Register(nameof(LineWidth), typeof(int), typeof(ImageDisplayControl),
                new PropertyMetadata(4));

        public int LineWidth
        {
            get => (int)GetValue(LineWidthProperty);
            set => SetValue(LineWidthProperty, value);
        }

        public static readonly DependencyProperty TextBrushProperty =
            DependencyProperty.Register(nameof(TextBrush), typeof(Brush), typeof(ImageDisplayControl),
                new PropertyMetadata(Brushes.White));

        public Brush TextBrush
        {
            get => (Brush)GetValue(TextBrushProperty);
            set => SetValue(TextBrushProperty, value);
        }

        public static readonly DependencyProperty ScaleBackgroundBrushProperty =
            DependencyProperty.Register(nameof(ScaleBackgroundBrush), typeof(Brush), typeof(ImageDisplayControl),
                new PropertyMetadata(Brushes.Gray));

        public Brush ScaleBackgroundBrush
        {
            get => (Brush)GetValue(ScaleBackgroundBrushProperty);
            set => SetValue(ScaleBackgroundBrushProperty, value);
        }

        public static readonly DependencyProperty ScaleFontWeightProperty =
            DependencyProperty.Register(nameof(ScaleFontWeight), typeof(FontWeight), typeof(ImageDisplayControl),
                new PropertyMetadata(FontWeights.Bold));

        public FontWeight ScaleFontWeight
        {
            get => (FontWeight)GetValue(ScaleFontWeightProperty);
            set => SetValue(ScaleFontWeightProperty, value);
        }

        public static readonly DependencyProperty FontVisibilityProperty =
            DependencyProperty.Register(nameof(FontVisibility), typeof(Visibility), typeof(ImageDisplayControl),
                new PropertyMetadata(Visibility.Visible));

        public Visibility FontVisibility
        {
            get => (Visibility)GetValue(FontVisibilityProperty);
            set => SetValue(FontVisibilityProperty, value);
        }

        public static readonly DependencyProperty ScaleFontFamilyProperty =
            DependencyProperty.Register(nameof(ScaleFontFamily), typeof(string), typeof(ImageDisplayControl),
                new PropertyMetadata("Arial"));

        public string ScaleFontFamily
        {
            get => (string)GetValue(ScaleFontFamilyProperty);
            set => SetValue(ScaleFontFamilyProperty, value);
        }

        public static readonly DependencyProperty PositionProperty =
            DependencyProperty.Register(nameof(Position), typeof(string), typeof(ImageDisplayControl),
                new PropertyMetadata("左上"));

        public string Position
        {
            get => (string)GetValue(PositionProperty);
            set => SetValue(PositionProperty, value);
        }

        public static readonly DependencyProperty DrawModeProperty =
            DependencyProperty.Register(nameof(DrawMode), typeof(string), typeof(ImageDisplayControl),
                new PropertyMetadata("水平"));

        public string DrawMode
        {
            get => (string)GetValue(DrawModeProperty);
            set => SetValue(DrawModeProperty, value);
        }

        #endregion

        #region 比例尺模式相关属性

        public static readonly DependencyProperty IsSetScaleModeProperty =
            DependencyProperty.Register(nameof(IsSetScaleMode), typeof(bool), typeof(ImageDisplayControl),
                new PropertyMetadata(true));

        public bool IsSetScaleMode
        {
            get => (bool)GetValue(IsSetScaleModeProperty);
            set => SetValue(IsSetScaleModeProperty, value);
        }

        public static readonly DependencyProperty WidthinUnitProperty =
            DependencyProperty.Register(nameof(WidthinUnit), typeof(double), typeof(ImageDisplayControl),
                new PropertyMetadata(2.0));

        public double WidthinUnit
        {
            get => (double)GetValue(WidthinUnitProperty);
            set => SetValue(WidthinUnitProperty, value);
        }

        public static readonly DependencyProperty HeightinUnitProperty =
            DependencyProperty.Register(nameof(HeightinUnit), typeof(double), typeof(ImageDisplayControl),
                new PropertyMetadata(2.0));

        public double HeightinUnit
        {
            get => (double)GetValue(HeightinUnitProperty);
            set => SetValue(HeightinUnitProperty, value);
        }

        public static readonly DependencyProperty ScaleProperty =
            DependencyProperty.Register(nameof(Scale), typeof(double), typeof(ImageDisplayControl),
                new PropertyMetadata(50.0));

        public double Scale
        {
            get => (double)GetValue(ScaleProperty);
            set => SetValue(ScaleProperty, value);
        }

        public static readonly DependencyProperty PixelAspectRatioProperty =
            DependencyProperty.Register(nameof(PixelAspectRatio), typeof(double), typeof(ImageDisplayControl),
                new PropertyMetadata(1.0));

        public double PixelAspectRatio
        {
            get => (double)GetValue(PixelAspectRatioProperty);
            set => SetValue(PixelAspectRatioProperty, value);
        }

        public static readonly DependencyProperty UnitProperty =
            DependencyProperty.Register(nameof(Unit), typeof(string), typeof(ImageDisplayControl),
                new PropertyMetadata("μm"));

        public string Unit
        {
            get => (string)GetValue(UnitProperty);
            set => SetValue(UnitProperty, value);
        }

        #endregion

        #region 缩放和平移相关属性

        public static readonly DependencyProperty MinScaleProperty =
            DependencyProperty.Register(nameof(MinScale), typeof(double), typeof(ImageDisplayControl),
                new PropertyMetadata(0.5));

        public double MinScale
        {
            get => (double)GetValue(MinScaleProperty);
            set => SetValue(MinScaleProperty, value);
        }

        public static readonly DependencyProperty MaxScaleProperty =
            DependencyProperty.Register(nameof(MaxScale), typeof(double), typeof(ImageDisplayControl),
                new PropertyMetadata(10.0));

        public double MaxScale
        {
            get => (double)GetValue(MaxScaleProperty);
            set => SetValue(MaxScaleProperty, value);
        }

        public static readonly DependencyProperty PanSensitivityProperty =
            DependencyProperty.Register(nameof(PanSensitivity), typeof(double), typeof(ImageDisplayControl),
                new PropertyMetadata(1.3));

        public double PanSensitivity
        {
            get => (double)GetValue(PanSensitivityProperty);
            set => SetValue(PanSensitivityProperty, value);
        }

        #endregion

        /// <summary>
        /// 获取内部ScaleBarEx控件的引用
        /// </summary>
        public WpfCustomControlLibrary1.ScaleBarEx GetScaleBar() => ScaleBar;
    }
}
