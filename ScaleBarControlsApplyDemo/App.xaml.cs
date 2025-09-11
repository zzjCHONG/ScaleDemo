using System.Windows;

namespace ScaleBarControlsApplyDemo
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // 可以在这里添加全局异常处理
            this.DispatcherUnhandledException += App_DispatcherUnhandledException;
        }

        private void App_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            MessageBox.Show($"发生未处理的异常：{e.Exception.Message}", "错误",
                MessageBoxButton.OK, MessageBoxImage.Error);
            e.Handled = true;
        }

    }

}
