using Microsoft.Win32;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WpfApp5
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private BitmapImage? _loadedImage;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void LoadImage_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new OpenFileDialog
            {
                Filter = "TIF|*.tif|PNG|*.png|JPG|*.jpg;*.jpeg|BMP|*.bmp",
                Title = "存储图片",
                InitialDirectory = "E:\\imagesource",
            };
            if (dlg.ShowDialog() == true)
            {
                _loadedImage = new BitmapImage(new Uri(dlg.FileName));
                MainImage.Source = _loadedImage;
                OverlayCanvas.Children.Clear();
                UpdateScaleBar();
            }
        }

        private void ExportImage_Click(object sender, RoutedEventArgs e)
        {
            if (_loadedImage == null) return;

            // 1. 选择保存合成图
            var dlg = new SaveFileDialog
            {
                Filter = "PNG图像|*.png",
                InitialDirectory = "C:\\Users\\Administrator\\Desktop\\Image",
                FileName = $"{DateTime.Now:mm-ss-fff}.png"
            };

            if (dlg.ShowDialog() != true) return;

            var rtb = new RenderTargetBitmap(
                (int)_loadedImage.PixelWidth,
                (int)_loadedImage.PixelHeight,
                96, 96, PixelFormats.Pbgra32);

            var dv = new DrawingVisual();
            using (var dc = dv.RenderOpen())
            {
                // 绘制原图
                dc.DrawImage(_loadedImage, new Rect(0, 0, _loadedImage.PixelWidth, _loadedImage.PixelHeight));

                // 绘制Overlay
                foreach (UIElement child in OverlayCanvas.Children)
                {
                    if (child is Line line)
                    {
                        var pen = new Pen(((SolidColorBrush)line.Stroke), line.StrokeThickness);
                        dc.DrawLine(pen,
                            new Point(Canvas.GetLeft(line) + line.X1, Canvas.GetTop(line) + line.Y1),
                            new Point(Canvas.GetLeft(line) + line.X2, Canvas.GetTop(line) + line.Y2));
                    }
                    else if (child is TextBlock tb)
                    {
                        var ft = new FormattedText(
                            tb.Text,
                            System.Globalization.CultureInfo.CurrentCulture,
                            FlowDirection.LeftToRight,
                            new Typeface("Arial"),
                            tb.FontSize,
                            tb.Foreground,
                            96);

                        dc.DrawText(ft, new Point(Canvas.GetLeft(tb), Canvas.GetTop(tb)));
                    }
                }
            }
            rtb.Render(dv);

            using var fs = new FileStream(dlg.FileName, FileMode.Create);
            var encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(rtb));
            encoder.Save(fs);

            string openFilepath = "C:\\Users\\Administrator\\Desktop\\Image";
            System.Diagnostics.Process.Start("Explorer.exe", $"\"{openFilepath}\"");
        }

        private void ScaleParamsChanged(object sender, EventArgs e)
        {
            UpdateScaleBar();
        }

        private void UpdateScaleBar()
        {
            if (_loadedImage == null) return;

            OverlayCanvas.Children.Clear();

            // 获取参数
            if (!int.TryParse(ScaleLengthWidthBox.Text, out int scaleWidth)) scaleWidth = 50;
            if (!int.TryParse(ScaleLengthHeightBox.Text, out int scaleHeight)) scaleHeight = 50;
            if (!int.TryParse(FontSizeBox.Text, out int fontSize)) fontSize = 12;
            if (!int.TryParse(ThicknessBox.Text, out int lineWidth)) lineWidth = 2;

            scaleWidth = Math.Max(1, scaleWidth);
            scaleHeight = Math.Max(1, scaleHeight);
            fontSize = Math.Max(1, fontSize);
            lineWidth = Math.Max(1, lineWidth);

            // 颜色
            Brush textBrush = Brushes.Black;
            if (ColorBox.SelectedItem is ComboBoxItem cbi)
            {
                textBrush = cbi.Content.ToString() switch
                {
                    "白色" => Brushes.White,
                    "红色" => Brushes.Red,
                    "绿色" => Brushes.Green,
                    "蓝色" => Brushes.Blue,
                    "黄色" => Brushes.Yellow,
                    _ => Brushes.Black,
                };
            }

            double margin = 30;
            double x = margin, y = margin;
            bool horizontalRight = true;
            bool verticalDown = true;

            // 四个角位置
            if (PositionBox.SelectedItem is ComboBoxItem pos)
            {
                switch (pos.Content.ToString())
                {
                    case "左上":
                        x = margin; y = margin;
                        horizontalRight = true; verticalDown = true;
                        break;
                    case "右上":
                        x = _loadedImage.PixelWidth - margin; y = margin;
                        horizontalRight = false; verticalDown = true;
                        break;
                    case "左下":
                        x = margin; y = _loadedImage.PixelHeight - margin;
                        horizontalRight = true; verticalDown = false;
                        break;
                    case "右下":
                        x = _loadedImage.PixelWidth - margin; y = _loadedImage.PixelHeight - margin;
                        horizontalRight = false; verticalDown = false;
                        break;
                }
            }

            void DrawScale(bool horizontal, int length)
            {
                var line = new Line
                {
                    Stroke = textBrush,
                    StrokeThickness = lineWidth
                };
                OverlayCanvas.Children.Add(line);

                var label = new TextBlock
                {
                    Text = $"{length} pixel",
                    FontSize = fontSize,
                    Foreground = textBrush
                };
                OverlayCanvas.Children.Add(label);

                if (horizontal)
                {
                    // 水平线：固定底部 y
                    double yBase = verticalDown ? y : y; // 你想固定的底部y
                    line.X1 = horizontalRight ? x : x - length;
                    line.X2 = horizontalRight ? x + length : x;
                    // 调整Y，使底部固定
                    line.Y1 = line.Y2 = verticalDown ? yBase + lineWidth / 2 : yBase - lineWidth / 2;

                    // 文字在线条外侧（远离角落）
                    double lx = (line.X1 + line.X2) / 2 - fontSize - 20; // 居中
                    double ly = verticalDown ? yBase - fontSize - 2 - lineWidth / 2 : yBase + 2 + lineWidth / 2;
                    Canvas.SetLeft(label, lx);
                    Canvas.SetTop(label, ly);
                }
                else
                {
                    // 垂直线：固定左边 x
                    double xBase = horizontalRight ? x : x; // 你想固定的左边x
                    line.Y1 = verticalDown ? y : y - length;
                    line.Y2 = verticalDown ? y + length : y;
                    // 调整X，使左边固定
                    line.X1 = line.X2 = horizontalRight ? xBase + lineWidth / 2 : xBase - lineWidth / 2;

                    // 文字旋转，在线条外侧（远离角落）
                    label.RenderTransform = new RotateTransform(-90);
                    label.RenderTransformOrigin = new Point(0, 0);
                    double lx = horizontalRight ? xBase - fontSize - 2 - lineWidth / 2 : xBase + lineWidth / 2;
                    double ly = (line.Y1 + line.Y2) / 2 - fontSize / 2 + 40;
                    Canvas.SetLeft(label, lx);
                    Canvas.SetTop(label, ly);
                }
            }

            if (DrawModeBox.SelectedItem is ComboBoxItem drawMode)
            {
                switch (drawMode.Content.ToString())
                {
                    case "水平":
                        DrawScale(true, scaleWidth);
                        break;
                    case "竖直":
                        DrawScale(false, scaleHeight);
                        break;
                    case "共存":
                        DrawScale(true, scaleWidth);
                        DrawScale(false, scaleHeight);
                        break;
                }
            }

        }

    }
}
