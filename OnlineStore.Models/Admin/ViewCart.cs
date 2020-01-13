using OnlineStore.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineStore.Models.Admin
{
    public class ViewCart
    {
        public int ID { get; set; }

        public string UserID { get; set; }

        public string UserName { get; set; }

        public string Firstname { get; set; }

        public string Lastname { get; set; }

        public SendMethodType SendMethodType { get; set; }

        public PaymentMethodType PaymentMethodType { get; set; }

        public int? Total { get; set; }

        public int? ToPay { get; set; }

        public CartStatus CartStatus { get; set; }

        public SendStatus SendStatus { get; set; }

        public int Tax { get; set; }

        public DateTime? DateTime { get; set; }

        public DateTime LastUpdate { get; set; }

        public string OrderID { get; set; }

        public string SaleReferenceID { get; set; }
    }
}
