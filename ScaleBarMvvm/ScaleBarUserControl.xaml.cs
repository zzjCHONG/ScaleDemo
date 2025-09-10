using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ScaleBarMvvm
{
    /// <summary>
    /// ScaleBarUserControl.xaml 的交互逻辑
    /// </summary>
    public partial class ScaleBarUserControl : UserControl
    {
        public ScaleBarUserControl()
        {
            InitializeComponent();

            // 创建ViewModel实例
            ViewModel = new ScaleBarExViewModel();

            // 设置DataContext
            this.DataContext = ViewModel;
        }

        // 公开ViewModel供外部访问
        public ScaleBarExViewModel ViewModel { get; private set; }

        // 依赖属性：ImageSource
        public ImageSource ImageSource
        {
            get => (ImageSource)GetValue(ImageSourceProperty);
            set => SetValue(ImageSourceProperty, value);
        }

        public static readonly DependencyProperty ImageSourceProperty =
            DependencyProperty.Register(
                nameof(ImageSource),
                typeof(ImageSource),
                typeof(ScaleBarUserControl),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnImageSourceChanged));

        private static void OnImageSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ScaleBarUserControl control && control.ViewModel != null)
            {
                // 同步ImageSource到ViewModel
                control.ViewModel.ImageSource = e.NewValue as ImageSource;

                // 调试输出
                System.Diagnostics.Debug.WriteLine($"ScaleBarUserControl: ImageSource changed to {e.NewValue}");
            }
        }
    }
}
