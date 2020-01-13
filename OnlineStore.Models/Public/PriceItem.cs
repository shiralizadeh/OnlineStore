using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineStore.Models.Public
{
    public class PriceItem
    {
        public int ID { get; set; }
        public int Price { get; set; }
        public int DiscountPrice { get; set; }
        public float DiscountPercent { get; set; }
    }
}
