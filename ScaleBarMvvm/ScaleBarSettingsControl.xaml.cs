using System.Windows;
using System.Windows.Controls;

namespace ScaleBarMvvm
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

        // 依赖属性：ViewModel - 外部传入共享的ViewModel
        public ScaleBarExViewModel ViewModel
        {
            get => (ScaleBarExViewModel)GetValue(ViewModelProperty);
            set => SetValue(ViewModelProperty, value);
        }

        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register(
                nameof(ViewModel),
                typeof(ScaleBarExViewModel),
                typeof(ScaleBarSettingsControl),
                new PropertyMetadata(null, OnViewModelChanged));

        private static void OnViewModelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ScaleBarSettingsControl control)
            {
                // 设置DataContext为传入的ViewModel
                control.DataContext = e.NewValue;
            }
        }
    }
}
