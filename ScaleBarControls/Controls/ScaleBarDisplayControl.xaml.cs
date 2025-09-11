using ScaleBarControls.ViewModels;
using System.Windows;
using System.Windows.Controls;
using WpfCustomControlLibrary1;

namespace ScaleBarControls.Controls
{
    /// <summary>
    /// ScaleBarDisplayControl.xaml 的交互逻辑
    /// </summary>
    public partial class ScaleBarDisplayControl : UserControl
    {
        #region 依赖属性

        /// <summary>
        /// ViewModel依赖属性
        /// </summary>
        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register(
                nameof(ViewModel),
                typeof(ScaleBarViewModel),
                typeof(ScaleBarDisplayControl),
                new PropertyMetadata(null, OnViewModelChanged));

        /// <summary>
        /// 背景色依赖属性
        /// </summary>
        public static readonly DependencyProperty DisplayBackgroundProperty =
            DependencyProperty.Register(
                nameof(DisplayBackground),
                typeof(System.Windows.Media.Brush),
                typeof(ScaleBarDisplayControl),
                new PropertyMetadata(System.Windows.Media.Brushes.DarkGray));

        /// <summary>
        /// 是否显示边框依赖属性
        /// </summary>
        public static readonly DependencyProperty ShowBorderProperty =
            DependencyProperty.Register(
                nameof(ShowBorder),
                typeof(bool),
                typeof(ScaleBarDisplayControl),
                new PropertyMetadata(true));

        #endregion

        #region 属性

        /// <summary>
        /// ViewModel属性
        /// </summary>
        public ScaleBarViewModel? ViewModel
        {
            get => (ScaleBarViewModel?)GetValue(ViewModelProperty);
            set => SetValue(ViewModelProperty, value);
        }

        /// <summary>
        /// 显示背景色
        /// </summary>
        public System.Windows.Media.Brush DisplayBackground
        {
            get => (System.Windows.Media.Brush)GetValue(DisplayBackgroundProperty);
            set => SetValue(DisplayBackgroundProperty, value);
        }

        /// <summary>
        /// 是否显示边框
        /// </summary>
        public bool ShowBorder
        {
            get => (bool)GetValue(ShowBorderProperty);
            set => SetValue(ShowBorderProperty, value);
        }

        /// <summary>
        /// 获取内部ScaleBarEx控件的引用
        /// </summary>
        public ScaleBarEx? ScaleBarControl => scaleBarEx;

        #endregion

        #region 构造函数

        public ScaleBarDisplayControl()
        {
            InitializeComponent();
        }

        #endregion

        #region 事件处理

        /// <summary>
        /// ViewModel变化时的处理
        /// </summary>
        private static void OnViewModelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ScaleBarDisplayControl control)
            {
                control.DataContext = e.NewValue;
            }
        }

        #endregion

        #region 公共方法

        /// <summary>
        /// 截取当前显示内容为图片
        /// </summary>
        /// <param name="format">图片格式</param>
        /// <returns>截取的图片</returns>
        public System.Windows.Media.Imaging.BitmapImage? CaptureImage(
            WpfCustomControlLibrary1.ScaleBarEx.ImageFormat format = WpfCustomControlLibrary1.ScaleBarEx.ImageFormat.Png)
        {
            return scaleBarEx?.CaptureGridContent(format);
        }

        /// <summary>
        /// 重置图像到初始状态（平铺显示）
        /// </summary>
        public void ResetImageView()
        {
            // 这里可以通过反射或者扩展ScaleBarEx来调用TileImage方法
            // 或者通过设置ImagePanelScale来触发重置
            if (ViewModel != null && scaleBarEx != null)
            {
                // 触发图像重新平铺
                var currentSource = ViewModel.ImageSource;
                if (currentSource != null)
                {
                    ViewModel.ImageSource = null;
                    ViewModel.ImageSource = currentSource;
                }
            }
        }

        /// <summary>
        /// 获取当前缩放比例
        /// </summary>
        /// <returns>当前缩放比例</returns>
        public double GetCurrentScale()
        {
            return scaleBarEx?.ImagePanelScale ?? -1;
        }

        #endregion
    }
}
