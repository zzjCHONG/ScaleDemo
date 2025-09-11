using Microsoft.Win32;
using ScaleBarControls.ViewModels;
using System.Windows;

namespace ScaleBarControlsApplyDemo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// 共享的ViewModel实例，用于两个控件间的数据共享
        /// </summary>
        public ScaleBarViewModel SharedViewModel { get; }

        public MainWindow()
        {
            InitializeComponent();

            // 创建共享的ViewModel
            SharedViewModel = new ScaleBarViewModel();

            // 设置DataContext
            DataContext = this;

            // 为控件设置ViewModel
            displayControl1.ViewModel = SharedViewModel;
            displayControl2.ViewModel = SharedViewModel;
            settingsControl.ViewModel = SharedViewModel;

            // 加载示例图片（如果存在）
            LoadSampleImageIfExists();
        }

        /// <summary>
        /// 尝试加载示例图片
        /// </summary>
        private void LoadSampleImageIfExists()
        {
            try
            {
                // 尝试加载项目中的示例图片
                var sampleImagePath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "SampleImages", "sample.png");
                if (System.IO.File.Exists(sampleImagePath))
                {
                    SharedViewModel.LoadImageCommand.Execute(sampleImagePath);
                }
            }
            catch
            {
                // 忽略加载示例图片的错误
            }
        }

        /// <summary>
        /// 加载图片按钮点击事件
        /// </summary>
        private void LoadImageButton_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Title = "选择图片文件",
                Filter = "图片文件 (*.png;*.jpg;*.jpeg;*.bmp;*.tiff;*.tif)|*.png;*.jpg;*.jpeg;*.bmp;*.tiff;*.tif|所有文件 (*.*)|*.*",
                Multiselect = false
            };

            if (openFileDialog.ShowDialog() == true)
            {
                SharedViewModel.LoadImageCommand.Execute(openFileDialog.FileName);
            }
        }

        /// <summary>
        /// 重置视图按钮点击事件
        /// </summary>
        private void ResetViewButton_Click(object sender, RoutedEventArgs e)
        {
            displayControl1.ResetImageView();
            displayControl2.ResetImageView();
        }

        /// <summary>
        /// 截图按钮点击事件
        /// </summary>
        private void CaptureButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var bitmap = displayControl1.CaptureImage();
                if (bitmap != null)
                {
                    var saveFileDialog = new SaveFileDialog
                    {
                        Title = "保存截图",
                        Filter = "PNG文件 (*.png)|*.png|JPEG文件 (*.jpg)|*.jpg|BMP文件 (*.bmp)|*.bmp",
                        DefaultExt = ".png"
                    };

                    if (saveFileDialog.ShowDialog() == true)
                    {
                        // 这里需要实现保存bitmap的逻辑
                        SaveBitmapToFile(bitmap, saveFileDialog.FileName);
                        MessageBox.Show("截图已保存！", "成功", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"截图保存失败: {ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// 保存位图到文件
        /// </summary>
        private void SaveBitmapToFile(System.Windows.Media.Imaging.BitmapImage bitmap, string fileName)
        {
            System.Windows.Media.Imaging.BitmapEncoder encoder;

            string extension = System.IO.Path.GetExtension(fileName).ToLower();
            switch (extension)
            {
                case ".jpg":
                case ".jpeg":
                    encoder = new System.Windows.Media.Imaging.JpegBitmapEncoder();
                    break;
                case ".bmp":
                    encoder = new System.Windows.Media.Imaging.BmpBitmapEncoder();
                    break;
                default:
                    encoder = new System.Windows.Media.Imaging.PngBitmapEncoder();
                    break;
            }

            encoder.Frames.Add(System.Windows.Media.Imaging.BitmapFrame.Create(bitmap));

            using (var fileStream = new System.IO.FileStream(fileName, System.IO.FileMode.Create))
            {
                encoder.Save(fileStream);
            }
        }

        /// <summary>
        /// 显示关于对话框
        /// </summary>
        private void AboutButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(
                "ScaleBar控件演示程序\n\n" +
                "功能特色：\n" +
                "• 图像缩放和平移\n" +
                "• 可自定义的比例尺显示\n" +
                "• 多种预设配置\n" +
                "• 实时设置预览\n" +
                "• 图像截图功能\n\n" +
                "基于MVVM模式和CommunityToolkit.Mvvm框架",
                "关于",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }

        /// <summary>
        /// 窗口关闭事件
        /// </summary>
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // 可以在这里添加保存设置的逻辑
        }
    }
}