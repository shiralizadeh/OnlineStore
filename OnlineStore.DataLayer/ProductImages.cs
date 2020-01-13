using OnlineStore.Models.Admin;
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
    public class ProductImage : EntityBase
    {
        [ForeignKey("Product")]
        public int ProductID { get; set; }
        public Product Product { get; set; }

        [MaxLength(200)]
        public string Filename { get; set; }

        public ProductImagePlace ProductImagePlace { get; set; }
    }

    public static class ProductImages
    {
        public static List<EditProductImage> GetByProductID(int productID)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var query = from item in db.ProductImages
                            where item.ProductID == productID
                            select new EditProductImage
                            {
                                ID = item.ID,
                                Filename = item.Filename,
                                ProductImagePlace = item.ProductImagePlace,
                            };

                return query.ToList();
            }
        }

        public static List<EditProductImage> GetGalleryImages(int productID)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var query = from item in db.ProductImages
                            where item.ProductID == productID
                            select new EditProductImage
                            {
                                ID = item.ID,
                                Filename = item.Filename,
                                ProductImagePlace = item.ProductImagePlace
                            };

                query = query.OrderByDescending(item => item.ProductImagePlace);

                return query.ToList();
            }
        }

        public static EditProductImage GetDefaultImage(int productID)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var query = from item in db.ProductImages
                            where item.ProductID == productID
                            && item.ProductImagePlace == ProductImagePlace.Home
                            select new EditProductImage
                            {
                                ID = item.ID,
                                Filename = item.Filename,
                                ProductImagePlace = item.ProductImagePlace
                            };

                return query.FirstOrDefault();
            }
        }

        public static int Count()
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var query = from item in db.ProductImages
                            select item;

                return query.Count();
            }
        }

        public static ProductImage GetByID(int id)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var productImage = db.ProductImages.Where(item => item.ID == id).Single();

                return productImage;
            }
        }

        public static void Delete(int id)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var productImage = (from item in db.ProductImages
                                    where item.ID == id
                                    select item).Single();

                db.ProductImages.Remove(productImage);

                db.SaveChanges();
            }
        }

        public static void Insert(ProductImage productImage)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                db.ProductImages.Add(productImage);

                db.SaveChanges();
            }
        }

        public static void Update(ProductImage productImage)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var orgProductImage = db.ProductImages.Where(item => item.ID == productImage.ID).Single();

                orgProductImage.ProductID = productImage.ProductID;
                orgProductImage.Filename = productImage.Filename;
                orgProductImage.ProductImagePlace = productImage.ProductImagePlace;
                orgProductImage.LastUpdate = productImage.LastUpdate;

                db.SaveChanges();
            }
        }

        public static void UpdateProductImagePlace(int id, ProductImagePlace productImagePlace)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var orgProductImage = db.ProductImages.Where(item => item.ID == id).Single();

                orgProductImage.ProductImagePlace = productImagePlace;

                db.SaveChanges();
            }
        }
    }
}
