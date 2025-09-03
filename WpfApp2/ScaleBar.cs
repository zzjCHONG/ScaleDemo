using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace WpfApp2
{
    public class ScaleBar : Canvas
    {
        public double PixelsPerUnit { get; set; } = 10; // 每单位多少像素，例如 10px = 1μm
        public double BarLengthUnits { get; set; } = 50; // 比例尺的实际长度（单位）
        public bool IsVertical { get; set; } = false; // 水平/垂直
        public string Unit { get; set; } = "μm"; // 单位
        public ScaleBarPosition Position { get; set; } = ScaleBarPosition.BottomRight;

        private Rectangle _background;
        private Line _bar;
        private TextBlock _label;

        public ScaleBar()
        {
            _background = new Rectangle
            {
                Fill = Brushes.Black,
                RadiusX = 3,
                RadiusY = 3
            };
            _bar = new Line
            {
                Stroke = Brushes.White,
                StrokeThickness = 3
            };
            _label = new TextBlock
            {
                Foreground = Brushes.White,
                FontSize = 14
            };

            this.Children.Add(_background);
            this.Children.Add(_bar);
            this.Children.Add(_label);
        }

        public void UpdateSize(Size imageSize)
        {
            this.Width = imageSize.Width;
            this.Height = imageSize.Height;

            double barLengthPixels = BarLengthUnits * PixelsPerUnit;

            if (IsVertical)
            {
                _bar.X1 = 0;
                _bar.Y1 = 0;
                _bar.X2 = 0;
                _bar.Y2 = barLengthPixels;
                _label.Text = $"{BarLengthUnits}{Unit}";

                _background.Width = _label.ActualWidth + 10;
                _background.Height = barLengthPixels + _label.ActualHeight + 10;
            }
            else
            {
                _bar.X1 = 0;
                _bar.Y1 = 0;
                _bar.X2 = barLengthPixels;
                _bar.Y2 = 0;
                _label.Text = $"{BarLengthUnits}{Unit}";

                _background.Width = barLengthPixels + _label.ActualWidth + 10;
                _background.Height = _label.ActualHeight + 10;
            }

            PositionElements(imageSize, barLengthPixels);
        }

        private void PositionElements(Size imageSize, double barLengthPixels)
        {
            double margin = 20;

            double x = 0, y = 0;
            switch (Position)
            {
                case ScaleBarPosition.BottomRight:
                    x = imageSize.Width - _background.Width - margin;
                    y = imageSize.Height - _background.Height - margin;
                    break;
                case ScaleBarPosition.BottomLeft:
                    x = margin;
                    y = imageSize.Height - _background.Height - margin;
                    break;
                case ScaleBarPosition.TopRight:
                    x = imageSize.Width - _background.Width - margin;
                    y = margin;
                    break;
                case ScaleBarPosition.TopLeft:
                    x = margin;
                    y = margin;
                    break;
            }

            Canvas.SetLeft(_background, x);
            Canvas.SetTop(_background, y);

            if (IsVertical)
            {
                Canvas.SetLeft(_bar, x + _background.Width / 2);
                Canvas.SetTop(_bar, y + 5);

                Canvas.SetLeft(_label, x + (_background.Width - _label.ActualWidth) / 2);
                Canvas.SetTop(_label, y + barLengthPixels + 5);
            }
            else
            {
                Canvas.SetLeft(_bar, x + 5);
                Canvas.SetTop(_bar, y + _label.ActualHeight + 2);

                Canvas.SetLeft(_label, x + barLengthPixels + 8);
                Canvas.SetTop(_label, y + 2);
            }
        }
    }

    public enum ScaleBarPosition
    {
        TopLeft,
        TopRight,
        BottomLeft,
        BottomRight
    }
}
