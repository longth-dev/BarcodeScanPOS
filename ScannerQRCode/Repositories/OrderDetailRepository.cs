using Microsoft.EntityFrameworkCore;
using ScannerQRCode.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScannerQRCode.Repositories
{
    public class OrderDetailRepository
    {
        private QRContext _context;

        public List<OrderDetail> GetOrderDetailsByOrderId(string orderId)
        {
            _context = new QRContext(); 

            return _context.OrderDetails
                .Include(od => od.Product)  // Lấy thông tin sản phẩm
                .Where(od => od.OrderId == orderId)  // Lọc theo OrderId
                .ToList();  // Trả về danh sách OrderDetail
        }
    }
}
