using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace WpfCustomControlLibrary1
{
    public class ScaleBarEx : ContentControl
    {
        private readonly TranslateTransform _panTransform = new();
        private readonly ScaleTransform _scaleTransform = new();
        private readonly TransformGroup _transformGroup = new();
        private bool _isDragging = false;
        private DispatcherTimer? _doubleClickTimer;
        private bool _waitingForDoubleClick = false;
        private bool _isUpdatingFromTransform = false;

        static ScaleBarEx()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ScaleBarEx), new FrameworkPropertyMetadata(typeof(ScaleBarEx)));
        }

        public ScaleBarEx()
        {
            this.Loaded += ScaleBarEx_Loaded;
            this.Unloaded += ScaleBarEx_Unloaded;

            // 初始化Transform组
            _transformGroup.Children.Add(_scaleTransform);
            _transformGroup.Children.Add(_panTransform);

            // 初始化双击计时器
            _doubleClickTimer = new DispatcherTimer();
            _doubleClickTimer.Interval = TimeSpan.FromMilliseconds(300); // 300ms 双击间隔
            _doubleClickTimer.Tick += DoubleClickTimer_Tick;
        }

        private void ScaleBarEx_Loaded(object sender, RoutedEventArgs e)
        {
            if (MainPanel != null)
            {
                MainPanel.MouseWheel += OnZoomChanged;
                MainPanel.MouseMove += OnMouseMove;
                MainPanel.PreviewMouseDown += OnPreviewMouseDown;
                MainPanel.PreviewMouseUp += OnPreviewMouseUp;
            }
        }

        private void ScaleBarEx_Unloaded(object sender, RoutedEventArgs e)
        {
            if (MainPanel != null)
            {
                MainPanel.MouseWheel -= OnZoomChanged;
                MainPanel.MouseMove -= OnMouseMove;
                MainPanel.PreviewMouseDown -= OnPreviewMouseDown;
                MainPanel.PreviewMouseUp -= OnPreviewMouseUp;
            }
            this.Loaded -= ScaleBarEx_Loaded;
            this.Unloaded -= ScaleBarEx_Unloaded;

            // 清理计时器
            _doubleClickTimer?.Stop();
            _doubleClickTimer = null;
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            MainPanel = Template.FindName(NamePartMainPanel, this) as Panel;
            Scroll = Template.FindName(NamePartScrollView, this) as ScrollViewer;
            Viewbox = Template.FindName(NamePartViewBox, this) as Viewbox;
            Canvas = Template.FindName(NamePartCanvas, this) as Canvas;
            ImageMain = Template.FindName(NamePartImage, this) as Image;

            // 为Viewbox设置Transform
            if (Viewbox != null)
            {
                Viewbox.RenderTransform = _transformGroup;
            }

            UpdateImageInfo();
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);
        }

        #region Name Parts

        private Panel? MainPanel;
        private ScrollViewer? Scroll;
        private Viewbox? Viewbox;
        private Canvas? Canvas;
        private Image? ImageMain;

        public const string NamePartMainPanel = "PART_MAIN_PANEL";
        public const string NamePartScrollView = "PART_SCROLL";
        public const string NamePartViewBox = "PART_VIEWBOX";
        public const string NamePartCanvas = "PART_CANVAS";
        public const string NamePartImage = "PART_IMAGEMAIN";

        #endregion

        #region Render Size Info (Zoom & Pan)

        double DefaultImagePanelScale = 0; // 图片在控件内“平铺”时的缩放比例
        (double Width, double Height) DefaultImageSize; // 图片的原始尺寸

        public static readonly DependencyProperty ImagePanelScaleProperty = DependencyProperty.Register(
            nameof(ImagePanelScale), typeof(double), typeof(ScaleBarEx), new PropertyMetadata((double)-1, OnImagePanelScaleChanged));

        private static void OnImagePanelScaleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is not ScaleBarEx ex || e.NewValue is not double n || n <= 0) return;
            if (ex.Viewbox is null || ex.DefaultImagePanelScale <= 0) return;

            // 计算相对于默认缩放的比例
            var scaleRatio = n / ex.DefaultImagePanelScale;

            // 只有在不是通过Transform手动设置时才更新ScaleTransform
            if (!ex._isUpdatingFromTransform)
            {
                ex._scaleTransform.ScaleX = scaleRatio;
                ex._scaleTransform.ScaleY = scaleRatio;
            }

            // 同时更新Viewbox尺寸以保持兼容性（可选）
            ex.Viewbox.Width = ex.DefaultImageSize.Width * ex.ImagePanelScale;
            ex.Viewbox.Height = ex.DefaultImageSize.Height * ex.ImagePanelScale;
        }

        public double MinScale
        {
            get { return (double)GetValue(MinScaleProperty); }
            set { SetValue(MinScaleProperty, value); }
        }

        public static readonly DependencyProperty MinScaleProperty =
            DependencyProperty.Register("MinScale", typeof(double), typeof(ScaleBarEx), new FrameworkPropertyMetadata(0.5));

        public double MaxScale
        {
            get { return (double)GetValue(MaxScaleProperty); }
            set { SetValue(MaxScaleProperty, value); }
        }

        public static readonly DependencyProperty MaxScaleProperty =
            DependencyProperty.Register("MaxScale", typeof(double), typeof(ScaleBarEx), new FrameworkPropertyMetadata(10.0));

        public double PanSensitivity
        {
            get { return (double)GetValue(PanSensitivityProperty); }
            set { SetValue(PanSensitivityProperty, value); }
        }

        public static readonly DependencyProperty PanSensitivityProperty =
            DependencyProperty.Register("PanSensitivity", typeof(double), typeof(ScaleBarEx), new FrameworkPropertyMetadata(1.3, FrameworkPropertyMetadataOptions.None));

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
            if (DefaultImagePanelScale <= 0) return;

            // 直接设置到初始状态
            _isUpdatingFromTransform = true;
            ImagePanelScale = DefaultImagePanelScale;
            _isUpdatingFromTransform = false;

            // 重置所有变换
            _panTransform.X = 0;
            _panTransform.Y = 0;
            _scaleTransform.ScaleX = 1;
            _scaleTransform.ScaleY = 1;
        }

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

            UpdateImageInfo();
            TileImage();
        }

        // 修改缩放方法，使用Transform而不是ScrollViewer
        private void OnZoomChanged(object sender, MouseWheelEventArgs e)
        {
            if (ImageMain == null || ImageSource == null || MainPanel == null) return;

            var oldScale = ImagePanelScale;
            if (oldScale <= 0 || DefaultImagePanelScale <= 0) return;

            // 获取鼠标相对于MainPanel的位置
            var mousePosition = e.GetPosition(MainPanel);

            const double ZoomFactor = 1.2;
            var multiplier = Math.Pow(ZoomFactor, e.Delta / 120.0);
            var newScale = oldScale * multiplier;

            // 使用用户定义的缩放限制
            var minScaleLimit = DefaultImagePanelScale * MinScale;
            var maxScaleLimit = DefaultImagePanelScale * MaxScale;

            // 严格限制缩放比例
            newScale = Math.Max(minScaleLimit, Math.Min(maxScaleLimit, newScale));

            if (Math.Abs(newScale - oldScale) < 0.0001) return;

            // 计算缩放中心
            var scaleRatio = newScale / oldScale;

            // 获取当前Viewbox的中心点
            var viewboxCenter = new Point(MainPanel.ActualWidth / 2, MainPanel.ActualHeight / 2);

            // 计算鼠标位置相对于中心的偏移
            var mouseOffset = new Point(mousePosition.X - viewboxCenter.X, mousePosition.Y - viewboxCenter.Y);

            // 应用缩放到ScaleTransform
            var currentScaleRatio = _scaleTransform.ScaleX;
            var newScaleRatio = (newScale / DefaultImagePanelScale);
            var transformScaleRatio = newScaleRatio / currentScaleRatio;

            _scaleTransform.ScaleX = newScaleRatio;
            _scaleTransform.ScaleY = newScaleRatio;

            // 调整平移以保持鼠标点不动
            var newMouseOffset = new Point(mouseOffset.X * transformScaleRatio, mouseOffset.Y * transformScaleRatio);
            var panAdjustment = new Point(mouseOffset.X - newMouseOffset.X, mouseOffset.Y - newMouseOffset.Y);

            _panTransform.X += panAdjustment.X;
            _panTransform.Y += panAdjustment.Y;

            // 更新ImagePanelScale属性以保持一致性
            _isUpdatingFromTransform = true;
            ImagePanelScale = newScale;
            _isUpdatingFromTransform = false;

            e.Handled = true;
        }

        private Point _startCursorPos = new(-1, -1);
        private Point _startPanOffset = new(0, 0);

        private void OnPreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (MainPanel == null) return;

            _startCursorPos = e.GetPosition(MainPanel);
            _startPanOffset = new Point(_panTransform.X, _panTransform.Y);

            if (e.LeftButton == MouseButtonState.Pressed)//左键实现拖拽
            {
                _isDragging = true;
                MainPanel.CaptureMouse();
            }

            if (e.MiddleButton == MouseButtonState.Pressed)//滚轮双击实现恢复平铺
            {
                // 处理双击逻辑
                if (_waitingForDoubleClick)
                {
                    _waitingForDoubleClick = false;
                    _doubleClickTimer?.Stop();
                    OnDoubleClick();
                    e.Handled = true;
                    return;
                }
                else
                {
                    _waitingForDoubleClick = true;
                    _doubleClickTimer?.Start();
                }
            }

        }

        private void OnPreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (_isDragging)
            {
                _isDragging = false;
                _startCursorPos = new(-1, -1);

                if (MainPanel != null && MainPanel.IsMouseCaptured)
                {
                    MainPanel.ReleaseMouseCapture();
                }
            }
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton != MouseButtonState.Pressed || !_isDragging) return;
            if (_startCursorPos.X < 0 || _startCursorPos.Y < 0) return;
            if (MainPanel == null || !MainPanel.IsMouseCaptured) return;

            var currentPos = e.GetPosition(MainPanel);
            var totalOffset = currentPos - _startCursorPos;

            // 应用平移变换
            _panTransform.X = _startPanOffset.X + totalOffset.X * PanSensitivity;
            _panTransform.Y = _startPanOffset.Y + totalOffset.Y * PanSensitivity;

            e.Handled = true;
        }

        // 双击处理方法
        private void OnDoubleClick()
        {
            // 双击回到初始状态，使用平滑动画
            TileImage();
        }

        // 双击计时器超时处理
        private void DoubleClickTimer_Tick(object sender, EventArgs e)
        {
            _waitingForDoubleClick = false;
            _doubleClickTimer?.Stop();
        }

        #endregion

        #region DependencyProperty

        /// <summary>
        /// 图像输入
        /// </summary>
        public ImageSource ImageSource
        {
            get => (ImageSource)GetValue(ImageSourceProperty);
            set => SetValue(ImageSourceProperty, value);
        }

        public static readonly DependencyProperty ImageSourceProperty =
            DependencyProperty.Register("ImageSource", typeof(ImageSource), typeof(ScaleBarEx), new PropertyMetadata(null, (o, p) =>
            {
                if (o is not ScaleBarEx ex) return;

                ex.UpdateImageInfo();// 先更新图片信息，这将触发比例尺更新

                if (p.OldValue is not BitmapSource s1)
                {
                    ex.TileImage();
                    return;
                }

                if (p.NewValue is not BitmapSource s2)
                    return;

                // 图像切换尺寸差异，则触发平铺
                if (Math.Abs(s1.Width - s2.Width) > 0.001
                    || Math.Abs(s1.Height - s2.Height) > 0.001) ex.TileImage();

            }));

        /// <summary>
        /// 比例尺是否可见
        /// </summary>
        public bool IsScaleBarVisible
        {
            get { return (bool)GetValue(IsScaleBarVisibleProperty); }
            set { SetValue(IsScaleBarVisibleProperty, value); }
        }

        public static readonly DependencyProperty IsScaleBarVisibleProperty =
            DependencyProperty.Register("IsScaleBarVisible", typeof(bool), typeof(ScaleBarEx), new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.None, OnScaleBarPropertyChanged));

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
            DependencyProperty.Register("ScaleLengthWidth", typeof(int), typeof(ScaleBarEx), new FrameworkPropertyMetadata(50, FrameworkPropertyMetadataOptions.None, OnScaleBarPropertyChanged));

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
            DependencyProperty.Register("ScaleLengthHeight", typeof(int), typeof(ScaleBarEx), new FrameworkPropertyMetadata(50, FrameworkPropertyMetadataOptions.None, OnScaleBarPropertyChanged));

        /// <summary>
        /// 字体大小
        /// </summary>
        public new int FontSize
        {
            get { return (int)GetValue(FontSizeProperty); }
            set { SetValue(FontSizeProperty, value); }
        }

        public static new readonly DependencyProperty FontSizeProperty =
            DependencyProperty.Register("FontSize", typeof(int), typeof(ScaleBarEx), new FrameworkPropertyMetadata(14, FrameworkPropertyMetadataOptions.None, OnScaleBarPropertyChanged));

        /// <summary>
        /// 线条宽度
        /// </summary>
        public int LineWidth
        {
            get { return (int)GetValue(LineWidthProperty); }
            set { SetValue(LineWidthProperty, value); }
        }

        public static readonly DependencyProperty LineWidthProperty =
            DependencyProperty.Register("LineWidth", typeof(int), typeof(ScaleBarEx), new FrameworkPropertyMetadata(4, FrameworkPropertyMetadataOptions.None, OnScaleBarPropertyChanged));

        /// <summary>
        /// 字体颜色
        /// </summary>
        public Brush TextBrush
        {
            get { return (Brush)GetValue(TextBrushProperty); }
            set { SetValue(TextBrushProperty, value); }
        }

        public static readonly DependencyProperty TextBrushProperty =
            DependencyProperty.Register("TextBrush", typeof(Brush), typeof(ScaleBarEx), new FrameworkPropertyMetadata(Brushes.White, FrameworkPropertyMetadataOptions.None, OnScaleBarPropertyChanged));

        /// <summary>
        /// 背景颜色
        /// </summary>
        public Brush BackgroundBrush
        {
            get { return (Brush)GetValue(BackgroundBrushProperty); }
            set { SetValue(BackgroundBrushProperty, value); }
        }

        public static readonly DependencyProperty BackgroundBrushProperty =
            DependencyProperty.Register("BackgroundBrush", typeof(Brush), typeof(ScaleBarEx), new FrameworkPropertyMetadata(Brushes.Gray, FrameworkPropertyMetadataOptions.None, OnScaleBarPropertyChanged));

        /// <summary>
        /// 字体样式
        /// </summary>
        public new FontWeight FontWeight
        {
            get { return (FontWeight)GetValue(FontWeightProperty); }
            set { SetValue(FontWeightProperty, value); }
        }

        public static new readonly DependencyProperty FontWeightProperty =
            DependencyProperty.Register("FontWeight", typeof(FontWeight), typeof(ScaleBarEx), new FrameworkPropertyMetadata(FontWeights.Bold, FrameworkPropertyMetadataOptions.None, OnScaleBarPropertyChanged));

        /// <summary>
        /// 字体是否可见
        /// </summary>
        public Visibility FontVisibility
        {
            get { return (Visibility)GetValue(FontVisibilityProperty); }
            set { SetValue(FontVisibilityProperty, value); }
        }

        public static readonly DependencyProperty FontVisibilityProperty =
            DependencyProperty.Register("FontVisibility", typeof(Visibility), typeof(ScaleBarEx), new FrameworkPropertyMetadata(Visibility.Visible, FrameworkPropertyMetadataOptions.None, OnScaleBarPropertyChanged));

        /// <summary>
        /// 字体类型
        /// </summary>
        public new string FontFamily
        {
            get { return (string)GetValue(FontFamilyProperty); }
            set { SetValue(FontFamilyProperty, value); }
        }

        public static new readonly DependencyProperty FontFamilyProperty =
            DependencyProperty.Register("FontFamily", typeof(string), typeof(ScaleBarEx), new FrameworkPropertyMetadata("Arial", FrameworkPropertyMetadataOptions.None, OnScaleBarPropertyChanged));

        /// <summary>
        /// 摆放位置-四角
        /// </summary>
        public string Position
        {
            get { return (string)GetValue(PositionProperty); }
            set { SetValue(PositionProperty, value); }
        }

        public static readonly DependencyProperty PositionProperty =
            DependencyProperty.Register("Position", typeof(string), typeof(ScaleBarEx), new FrameworkPropertyMetadata("左上", FrameworkPropertyMetadataOptions.None, OnScaleBarPropertyChanged));

        /// <summary>
        /// 绘制模式（水平/垂直/共存）
        /// </summary>
        public string DrawMode
        {
            get { return (string)GetValue(DrawModeProperty); }
            set { SetValue(DrawModeProperty, value); }
        }

        public static readonly DependencyProperty DrawModeProperty =
            DependencyProperty.Register("DrawMode", typeof(string), typeof(ScaleBarEx), new FrameworkPropertyMetadata("水平", FrameworkPropertyMetadataOptions.None, OnScaleBarPropertyChanged));

        /// <summary>
        /// 是否已设置比例尺模式（显示形式有区别）
        /// </summary>
        public bool IsSetScaleMode
        {
            get { return (bool)GetValue(IsSetScaleModeProperty); }
            set { SetValue(IsSetScaleModeProperty, value); }
        }

        public static readonly DependencyProperty IsSetScaleModeProperty =
            DependencyProperty.Register("IsSetScaleMode", typeof(bool), typeof(ScaleBarEx), new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.None, OnScaleBarPropertyChanged));

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
            DependencyProperty.Register("WidthinUnit", typeof(double), typeof(ScaleBarEx), new FrameworkPropertyMetadata(2.0, FrameworkPropertyMetadataOptions.None, OnScaleBarPropertyChanged));

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
            DependencyProperty.Register("HeightinUnit", typeof(double), typeof(ScaleBarEx), new FrameworkPropertyMetadata(2.0, FrameworkPropertyMetadataOptions.None, OnScaleBarPropertyChanged));

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
            DependencyProperty.Register("Scale", typeof(double), typeof(ScaleBarEx), new FrameworkPropertyMetadata(50.0, FrameworkPropertyMetadataOptions.None, OnScaleBarPropertyChanged));

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
            DependencyProperty.Register("PixelAspectRatio", typeof(double), typeof(ScaleBarEx), new FrameworkPropertyMetadata(1.0, FrameworkPropertyMetadataOptions.None, OnScaleBarPropertyChanged));

        /// <summary>
        /// 单位
        /// </summary>
        public string Unit
        {
            get { return (string)GetValue(UnitProperty); }
            set { SetValue(UnitProperty, value); }
        }

        public static readonly DependencyProperty UnitProperty =
            DependencyProperty.Register("Unit", typeof(string), typeof(ScaleBarEx), new FrameworkPropertyMetadata("μm", FrameworkPropertyMetadataOptions.None, OnScaleBarPropertyChanged));

        // 当比例尺相关属性改变时，重新绘制 Canvas 上的比例尺元素
        private static void OnScaleBarPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ScaleBarEx ex)
            {
                ex.UpdateScaleBarElements();
            }
        }

        #endregion

        #region Scale

        /// <summary>
        /// 更新 Canvas 上的比例尺 UI 元素。
        /// 此方法会在 ImageSource 改变、控件尺寸改变或比例尺相关属性改变时调用。
        /// </summary>
        private void UpdateScaleBarElements()
        {
            if (Canvas == null) return;

            // 清除 Canvas 上所有现有的比例尺元素
            Canvas.Children.Clear();

            // 如果比例尺不可见，直接返回
            if (!IsScaleBarVisible) return;

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
                unitX = Math.Max(0.0, WidthinUnit);
                unitY = Math.Max(0.0, HeightinUnit);
                var scale = Math.Max(0.0, Scale); // pixel/unit (每单位对应的像素数)
                var pixelAspectRatio = Math.Max(0.0, PixelAspectRatio); // 像素宽高比

                // 计算在图片原始像素坐标系中的长度
                scaleX = (int)Math.Round(unitX * scale);
                scaleY = (int)Math.Round(unitY * scale * pixelAspectRatio);
            }
            else // 如果是纯像素模式
            {
                // 直接使用设置的像素长度
                scaleX = Math.Max(1, ScaleLengthWidth);
                scaleY = Math.Max(1, ScaleLengthHeight);
            }

            int fontSize = Math.Max(1, FontSize);
            int lineWidth = Math.Max(1, LineWidth);

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
            if (position.Length > 2)
                position = position.Split(':')[1].Trim();

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
            // 注意：FormattedText 的 DPI 参数应从 VisualTreeHelper.GetDpi(this) 获取
            double dpi = VisualTreeHelper.GetDpi(this).PixelsPerDip;

            FormattedText hText = new(texttoFormatX, CultureInfo.CurrentCulture,
                FlowDirection.LeftToRight, new Typeface(fontFamily), fontSize,
                textBrush, dpi);

            FormattedText vText = new(texttoFormatY, CultureInfo.CurrentCulture,
                FlowDirection.LeftToRight, new Typeface(fontFamily), fontSize,
                textBrush, dpi);

            double hTextWidth = hText.Width, hTextHeight = hText.Height;
            double vTextWidth = vText.Width, vTextHeight = vText.Height; // vTextWidth 是竖直文本旋转前的宽度

            string drawMode = DrawMode;
            if (drawMode.Length > 2)
                drawMode = drawMode.Split(':')[1].Trim();

            if (drawMode is "共存" || drawMode is "水平")
                DrawScaleElement(true, scaleX, x, y, horizontalRight, verticalDown,
                    textBrush, bgBrush, fontWeight, showFont, fontFamily, fontSize, lineWidth, hTextWidth, hTextHeight, texttoFormatX, texttoFormatY);

            if (drawMode is "共存" || drawMode is "竖直")
                DrawScaleElement(false, scaleY, x, y, horizontalRight, verticalDown,
                    textBrush, bgBrush, fontWeight, showFont, fontFamily, fontSize, lineWidth, vTextWidth, vTextHeight, texttoFormatX, texttoFormatY);
        }

        /// <summary>
        /// 创建并添加比例尺的 UI 元素 (背景矩形、线条、文本标签) 到 Canvas。
        /// 所有坐标和长度都基于 Canvas 的逻辑像素 (即图片原始像素)。
        /// </summary>
        /// <param name="horizontal">是否绘制水平比例尺</param>
        /// <param name="length">比例尺的长度（像素）</param>
        /// <param name="x">起始X坐标（图片原始像素坐标）</param>
        /// <param name="y">起始Y坐标（图片原始像素坐标）</param>
        /// <param name="horizontalRight">水平线是否向右绘制</param>
        /// <param name="verticalDown">垂直线是否向下绘制</param>
        /// <param name="textBrush">文本颜色</param>
        /// <param name="bgBrush">背景颜色</param>
        /// <param name="fontWeight">字体粗细</param>
        /// <param name="showFont">是否显示文本</param>
        /// <param name="fontFamily">字体家族</param>
        /// <param name="fontSize">字体大小</param>
        /// <param name="lineWidth">线条宽度</param>
        /// <param name="textWidth">文本的原始宽度 (FormattedText.Width)</param>
        /// <param name="textHeight">文本的原始高度 (FormattedText.Height)</param>
        /// <param name="texttoFormatX">水平比例尺的文本内容</param>
        /// <param name="texttoFormatY">垂直比例尺的文本内容</param>
        private void DrawScaleElement(
            bool horizontal, int length, double x, double y, bool horizontalRight, bool verticalDown,
            Brush textBrush, Brush bgBrush, FontWeight fontWeight, bool showFont,
            string fontFamily, int fontSize, int lineWidth, double textWidth, double textHeight, string texttoFormatX, string texttoFormatY)
        {
            //todo，若字体加粗且字体字号过大，可能会出现背景无法覆盖完整的情况
            double bgPadding = 5;
            double halfLine = lineWidth / 2.0;

            // 调整垂直线起始位置的偏移量
            double offset = 0;

            // 背景矩形尺寸与位置
            double bgWidth, bgHeight, bgLeft, bgTop;

            if (horizontal)
            {
                double maxLen = Math.Max(showFont ? textWidth : 0, length);
                bgWidth = maxLen + 2 * bgPadding;
                bgHeight = lineWidth + (showFont ? textHeight : 0) + 2 * bgPadding;

                bgLeft = horizontalRight
                    ? x - bgPadding - (showFont && textWidth > length ? (textWidth - length) / 2 : 0)
                    : x - length - bgPadding - (showFont && textWidth > length ? (textWidth - length) / 2 : 0);

                bgTop = verticalDown
                    ? y - (showFont ? textHeight : 0) - bgPadding
                    : y - lineWidth - bgPadding;
            }
            else // Vertical
            {
                double maxLen = Math.Max(showFont ? textWidth : 0, length);
                bgWidth = lineWidth + (showFont ? textHeight : 0) + 2 * bgPadding;
                bgHeight = maxLen + 2 * bgPadding;

                if (this.DrawMode is "共存")
                    offset = 15;

                bgLeft = horizontalRight
                    ? x - (showFont ? textHeight : 0) - bgPadding - offset
                    : x - lineWidth - bgPadding + offset;

                bgTop = verticalDown
                    ? y - bgPadding - (showFont && textWidth > length ? (textWidth - length) / 2 : 0) + offset
                    : y - length - bgPadding - (showFont && textWidth > length ? (textWidth - length) / 2 : 0) - offset;
            }

            // 创建并添加背景矩形
            var rect = new Rectangle { Width = bgWidth, Height = bgHeight, Fill = bgBrush };
            Canvas.SetLeft(rect, bgLeft);
            Canvas.SetTop(rect, bgTop);
            Canvas?.Children.Add(rect);

            // 创建并添加线条
            var line = new Line { Stroke = textBrush, StrokeThickness = lineWidth };
            Canvas?.Children.Add(line);

            // 创建并添加文本标签
            var label = new TextBlock
            {
                Text = horizontal ? texttoFormatX : texttoFormatY,
                FontSize = fontSize,
                Foreground = textBrush,
                FontWeight = fontWeight,
                FontFamily = new FontFamily(fontFamily),
                Visibility = showFont ? Visibility.Visible : Visibility.Collapsed
            };
            Canvas?.Children.Add(label);

            // 坐标
            double labelLeft, labelTop;

            if (horizontal)
            {
                line.X1 = horizontalRight ? x : x - length;
                line.X2 = horizontalRight ? x + length : x;
                line.Y1 = line.Y2 = verticalDown ? y + halfLine : y - halfLine;

                labelLeft = (line.X1 + line.X2) / 2 - textWidth / 2;
                labelTop = verticalDown ? y - textHeight : y;
            }
            else // Vertical
            {
                line.Y1 = verticalDown ? y + offset : y - length - offset;
                line.Y2 = verticalDown ? y + offset + length : y - offset;

                line.X1 = line.X2 = horizontalRight ? x + halfLine - offset : x - halfLine + offset;

                labelLeft = horizontalRight ? x - textHeight - offset : x + offset;
                labelTop = (line.Y1 + line.Y2) / 2 + textWidth / 2;

                label.RenderTransform = new RotateTransform(-90);
                label.RenderTransformOrigin = new Point(0, 0);
            }

            Canvas.SetLeft(label, labelLeft);
            Canvas.SetTop(label, labelTop);
        }

        #endregion
    }
}
