using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineStore.DataLayer
{
    public class ProductRate : EntityBase
    {
        [MaxLength(15)]
        public string IP { get; set; }

        [MaxLength(128)]
        public string UserID { get; set; }
        public float Rate { get; set; }

        [ForeignKey("Product")]
        public int ProductID { get; set; }
        public Product Product { get; set; }
    }

    public static class ProductRates
    {
        public static void Insert(ProductRate productRate)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                db.ProductRates.Add(productRate);
                db.SaveChanges();
            }
        }

        public static int RepeatPreferentials(int productID, string ip, string userID)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                if (!String.IsNullOrWhiteSpace(userID))
                {
                    return db.ProductRates.Where(item => item.UserID == userID && item.ProductID == productID).Count();
                }

                return db.ProductRates.Where(item => item.IP == ip && item.ProductID == productID).Count();
            }
        }
    }
}
