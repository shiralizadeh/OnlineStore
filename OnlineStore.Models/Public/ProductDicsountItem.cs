using OnlineStore.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineStore.Models.Public
{
    public class ProductDiscountItem
    {
        public int ID { get; set; }
        public DiscountType DiscountType { get; set; }
        public float Value { get; set; }
        public float Price_01 { get; set; }
    }
}
