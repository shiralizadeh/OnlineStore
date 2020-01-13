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

namespace OnlineStore.DataLayer
{
    public class ProductVarientAttribute : EntityBase
    {
        [ForeignKey("ProductVarient")]
        public int ProductVarientID { get; set; }
        public ProductVarient ProductVarient { get; set; }

        [ForeignKey("Attribute")]
        public int AttributeID { get; set; }
        public Attribute Attribute { get; set; }

        [ForeignKey("AttributeOption")]
        public int? AttributeOptionID { get; set; }
        public AttributeOption AttributeOption { get; set; }
    }

    public static class ProductVarientAttributes
    {
        public static List<ProductVarientAttribute> GetByProductVarientID(int productVarientID)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var query = from item in db.ProductVarientAttributes
                            where item.ProductVarientID == productVarientID
                            select item;

                return query.ToList();
            }
        }

        public static List<JsonVarientAttribute> GetJsonByProductVarientID(int productVarientID)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var query = from item in db.ProductVarientAttributes
                            where item.ProductVarientID == productVarientID
                            select new JsonVarientAttribute()
                            {
                                AttributeID = item.AttributeID,
                                AttributeOptionID = item.AttributeOptionID.Value,
                                AttributeOptionTitle = item.AttributeOption.Title,
                            };

                return query.ToList();
            }
        }

        public static string GetVarients(int productVarientID)
        {
            var attrs = new List<string>();
            foreach (var attr in ProductVarientAttributes.GetJsonByProductVarientID(productVarientID))
            {
                attrs.Add(Attributes.GetByID(attr.AttributeID).Title + ": " + attr.AttributeOptionTitle);
            }

            return "(" + String.Join(" + ", attrs) + ")";
        }

        public static int Count()
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var query = from item in db.ProductVarientAttributes
                            select item;

                return query.Count();
            }
        }

        public static ProductVarientAttribute GetByID(int id)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var productVarientPrice = db.ProductVarientAttributes.Where(item => item.ID == id).Single();

                return productVarientPrice;
            }
        }

        public static void Delete(int id)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var productVarientPrice = (from item in db.ProductVarientAttributes
                                           where item.ID == id
                                           select item).Single();

                db.ProductVarientAttributes.Remove(productVarientPrice);

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

        public static void Insert(ProductVarientAttribute productVarientPrice)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                db.ProductVarientAttributes.Add(productVarientPrice);

                db.SaveChanges();
            }
        }

        public static void Update(ProductVarientAttribute productVarientPrice)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var orgProductVarientPrice = db.ProductVarientAttributes.Where(item => item.ID == productVarientPrice.ID).Single();

                orgProductVarientPrice.ProductVarientID = productVarientPrice.ProductVarientID;
                orgProductVarientPrice.AttributeID = productVarientPrice.AttributeID;
                orgProductVarientPrice.AttributeOptionID = productVarientPrice.AttributeOptionID;
                orgProductVarientPrice.LastUpdate = productVarientPrice.LastUpdate;

                db.SaveChanges();
            }
        }
    }
}
