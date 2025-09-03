using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            // 示例：1 µm = 5 px，绘制一个 20 µm 的比例尺
            DrawScaleBar(pixelSize: 5, scaleLengthUm: 20, posX: 50, posY: 500, useSerif: true);
        }

        private void DrawScaleBar(double pixelSize, double scaleLengthUm,double posX, double posY, bool useSerif)
        {
            // 计算比例尺像素长度
            double scaleLengthPx = scaleLengthUm * pixelSize;

            // 比例尺矩形
            var bar = new Rectangle
            {
                Width = scaleLengthPx,
                Height = 6,   // 线条厚度
                Fill = Brushes.White
            };
            Canvas.SetLeft(bar, posX);
            Canvas.SetTop(bar, posY);

            // 比例尺文字
            var text = new TextBlock
            {
                Text = $"{scaleLengthUm} µm",
                Foreground = Brushes.White,
                FontSize = 18,
                FontFamily = useSerif ? new FontFamily("Times New Roman") : new FontFamily("Segoe UI"),
                FontWeight = FontWeights.Bold
            };
            Canvas.SetLeft(text, posX);
            Canvas.SetTop(text, posY - 25);

            // 添加到画布
            ImageCanvas.Children.Add(bar);
            ImageCanvas.Children.Add(text);
        }
    }
}