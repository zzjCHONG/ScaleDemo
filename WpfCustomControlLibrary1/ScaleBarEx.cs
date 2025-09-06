using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes; // 引入 Line 和 Rectangle 控件

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
            ImageMain = Template.FindName(NamePartImage, this) as Image; // 获取Image控件

            OnViewerLoad();
            // 第一次应用模板时，更新图片信息并绘制比例尺
            // 这会设置Canvas的尺寸并调用 UpdateScaleBarElements
            UpdateImageInfo();
        }

        // 移除 OnRender 方法中关于比例尺的绘制。
        // 现在比例尺是Canvas的子元素，由Canvas自身渲染。
        // OnRender 仍然会被调用，但不再用于绘制比例尺本身。
        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);
            // 这里可以放置与控件自身渲染相关的逻辑，例如绘制边框或调试信息
            // 但不应再绘制比例尺。
        }

        static ScaleBarEx()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ScaleBarEx), new FrameworkPropertyMetadata(typeof(ScaleBarEx)));
        }

        // 注意：析构函数在.NET中不保证何时调用，通常不用于资源清理。
        // 对于WPF控件，更推荐在 Loaded/Unloaded 事件中管理事件订阅。
        ~ScaleBarEx()
        {
            OnUnload();
        }

        #region Name

        private Panel? MainPanel;
        private ScrollViewer? Scroll;
        private Viewbox? Viewbox;
        private Canvas? Canvas;
        private Image? ImageMain; // 对 Image 控件的引用

        public const string NamePartMainPanel = "PART_MAIN_PANEL";
        public const string NamePartScrollView = "PART_SCROLL";
        public const string NamePartViewBox = "PART_VIEWBOX";
        public const string NamePartCanvas = "PART_CANVAS";
        public const string NamePartImage = "PART_IMAGEMAIN";

        #endregion

        #region Render Size Info

        double DefaultImagePanelScale = 0; // 图片在控件内“平铺”时的缩放比例
        (double Width, double Height) DefaultImageSize; // 图片的原始尺寸

        public static readonly DependencyProperty ImagePanelScaleProperty = DependencyProperty.Register(
            nameof(ImagePanelScale), typeof(double), typeof(ScaleBarEx), new PropertyMetadata((double)-1, OnImagePanelScaleChanged));

        private static void OnImagePanelScaleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is not ScaleBarEx ex || e.NewValue is not double n || n <= 0) return;
            if (ex.Viewbox is null) return;

            // 设置 Viewbox 的 Width 和 Height，这会控制 Viewbox 内部内容的缩放。
            // 由于 Canvas 与 Image 在同一个 Grid 中，Canvas 上的比例尺元素会随之缩放。
            ex.Viewbox.Width = ex.DefaultImageSize.Width * ex.ImagePanelScale;
            ex.Viewbox.Height = ex.DefaultImageSize.Height * ex.ImagePanelScale;

            // 理论上，Viewbox的缩放会自动处理Canvas子元素的缩放，
            // 所以这里不需要再次调用 UpdateScaleBarElements。
        }

        [Browsable(true)]
        [Category("SizeInfo")]
        [ReadOnly(true)]
        public double ImagePanelScale
        {
            get => (double)GetValue(ImagePanelScaleProperty);
            set => SetValue(ImagePanelScaleProperty, value);
        }

        /// <summary>
        /// 将图片缩放至默认平铺大小并滚动到顶部/左侧。
        /// </summary>
        private void TileImage()
        {
            ImagePanelScale = DefaultImagePanelScale;

            if (Scroll is null) return;

            Scroll.ScrollToVerticalOffset(0);
            Scroll.ScrollToHorizontalOffset(0);
        }

        /// <summary>
        /// 更新图片尺寸信息，并设置 Canvas 的尺寸以匹配图片原始尺寸。
        /// </summary>
        private void UpdateImageInfo()
        {
            // 如果没有图片源或图片尺寸无效，则清空相关信息并移除比例尺
            if (ImageSource is null || ImageSource.Width == 0 || ImageSource.Height == 0)
            {
                DefaultImagePanelScale = 0;
                DefaultImageSize = (0, 0);
                if (Canvas != null)
                {
                    Canvas.Width = 0;
                    Canvas.Height = 0;
                }
                UpdateScaleBarElements(); // 清除比例尺
                return;
            }

            // 计算图片在控件内“平铺”时的初始缩放比例
            DefaultImagePanelScale = Math.Min(ActualWidth / ImageSource.Width,
                ActualHeight / ImageSource.Height);

            // 存储图片的原始尺寸
            DefaultImageSize = (ImageSource.Width, ImageSource.Height);

            // 关键步骤：设置 Canvas 的 Width 和 Height 与 ImageSource 的原始尺寸一致。
            // 这样，Canvas 内部的坐标系就与图片的像素坐标系完全匹配。
            if (Canvas != null)
            {
                Canvas.Width = ImageSource.Width;
                Canvas.Height = ImageSource.Height;
            }

            // 图片信息更新后，重新绘制比例尺元素
            UpdateScaleBarElements();
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);

            // 当控件自身尺寸改变时，更新图片信息（包括Canvas尺寸）并重新平铺
            UpdateImageInfo();
            TileImage();
        }

        private Point _cursor = new(-1, -1);

        private void OnZoomChanged(object sender, MouseWheelEventArgs e)
        {
            if (ImageMain == null || ImageSource == null) return;

            var oldScale = ImagePanelScale;
            if (oldScale <= 0) return;

            // 获取鼠标相对于 ImageMain (图片内容) 的坐标。
            // 这样无论图片如何缩放，这个坐标都对应图片上的同一个“像素点”。
            var pos = e.GetPosition(ImageMain);

            // 计算缩放增量，采用对数方式使缩放更平滑
            var scaleFactor = Math.Log10(ImagePanelScale / DefaultImagePanelScale);
            scaleFactor = (scaleFactor <= 0 ? 0.1 : Math.Pow(10, Math.Floor(scaleFactor))) * DefaultImagePanelScale;

            // e.Delta 小于或等于 0，表示滚轮向下滚动，进行缩小操作
            if (e.Delta <= 0)
            {
                ImagePanelScale -= DefaultImagePanelScale * scaleFactor;

                // 设置最小缩放比例，例如最小为平铺时的 0.1 倍
                if (ImagePanelScale <= DefaultImagePanelScale * 0.1)
                    ImagePanelScale = DefaultImagePanelScale * 0.1;
            }
            else // 滚轮向上滚动，进行放大操作
            {
                ImagePanelScale += DefaultImagePanelScale * scaleFactor;
            }

            // 如果缩放比例没有实际变化，则不进行滚动调整
            if (Math.Abs(ImagePanelScale - oldScale) < 0.0001) return;

            // 计算新的缩放比例与旧的缩放比例之比
            var transform = ImagePanelScale / oldScale;

            // 计算鼠标位置在新的缩放下的目标点
            var target = new Point(pos.X * transform, pos.Y * transform);

            // 计算滚动偏移量：目标点与当前鼠标点之间的差值
            var offset = target - pos;

            // 调整 ScrollViewer 的滚动位置
            Scroll!.ScrollToHorizontalOffset(Scroll.HorizontalOffset + offset.X);
            Scroll!.ScrollToVerticalOffset(Scroll.VerticalOffset + offset.Y);
        }

        private void OnPreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (ImageMain == null) return;
            // 记录鼠标按下时相对于 ImageMain 的坐标
            _cursor = e.GetPosition(ImageMain);
        }

        private void OnPreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            _cursor = new(-1, -1);
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton != MouseButtonState.Pressed) return;
            if (_cursor.X < 0 || _cursor.Y < 0) return; // 鼠标未按下或初始位置无效
            if (ImageMain == null) return;

            // 获取当前鼠标相对于 ImageMain 的坐标
            var pos = e.GetPosition(ImageMain);
            // 计算鼠标移动的偏移量
            var offset = pos - _cursor;
            // 更新 _cursor 为当前位置，为下一次移动做准备
            _cursor = pos;

            // 调整 ScrollViewer 的滚动位置以实现拖动
            // 注意：这里是减去 offset，因为鼠标向右移动，内容应该向左移动
            Scroll!.ScrollToHorizontalOffset(Scroll.HorizontalOffset - offset.X);
            Scroll.ScrollToVerticalOffset(Scroll.VerticalOffset - offset.Y);
        }

        private void OnViewerLoad()
        {
            // 订阅事件，确保控件加载后功能可用
            if (MainPanel != null) MainPanel.MouseWheel += OnZoomChanged;
            if (Scroll != null)
            {
                Scroll.MouseMove += OnMouseMove;
                Scroll.PreviewMouseDown += OnPreviewMouseDown;
                Scroll.PreviewMouseUp += OnPreviewMouseUp;
            }
        }

        private void OnUnload()
        {
            // 取消事件订阅，防止内存泄漏
            if (MainPanel != null) MainPanel.MouseWheel -= OnZoomChanged;
            if (Scroll != null)
            {
                Scroll.MouseMove -= OnMouseMove;
                Scroll.PreviewMouseDown -= OnPreviewMouseDown;
                Scroll.PreviewMouseUp -= OnPreviewMouseUp;
            }
        }

        #endregion

        #region DependencyProperty

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

        /// <summary>
        /// 比例尺横向长度（像素）
        /// 未设置比例尺模式
        /// </summary>
        public int ScaleLengthWidth
        {
            get { return (int)GetValue(ScaleLengthWidthProperty); }
            set { SetValue(ScaleLengthWidthProperty, value); }
        }

        public static readonly DependencyProperty ScaleLengthWidthProperty =
            DependencyProperty.Register("ScaleLengthWidth", typeof(int), typeof(ScaleBarEx), new FrameworkPropertyMetadata(50, FrameworkPropertyMetadataOptions.AffectsRender, OnRenderInvalidated));

        /// <summary>
        /// 比例尺纵向长度（像素）
        /// 未设置比例尺模式
        /// </summary>
        public int ScaleLengthHeight
        {
            get { return (int)GetValue(ScaleLengthHeightProperty); }
            set { SetValue(ScaleLengthHeightProperty, value); }
        }

        public static readonly DependencyProperty ScaleLengthHeightProperty =
            DependencyProperty.Register("ScaleLengthHeight", typeof(int), typeof(ScaleBarEx), new FrameworkPropertyMetadata(50, FrameworkPropertyMetadataOptions.AffectsRender, OnRenderInvalidated));

        /// <summary>
        /// 字体大小
        /// </summary>
        public new int FontSize
        {
            get { return (int)GetValue(FontSizeProperty); }
            set { SetValue(FontSizeProperty, value); }
        }

        public static new readonly DependencyProperty FontSizeProperty =
            DependencyProperty.Register("FontSize", typeof(int), typeof(ScaleBarEx), new FrameworkPropertyMetadata(14, FrameworkPropertyMetadataOptions.AffectsRender, OnRenderInvalidated));

        /// <summary>
        /// 线条宽度
        /// </summary>
        public int LineWidth
        {
            get { return (int)GetValue(LineWidthProperty); }
            set { SetValue(LineWidthProperty, value); }
        }

        public static readonly DependencyProperty LineWidthProperty =
            DependencyProperty.Register("LineWidth", typeof(int), typeof(ScaleBarEx), new FrameworkPropertyMetadata(4, FrameworkPropertyMetadataOptions.AffectsRender, OnRenderInvalidated));

        /// <summary>
        /// 字体颜色
        /// </summary>
        public Brush TextBrush
        {
            get { return (Brush)GetValue(TextBrushProperty); }
            set { SetValue(TextBrushProperty, value); }
        }

        public static readonly DependencyProperty TextBrushProperty =
            DependencyProperty.Register("TextBrush", typeof(Brush), typeof(ScaleBarEx), new FrameworkPropertyMetadata(Brushes.White, FrameworkPropertyMetadataOptions.AffectsRender, OnRenderInvalidated));

        /// <summary>
        /// 背景颜色
        /// </summary>
        public Brush BackgroundBrush
        {
            get { return (Brush)GetValue(BackgroundBrushProperty); }
            set { SetValue(BackgroundBrushProperty, value); }
        }

        public static readonly DependencyProperty BackgroundBrushProperty =
            DependencyProperty.Register("BackgroundBrush", typeof(Brush), typeof(ScaleBarEx), new FrameworkPropertyMetadata(Brushes.Gray, FrameworkPropertyMetadataOptions.AffectsRender, OnRenderInvalidated));

        /// <summary>
        /// 字体样式
        /// </summary>
        public new FontWeight FontWeight
        {
            get { return (FontWeight)GetValue(FontWeightProperty); }
            set { SetValue(FontWeightProperty, value); }
        }

        public static new readonly DependencyProperty FontWeightProperty =
            DependencyProperty.Register("FontWeight", typeof(FontWeight), typeof(ScaleBarEx), new FrameworkPropertyMetadata(FontWeights.Bold, FrameworkPropertyMetadataOptions.AffectsRender, OnRenderInvalidated));

        /// <summary>
        /// 字体是否可见
        /// </summary>
        public Visibility FontVisibility
        {
            get { return (Visibility)GetValue(FontVisibilityProperty); }
            set { SetValue(FontVisibilityProperty, value); }
        }

        public static readonly DependencyProperty FontVisibilityProperty =
            DependencyProperty.Register("FontVisibility", typeof(Visibility), typeof(ScaleBarEx), new FrameworkPropertyMetadata(Visibility.Visible, FrameworkPropertyMetadataOptions.AffectsRender, OnRenderInvalidated));

        /// <summary>
        /// 字体类型
        /// </summary>
        public new string FontFamily
        {
            get { return (string)GetValue(FontFamilyProperty); }
            set { SetValue(FontFamilyProperty, value); }
        }

        public static new readonly DependencyProperty FontFamilyProperty =
            DependencyProperty.Register("FontFamily", typeof(string), typeof(ScaleBarEx), new FrameworkPropertyMetadata("Segoe UI", FrameworkPropertyMetadataOptions.AffectsRender, OnRenderInvalidated));

        /// <summary>
        /// 摆放位置-四角
        /// </summary>
        public string Position
        {
            get { return (string)GetValue(PositionProperty); }
            set { SetValue(PositionProperty, value); }
        }

        public static readonly DependencyProperty PositionProperty =
            DependencyProperty.Register("Position", typeof(string), typeof(ScaleBarEx), new FrameworkPropertyMetadata("左上", FrameworkPropertyMetadataOptions.AffectsRender, OnRenderInvalidated));

        /// <summary>
        /// 绘制模式（水平/垂直）
        /// </summary>
        public string DrawMode
        {
            get { return (string)GetValue(DrawModeProperty); }
            set { SetValue(DrawModeProperty, value); }
        }

        public static readonly DependencyProperty DrawModeProperty =
            DependencyProperty.Register("DrawMode", typeof(string), typeof(ScaleBarEx), new FrameworkPropertyMetadata("水平", FrameworkPropertyMetadataOptions.AffectsRender, OnRenderInvalidated));

        /// <summary>
        /// 是否已设置比例尺模式（显示形式有区别）
        /// </summary>
        public bool IsSetScaleMode
        {
            get { return (bool)GetValue(IsSetScaleModeProperty); }
            set { SetValue(IsSetScaleModeProperty, value); }
        }

        public static readonly DependencyProperty IsSetScaleModeProperty =
            DependencyProperty.Register("IsSetScaleMode", typeof(bool), typeof(ScaleBarEx), new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.AffectsRender, OnRenderInvalidated));

        /// <summary>
        /// 横向数值显示
        /// 已设置比例尺模式
        /// </summary>
        public double WidthinUnit
        {
            get { return (double)GetValue(WidthinUnitProperty); }
            set { SetValue(WidthinUnitProperty, value); }
        }

        public static readonly DependencyProperty WidthinUnitProperty =
            DependencyProperty.Register("WidthinUnit", typeof(double), typeof(ScaleBarEx), new FrameworkPropertyMetadata(2.0, FrameworkPropertyMetadataOptions.AffectsRender, OnRenderInvalidated));

        /// <summary>
        /// 纵向数值显示
        /// 已设置比例尺模式
        /// </summary>
        public double HeightinUnit
        {
            get { return (double)GetValue(HeightinUnitProperty); }
            set { SetValue(HeightinUnitProperty, value); }
        }

        public static readonly DependencyProperty HeightinUnitProperty =
            DependencyProperty.Register("HeightinUnit", typeof(double), typeof(ScaleBarEx), new FrameworkPropertyMetadata(2.0, FrameworkPropertyMetadataOptions.AffectsRender, OnRenderInvalidated));

        /// <summary>
        /// 比例尺（pixel/unit）
        /// 已设置比例尺模式
        /// </summary>
        public double Scale
        {
            get { return (double)GetValue(ScaleProperty); }
            set { SetValue(ScaleProperty, value); }
        }

        public static readonly DependencyProperty ScaleProperty =
            DependencyProperty.Register("Scale", typeof(double), typeof(ScaleBarEx), new FrameworkPropertyMetadata(50.0, FrameworkPropertyMetadataOptions.AffectsRender, OnRenderInvalidated));

        /// <summary>
        /// 宽高比
        /// 已设置比例尺模式
        /// </summary>
        public double PixelAspectRatio
        {
            get { return (double)GetValue(PixelAspectRatioProperty); }
            set { SetValue(PixelAspectRatioProperty, value); }
        }

        public static readonly DependencyProperty PixelAspectRatioProperty =
            DependencyProperty.Register("PixelAspectRatio", typeof(double), typeof(ScaleBarEx), new FrameworkPropertyMetadata(1.0, FrameworkPropertyMetadataOptions.AffectsRender, OnRenderInvalidated));

        /// <summary>
        /// 单位
        /// </summary>
        public string Unit
        {
            get { return (string)GetValue(UnitProperty); }
            set { SetValue(UnitProperty, value); }
        }

        public static readonly DependencyProperty UnitProperty =
            DependencyProperty.Register("Unit", typeof(string), typeof(ScaleBarEx), new FrameworkPropertyMetadata("μm", FrameworkPropertyMetadataOptions.AffectsRender, OnRenderInvalidated));


        private static void OnRenderInvalidated(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ScaleBarEx)d).InvalidateVisual();
        }

        #endregion

        /// <summary>
        /// 更新 Canvas 上的比例尺 UI 元素。
        /// 此方法会在 ImageSource 改变、控件尺寸改变或比例尺相关属性改变时调用。
        /// </summary>
        private void UpdateScaleBarElements()
        {
            if (Canvas == null) return;

            // 清除 Canvas 上所有现有的比例尺元素
            Canvas.Children.Clear();

            // 如果没有图片源或图片尺寸无效，则不绘制比例尺
            if (ImageSource is null || ImageSource.Width == 0 || ImageSource.Height == 0) return;

            // 获取图片的原始尺寸。
            // 由于 Canvas 的尺寸已设置为与图片原始尺寸一致，
            // 这里的 imageWidth/imageHeight 就是 Canvas 的逻辑尺寸。
            double imageWidth = ImageSource.Width;
            double imageHeight = ImageSource.Height;

            // 比例尺的计算参数
            int scaleX, scaleY;
            double unitX = 0;
            double unitY = 0;

            if (IsSetScaleMode) // 如果是设置了实际单位的比例尺模式
            {
                unitX = WidthinUnit;
                unitY = HeightinUnit;
                var scale = Scale; // pixel/unit (每单位对应的像素数)
                var pixelAspectRatio = PixelAspectRatio; // 像素宽高比

                // 计算在图片原始像素坐标系中的长度
                scaleX = (int)Math.Round(unitX * scale);
                scaleY = (int)Math.Round(unitY * scale * pixelAspectRatio);
            }
            else // 如果是纯像素模式
            {
                // 直接使用设置的像素长度
                scaleX = ScaleLengthWidth;
                scaleY = ScaleLengthHeight;
            }

            int fontSize = FontSize;
            int lineWidth = LineWidth;

            // 颜色和字体样式
            Brush textBrush = TextBrush;
            Brush bgBrush = BackgroundBrush;
            FontWeight fontWeight = FontWeight;
            bool showFont = FontVisibility == Visibility.Visible;
            string fontFamily = FontFamily;

            // 基础位置 (相对于图片原始尺寸的边距)
            double margin = 50; // 边距，单位是图片原始像素
            double x = margin, y = margin; // 默认左上角
            bool horizontalRight = true, verticalDown = true; // 默认水平向右，垂直向下绘制

            string position = Position;
            switch (position)
            {
                case "右上":
                    x = imageWidth - margin;
                    y = margin;
                    horizontalRight = false; // 水平向左绘制
                    verticalDown = true;
                    break;
                case "左下":
                    x = margin;
                    y = imageHeight - margin;
                    horizontalRight = true;
                    verticalDown = false; // 垂直向上绘制
                    break;
                case "右下":
                    x = imageWidth - margin;
                    y = imageHeight - margin;
                    horizontalRight = false; // 水平向左绘制
                    verticalDown = false; // 垂直向上绘制
                    break;
                case "左上":
                default:
                    // 默认值已设置
                    break;
            }

            string texttoFormatX = IsSetScaleMode ? $"{unitX} {Unit}" : $"{scaleX} Pixel";
            string texttoFormatY = IsSetScaleMode ? $"{unitY} {Unit}" : $"{scaleY} Pixel";

            // 计算文本尺寸 (使用 FormattedText 来预估尺寸，以便计算背景框大小)
            // FormattedText 提供了精确的文本测量，即使文本尚未渲染到 UI。
            FormattedText hText = new(texttoFormatX, CultureInfo.CurrentCulture,
                FlowDirection.LeftToRight, new Typeface(fontFamily), fontSize,
                textBrush, VisualTreeHelper.GetDpi(this).PixelsPerDip);

            FormattedText vText = new(texttoFormatY, CultureInfo.CurrentCulture,
                FlowDirection.LeftToRight, new Typeface(fontFamily), fontSize,
                textBrush, VisualTreeHelper.GetDpi(this).PixelsPerDip);

            double hTextWidth = hText.Width, hTextHeight = hText.Height;
            double vTextWidth = vText.Width, vTextHeight = vText.Height; // vTextWidth 是竖直文本旋转前的宽度

            // 根据 DrawMode 绘制水平或垂直比例尺
            string drawMode = DrawMode;
            if (drawMode is "共存" or "水平")
                DrawScaleElement(true, scaleX, x, y, horizontalRight, verticalDown,
                    textBrush, bgBrush, fontWeight, showFont, fontFamily, fontSize, lineWidth, hTextWidth, hTextHeight, texttoFormatX, texttoFormatY);

            if (drawMode is "共存" or "竖直")
                DrawScaleElement(false, scaleY, x, y, horizontalRight, verticalDown,
                    textBrush, bgBrush, fontWeight, showFont, fontFamily, fontSize, lineWidth, vTextWidth, vTextHeight, texttoFormatX, texttoFormatY);
        }

        /// <summary>
        /// 创建并添加比例尺的 UI 元素 (背景矩形、线条、文本标签) 到 Canvas。
        /// 所有坐标和长度都基于 Canvas 的逻辑像素 (即图片原始像素)。
        /// </summary>
        private void DrawScaleElement(bool horizontal, int length, double x, double y, bool horizontalRight,
            bool verticalDown, Brush textBrush, Brush bgBrush, FontWeight fontWeight, bool showFont, string fontFamily, int fontSize,
            int lineWidth, double textWidth, double textHeight, string texttoFormatX, string texttoFormatY)
        {
            double bgPadding = 5; // 背景相对线段和文本的内边距
            double halfLineWidth = lineWidth / 2.0;

            // 垂直线与水平线之间的偏移量，用于“共存”模式下避免重叠
            double verticalOffset = 0;
            if (!horizontal && DrawMode == "共存")
                verticalOffset = 15;

            // --- 1. 计算背景矩形的位置和尺寸 ---
            double bgWidth, bgHeight, bgLeft, bgTop;

            if (horizontal) // 水平比例尺的背景
            {
                double effectiveTextWidth = showFont ? textWidth : 0; // 文本实际宽度
                double contentWidth = Math.Max(length, effectiveTextWidth); // 内容的最大宽度（线长或文本宽）

                bgWidth = contentWidth + 2 * bgPadding;
                bgHeight = lineWidth + effectiveTextWidth + 2 * bgPadding; // 线宽 + 文本高度 + 边距

                // 计算背景矩形的左上角 X 坐标
                if (horizontalRight) // 水平向右绘制 (左上角在 x 处)
                {
                    bgLeft = x - bgPadding;
                    if (effectiveTextWidth > length) // 如果文本比线长，需要调整背景左移以居中
                    {
                        bgLeft -= (effectiveTextWidth - length) / 2;
                    }
                }
                else // 水平向左绘制 (右上角在 x 处)
                {
                    bgLeft = x - length - bgPadding;
                    if (effectiveTextWidth > length) // 如果文本比线长，需要调整背景左移以居中
                    {
                        bgLeft -= (effectiveTextWidth - length) / 2;
                    }
                }

                // 计算背景矩形的左上角 Y 坐标
                bgTop = verticalDown
                    ? y - effectiveTextWidth - bgPadding // 文本在上方 (Y 减小)
                    : y - lineWidth - bgPadding;         // 文本在下方 (Y 减小)
            }
            else // 垂直比例尺的背景
            {
                double effectiveTextHeight = showFont ? textHeight : 0; // 文本实际高度 (旋转后是宽度)
                double effectiveTextWidthForVertical = showFont ? textWidth : 0; // 文本实际宽度 (旋转后是高度)
                double contentHeight = Math.Max(length, effectiveTextWidthForVertical); // 内容的最大高度（线长或文本高）

                bgWidth = lineWidth + effectiveTextHeight + 2 * bgPadding; // 线宽 + 文本高度 (旋转后) + 边距
                bgHeight = contentHeight + 2 * bgPadding;

                // 计算背景矩形的左上角 X 坐标
                if (horizontalRight) // 垂直线在水平线右侧 (左上角 X 减小)
                {
                    bgLeft = x - effectiveTextHeight - bgPadding - verticalOffset;
                }
                else // 垂直线在水平线左侧 (左上角 X 增大)
                {
                    bgLeft = x - lineWidth - bgPadding + verticalOffset;
                }

                // 计算背景矩形的左上角 Y 坐标
                bgTop = verticalDown
                    ? y - bgPadding // 垂直向下绘制 (左上角在 y 处)
                    : y - length - bgPadding; // 垂直向上绘制 (左下角在 y 处)

                if (showFont && effectiveTextWidthForVertical > length) // 如果文本比线长，调整背景上移以居中
                {
                    bgTop -= (effectiveTextWidthForVertical - length) / 2;
                }
            }

            // 创建并添加背景矩形
            Rectangle backgroundRect = new Rectangle
            {
                Fill = bgBrush,
                Width = bgWidth,
                Height = bgHeight
            };
            Canvas.SetLeft(backgroundRect, bgLeft);
            Canvas.SetTop(backgroundRect, bgTop);
            Canvas?.Children.Add(backgroundRect);

            // --- 2. 创建并添加线条 ---
            Line line = new Line
            {
                Stroke = textBrush,
                StrokeThickness = lineWidth,
                StrokeStartLineCap = PenLineCap.Flat, // 线段末端样式
                StrokeEndLineCap = PenLineCap.Flat
            };

            if (horizontal) // 水平线
            {
                line.X1 = horizontalRight ? x : x - length;
                line.Y1 = verticalDown ? y + halfLineWidth : y - halfLineWidth;
                line.X2 = horizontalRight ? x + length : x;
                line.Y2 = line.Y1; // Y 坐标不变
            }
            else // 垂直线
            {
                line.X1 = horizontalRight ? x + halfLineWidth - verticalOffset : x - halfLineWidth + verticalOffset;
                line.Y1 = verticalDown ? y + verticalOffset : y - length - verticalOffset;
                line.X2 = line.X1; // X 坐标不变
                line.Y2 = verticalDown ? y + verticalOffset + length : y - verticalOffset;
            }
            Canvas?.Children.Add(line);

            // --- 3. 创建并添加文本标签 ---
            if (showFont)
            {
                string textContent = horizontal ? texttoFormatX : texttoFormatY;
                TextBlock textBlock = new TextBlock
                {
                    Text = textContent,
                    Foreground = textBrush,
                    FontFamily = new FontFamily(fontFamily),
                    FontSize = fontSize,
                    FontWeight = fontWeight,
                    TextAlignment = TextAlignment.Center // 文本块内部文本居中
                };

                double labelLeft, labelTop;

                if (horizontal) // 水平文本
                {
                    // 水平文本居中于线段上方或下方
                    labelLeft = (line.X1 + line.X2) / 2 - textWidth / 2; // X 坐标使其居中
                    labelTop = verticalDown ? y - textHeight : y;       // Y 坐标使其位于线条上方或下方
                }
                else // 垂直文本 (需要旋转)
                {
                    // 设置旋转中心为文本块自身中心
                    textBlock.RenderTransformOrigin = new Point(0.5, 0.5);
                    // 旋转 -90 度 (顺时针旋转)
                    textBlock.RenderTransform = new RotateTransform(-90);

                    // 对于一个旋转了 -90 度的 TextBlock：
                    // 它的视觉宽度是其原始高度 (textHeight)。
                    // 它的视觉高度是其原始宽度 (textWidth)。
                    double rotatedTextVisualWidth = textHeight;  // 文本旋转后的视觉宽度
                    double rotatedTextVisualHeight = textWidth; // 文本旋转后的视觉高度

                    // 计算线条的中心点，用于文本的对齐
                    double lineCenterX = line.X1; // 垂直线的 X 坐标是固定的
                    double lineCenterY = (line.Y1 + line.Y2) / 2.0; // 垂直线的 Y 轴中心

                    if (horizontalRight) // 垂直线在图片右侧 (文本应在线条左侧)
                    {
                        // 文本的右边缘与线条的左边缘对齐，并考虑线条宽度和偏移量
                        labelLeft = lineCenterX - halfLineWidth - rotatedTextVisualWidth - verticalOffset;
                        // 文本的垂直中心与线条的垂直中心对齐
                        labelTop = lineCenterY - rotatedTextVisualHeight / 2.0;
                    }
                    else // 垂直线在图片左侧 (文本应在线条右侧)
                    {
                        // 文本的左边缘与线条的右边缘对齐，并考虑线条宽度和偏移量
                        labelLeft = lineCenterX + halfLineWidth + verticalOffset;
                        // 文本的垂直中心与线条的垂直中心对齐
                        labelTop = lineCenterY - rotatedTextVisualHeight / 2.0;
                    }
                }

                // 将文本块添加到 Canvas
                Canvas.SetLeft(textBlock, labelLeft);
                Canvas.SetTop(textBlock, labelTop);
                Canvas?.Children.Add(textBlock);
            }
        }
        
    }
}
