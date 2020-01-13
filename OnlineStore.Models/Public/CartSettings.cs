using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineStore.Models.Public
{
    public class CartSettings
    {
        public List<ViewSendMethod> SendMethods { get; set; }
        public List<ViewPaymentMethod> PaymentMethods { get; set; }
        public bool IsAuthentication { get; set; }
        public ViewBuyerInfo BuyerInfo { get; set; }
        public bool IsSuccess { get; set; }
    }
}
