using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using OpenCvSharp;
using OpenCvSharp.WpfExtensions;

namespace WpfDemoApp.ViewModels
{
    public partial class DemoWindowViewModel : ObservableObject
    {
        // 两个独立的图像显示ViewModel
        public ImageDisplayViewModel DisplayOneViewModel { get; } = new();
        public ImageDisplayViewModel DisplayTwoViewModel { get; } = new();

        // 一个共享的比例尺设置ViewModel
        public ScaleBarSettingsViewModel GlobalScaleSettingsViewModel { get; } = new();

        [RelayCommand]
        private void LoadImageOne()
        {
            string path = @"C:\\Users\\Administrator\\Desktop\\1_DAPI-405nm_Origin.tif";
            var mat = Cv2.ImRead(path, ImreadModes.Unchanged);
            var source = mat.ToBitmapSource();

            DisplayOneViewModel.ImageSource = source;
        }

        [RelayCommand]
        private void LoadImageTwo()
        {
            string path = @"C:\Users\Administrator\Desktop\1_3.tif";
            var mat = Cv2.ImRead(path, ImreadModes.Unchanged);
            var source = mat.ToBitmapSource();

            DisplayTwoViewModel.ImageSource = source;
        }

    }
}
