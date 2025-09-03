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

namespace WpfApp2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            var scaleBar = new ScaleBar
            {
                PixelsPerUnit = 100,       // 设置像素/单位
                BarLengthUnits = 20,     // 20 μm
                IsVertical = true,      // false = 水平, true = 垂直
                Position = ScaleBarPosition.TopLeft,
                Unit = "μm"
            };

            MyCanvas.Children.Add(scaleBar);

            // 假设 imageSize 来自实际图像
            scaleBar.UpdateSize(new Size(800, 600));
        }
    }
}