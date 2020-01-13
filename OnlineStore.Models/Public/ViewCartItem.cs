using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineStore.Models.Public
{
    public class ViewCartItem
    {
        public ViewCartItem()
        {
            Gifts = new List<ViewCartItemGift>();
        }

        public int ID { get; set; }
        public int CartProductID { get; set; }

        public int? ProductVarientID { get; set; }
        public int? ProductID { get; set; }
        public int? PackageID { get; set; }

        public int Quantity { get; set; }
        public string Title { get; set; }
        public string VarientTitle { get; set; }
        public string Url { get; set; }
        public string Image { get; set; }
        public int Price { get; set; }
        public int DiscountPrice { get; set; }
        public float DiscountPercent { get; set; }
        public bool IsFreeDelivery { get; set; }
        public bool HasVarients { get; set; }
        public List<ViewCartItemGift> Gifts { get; set; }
    }
}
