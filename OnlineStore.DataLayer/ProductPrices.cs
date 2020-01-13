using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OnlineStore.Models.Enums;
using OnlineStore.Models.Admin;

namespace OnlineStore.DataLayer
{
    public class ProductPrice : EntityBase
    {
        [ForeignKey("Product")]
        public int ProductID { get; set; }
        public Product Product { get; set; }

        public int Price { get; set; }

        public PriceType PriceType { get; set; }

        public string Description { get; set; }
    }

    public static class ProductPrices
    {
        public static List<EditProductPrice> GetByProductID(int productID)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var query = from item in db.ProductPrices
                            where item.ProductID == productID
                            select new EditProductPrice
                            {
                                Price = item.Price,
                                PriceType = item.PriceType,
                                Description = item.Description,
                                LastUpdate = item.LastUpdate
                            };

                return query.ToList();
            }
        }

        public static List<ExcelPrice> GetByGroupIDs(List<int?> groupIDs)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var query = from item in db.Products
                            where groupIDs.Contains(item.GroupID) && !item.HasVarients
                            select new ExcelPrice
                            {
                                ProductID = item.ID,
                                PriceCode = item.PriceCode,
                                Title = item.Title
                            };

                var result = query.ToList();

                foreach (var item in result)
                {
                    var minprice = db.ProductPrices.Where(price => price.ProductID == item.ProductID).OrderByDescending(price => price.LastUpdate).FirstOrDefault();

                    if (minprice != null)
                    {
                        item.PriceID = minprice.ID;
                        item.Price = minprice.Price;
                        item.PriceType = minprice.PriceType;
                    }
                }

                return result.ToList();
            }
        }

        public static ProductPrice GetLatestPrice(int productID, PriceType priceType)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var query = from item in db.ProductPrices
                            where 
                            item.ProductID == productID
                            && item.PriceType == priceType
                            orderby item.LastUpdate descending
                            select item;

                return query.FirstOrDefault();
            }
        }

        public static int Count()
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var query = from item in db.ProductPrices
                            select item;

                return query.Count();
            }
        }

        public static ProductPrice GetByID(int id)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var productPrice = db.ProductPrices.Where(item => item.ID == id).Single();

                return productPrice;
            }
        }

        public static void Delete(int id)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var productPrice = (from item in db.ProductPrices
                                    where item.ID == id
                                    select item).Single();

                db.ProductPrices.Remove(productPrice);

                db.SaveChanges();
            }
        }

        public static void Insert(ProductPrice productPrice)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                db.ProductPrices.Add(productPrice);

                db.SaveChanges();
            }
        }

        public static void Update(ProductPrice productPrice)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var orgProductPrice = db.ProductPrices.Where(item => item.ID == productPrice.ID).Single();

                orgProductPrice.ProductID = productPrice.ProductID;
                orgProductPrice.Price = productPrice.Price;
                orgProductPrice.PriceType = productPrice.PriceType;
                orgProductPrice.Description = productPrice.Description;
                orgProductPrice.LastUpdate = productPrice.LastUpdate;

                db.SaveChanges();
            }
        }
    }
}
