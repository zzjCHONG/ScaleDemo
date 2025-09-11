using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfDemoAppClaude.ViewModels
{
    /// <summary>
    /// 主窗口ViewModel
    /// </summary>
    public partial class MainViewModel : ObservableObject
    {
        [ObservableProperty]
        private ScaleSettingsViewModel _scaleSettings;

        [ObservableProperty]
        private ImageSourceViewModel _displayFirst;

        [ObservableProperty]
        private ImageSourceViewModel _displaySecond;

        [ObservableProperty]
        private string _title = "图像显示系统";

        public MainViewModel()
        {
            // 初始化比例尺设置
            ScaleSettings = new ScaleSettingsViewModel();

            // 初始化两个不同的图像源
            DisplayFirst = new ImageSourceViewModel("主显示源");
            DisplaySecond = new ImageSourceViewModel("副显示源 - 实时");

            // 订阅设置应用事件
            ScaleSettings.SettingsApplied += OnSettingsApplied;
        }

        private void OnSettingsApplied()
        {
            // 这里可以处理设置应用后的逻辑
            // 比如保存配置、通知其他组件等
        }

        [RelayCommand]
        private void ShowAbout()
        {
            System.Windows.MessageBox.Show(
                "图像显示系统\n\n基于 ScaleBarEx 控件\n支持图像缩放、拖拽和比例尺显示",
                "关于",
                System.Windows.MessageBoxButton.OK,
                System.Windows.MessageBoxImage.Information);
        }

        [RelayCommand]
        private void ExitApplication()
        {
            System.Windows.Application.Current.Shutdown();
        }

        protected override void OnPropertyChanged(System.ComponentModel.PropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);

            // 这里可以添加属性变化的处理逻辑
        }

        public void Cleanup()
        {
            DisplayFirst?.Dispose();
            DisplaySecond?.Dispose();

            if (ScaleSettings != null)
            {
                ScaleSettings.SettingsApplied -= OnSettingsApplied;
            }
        }
    }
}
