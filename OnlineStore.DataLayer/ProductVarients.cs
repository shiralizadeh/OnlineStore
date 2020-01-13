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
using OnlineStore.Models.Admin;

namespace OnlineStore.DataLayer
{
    public class ProductVarient : EntityBase
    {
        public ProductVarient()
        {
            IsEnabled = true;
        }

        [ForeignKey("Product")]
        public int ProductID { get; set; }
        public Product Product { get; set; }

        public string Title { get; set; }

        public string PriceCode { get; set; }

        public bool IsEnabled { get; set; }
    }

    public static class ProductVarients
    {
        public static List<EditProductVarient> GetByProductID(int productID)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var query = from varient in db.ProductVarients
                            where varient.ProductID == productID
                            select new EditProductVarient
                            {
                                ID = varient.ID,
                                PriceCode = varient.PriceCode,
                                IsEnabled = varient.IsEnabled,
                                LastUpdate = varient.LastUpdate,
                                Attributes = (from attr in db.ProductVarientAttributes
                                              where attr.ProductVarientID == varient.ID
                                              select new EditProductVarientAttribute
                                              {
                                                  AttributeID = attr.AttributeID,
                                                  AttributeOptionTitle = attr.AttributeOption.Title,
                                                  AttributeOptionID = attr.AttributeOptionID,
                                              }).ToList()
                            };

                return query.ToList();
            }
        }

        public static int Count()
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var query = from item in db.ProductVarients
                            select item;

                return query.Count();
            }
        }

        public static ProductVarient GetByID(int id)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var productVarient = db.ProductVarients.Where(item => item.ID == id).Single();

                return productVarient;
            }
        }

        public static int GetProductID(int id)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var productID = db.ProductVarients.Where(item => item.ID == id).Select(item => item.ProductID).Single();

                return productID;
            }
        }

        public static void Delete(int id)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var productVarient = (from item in db.ProductVarients
                                      where item.ID == id
                                      select item).Single();

                //ProductDiscounts.DeleteByProductVarientID(productVarient.ID);
                //ProductVarientAttributes.DeleteByProductVarientID(productVarient.ID);

                db.ProductVarients.Remove(productVarient);

                db.SaveChanges();
            }
        }

        public static void DeleteWithAttributes(int id)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var productVarient = (from item in db.ProductVarients
                                      where item.ID == id
                                      select item).Single();

                ProductVarientAttributes.DeleteByProductVarientID(productVarient.ID);

                db.ProductVarients.Remove(productVarient);

                db.SaveChanges();
            }
        }

        public static void Insert(ProductVarient productVarient)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                db.ProductVarients.Add(productVarient);

                db.SaveChanges();
            }
        }

        public static void Update(ProductVarient productVarient)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var orgProductVarient = db.ProductVarients.Where(item => item.ID == productVarient.ID).Single();

                orgProductVarient.ProductID = productVarient.ProductID;
                orgProductVarient.Title = productVarient.Title;
                orgProductVarient.PriceCode = productVarient.PriceCode;
                orgProductVarient.IsEnabled = productVarient.IsEnabled;
                orgProductVarient.LastUpdate = productVarient.LastUpdate;

                db.SaveChanges();
            }
        }

        public static List<JsonShortProductVarient> GetShortVarientByProductID(int productID)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var query = from varient in db.ProductVarients
                            where varient.ProductID == productID && varient.IsEnabled
                            select new JsonShortProductVarient
                            {
                                ID = varient.ID,
                                Title = varient.Title,
                            };

                return query.ToList();
            }
        }

    }
}
