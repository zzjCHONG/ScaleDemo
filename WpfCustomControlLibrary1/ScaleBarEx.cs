using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace WpfCustomControlLibrary1
{
    public class ScaleBarEx : ContentControl
    {
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            MainPanel = Template.FindName(NamePartMainPanel, this) as Panel;
            Scroll = Template.FindName(NamePartScrollView, this) as ScrollViewer;
            Viewbox = Template.FindName(NamePartViewBox, this) as Viewbox;
            Canvas = Template.FindName(NamePartCanvas, this) as Canvas;

            OnViewerLoad();
        }

        static ScaleBarEx()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ScaleBarEx), new FrameworkPropertyMetadata(typeof(ScaleBarEx)));
        }

        ~ScaleBarEx()
        {
            OnUnload();
        }

        private Panel? MainPanel;

        private ScrollViewer? Scroll;

        private Viewbox? Viewbox;

        private Canvas? Canvas;

        public const string NamePartMainPanel = "PART_MAIN_PANEL";

        public const string NamePartScrollView = "PART_SCROLL";

        public const string NamePartViewBox = "PART_VIEWBOX";

        public const string NamePartCanvas = "PART_CANVAS";

        public const string NamePartImage = "PART_IMAGEMAIN";

        #region Render Size Info

        double DefaultImagePanelScale = 0;
        (double Width, double Height) DefaultImageSize;

        public static readonly DependencyProperty ImagePanelScaleProperty = DependencyProperty.Register(
            nameof(ImagePanelScale), typeof(double), typeof(ScaleBarEx), new PropertyMetadata((double)-1, OnImagePanelScale));

        private static void OnImagePanelScale(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is not ScaleBarEx ex || e.NewValue is not double n || n <= 0) return;
            if (ex.Viewbox is null) return;

            ex.Viewbox.Height = ex.DefaultImageSize.Height * ex.ImagePanelScale;
            ex.Viewbox.Width = ex.DefaultImageSize.Width * ex.ImagePanelScale;
        }

        [Browsable(true)]
        [Category("SizeInfo")]
        [ReadOnly(true)]
        public double ImagePanelScale
        {
            get => (double)GetValue(ImagePanelScaleProperty);
            set => SetValue(ImagePanelScaleProperty, value);
        }

        private void TileImage()
        {
            ImagePanelScale = DefaultImagePanelScale;

            if (Scroll is null) return;

            Scroll.ScrollToVerticalOffset(0);
            Scroll.ScrollToHorizontalOffset(0);
        }

        private void UpdateImageInfo()
        {
            if (ImageSource is null) return;

            DefaultImagePanelScale = Math.Min(ActualWidth / ImageSource.Width,
                ActualHeight / ImageSource.Height);

            DefaultImageSize = (ImageSource.Width, ImageSource.Height);
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);

            UpdateImageInfo();
            TileImage();
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);

            UpdateImageInfo();
        }

        private Point _cursor = new(-1, -1);

        private void OnZoomChanged(object sender, MouseWheelEventArgs e)
        {
            var oldScale = ImagePanelScale;
            if (oldScale <= 0) return;

            var scale = Math.Log10(ImagePanelScale / DefaultImagePanelScale);

            scale = (scale <= 0 ? 0.1 : Math.Pow(10, Math.Floor(scale))) * DefaultImagePanelScale;

            //e.Delta 小于或等于 0，表示滚轮向下滚动，进行缩小操作
            if (e.Delta <= 0)
            {
                ImagePanelScale -= DefaultImagePanelScale * scale;

                // 最小为缩小10倍
                if (ImagePanelScale <= DefaultImagePanelScale * 0.1)
                    ImagePanelScale = DefaultImagePanelScale * 0.1;
            }
            else
            {
                ImagePanelScale += DefaultImagePanelScale * scale;
            }
            if (ImagePanelScale <= DefaultImagePanelScale) return;

            var transform = ImagePanelScale / oldScale;
            var pos = e.GetPosition(Viewbox);
            var target = new Point(pos.X * transform, pos.Y * transform);
            var offset = target - pos;

            Scroll!.ScrollToHorizontalOffset(Scroll.HorizontalOffset + offset.X);
            Scroll!.ScrollToVerticalOffset(Scroll.VerticalOffset + offset.Y);
        }

        private void OnPreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            //PreviewMouseDown-常用于需要拦截事件的场景，比如实现自定义行为、阻止某些默认行为（例如在某些条件下阻止进一步的事件传递）等
            //MouseDown-通常用于处理实际的鼠标点击事件，例如启动某个操作或更新 UI 状态。这个事件通常在确认用户已经点击了目标元素后处理

            _cursor = e.GetPosition(this);
        }

        private void OnPreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            _cursor = new(-1, -1);
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton != MouseButtonState.Pressed) return;
            if (_cursor.X < 0 || _cursor.Y < 0) return;

            var pos = e.GetPosition(this);
            var offset = pos - _cursor;
            _cursor = pos;

            Scroll!.ScrollToHorizontalOffset(Scroll.HorizontalOffset - offset.X);
            Scroll.ScrollToVerticalOffset(Scroll.VerticalOffset - offset.Y);
        }

        private void OnViewerLoad()
        {
            MainPanel!.MouseWheel += OnZoomChanged;
            Scroll!.MouseMove += OnMouseMove;
            Scroll.PreviewMouseDown += OnPreviewMouseDown; ;
            Scroll.PreviewMouseUp += OnPreviewMouseUp;
        }

        private void OnUnload()
        {
            MainPanel!.MouseWheel -= OnZoomChanged;
            Scroll!.MouseMove -= OnMouseMove;
            Scroll.PreviewMouseDown -= OnPreviewMouseDown; ;
            Scroll.PreviewMouseUp -= OnPreviewMouseUp;
        }

        #endregion

        #region ImageSourceProperty

        public static readonly DependencyProperty ImageSourceProperty =
            DependencyProperty.Register("ImageSource", typeof(ImageSource), typeof(ScaleBarEx), new PropertyMetadata(null, (o, p) =>
            {
                //ImageSource变化时回调，平铺对象
                //o 是更改了属性的对象，p 是一个包含旧值和新值的 DependencyPropertyChangedEventArgs 对象

                if (o is not ScaleBarEx ex) return;
                ex.UpdateImageInfo();

                if (p.OldValue is not BitmapSource s1)
                {
                    ex.TileImage();
                    return;
                }

                if (p.NewValue is not BitmapSource s2)
                    return;

                //差异超过0.001，则触发
                if (Math.Abs(s1.Width - s2.Width) > 0.001
                    || Math.Abs(s1.Height - s2.Height) > 0.001) ex.TileImage();

            }));

        public ImageSource ImageSource
        {
            get => (ImageSource)GetValue(ImageSourceProperty);
            set => SetValue(ImageSourceProperty, value);
        }

        #endregion
    }
}