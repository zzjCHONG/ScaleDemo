using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace WpfDemoAppClaude.ViewModels
{
    /// <summary>
    /// 图像源ViewModel，模拟不同的图像输入源
    /// </summary>
    public partial class ImageSourceViewModel : ObservableObject
    {
        [ObservableProperty]
        private BitmapImage? _frame;

        [ObservableProperty]
        private string _sourceName = "";

        [ObservableProperty]
        private bool _isActive = false;

        private readonly Random _random = new();
        private readonly Timer? _updateTimer;

        public ImageSourceViewModel(string sourceName)
        {
            SourceName = sourceName;

            // 模拟实时更新的图像源（可选）
            if (sourceName.Contains("实时"))
            {
                _updateTimer = new Timer(GenerateRandomImage, null, TimeSpan.Zero, TimeSpan.FromSeconds(2));
            }
        }

        [RelayCommand]
        private void LoadImageFromFile()
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "图像文件|*.jpg;*.jpeg;*.png;*.bmp;*.gif;*.tiff|所有文件|*.*",
                Title = $"选择图像 - {SourceName}"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    var bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.UriSource = new Uri(openFileDialog.FileName);
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.EndInit();

                    Frame = bitmap;
                    IsActive = true;
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show($"加载图像失败: {ex.Message}", "错误",
                        System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                }
            }
        }

        [RelayCommand]
        private void GenerateTestImage()
        {
            Frame = CreateTestImage();
            IsActive = true;
        }

        [RelayCommand]
        private void ClearImage()
        {
            Frame = null;
            IsActive = false;
        }

        private void GenerateRandomImage(object? state)
        {
            if (!IsActive) return;

            System.Windows.Application.Current?.Dispatcher.Invoke(() =>
            {
                Frame = CreateTestImage();
            });
        }

        private BitmapImage CreateTestImage()
        {
            const int width = 800;
            const int height = 600;
            const int bytesPerPixel = 4; // BGRA

            var pixels = new byte[width * height * bytesPerPixel];

            // 创建渐变背景
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int index = (y * width + x) * bytesPerPixel;

                    // 创建彩色渐变效果
                    var r = (byte)((x * 255) / width);
                    var g = (byte)((y * 255) / height);
                    var b = (byte)((x + y) * 127 / (width + height));

                    // 添加随机噪点（仅对实时源）
                    if (SourceName.Contains("实时"))
                    {
                        r = (byte)Math.Min(255, r + _random.Next(-30, 30));
                        g = (byte)Math.Min(255, g + _random.Next(-30, 30));
                        b = (byte)Math.Min(255, b + _random.Next(-30, 30));
                    }

                    pixels[index] = b;     // Blue
                    pixels[index + 1] = g; // Green
                    pixels[index + 2] = r; // Red
                    pixels[index + 3] = 255; // Alpha
                }
            }

            // 添加一些几何图形
            AddGeometricShapes(pixels, width, height);

            // 创建BitmapSource
            var bitmap = BitmapSource.Create(
                width, height, 96, 96,
                System.Windows.Media.PixelFormats.Bgra32,
                null, pixels, width * bytesPerPixel);

            // 转换为BitmapImage
            var bitmapImage = new BitmapImage();
            using var memoryStream = new MemoryStream();

            var encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(bitmap));
            encoder.Save(memoryStream);

            bitmapImage.BeginInit();
            bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
            bitmapImage.StreamSource = new MemoryStream(memoryStream.ToArray());
            bitmapImage.EndInit();

            return bitmapImage;
        }

        private void AddGeometricShapes(byte[] pixels, int width, int height)
        {
            const int bytesPerPixel = 4;

            // 绘制圆形
            int centerX = width / 3;
            int centerY = height / 3;
            int radius = Math.Min(width, height) / 8;

            for (int y = centerY - radius; y <= centerY + radius; y++)
            {
                for (int x = centerX - radius; x <= centerX + radius; x++)
                {
                    if (x >= 0 && x < width && y >= 0 && y < height)
                    {
                        double distance = Math.Sqrt((x - centerX) * (x - centerX) + (y - centerY) * (y - centerY));
                        if (distance <= radius)
                        {
                            int index = (y * width + x) * bytesPerPixel;
                            pixels[index] = 255;     // Blue
                            pixels[index + 1] = 255; // Green
                            pixels[index + 2] = 0;   // Red
                            pixels[index + 3] = 255; // Alpha
                        }
                    }
                }
            }

            // 绘制矩形
            int rectX = width * 2 / 3;
            int rectY = height * 2 / 3;
            int rectWidth = width / 6;
            int rectHeight = height / 8;

            for (int y = rectY; y < rectY + rectHeight && y < height; y++)
            {
                for (int x = rectX; x < rectX + rectWidth && x < width; x++)
                {
                    if (x >= 0 && y >= 0)
                    {
                        int index = (y * width + x) * bytesPerPixel;
                        pixels[index] = 0;       // Blue
                        pixels[index + 1] = 255; // Green
                        pixels[index + 2] = 255; // Red
                        pixels[index + 3] = 255; // Alpha
                    }
                }
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _updateTimer?.Dispose();
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
