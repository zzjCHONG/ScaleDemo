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

            // 创建并设置ViewModel
            ViewModel = new ScaleBarExViewModel();
            DataContext = ViewModel;
        }

        // 公开ViewModel供外部访问
        public ScaleBarExViewModel ViewModel { get; }

        // 依赖属性：ImageSource - 外部可以直接设置
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
                new PropertyMetadata(null, OnImageSourceChanged));

        private static void OnImageSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ScaleBarUserControl control)
            {
                // 将外部设置的ImageSource同步到ViewModel
                control.ViewModel.ImageSource = e.NewValue as ImageSource;
            }
        }
    }
}
