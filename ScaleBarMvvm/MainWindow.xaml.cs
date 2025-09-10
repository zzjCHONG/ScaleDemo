using Microsoft.Win32;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ScaleBarMvvm
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainViewModel ViewModel { get; private set; }

        public MainWindow()
        {
            InitializeComponent();

            // 初始化ViewModel
            ViewModel = new MainViewModel();
            this.DataContext = ViewModel;

            // 调试：输出初始状态
            System.Diagnostics.Debug.WriteLine("MainWindow initialized");
            System.Diagnostics.Debug.WriteLine($"Initial CurrentImage: {ViewModel.CurrentImage}");
        }

        /// <summary>
        /// 加载图片
        /// </summary>
        private void LoadImage_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "图片文件|*.jpg;*.jpeg;*.png;*.bmp;*.gif|所有文件|*.*"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    BitmapImage bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.UriSource = new Uri(openFileDialog.FileName);
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.EndInit();
                    bitmap.Freeze();

                    // 设置图片前先输出调试信息
                    System.Diagnostics.Debug.WriteLine($"Loading image: {openFileDialog.FileName}");
                    System.Diagnostics.Debug.WriteLine($"Bitmap created: {bitmap}");

                    ViewModel.CurrentImage = bitmap;

                    // 设置图片后输出调试信息
                    System.Diagnostics.Debug.WriteLine($"ViewModel.CurrentImage set to: {ViewModel.CurrentImage}");
                    System.Diagnostics.Debug.WriteLine($"ScaleBarControl.ViewModel.ImageSource: {ScaleBarControl.ViewModel.ImageSource}");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"加载图片失败: {ex.Message}", "错误");
                    System.Diagnostics.Debug.WriteLine($"Error loading image: {ex}");
                }
            }
        }

        /// <summary>
        /// 创建测试图片
        /// </summary>
        private void CreateTestImage_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("Creating test image...");

                var drawingVisual = new DrawingVisual();
                using (var drawingContext = drawingVisual.RenderOpen())
                {
                    // 绘制彩色背景
                    drawingContext.DrawRectangle(
                        new LinearGradientBrush(Colors.LightBlue, Colors.LightGreen, 45),
                        new Pen(Brushes.DarkBlue, 3),
                        new Rect(0, 0, 400, 300));

                    // 绘制文字
                    var formattedText = new FormattedText(
                        "测试图片\nTest Image",
                        System.Globalization.CultureInfo.CurrentCulture,
                        FlowDirection.LeftToRight,
                        new Typeface("Arial"),
                        24, Brushes.DarkRed, 1.0);

                    drawingContext.DrawText(formattedText, new Point(50, 100));

                    // 绘制一些图形
                    drawingContext.DrawEllipse(Brushes.Yellow, new Pen(Brushes.Orange, 2), new Point(300, 100), 50, 30);
                    drawingContext.DrawRectangle(Brushes.Pink, new Pen(Brushes.Purple, 2), new Rect(50, 200, 100, 60));
                }

                var renderBitmap = new RenderTargetBitmap(400, 300, 96, 96, PixelFormats.Pbgra32);
                renderBitmap.Render(drawingVisual);

                System.Diagnostics.Debug.WriteLine($"Test image created: {renderBitmap}");

                ViewModel.CurrentImage = renderBitmap;

                System.Diagnostics.Debug.WriteLine($"Test image set to ViewModel: {ViewModel.CurrentImage}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"创建测试图片失败: {ex.Message}", "错误");
                System.Diagnostics.Debug.WriteLine($"Error creating test image: {ex}");
            }
        }

        /// <summary>
        /// 清空图片
        /// </summary>
        private void ClearImage_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Clearing image...");
            ViewModel.CurrentImage = null;
            System.Diagnostics.Debug.WriteLine($"Image cleared. CurrentImage: {ViewModel.CurrentImage}");
        }

        /// <summary>
        /// 显示调试信息
        /// </summary>
        private void DebugInfo_Click(object sender, RoutedEventArgs e)
        {
            string info = "=== 绑定调试信息 ===\n\n";

            info += $"MainWindow.DataContext: {this.DataContext}\n";
            info += $"MainWindow.ViewModel: {this.ViewModel}\n";
            info += $"MainWindow.ViewModel.CurrentImage: {this.ViewModel?.CurrentImage}\n\n";

            info += $"ScaleBarControl: {ScaleBarControl}\n";
            info += $"ScaleBarControl.DataContext: {ScaleBarControl?.DataContext}\n";
            info += $"ScaleBarControl.ViewModel: {ScaleBarControl?.ViewModel}\n";
            info += $"ScaleBarControl.ImageSource: {ScaleBarControl?.ImageSource}\n";
            info += $"ScaleBarControl.ViewModel.ImageSource: {ScaleBarControl?.ViewModel?.ImageSource}\n\n";

            // 绑定表达式测试
            var binding = System.Windows.Data.BindingOperations.GetBinding(ScaleBarControl, ScaleBarUserControl.ImageSourceProperty);
            info += $"ImageSource绑定表达式: {binding?.Path?.Path}\n";
            info += $"ImageSource绑定源: {binding?.Source}\n";

            MessageBox.Show(info, "调试信息", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        /// <summary>
        /// 窗口加载完成后的测试
        /// </summary>
        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);

            System.Diagnostics.Debug.WriteLine("=== Window Source Initialized ===");
            System.Diagnostics.Debug.WriteLine($"DataContext: {this.DataContext}");
            System.Diagnostics.Debug.WriteLine($"ViewModel: {this.ViewModel}");
            System.Diagnostics.Debug.WriteLine($"ScaleBarControl: {ScaleBarControl}");
        }
    }
}