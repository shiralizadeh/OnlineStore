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
    public class PackageImage : EntityBase
    {
        [ForeignKey("Package")]
        public int PackageID { get; set; }
        public Package Package { get; set; }

        [MaxLength(200)]
        public string Filename { get; set; }

        public ProductImagePlace PackageImagePlace { get; set; }
    }

    public static class PackageImages
    {
        public static List<EditProductImage> GetByPackageID(int packageID)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var query = from item in db.PackageImages
                            where item.PackageID == packageID
                            select new EditProductImage
                            {
                                ID = item.ID,
                                Filename = item.Filename,
                                ProductImagePlace = item.PackageImagePlace,
                            };

                return query.ToList();
            }
        }

        public static PackageImage GetByID(int id)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var packageImage = db.PackageImages.Where(item => item.ID == id).Single();

                return packageImage;
            }
        }

        public static void Delete(int id)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var packageImage = (from item in db.PackageImages
                                    where item.ID == id
                                    select item).Single();

                db.PackageImages.Remove(packageImage);

                db.SaveChanges();
            }
        }

        public static void Insert(PackageImage packageImage)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                db.PackageImages.Add(packageImage);

                db.SaveChanges();
            }
        }

        public static void UpdatePackageImagePlace(int id, ProductImagePlace packageImagePlace)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var orgPackageImage = db.PackageImages.Where(item => item.ID == id).Single();

                orgPackageImage.PackageImagePlace = packageImagePlace;

                db.SaveChanges();
            }
        }
    }

}
