using Microsoft.Win32;
using ScaleBarControls.ViewModels;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace ScaleBarControls.Controls
{
    /// <summary>
    /// ScaleBarSettingsControl.xaml 的交互逻辑
    /// </summary>
    public partial class ScaleBarSettingsControl : UserControl
    {
        #region 依赖属性

        /// <summary>
        /// ViewModel依赖属性
        /// </summary>
        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register(
                nameof(ViewModel),
                typeof(ScaleBarViewModel),
                typeof(ScaleBarSettingsControl),
                new PropertyMetadata(null, OnViewModelChanged));

        #endregion

        #region 属性

        /// <summary>
        /// ViewModel属性
        /// </summary>
        public ScaleBarViewModel? ViewModel
        {
            get => (ScaleBarViewModel?)GetValue(ViewModelProperty);
            set => SetValue(ViewModelProperty, value);
        }

        /// <summary>
        /// 位置选项
        /// </summary>
        public ObservableCollection<string> PositionOptions { get; } = new()
        {
            "左上", "右上", "左下", "右下"
        };

        /// <summary>
        /// 绘制模式选项
        /// </summary>
        public ObservableCollection<string> DrawModeOptions { get; } = new()
        {
            "水平", "竖直", "共存"
        };

        /// <summary>
        /// 字体可见性选项
        /// </summary>
        public ObservableCollection<string> FontVisibilityOptions { get; } = new()
        {
            "Visible", "Hidden", "Collapsed"
        };

        /// <summary>
        /// 单位选项
        /// </summary>
        public ObservableCollection<string> UnitOptions { get; } = new()
        {
            "μm", "mm", "cm", "m", "km", "nm", "pm", "inch", "ft", "pixel"
        };

        /// <summary>
        /// 字体系列选项
        /// </summary>
        public ObservableCollection<string> FontFamilyOptions { get; } = new()
        {
            "Arial", "Times New Roman", "Calibri", "Verdana", "Tahoma", "Courier New", "宋体", "微软雅黑", "黑体"
        };

        /// <summary>
        /// 预设配置选项
        /// </summary>
        public ObservableCollection<PresetConfig> PresetConfigs { get; } = new()
        {
            new PresetConfig("显微镜", "Microscopy", "适用于显微镜图像的比例尺设置"),
            new PresetConfig("摄影", "Photography", "适用于摄影图像的比例尺设置"),
            new PresetConfig("工程制图", "Engineering", "适用于工程制图的比例尺设置"),
            new PresetConfig("默认", "Default", "恢复默认设置")
        };

        #endregion

        #region 构造函数

        public ScaleBarSettingsControl()
        {
            InitializeComponent();
            DataContext = this;
        }

        #endregion

        #region 事件处理

        /// <summary>
        /// ViewModel变化时的处理
        /// </summary>
        private static void OnViewModelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ScaleBarSettingsControl control)
            {
                // 可以在这里添加额外的初始化逻辑
            }
        }

        /// <summary>
        /// 加载图片按钮点击事件
        /// </summary>
        private void LoadImageButton_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Title = "选择图片文件",
                Filter = "图片文件 (*.png;*.jpg;*.jpeg;*.bmp;*.tiff)|*.png;*.jpg;*.jpeg;*.bmp;*.tiff|所有文件 (*.*)|*.*",
                Multiselect = false
            };

            if (openFileDialog.ShowDialog() == true)
            {
                ViewModel?.LoadImageCommand.Execute(openFileDialog.FileName);
            }
        }

        /// <summary>
        /// 文本颜色选择按钮点击事件
        /// </summary>
        private void SelectTextColorButton_Click(object sender, RoutedEventArgs e)
        {
            //var colorDialog = new System.Windows.Forms.ColorDialog();
            //if (colorDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            //{
            //    var wpfColor = Color.FromArgb(colorDialog.Color.A, colorDialog.Color.R, colorDialog.Color.G, colorDialog.Color.B);
            //    if (ViewModel != null)
            //    {
            //        ViewModel.TextBrush = new SolidColorBrush(wpfColor);
            //    }
            //}
        }

        /// <summary>
        /// 背景颜色选择按钮点击事件
        /// </summary>
        private void SelectBackgroundColorButton_Click(object sender, RoutedEventArgs e)
        {
            //var colorDialog = new System.Windows.Forms.ColorDialog();
            //if (colorDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            //{
            //    var wpfColor = Color.FromArgb(colorDialog.Color.A, colorDialog.Color.R, colorDialog.Color.G, colorDialog.Color.B);
            //    if (ViewModel != null)
            //    {
            //        ViewModel.BackgroundBrush = new SolidColorBrush(wpfColor);
            //    }
            //}
        }

        /// <summary>
        /// 应用预设配置
        /// </summary>
        private void ApplyPresetButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is string presetKey)
            {
                ViewModel?.ApplyPresetCommand.Execute(presetKey);
            }
        }

        #endregion
    }

    /// <summary>
    /// 预设配置数据类
    /// </summary>
    public class PresetConfig
    {
        public string Name { get; set; }
        public string Key { get; set; }
        public string Description { get; set; }

        public PresetConfig(string name, string key, string description)
        {
            Name = name;
            Key = key;
            Description = description;
        }
    }
}

