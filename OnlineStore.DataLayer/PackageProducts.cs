using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OnlineStore.Models.Admin;
using OnlineStore.Models.Public;
using OnlineStore.Models.Enums;

namespace OnlineStore.DataLayer
{
    public class PackageProduct : EntityBase
    {
        [ForeignKey("Package")]
        public int PackageID { get; set; }
        public Package Package { get; set; }

        [ForeignKey("Product")]
        public int ProductID { get; set; }
        public Product Product { get; set; }

        [ForeignKey("ProductVarient")]
        public int? ProductVarientID { get; set; }
        public ProductVarient ProductVarient { get; set; }

        [Display(Name = "قیمت قدیم")]
        public int OldPrice { get; set; }

        [Display(Name = "قیمت جدید")]
        public int NewPrice { get; set; }
    }

    public static class PackageProducts
    {
        public static List<PackageProduct> GetByPackageID(int packageID)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var query = from item in db.PackageProducts
                            where item.PackageID == packageID
                            select item;

                return query.ToList();
            }
        }

        public static void Delete(int id)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var packageProduct = (from item in db.PackageProducts
                                      where item.ID == id
                                      select item).Single();

                db.PackageProducts.Remove(packageProduct);

                db.SaveChanges();
            }
        }

        public static void Insert(PackageProduct packageProduct)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                db.PackageProducts.Add(packageProduct);

                db.SaveChanges();
            }
        }

        public static void Update(PackageProduct packageProduct)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var orgPackageProduct = db.PackageProducts.Where(item => item.ID == packageProduct.ID).Single();

                orgPackageProduct.ProductID = packageProduct.ProductID;
                orgPackageProduct.ProductVarientID = packageProduct.ProductVarientID;
                orgPackageProduct.OldPrice = packageProduct.OldPrice;
                orgPackageProduct.NewPrice = packageProduct.NewPrice;
                orgPackageProduct.LastUpdate = packageProduct.LastUpdate;

                db.SaveChanges();
            }
        }

        public static List<ViewPackageProduct> GetProducts(int packageID)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var query = from item in db.PackageProducts
                            where item.PackageID == packageID
                            select new ViewPackageProduct
                            {
                                OldPrice = item.OldPrice,
                                NewPrice = item.NewPrice,
                                DisplayTitleType = item.Product.DisplayTitleType,
                                Title = item.Product.Title,
                                Title_En = item.Product.Title_En,
                                GroupID = item.Product.GroupID.Value,
                                ProductID = item.ProductID,
                                ProductVarientID = item.ProductVarientID,
                                ImageFile = (from img in db.ProductImages
                                             where img.ProductID == item.ProductID && img.ProductImagePlace == ProductImagePlace.Home
                                             select img.Filename).FirstOrDefault()
                            };

                var result = query.ToList();

                foreach (var item in result)
                {
                    if (item.ProductVarientID.HasValue)
                    {
                        item.ProductVarientTitle = ProductVarientAttributes.GetVarients(item.ProductVarientID.Value);
                    }
                }

                return result;
            }
        }
    }
}
