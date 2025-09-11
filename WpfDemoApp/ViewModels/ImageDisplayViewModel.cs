using CommunityToolkit.Mvvm.ComponentModel;
using System.Windows.Media;

namespace WpfDemoApp.ViewModels
{
    public partial class ImageDisplayViewModel : ObservableObject
    {
        [ObservableProperty]
        private ImageSource? _imageSource;
    }
}
