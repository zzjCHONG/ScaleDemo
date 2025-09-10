using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ScaleBarIndependent
{
    /// <summary>
    /// ScaleBarUserControl.xaml 的交互逻辑
    /// </summary>
    public partial class ScaleBarUserControl : UserControl
    {
        public ScaleBarUserControl()
        {
            InitializeComponent();
        }

        // 依赖属性：ImageSource - 外部可以直接设置 (独立于每个实例)
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
                new PropertyMetadata(null)); // ImageSource 仅用于显示，无需同步到 ViewModel

        // 新增依赖属性：GlobalSettingsViewModel - 外部传入共享的全局设置 ViewModel
        public ScaleBarGlobalSettingsViewModel GlobalSettingsViewModel
        {
            get => (ScaleBarGlobalSettingsViewModel)GetValue(GlobalSettingsViewModelProperty);
            set => SetValue(GlobalSettingsViewModelProperty, value);
        }

        public static readonly DependencyProperty GlobalSettingsViewModelProperty =
            DependencyProperty.Register(
                nameof(GlobalSettingsViewModel),
                typeof(ScaleBarGlobalSettingsViewModel),
                typeof(ScaleBarUserControl),
                new PropertyMetadata(null, OnGlobalSettingsViewModelChanged));

        private static void OnGlobalSettingsViewModelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ScaleBarUserControl control)
            {
                // 将 DataContext 设置为传入的全局设置 ViewModel，以便内部控件可以绑定到它
                control.DataContext = e.NewValue;
            }
        }
    }
}
