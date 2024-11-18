using ScannerQRCode.Entities;
using ScannerQRCode.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScannerQRCode.Services
{
    public class OrderDetailService
    {
        private OrderDetailRepository _repo = new OrderDetailRepository();

        public List<OrderDetail> GetOrderDetailsByOrderId(string orderId)
        {
            return _repo.GetOrderDetailsByOrderId(orderId);
        }
    }
}
