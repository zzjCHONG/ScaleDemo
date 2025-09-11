using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace WpfDemoOPUS.ViewModels
{
    /// <summary>
    /// 主窗口ViewModel，管理设置和多个显示
    /// </summary>
    public partial class MainWindowViewModel : ObservableObject
    {
        public MainWindowViewModel()
        {
            // 初始化设置
            Settings = new ScaleBarSettingsViewModel();

            // 初始化两个显示
            DisplayFirst = new ImageDisplayViewModel { DisplayName = "主显示" };
            DisplaySecond = new ImageDisplayViewModel { DisplayName = "副显示" };

            // 加载测试图像
            LoadTestImages();
        }

        [ObservableProperty]
        private ScaleBarSettingsViewModel settings;

        [ObservableProperty]
        private ImageDisplayViewModel displayFirst;

        [ObservableProperty]
        private ImageDisplayViewModel displaySecond;

        /// <summary>
        /// 加载测试图像
        /// </summary>
        private void LoadTestImages()
        {
            // 为主显示创建一个渐变图像
            DisplayFirst.CreateTestImage();

            // 为副显示创建一个不同的测试图像
            CreateCheckerboardImage();
        }

        /// <summary>
        /// 为副显示创建棋盘格图像
        /// </summary>
        private void CreateCheckerboardImage()
        {
            int width = 640;
            int height = 480;
            int stride = width * 4;
            byte[] pixels = new byte[height * stride];
            int squareSize = 20;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int index = y * stride + x * 4;
                    bool isWhite = ((x / squareSize) + (y / squareSize)) % 2 == 0;

                    byte color = isWhite ? (byte)255 : (byte)0;
                    pixels[index] = color;     // Blue
                    pixels[index + 1] = color; // Green
                    pixels[index + 2] = color; // Red
                    pixels[index + 3] = 255;   // Alpha
                }
            }

            var bitmap = BitmapSource.Create(width, height, 96, 96,
                PixelFormats.Bgra32, null, pixels, stride);
            DisplaySecond.Frame = bitmap;
        }

        /// <summary>
        /// 加载图像到主显示
        /// </summary>
        [RelayCommand]
        private void LoadImageToFirst()
        {
            var dialog = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "Image files (*.png;*.jpg;*.bmp;*.tif)|*.png;*.jpg;*.bmp;*.tif|All files (*.*)|*.*"
            };

            if (dialog.ShowDialog() == true)
            {
                DisplayFirst.LoadTestImage(dialog.FileName);
            }
        }

        /// <summary>
        /// 加载图像到副显示
        /// </summary>
        [RelayCommand]
        private void LoadImageToSecond()
        {
            var dialog = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "Image files (*.png;*.jpg;*.bmp;*.tif)|*.png;*.jpg;*.bmp;*.tif|All files (*.*)|*.*"
            };

            if (dialog.ShowDialog() == true)
            {
                DisplaySecond.LoadTestImage(dialog.FileName);
            }
        }

        /// <summary>
        /// 生成随机图像到主显示
        /// </summary>
        [RelayCommand]
        private void GenerateRandomImageFirst()
        {
            DisplayFirst.Frame = GenerateRandomImage();
        }

        /// <summary>
        /// 生成随机图像到副显示
        /// </summary>
        [RelayCommand]
        private void GenerateRandomImageSecond()
        {
            DisplaySecond.Frame = GenerateRandomImage();
        }

        /// <summary>
        /// 生成随机噪声图像
        /// </summary>
        private ImageSource GenerateRandomImage()
        {
            Random rand = new Random();
            int width = 800;
            int height = 600;
            int stride = width * 4;
            byte[] pixels = new byte[height * stride];

            for (int i = 0; i < pixels.Length; i += 4)
            {
                pixels[i] = (byte)rand.Next(256);     // Blue
                pixels[i + 1] = (byte)rand.Next(256); // Green
                pixels[i + 2] = (byte)rand.Next(256); // Red
                pixels[i + 3] = 255;                  // Alpha
            }

            return BitmapSource.Create(width, height, 96, 96,
                PixelFormats.Bgra32, null, pixels, stride);
        }
    }
}
