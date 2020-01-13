using OnlineStore.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineStore.Models.Public
{
    public class Factor
    {
        public int ID { get; set; }

        public string UserID { get; set; }

        public int? Total { get; set; }

        public int? ToPay { get; set; }

        public int Tax { get; set; }

        public int? DelivaryPrice { get; set; }

        public DateTime DateTime { get; set; }

        public string SaleReferenceID { get; set; }

        public int ResCode { get; set; }

        public CartStatus CartStatus { get; set; }

        public PaymentMethodType PaymentMethodType { get; set; }
        public SendMethodType SendMethodType { get; set; }
        public string UserDescription { get; set; }

    }
}
