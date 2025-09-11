using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using WpfDemoApp.ViewModels;

namespace WpfDemoApp.Views
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

        // 现有的 ScaleSettings 依赖属性
        public static readonly DependencyProperty ScaleSettingsProperty =
            DependencyProperty.Register(
                nameof(ScaleSettings),
                typeof(ScaleBarSettingsViewModel),
                typeof(ImageDisplayControl),
                new PropertyMetadata(null));

        public ScaleBarSettingsViewModel ScaleSettings
        {
            get { return (ScaleBarSettingsViewModel)GetValue(ScaleSettingsProperty); }
            set { SetValue(ScaleSettingsProperty, value); }
        }

        // 新增 ImageSource 依赖属性
        public static readonly DependencyProperty ImageSourceProperty =
            DependencyProperty.Register(
                nameof(ImageSource),
                typeof(ImageSource), // 属性类型为 System.Windows.Media.ImageSource
                typeof(ImageDisplayControl),
                new PropertyMetadata(null)); // 默认值为 null

        public ImageSource ImageSource
        {
            get { return (ImageSource)GetValue(ImageSourceProperty); }
            set { SetValue(ImageSourceProperty, value); }
        }
    }
}
