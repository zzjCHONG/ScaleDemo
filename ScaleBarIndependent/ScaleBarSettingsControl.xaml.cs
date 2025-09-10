using System.Windows;
using System.Windows.Controls;

namespace ScaleBarIndependent
{
    /// <summary>
    /// ScaleBarSettingsControl.xaml 的交互逻辑
    /// </summary>
    public partial class ScaleBarSettingsControl : UserControl
    {
        public ScaleBarSettingsControl()
        {
            InitializeComponent();
        }

        // 依赖属性：GlobalSettingsViewModel - 外部传入共享的全局设置 ViewModel
        // 将原 ViewModel 属性的类型改为 ScaleBarGlobalSettingsViewModel
        public ScaleBarGlobalSettingsViewModel GlobalSettingsViewModel
        {
            get => (ScaleBarGlobalSettingsViewModel)GetValue(GlobalSettingsViewModelProperty);
            set => SetValue(GlobalSettingsViewModelProperty, value);
        }

        public static readonly DependencyProperty GlobalSettingsViewModelProperty =
            DependencyProperty.Register(
                nameof(GlobalSettingsViewModel),
                typeof(ScaleBarGlobalSettingsViewModel),
                typeof(ScaleBarSettingsControl),
                new PropertyMetadata(null, OnGlobalSettingsViewModelChanged));

        private static void OnGlobalSettingsViewModelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ScaleBarSettingsControl control)
            {
                // 设置DataContext为传入的全局设置ViewModel
                control.DataContext = e.NewValue;
            }
        }
    }
}
