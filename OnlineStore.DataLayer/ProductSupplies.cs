using OnlineStore.Models.Admin;
using OnlineStore.Models.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineStore.DataLayer
{
    public class ProductSupply : EntityBase
    {
        [ForeignKey("Product")]
        public int ProductID { get; set; }
        public Product Product { get; set; }

        public SupplyType SupplyType { get; set; }

        public int Count { get; set; }

        public string Description { get; set; }
    }

    public static class ProductSupplies
    {
        public static List<EditProductSupply> GetByProductID(int productID)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var query = from item in db.ProductSupplies
                            where item.ProductID == productID
                            select new EditProductSupply
                            {
                                Count = item.Count,
                                SupplyType = item.SupplyType,
                                Description = item.Description,
                                LastUpdate = item.LastUpdate
                            };

                return query.ToList();
            }
        }

        public static int Count()
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var query = from item in db.ProductSupplies
                            select item;

                return query.Count();
            }
        }

        public static ProductSupply GetByID(int id)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var productSupply = db.ProductSupplies.Where(item => item.ID == id).Single();

                return productSupply;
            }
        }

        public static void Delete(int id)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var productSupply = (from item in db.ProductSupplies
                                   where item.ID == id
                                   select item).Single();

                db.ProductSupplies.Remove(productSupply);

                db.SaveChanges();
            }
        }

        public static void Insert(ProductSupply productSupply)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                db.ProductSupplies.Add(productSupply);

                db.SaveChanges();
            }
        }

        public static void Update(ProductSupply productSupply)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var orgProductSupply = db.ProductSupplies.Where(item => item.ID == productSupply.ID).Single();

                orgProductSupply.ProductID = productSupply.ProductID;
                orgProductSupply.Count = productSupply.Count;
                orgProductSupply.SupplyType = productSupply.SupplyType;
                orgProductSupply.Description = productSupply.Description;
                orgProductSupply.LastUpdate = productSupply.LastUpdate;

                db.SaveChanges();
            }
        }
    }
}
