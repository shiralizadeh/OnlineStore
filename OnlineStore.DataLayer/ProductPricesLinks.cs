using OnlineStore.Models.Enums;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OnlineStore.EntityFramework;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using OnlineStore.Models.Admin;

namespace OnlineStore.DataLayer
{
    public class ProductPricesLink : EntityBase
    {
        [ForeignKey("Product")]
        [Display(Name = "محصول")]
        public int ProductID { get; set; }
        public Product Product { get; set; }


        [Display(Name = "آدرس صفحه")]
        [MaxLength(300)]
        public string Link { get; set; }


        [Display(Name = "نام سایت فروشگاه")]
        public WebsiteName WebsiteName { get; set; }
    }

    public static class ProductPricesLinks
    {
        public static void Insert(ProductPricesLink productPricesLink)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                db.ProductPricesLinks.Add(productPricesLink);

                db.SaveChanges();
            }
        }

        public static void Update(EditProductPricesLink productPricesLink)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var orgProductPricesLink = db.ProductPricesLinks.Where(item => item.ID == productPricesLink.ID).Single();

                orgProductPricesLink.Link = productPricesLink.Link;
                orgProductPricesLink.WebsiteName = productPricesLink.WebsiteName;
                orgProductPricesLink.LastUpdate = DateTime.Now;

                db.SaveChanges();
            }
        }

        public static void Delete(int id)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var link = (from item in db.ProductPricesLinks
                            where item.ID == id
                            select item).Single();

                db.ProductPricesLinks.Remove(link);

                db.SaveChanges();
            }
        }

        public static IList Get(int pageIndex, int pageSize, string pageOrder, int productID)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var query = from item in db.ProductPricesLinks
                            where item.ProductID == productID
                            select item;

                if (!String.IsNullOrWhiteSpace(pageOrder))
                    query = query.OrderBy(pageOrder);

                query = query.Skip(pageIndex * pageSize).Take(pageSize);

                return query.ToList();
            }

        }

        public static ProductPricesLink GetByID(int id)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var link = db.ProductPricesLinks.Where(item => item.ID == id).Single();

                return link;
            }
        }

        public static List<EditProductPricesLink> GetByProductID(int productID)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var query = from item in db.ProductPricesLinks
                            where item.ProductID == productID
                            select new EditProductPricesLink
                            {
                                ID = item.ID,
                                Link = item.Link,
                                WebsiteName = item.WebsiteName,
                            };

                return query.ToList();
            }
        }

        public static int Count(int productID)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var query = from item in db.ProductPricesLinks
                            where item.ProductID == productID
                            select item;

                return query.Count();
            }
        }
    }
}
