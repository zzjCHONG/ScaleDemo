using Microsoft.Win32;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;
using WpfCustomControlLibrary1;

namespace ScaleBar
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.DataContext=new ScaleBarExViewModel();
        }

        private void LoadImage_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ExportImage_Click(object sender, RoutedEventArgs e)
        {
            SaveDumpImg();
        }

        public void SaveDumpImg()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            saveFileDialog.Filter = "TIFF files (*.tif)|*.tif|TIFF files (*.tiff)|*.tiff|All files (*.*)|*.*";
            saveFileDialog.DefaultExt = "tif";
            saveFileDialog.AddExtension = true;
            saveFileDialog.FileName = "DumpImage";

            bool? result = saveFileDialog.ShowDialog();
            if (result == true)
            {
                var bitmapImage = myScaleBar.CaptureGridContent(ScaleBarEx.ImageFormat.Tiff);
                SaveBitmapImage(bitmapImage!, saveFileDialog.FileName);
                OpenFolderAndSelectFile(saveFileDialog.FileName);
            }
        }

        private static void SaveBitmapImage(BitmapImage bitmapImage, string filePath)
        {
            BitmapEncoder encoder;
            string extension = Path.GetExtension(filePath).ToLower();

            switch (extension)
            {
                case ".png":
                    encoder = new PngBitmapEncoder();
                    break;
                case ".jpeg":
                case ".jpg":
                    encoder = new JpegBitmapEncoder();
                    break;
                case ".bmp":
                    encoder = new BmpBitmapEncoder();
                    break;
                case ".gif":
                    encoder = new GifBitmapEncoder();
                    break;
                case ".tiff":
                case ".tif":
                    encoder = new TiffBitmapEncoder();
                    break;
                default:
                    throw new NotSupportedException($"Unsupported file extension: {extension}");
            }

            encoder.Frames.Add(BitmapFrame.Create(bitmapImage));

            using (FileStream fileStream = new FileStream(filePath, FileMode.Create))
            {
                encoder.Save(fileStream);
            }
        }

        private static void OpenFolderAndSelectFile(string fileFullName)
        {
            ProcessStartInfo psi = new ProcessStartInfo("Explorer.exe");
            psi.Arguments = "/e,/select," + fileFullName;
            Process.Start(psi);
        }
    }
}