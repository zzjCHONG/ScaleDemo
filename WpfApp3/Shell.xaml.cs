using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WpfApp3
{
    /// <summary>
    /// Shell.xaml 的交互逻辑
    /// </summary>
    public partial class Shell : Window
    {

        public Shell()
        {
            InitializeComponent();

            SetPositionOverlay();
        }

        // 切换四角/ROI
        private void CmbPosition_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SetPositionOverlay();
        }

        // 设置 ROI/四角位置
        private void SetPositionOverlay()
        {
            if (CmbPosition.SelectedIndex == 0) // 四角
            {
                PositionOverlay.Visibility = Visibility.Collapsed;
            }
            else // ROI
            {
                PositionOverlay.Visibility = Visibility.Visible;
                PositionOverlay.Width = 100;
                PositionOverlay.Height = 100;
                Canvas.SetLeft(PositionOverlay, 50);
                Canvas.SetTop(PositionOverlay, 50);
            }
        }

        //private void ScaleChanged(object sender, RoutedEventArgs e)
        //{
        //    HorizontalScale.Visibility = ChkShowHorizontal.IsChecked == true ? Visibility.Visible : Visibility.Collapsed;
        //    VerticalScale.Visibility = ChkShowVertical.IsChecked == true ? Visibility.Visible : Visibility.Collapsed;
        //}

        // 自动适应图像大小
        private void AutoFitChanged(object sender, RoutedEventArgs e)
        {
            bool autoFit = ChkAutoFit.IsChecked == true;

            if (autoFit)
            {
                // 自动适应时显示两个方向比例尺
                ChkShowHorizontal.IsChecked = true;
                ChkShowVertical.IsChecked = true;

                MainImage.Stretch = Stretch.Uniform;
            }
            else
            {
                MainImage.Stretch = Stretch.None;
            }
        }

        private void ScaleChanged(object sender, RoutedEventArgs e)
        {

        }
    }
}
