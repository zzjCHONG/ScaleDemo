using System.IO;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WpfApp4
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            LoadSampleImage();
        }
        private void LoadSampleImage()
        {
            BitmapImage bmp = new BitmapImage(new Uri("E:\\imagesource\\1_DAPI-405nm_Origin.tif", UriKind.Absolute));
            MainImage.Source = bmp;
        }

        public class OverlayItem
        {
            public string Type { get; set; }
            public double Left { get; set; }
            public double Top { get; set; }
            public double Width { get; set; }
            public double Height { get; set; }
            public string Color { get; set; }
        }

        private void DrawRuler_Click(object sender, RoutedEventArgs e)
        {
            Rectangle ruler = new Rectangle
            {
                Width = 200,
                Height = 5,
                Fill = Brushes.Red,
                Stroke = Brushes.Black,
                StrokeThickness = 1
            };
            Canvas.SetLeft(ruler, 50);
            Canvas.SetTop(ruler, 50);
            OverlayCanvas.Children.Add(ruler);
        }

        // 保存合成图片（原图+Overlay）
        private void SaveCombinedImage_Click(object sender, RoutedEventArgs e)
        {
            if (MainImage.Source == null) return;

            // 创建 RenderTargetBitmap
            int width = (int)MainImage.Source.Width;
            int height = (int)MainImage.Source.Height;

            RenderTargetBitmap rtb = new RenderTargetBitmap(width, height, 96, 96, PixelFormats.Pbgra32);

            // 创建 Visual
            DrawingVisual dv = new DrawingVisual();
            using (DrawingContext dc = dv.RenderOpen())
            {
                // 画原图
                dc.DrawImage(MainImage.Source, new Rect(0, 0, width, height));

                // 画 OverlayCanvas
                foreach (UIElement child in OverlayCanvas.Children)
                {
                    if (child is Rectangle rect)
                    {
                        Brush fill = rect.Fill;
                        Pen pen = new Pen(rect.Stroke, rect.StrokeThickness);
                        double left = Canvas.GetLeft(rect);
                        double top = Canvas.GetTop(rect);
                        dc.DrawRectangle(fill, pen, new Rect(left, top, rect.Width, rect.Height));
                    }
                }
            }

            rtb.Render(dv);

            BitmapEncoder encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(rtb));
            using (FileStream fs = new FileStream("combined.png", FileMode.Create))
                encoder.Save(fs);

            MessageBox.Show("合成图片保存成功！");
        }

        private void SaveOverlayData_Click(object sender, RoutedEventArgs e)
        {
            List<OverlayItem> items = new List<OverlayItem>();
            foreach (var child in OverlayCanvas.Children)
            {
                if (child is Rectangle rect)
                {
                    items.Add(new OverlayItem
                    {
                        Type = "Rectangle",
                        Left = Canvas.GetLeft(rect),
                        Top = Canvas.GetTop(rect),
                        Width = rect.Width,
                        Height = rect.Height,
                        Color = rect.Fill.ToString()
                    });
                }
            }

            File.WriteAllText("overlay.json", JsonSerializer.Serialize(items));
            MessageBox.Show("Overlay 数据保存成功!");
        }

        private void LoadOverlayData_Click(object sender, RoutedEventArgs e)
        {
            if (!File.Exists("overlay.json"))
            {
                MessageBox.Show("没有找到 overlay.json 文件");
                return;
            }

            var items = JsonSerializer.Deserialize<List<OverlayItem>>(File.ReadAllText("overlay.json"));
            OverlayCanvas.Children.Clear();

            foreach (var item in items)
            {
                if (item.Type == "Rectangle")
                {
                    Rectangle rect = new Rectangle
                    {
                        Width = item.Width,
                        Height = item.Height,
                        Fill = (SolidColorBrush)(new BrushConverter().ConvertFromString(item.Color))
                    };
                    Canvas.SetLeft(rect, item.Left);
                    Canvas.SetTop(rect, item.Top);
                    OverlayCanvas.Children.Add(rect);
                }
            }
            MessageBox.Show("Overlay 数据加载完成!");
        }
    }
}
