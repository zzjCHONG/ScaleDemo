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
                    "灰色" => Brushes.Gray,
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

            double margin = 50;
            double x = margin, y = margin;
            bool horizontalRight = true;
            bool verticalDown = true;

            string position = (PositionBox.SelectedItem as ComboBoxItem)?.Content.ToString();
            if (position != null)
            {
                switch (position)
                {
                    case "左上": x = margin; y = margin; horizontalRight = true; verticalDown = true; break;
                    case "右上": x = MainImage.ActualWidth - margin; y = margin; horizontalRight = false; verticalDown = true; break;
                    case "左下": x = margin; y = MainImage.ActualHeight - margin; horizontalRight = true; verticalDown = false; break;
                    case "右下": x = MainImage.ActualWidth - margin; y = MainImage.ActualHeight - margin; horizontalRight = false; verticalDown = false; break;
                }
            }

            string drawMode = (DrawModeBox.SelectedItem as ComboBoxItem)?.Content.ToString() ?? "共存";

            // 计算水平和垂直方向的文本尺寸
            FormattedText horizontalFormattedText = new(
                $"{scaleWidth} px",
                CultureInfo.CurrentCulture,
                FlowDirection.LeftToRight,
                new Typeface(fontFamily),
                fontSize,
                textBrush,
                VisualTreeHelper.GetDpi(this).PixelsPerDip);

            FormattedText verticalFormattedText = new(
                $"{scaleHeight} px",
                CultureInfo.CurrentCulture,
                FlowDirection.LeftToRight,
                new Typeface(fontFamily),
                fontSize,
                textBrush,
                VisualTreeHelper.GetDpi(this).PixelsPerDip);

            double horizontalTextWidth = horizontalFormattedText.Width;
            double horizontalTextHeight = horizontalFormattedText.Height;
            double verticalTextWidth = verticalFormattedText.Width;
            double verticalTextHeight = verticalFormattedText.Height;

            // 计算背景矩形尺寸
            double horizontalRectWidth = scaleWidth + 10;
            double horizontalRectHeight = lineWidth + horizontalTextHeight + 10;
            double verticalRectWidth = lineWidth + verticalTextHeight + 10;
            double verticalRectHeight = scaleHeight + verticalTextWidth + 10;

            // 根据位置调整坐标
            double horizontalX = x, horizontalY = y;
            double verticalX = x, verticalY = y;

            if (position == "右上")
            {
                horizontalX = x - ((drawMode == "共存" || drawMode == "水平") ? horizontalRectWidth : 0);
                verticalX = x;
                horizontalY = y;
                verticalY = y + (horizontalTextHeight / 2);
            }
            else if (position == "左下")
            {
                horizontalX = x;
                verticalX = x + (verticalTextHeight / 2);
                horizontalY = y - ((drawMode == "共存" || drawMode == "竖直") ? verticalRectHeight : 0);
                verticalY = y;
            }
            else if (position == "右下")
            {
                horizontalX = x - ((drawMode == "共存" || drawMode == "水平") ? horizontalRectWidth : 0);
                verticalX = x;
                horizontalY = y - ((drawMode == "共存" || drawMode == "竖直") ? verticalRectHeight : 0);
                verticalY = y;
            }
            else if (position == "左上")
            {
                horizontalX = x;
                verticalX = x + (verticalTextHeight / 2);
                horizontalY = y;
                verticalY = y + (horizontalTextHeight / 2);
            }

            // 绘制水平比例尺
            if (drawMode == "共存" || drawMode == "水平")
            {
                DrawScale(true, scaleWidth, horizontalX, horizontalY, horizontalRight, verticalDown, textBrush, bgBrush, fontWeight, showFont, fontFamily, fontSize, lineWidth, horizontalTextWidth, horizontalTextHeight);
            }

            // 绘制垂直比例尺
            if (drawMode == "共存" || drawMode == "竖直")
            {
                DrawScale(false, scaleHeight, verticalX, verticalY, horizontalRight, verticalDown, textBrush, bgBrush, fontWeight, showFont, fontFamily, fontSize, lineWidth, verticalTextWidth, verticalTextHeight);
            }
        }

        private void DrawScale(bool horizontal, int length, double x, double y, bool horizontalRight, bool verticalDown, Brush textBrush, Brush bgBrush, FontWeight fontWeight, bool showFont, string fontFamily, int fontSize, int lineWidth, double textWidth, double textHeight)
        {
            double lineOffset = lineWidth / 2;
            double rectX, rectY, rectWidth, rectHeight;

            // 修改 DrawScale 方法，调整背景矩形尺寸
            if (horizontal)
            {
                rectWidth = length + 10;
                rectHeight = lineWidth + (showFont ? textHeight : 0) + 10;
                rectX = horizontalRight ? x - 5 : x - length - 5;
                rectY = verticalDown ? y + lineOffset - (showFont ? textHeight : 0) - 5: y + lineOffset - 5;
            }
            else
            {
                rectWidth = lineWidth + (showFont ? textHeight : 0) + 10;
                rectHeight = length + (showFont ? textWidth : 0) + 10;
                rectX = horizontalRight ? x - lineOffset - (showFont ? textHeight : 0) - 5 : x - lineOffset - 5;
                rectY = verticalDown? y - 5 : y - length - (showFont ? textWidth : 0) - 5 ;
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
                line.X1 = horizontalRight ? x : x - length;
                line.X2 = horizontalRight ? x + length : x;
                line.Y1 = line.Y2 = verticalDown ? y + lineOffset : y - lineOffset;

                double lx = (line.X1 + line.X2) / 2 - textWidth / 4 * 3;
                double ly = verticalDown ? y - textHeight : y ;

                Canvas.SetLeft(label, lx);
                Canvas.SetTop(label, ly);
            }
            else
            {
                line.Y1 = verticalDown ? y : y - length;
                line.Y2 = verticalDown ? y + length : y;
                line.X1 = line.X2 = horizontalRight ? x + lineOffset : x - lineOffset;

                double lx = horizontalRight ? x - textHeight : x ;
                double ly = (line.Y1 + line.Y2) / 2 + textWidth / 4 * 3;

                label.RenderTransform = new RotateTransform(-90);
                label.RenderTransformOrigin = new Point(0, 0);

                Canvas.SetLeft(label, lx);
                Canvas.SetTop(label, ly);
            }
        }

    }
}
