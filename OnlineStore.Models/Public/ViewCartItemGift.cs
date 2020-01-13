using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineStore.Models.Public
{
    public class ViewCartItemGift
    {
        public int ID { get; set; }
        public int GiftID { get; set; }
        public int? ProductID { get; set; }
        public string GiftTitle { get; set; }
        public string Url { get; set; }
        public int Price { get; set; }
    }
}
