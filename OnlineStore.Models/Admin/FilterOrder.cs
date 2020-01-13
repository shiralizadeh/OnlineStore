using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OnlineStore.Models.Public;

namespace OnlineStore.Models.Admin
{
    public class FilterOrder
    {
        public List<ViewSendMethod> SendMethods { get; set; }
        public List<ViewPaymentMethod> PaymentMethods { get; set; }

    }
}
