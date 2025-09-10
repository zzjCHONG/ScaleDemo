using CommunityToolkit.Mvvm.ComponentModel;
using System.Windows.Media;

namespace ScaleBarMvvm
{
    public partial class MainViewModel : ObservableObject
    {
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(ImageInfo))]
        private ImageSource? _currentImage;

        // 调试属性
        public string ImageInfo => CurrentImage?.ToString() ?? "无图像";

        partial void OnCurrentImageChanged(ImageSource? value)
        {
            System.Diagnostics.Debug.WriteLine($"MainViewModel: CurrentImage changed to {value}");
        }

    }
}
