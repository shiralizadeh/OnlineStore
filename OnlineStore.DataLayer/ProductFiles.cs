using OnlineStore.Models.Admin;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineStore.DataLayer
{
    public class ProductFile : EntityBase
    {
        [ForeignKey("Product")]
        public int ProductID { get; set; }
        public Product Product { get; set; }

        [MaxLength(50)]
        public string Title { get; set; }

        [MaxLength(50)]
        public string Filename { get; set; }
    }

    public static class ProductFiles
    {
        public static List<EditProductFile> GetByProductID(int productID)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var query = from item in db.ProductFiles
                            where item.ProductID == productID
                            select new EditProductFile
                            {
                                ID = item.ID,
                                Title = item.Title,
                                Filename = item.Filename,
                            };

                return query.ToList();
            }
        }

        public static int Count()
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var query = from item in db.ProductFiles
                            select item;

                return query.Count();
            }
        }

        public static ProductFile GetByID(int id)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var productFile = db.ProductFiles.Where(item => item.ID == id).Single();

                return productFile;
            }
        }

        public static void Delete(int id)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var productFile = (from item in db.ProductFiles
                                    where item.ID == id
                                    select item).Single();

                db.ProductFiles.Remove(productFile);

                db.SaveChanges();
            }
        }

        public static void Insert(ProductFile productFile)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                db.ProductFiles.Add(productFile);

                db.SaveChanges();
            }
        }

        public static void Update(ProductFile productFile)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var orgProductFile = db.ProductFiles.Where(item => item.ID == productFile.ID).Single();

                orgProductFile.ProductID = productFile.ProductID;
                orgProductFile.Title = productFile.Title;
                orgProductFile.Filename = productFile.Filename;
                orgProductFile.LastUpdate = productFile.LastUpdate;

                db.SaveChanges();
            }
        }

        public static void UpdateTitle(int id, string title)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var orgProductFile = db.ProductFiles.Where(item => item.ID == id).Single();

                orgProductFile.Title = title;

                db.SaveChanges();
            }
        }
    }
}
