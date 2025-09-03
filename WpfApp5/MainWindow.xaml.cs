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
            if (MainImage == null) return;
            if (MainImage.Source == null) return;

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

            // 背景
            Brush bgBrush = Brushes.Gray;
            if (BackgroundBox.SelectedItem is ComboBoxItem bgItem)
            {
                bgBrush = bgItem.Content.ToString() switch
                {
                    "白色" => Brushes.White,
                    "黑色" => Brushes.Black,
                    "透明" => Brushes.Transparent,
                    _ => Brushes.Gray,
                };
            }
            ImageCanvas.Background = bgBrush;

            // 字体
            FontWeight fontWeight = FontWeights.Normal;
            if (FontWeightBox.SelectedItem is ComboBoxItem fwItem && fwItem.Content.ToString() == "加粗")
                fontWeight = FontWeights.Bold;

            bool showFont = true;
            if (FontVisibilityBox.SelectedItem is ComboBoxItem fvItem && fvItem.Content.ToString() == "隐藏")
                showFont = false;

            string fontFamily = "Segoe UI";
            if (FontFamilyBox.SelectedItem is ComboBoxItem ffItem)
                fontFamily = ffItem.Content.ToString();

            double margin = 30;
            double x = margin, y = margin;
            bool horizontalRight = true;
            bool verticalDown = true;

            if (PositionBox.SelectedItem is ComboBoxItem pos)
            {
                switch (pos.Content.ToString())
                {
                    case "左上": x = margin; y = margin; horizontalRight = true; verticalDown = true; break;
                    case "右上": x = MainImage.ActualWidth - margin; y = margin; horizontalRight = false; verticalDown = true; break;
                    case "左下": x = margin; y = MainImage.ActualHeight - margin; horizontalRight = true; verticalDown = false; break;
                    case "右下": x = MainImage.ActualWidth - margin; y = MainImage.ActualHeight - margin; horizontalRight = false; verticalDown = false; break;
                }
            }

            void DrawScale(bool horizontal, int length)
            {
                double lineOffset = lineWidth / 2;
                double textOffset = fontSize + 2;

                // 用于测量文字尺寸
                var formattedText = new FormattedText(
                    $"{length} px",
                    CultureInfo.CurrentCulture,
                    FlowDirection.LeftToRight,
                    new Typeface(fontFamily),
                    fontSize,
                    textBrush,
                    VisualTreeHelper.GetDpi(this).PixelsPerDip);

                double textWidth = formattedText.Width;
                double textHeight = formattedText.Height;

                double rectX, rectY, rectWidth, rectHeight;

                if (horizontal)
                {
                    rectWidth = length + 10;
                    rectHeight = lineWidth + textHeight + 10;

                    rectX = horizontalRight ? x - 5 : x - length - 5;
                    rectY = verticalDown ? y - lineOffset - textHeight - 5 : y - lineOffset - 5;
                }
                else
                {
                    // 关键：旋转后，文字的宽度是textHeight，高度是textWidth
                    rectWidth = lineWidth + textHeight + 10;
                    rectHeight = length + textWidth + 10;

                    // 调整背景位置以适应文字的偏移
                    rectX = horizontalRight ? x - lineOffset - textHeight - 5 : x - lineOffset - 5;
                    rectY = verticalDown ? y - 5 -20 : y - length - textWidth - 5+20;
                }

                // 背景颜色
                Brush bgBrush = Brushes.Transparent;
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

                // 绘制背景矩形
                var rect = new Rectangle
                {
                    Width = rectWidth,
                    Height = rectHeight,
                    Fill = bgBrush
                };
                Canvas.SetLeft(rect, rectX);
                Canvas.SetTop(rect, rectY);
                OverlayCanvas.Children.Add(rect);

                // 绘制线条
                var line = new Line
                {
                    Stroke = textBrush,
                    StrokeThickness = lineWidth
                };
                OverlayCanvas.Children.Add(line);

                // 绘制文字
                var label = new TextBlock
                {
                    Text = $"{length} pixel",
                    FontSize = fontSize,
                    Foreground = textBrush,
                    FontWeight = fontWeight,
                    FontFamily = new FontFamily(fontFamily),
                    Visibility = showFont ? Visibility.Visible : Visibility.Collapsed
                };
                OverlayCanvas.Children.Add(label);

                if (horizontal)
                {
                    if (horizontalRight) { line.X1 = x; line.X2 = x + length; }
                    else { line.X1 = x - length; line.X2 = x; }
                    line.Y1 = line.Y2 = verticalDown ? y - lineOffset : y + lineOffset;

                    double lx = (line.X1 + line.X2) / 2 - textWidth / 2-50;
                    double ly = verticalDown ? y - textHeight - lineOffset : y + lineOffset + 2;
                    Canvas.SetLeft(label, lx);
                    Canvas.SetTop(label, ly);
                }
                else
                {
                    double xBase = horizontalRight ? x : x;
                    line.Y1 = verticalDown ? y : y - length;
                    line.Y2 = verticalDown ? y + length : y;
                    line.X1 = line.X2 = horizontalRight ? xBase + lineOffset : xBase - lineOffset;

                    label.RenderTransform = new RotateTransform(-90);
                    label.RenderTransformOrigin = new Point(0, 0);

                    double lx = horizontalRight ? xBase - textHeight - lineOffset : xBase + lineOffset + 2;
                    double ly = (line.Y1 + line.Y2) / 2 - textWidth / 2 + 50;
                    Canvas.SetLeft(label, lx);
                    Canvas.SetTop(label, ly);
                }
            }
            string drawMode = (DrawModeBox.SelectedItem as ComboBoxItem)?.Content.ToString() ?? "共存";
            switch (drawMode)
            {
                case "水平": DrawScale(true, scaleWidth); break;
                case "竖直": DrawScale(false, scaleHeight); break;
                case "共存": DrawScale(true, scaleWidth); DrawScale(false, scaleHeight); break;
            }
        }
    }
}
