using Microsoft.Win32;
using System.Globalization;
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
        public MainWindow()
        {
            InitializeComponent();
        }

        private void LoadImage_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog
            {
                Filter = "图像文件|*.png;*.jpg;*.jpeg;*.bmp;*.tif;*.tiff"
            };

            if (ofd.ShowDialog() == true)
            {
                BitmapImage bitmap = new BitmapImage(new Uri(ofd.FileName));
                MainImage.Source = bitmap;

                // 调整 Image 控件大小以适应 Canvas
                MainImage.Width = bitmap.PixelWidth;
                MainImage.Height = bitmap.PixelHeight;

                OverlayCanvas.Width = bitmap.PixelWidth;
                OverlayCanvas.Height = bitmap.PixelHeight;

                UpdateScaleBar();
            }
        }

        private void ExportImage_Click(object sender, RoutedEventArgs e)
        {
            if (MainImage.Source == null) return;

            RenderTargetBitmap rtb = new RenderTargetBitmap(
                (int)ImageCanvas.ActualWidth,
                (int)ImageCanvas.ActualHeight,
                96, 96, PixelFormats.Pbgra32);

            rtb.Render(ImageCanvas);

            SaveFileDialog sfd = new SaveFileDialog
            {
                Filter = "PNG 文件|*.png",
                FileName = "带比例尺图像.png"
            };

            if (sfd.ShowDialog() == true)
            {
                using (FileStream fs = new FileStream(sfd.FileName, FileMode.Create))
                {
                    PngBitmapEncoder encoder = new PngBitmapEncoder();
                    encoder.Frames.Add(BitmapFrame.Create(rtb));
                    encoder.Save(fs);
                }

                MessageBox.Show("导出成功！", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void ScaleParamsChanged(object sender, EventArgs e)
        {
            UpdateScaleBar();
        }

        private void UpdateScaleBar()
        {
            if (MainImage?.Source == null) return;
            OverlayCanvas.Children.Clear();

            // 参数
            int scaleX = int.TryParse(ScaleLengthWidthBox.Text, out int w) ? Math.Max(1, w) : 50;
            int scaleY = int.TryParse(ScaleLengthHeightBox.Text, out int h) ? Math.Max(1, h) : 50;
            int fontSize = int.TryParse(FontSizeBox.Text, out int f) ? Math.Max(1, f) : 12;
            int lineWidth = int.TryParse(ThicknessBox.Text, out int lw) ? Math.Max(1, lw) : 2;

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

            // 背景
            Brush bgBrush = Brushes.Gray;
            if (BackgroundBox.SelectedItem is ComboBoxItem bgItem)
            {
                bgBrush = bgItem.Content.ToString() switch
                {
                    "灰色" => Brushes.Gray,
                    "白色" => Brushes.White,
                    "黑色" => Brushes.Black,
                    "透明" => Brushes.Transparent,
                    _ => Brushes.Gray,
                };
            }

            // 字体
            FontWeight fontWeight = (FontWeightBox.SelectedItem as ComboBoxItem)?.Content.ToString() == "加粗"
                ? FontWeights.Bold : FontWeights.Normal;

            bool showFont = (FontVisibilityBox.SelectedItem as ComboBoxItem)?.Content.ToString() != "隐藏";
            string fontFamily = (FontFamilyBox.SelectedItem as ComboBoxItem)?.Content.ToString() ?? "Segoe UI";

            // 基础位置
            double margin = 50;
            double x = margin, y = margin;
            bool horizontalRight = true, verticalDown = true;

            string position = (PositionBox.SelectedItem as ComboBoxItem)?.Content.ToString() ?? "左上";
            switch (position)
            {
                case "右上": x = MainImage.ActualWidth - margin; y = margin; horizontalRight = false; verticalDown = true; break;
                case "左下": x = margin; y = MainImage.ActualHeight - margin; horizontalRight = true; verticalDown = false; break;
                case "右下": x = MainImage.ActualWidth - margin; y = MainImage.ActualHeight - margin; horizontalRight = false; verticalDown = false; break;
                case "左上": default: break;
            }

            string drawMode = (DrawModeBox.SelectedItem as ComboBoxItem)?.Content.ToString() ?? "共存";

            // 计算文本尺寸
            FormattedText hText = new($"{scaleX} {UnitBox.Text}", CultureInfo.CurrentCulture,
                FlowDirection.LeftToRight, new Typeface(fontFamily), fontSize,
                textBrush, VisualTreeHelper.GetDpi(this).PixelsPerDip);

            FormattedText vText = new($"{scaleY} {UnitBox.Text}", CultureInfo.CurrentCulture,
                FlowDirection.LeftToRight, new Typeface(fontFamily), fontSize,
                textBrush, VisualTreeHelper.GetDpi(this).PixelsPerDip);

            double hTextWidth = hText.Width, hTextHeight = hText.Height;
            double vTextWidth = vText.Width, vTextHeight = vText.Height;

            // 绘制
            if (drawMode is "共存" or "水平")
                DrawScale(true, scaleX, x, y, horizontalRight, verticalDown,
                    textBrush, bgBrush, fontWeight, showFont, fontFamily, fontSize, lineWidth, hTextWidth, hTextHeight);

            if (drawMode is "共存" or "竖直")
                DrawScale(false, scaleY, x, y, horizontalRight, verticalDown,
                    textBrush, bgBrush, fontWeight, showFont, fontFamily, fontSize, lineWidth, vTextWidth, vTextHeight);
        }

        private void DrawScale(
            bool horizontal, int length, double x, double y,
            bool horizontalRight, bool verticalDown,
            Brush textBrush, Brush bgBrush, FontWeight fontWeight, bool showFont,
            string fontFamily, int fontSize, int lineWidth,
            double textWidth, double textHeight)
        {
            double lineOffset = lineWidth / 2;
            double rectWidth, rectHeight, rectX, rectY;

            if (horizontal)
            {
                double maxLen = Math.Max(showFont ? textWidth : 0, length);
                rectWidth = maxLen + 10;
                rectHeight = lineWidth + (showFont ? textHeight : 0) + 10;
                rectX = horizontalRight ? x - 5 : x - maxLen - 5;
                rectY = verticalDown ? y - (showFont ? textHeight : 0) - 5 : y - lineWidth - 5;
            }
            else
            {
                rectWidth = lineWidth + (showFont ? textHeight : 0) + 10;
                rectHeight = length + 10;
                rectX = horizontalRight ? x - (showFont ? textHeight : 0) - 5 : x - lineWidth - 5;
                rectY = verticalDown ? y - 5 : y - length - 5;
            }

            // 背景
            var rect = new Rectangle { Width = rectWidth, Height = rectHeight, Fill = bgBrush };
            Canvas.SetLeft(rect, rectX);
            Canvas.SetTop(rect, rectY);
            OverlayCanvas.Children.Add(rect);

            // 线条
            var line = new Line { Stroke = textBrush, StrokeThickness = lineWidth };
            OverlayCanvas.Children.Add(line);

            // 标签
            var label = new TextBlock
            {
                Text = $"{length} {UnitBox.Text}",
                FontSize = fontSize,
                Foreground = textBrush,
                FontWeight = fontWeight,
                FontFamily = new FontFamily(fontFamily),
                Visibility = showFont ? Visibility.Visible : Visibility.Collapsed
            };
            OverlayCanvas.Children.Add(label);

            if (horizontal)
            {
                line.X1 = horizontalRight ? x : x - length;
                line.X2 = horizontalRight ? x + length : x;
                line.Y1 = line.Y2 = verticalDown ? y + lineOffset : y - lineOffset;

                double lx = (line.X1 + line.X2) / 2 - textWidth / 2;
                double ly = verticalDown ? y - textHeight : y;
                Canvas.SetLeft(label, lx);
                Canvas.SetTop(label, ly);
            }
            else
            {
                line.Y1 = verticalDown ? y : y - length;
                line.Y2 = verticalDown ? y + length : y;
                line.X1 = line.X2 = horizontalRight ? x + lineOffset : x - lineOffset;

                double lx = horizontalRight ? x - textHeight : x;
                double ly = (line.Y1 + line.Y2) / 2 + textWidth / 2;
                label.RenderTransform = new RotateTransform(-90);
                label.RenderTransformOrigin = new Point(0, 0);
                Canvas.SetLeft(label, lx);
                Canvas.SetTop(label, ly);
            }
        }

    }
}
