using System.Configuration;
using System.Data;
using System.Windows;

namespace WpfDemoAppClaude
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // 设置全局异常处理
            this.DispatcherUnhandledException += App_DispatcherUnhandledException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
        }

        private void App_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            MessageBox.Show(
                $"应用程序发生未处理的异常：\n{e.Exception.Message}",
                "错误",
                MessageBoxButton.OK,
                MessageBoxImage.Error);

            e.Handled = true;
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            if (e.ExceptionObject is Exception ex)
            {
                MessageBox.Show(
                    $"应用程序域发生未处理的异常：\n{ex.Message}",
                    "严重错误",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }
    }

}
