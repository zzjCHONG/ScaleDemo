using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace WpfDemoOPUS.ViewModels
{
    /// <summary>
    /// 单个图像显示的ViewModel
    /// </summary>
    public partial class ImageDisplayViewModel : ObservableObject
    {
        [ObservableProperty]
        private string displayName = "Display";

        [ObservableProperty]
        private ImageSource? frame;

        /// <summary>
        /// 加载测试图像
        /// </summary>
        public void LoadTestImage(string imagePath)
        {
            try
            {
                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new System.Uri(imagePath, System.UriKind.RelativeOrAbsolute);
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.EndInit();
                Frame = bitmap;
            }
            catch
            {
                // 如果加载失败，创建一个测试图像
                CreateTestImage();
            }
        }

        /// <summary>
        /// 创建测试图像
        /// </summary>
        public void CreateTestImage()
        {
            int width = 800;
            int height = 600;
            int stride = width * 4;
            byte[] pixels = new byte[height * stride];

            // 创建渐变测试图像
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int index = y * stride + x * 4;
                    pixels[index] = (byte)(x * 255 / width);     // Blue
                    pixels[index + 1] = (byte)(y * 255 / height); // Green
                    pixels[index + 2] = (byte)((x + y) * 255 / (width + height)); // Red
                    pixels[index + 3] = 255; // Alpha
                }
            }

            var bitmap = BitmapSource.Create(width, height, 96, 96,
                PixelFormats.Bgra32, null, pixels, stride);
            Frame = bitmap;
        }
    }
}
