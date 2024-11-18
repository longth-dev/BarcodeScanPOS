using Emgu.CV;
using Emgu.CV.Structure;
using ScannerQRCode.Entities;
using ScannerQRCode.Services;
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
using System.Windows.Threading;
using ZXing;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Menu;

namespace ScannerQRCode
{
    /// <summary>
    /// Interaction logic for CheckoutWindow.xaml
    /// </summary>
    public partial class CheckoutWindow : Window
    {
        private VideoCapture _captureVideo = new();
        private DispatcherTimer _timer = new();
        private bool _isQRCodeScanned = false;
        private QRCodeService _qrCodeService = new();
        private ProductService _productService = new();
        private string _barCode;
        private CameraService _cameraService = new();

        public CheckoutWindow()
        {
            InitializeComponent();
            _captureVideo.ImageGrabbed += ProcessFrame; // Đăng ký sự kiện xử lý mỗi khung hình từ camera
            _captureVideo.Start(); // Bắt đầu lấy hình ảnh từ camera
            _timer.Interval = TimeSpan.FromMilliseconds(2000); // Cài đặt thời gian chờ giữa các lần xử lý (30ms)
            _timer.Tick += (s, args) => { ProcessFrame(null, null); }; // Gọi ProcessFrame mỗi 30ms
            _timer.Start(); // Bắt đầu timer
            Product product = new()
            {
                Id = "1",
                ProductName = "Test",
                Price = 10,
            };
            List<OrderDetail> orderDetails = new();
            OrderDetail orderDetail = new()
            {
                ProductId = "1",    
            };
            orderDetails.Add(orderDetail);
            CheckoutData.ItemsSource = orderDetails;
        }

        private void ProcessFrame(object sender, EventArgs e)
        {
            using (var frame = _captureVideo.QueryFrame().ToImage<Bgr, byte>())
            {

                // Chuyển đổi hình ảnh trong luồng UI
                Dispatcher.Invoke(() =>
                {
                    var bitmap = _cameraService.ConvertToBitmap(frame);
                    var bitmapImage = _cameraService.BitmapToBitmapImage(bitmap);
                    CheckOutCam.Source = bitmapImage; // Hiển thị hình ảnh từ camera lên điều khiển Image

                    //Xử lí mã QR
                    Result result = _qrCodeService.ProcessQRCode(bitmap);
                    if (result != null)
                    {
                        _barCode = result.Text;
                        Product? product = _productService.GetProductById(_barCode);
                      
                    }
                });
            }
        }

        public void StopCamera()
        {
            if (_captureVideo != null && _captureVideo.IsOpened)
            {
                _captureVideo.Stop();
                _captureVideo.ImageGrabbed -= ProcessFrame; // Bỏ đăng ký sự kiện để dừng xử lý thêm khung hình 
            }

            // Đảm bảo dừng DispatcherTimer nếu chưa được dừng
            if (_timer.IsEnabled)
            {
                _timer.Stop();
            }

            _captureVideo?.Dispose(); // Dọn dẹp tài nguyên camera
        }

        private void CheckoutData_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            // Kiểm tra nếu cột chỉnh sửa là 'Unit Price' hoặc 'Quantity'
            MessageBox.Show($"Updated Quantity: ", "Quantity Updated", MessageBoxButton.OK);
            if (e.Column.Header.ToString() == "Unit Price" || e.Column.Header.ToString() == "Quantity")
            {
                // Lấy dòng dữ liệu được chỉnh sửa
                var editedRow = e.Row.Item as OrderDetail; // Thay YourDataType bằng kiểu dữ liệu của bạn

                if (editedRow != null)
                {
                    int rowIndex = e.Row.GetIndex();
                                                                                                         

                    // Thực hiện cập nhật giá trị vào UI nếu cần
                    CheckoutData.Items.Refresh();
                }
            }
        }


    }
}
