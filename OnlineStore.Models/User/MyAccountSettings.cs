using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OnlineStore.Models.Public;

namespace OnlineStore.Models.User
{
    public class MyAccountSettings
    {
        public ViewBuyerInfo UserInfo { get; set; }
        public int Orders { get; set; }
        public int OrdersSubmitted { get; set; }
        public int OrdersDelivered { get; set; }
        public int Comments { get; set; }
        public int CommentRates { get; set; }
        public int Wishes { get; set; }
        public int Posts { get; set; }
    }
}
