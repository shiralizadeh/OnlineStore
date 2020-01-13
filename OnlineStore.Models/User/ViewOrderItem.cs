using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OnlineStore.Models.Public;

namespace OnlineStore.Models.User
{
    public class ViewOrderItem
    {
        public string ProductTitle { get; set; }
        public string VarientTitle { get; set; }
        public int? ProductVarentID { get; set; }
        public int ProductID { get; set; }
        public int Price { get; set; }
        public int Quantity { get; set; }
        public List<ViewCartItemGift> Gifts { get; set; }
        public DateTime DateTime { get; set; }
        public int GroupID { get; set; }
    }
}
