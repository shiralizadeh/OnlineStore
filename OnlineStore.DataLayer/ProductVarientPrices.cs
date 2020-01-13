using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OnlineStore.EntityFramework;
using OnlineStore.Models.Enums;
using OnlineStore.Models.Public;
using OnlineStore.Models.Admin;

namespace OnlineStore.DataLayer
{
    public class ProductVarientPrice : EntityBase
    {
        //[ForeignKey("ProductVarient")]
        public int ProductVarientID { get; set; }
        //public ProductVarient ProductVarient { get; set; }

        public int Price { get; set; }

        public int Count { get; set; }

        public PriceType PriceType { get; set; }
    }

    public static class ProductVarientPrices
    {
        public static List<ProductVarientPrice> GetByProductVarientID(int productVarientID)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var query = from item in db.ProductVarientPrices
                            where item.ProductVarientID == productVarientID
                            select item;

                return query.ToList();
            }
        }

        public static List<ExcelPrice> GetByGroupIDs(List<int?> groups)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var query = from varient in db.ProductVarients
                            join product in db.Products on varient.ProductID equals product.ID
                            where groups.Contains(product.GroupID)
                            select new ExcelPrice()
                            {
                                ProductID = product.ID,
                                VarientID = varient.ID,
                                PriceCode = varient.PriceCode,
                                Title = product.Title
                            };


                var result = query.ToList();

                foreach (var item in result)
                {
                    var price = ProductVarients.GetByProductID(item.VarientID.Value).OrderByDescending(pr => pr.LastUpdate).First();

                    item.Price = price.Price;
                    item.PriceType = price.PriceType;
                }

                return result;
            }
        }

        public static int GetPriceByProductVarientID(int productVarientID)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var query = from item in db.ProductVarientPrices
                            where item.ProductVarientID == productVarientID
                            && item.PriceType == PriceType.Sell
                            orderby item.ID descending
                            select item.Price;

                return query.First();
            }
        }

        public static int Count()
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var query = from item in db.ProductVarientPrices
                            select item;

                return query.Count();
            }
        }

        public static ProductVarientPrice GetByID(int id)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var productVarientPrice = db.ProductVarientPrices.Where(item => item.ID == id).Single();

                return productVarientPrice;
            }
        }

        public static void Delete(int id)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var productVarientPrice = (from item in db.ProductVarientPrices
                                           where item.ID == id
                                           select item).Single();

                db.ProductVarientPrices.Remove(productVarientPrice);

                db.SaveChanges();
            }
        }

        public static void DeleteByProductVarientID(int productVarientID)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                foreach (var item in GetByProductVarientID(productVarientID))
                {
                    Delete(item.ID);
                }
            }
        }

        public static void Insert(ProductVarientPrice productVarientPrice)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                db.ProductVarientPrices.Add(productVarientPrice);

                db.SaveChanges();
            }
        }

        public static void Update(ProductVarientPrice productVarientPrice)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var orgProductVarientPrice = db.ProductVarientPrices.Where(item => item.ID == productVarientPrice.ID).Single();

                orgProductVarientPrice.ProductVarientID = productVarientPrice.ProductVarientID;
                orgProductVarientPrice.Price = productVarientPrice.Price;
                orgProductVarientPrice.Count = productVarientPrice.Count;
                orgProductVarientPrice.PriceType = productVarientPrice.PriceType;
                orgProductVarientPrice.LastUpdate = productVarientPrice.LastUpdate;

                db.SaveChanges();
            }
        }
    }
}
