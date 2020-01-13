using OnlineStore.Models.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OnlineStore.Models.Admin;

namespace OnlineStore.DataLayer
{
    public class ProductPoint : EntityBase
    {
        [Display(Name = "محصول")]
        [ForeignKey("Product")]
        public int ProductID { get; set; }
        public Product Product { get; set; }

        [Display(Name = "متن")]
        public string Text { get; set; }

        [Display(Name = "نوع")]
        public ProductPointType ProductPointType { get; set; }
    }

    public static class ProductPoints
    {
        public static List<EditProductPoint> GetByProductID(int productID)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var query = from item in db.ProductPoints
                            where item.ProductID == productID
                            select new EditProductPoint
                            {
                                ID = item.ID,
                                Text = item.Text,
                                ProductPointType = item.ProductPointType,
                            };

                return query.ToList();
            }
        }

        public static void Insert(ProductPoint productPoint)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                db.ProductPoints.Add(productPoint);

                db.SaveChanges();
            }
        }

        public static void Delete(int id)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var productPoint = (from item in db.ProductPoints
                                    where item.ID == id
                                    select item).Single();

                db.ProductPoints.Remove(productPoint);

                db.SaveChanges();
            }
        }

        public static void Update(EditProductPoint productPoint)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var orgProductPoint = db.ProductPoints.Where(item => item.ID == productPoint.ID).Single();

                orgProductPoint.Text = productPoint.Text;
                orgProductPoint.ProductPointType = productPoint.ProductPointType;

                db.SaveChanges();
            }
        }
    }
}
