using OnlineStore.Models.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineStore.DataLayer
{
    public class ProductProfit : EntityBase
    {
        [ForeignKey("Product")]
        public int ProductID { get; set; }
        public Product Product { get; set; }

        public byte Percent { get; set; }

        public ProfitType ProfitType { get; set; }
    }

    public static class ProductProfits
    {
        public static List<ProductProfit> GetByProductID(int productID)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var query = from item in db.ProductProfits
                            where item.ProductID == productID
                            select item;

                return query.ToList();
            }
        }

        public static int Count()
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var query = from item in db.ProductProfits
                            select item;

                return query.Count();
            }
        }

        public static ProductProfit GetByID(int id)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var productProfit = db.ProductProfits.Where(item => item.ID == id).Single();

                return productProfit;
            }
        }

        public static void Delete(int id)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var productProfit = (from item in db.ProductProfits
                                     where item.ID == id
                                     select item).Single();

                db.ProductProfits.Remove(productProfit);

                db.SaveChanges();
            }
        }

        public static void Insert(ProductProfit productProfit)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                db.ProductProfits.Add(productProfit);

                db.SaveChanges();
            }
        }

        public static void Insert(int productID, byte physicalSell, byte downloadSell)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var profit_PhysicalSell = new ProductProfit()
                {
                    ProductID = productID,
                    Percent = physicalSell,
                    ProfitType = ProfitType.PhysicalSell
                };

                var profit_DownloadSell = new ProductProfit()
                {
                    ProductID = productID,
                    Percent = downloadSell,
                    ProfitType = ProfitType.DownloadSell
                };

                db.ProductProfits.Add(profit_PhysicalSell);
                db.ProductProfits.Add(profit_DownloadSell);

                db.SaveChanges();
            }
        }

        public static void Update(ProductProfit productProfit)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var orgProductProfit = db.ProductProfits.Where(item => item.ID == productProfit.ID).Single();

                orgProductProfit.ProductID = productProfit.ProductID;
                orgProductProfit.Percent = productProfit.Percent;
                orgProductProfit.ProfitType = productProfit.ProfitType;
                orgProductProfit.LastUpdate = productProfit.LastUpdate;

                db.SaveChanges();
            }
        }
    }
}
