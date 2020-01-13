using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OnlineStore.Models.Enums;

namespace OnlineStore.Models.User
{
    public class ViewOrder
    {
        public int CartID { get; set; }
        public string SaleReferenceID { get; set; }
        public SendMethodType SendMethodType { get; set; }
        public PaymentMethodType PaymentMethodType { get; set; }
        public SendStatus SendStatus { get; set; }
        public int Tax { get; set; }
        public int Total { get; set; }
        public int ToPay { get; set; }
        public string OrderID { get; set; }
        public DateTime LastUpdate { get; set; }
        public DateTime? DateTime { get; set; }
    }
}
