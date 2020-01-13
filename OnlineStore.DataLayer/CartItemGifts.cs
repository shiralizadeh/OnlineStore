using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineStore.DataLayer
{
    public class CartItemGift : EntityBase
    {
        [Display(Name = "محصول")]
        [ForeignKey("CartItem")]
        public int CartItemID { get; set; }
        public CartItem CartItem { get; set; }

        [Display(Name = "هدیه")]
        [ForeignKey("Gift")]
        public int GiftID { get; set; }
        public Product Gift { get; set; }
        public int Price { get; set; }

    }

    public static class CartItemGifts
    {
        public static void Insert(List<CartItemGift> gifts)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                db.CartItemGifts.AddRange(gifts);

                db.SaveChanges();
            }
        }
        
    }
}
