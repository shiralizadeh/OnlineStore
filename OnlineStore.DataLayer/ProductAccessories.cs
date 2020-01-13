using OnlineStore.Models.Admin;
using OnlineStore.Models.Enums;
using OnlineStore.Models.Public;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineStore.DataLayer
{
    public class ProductAccessory : EntityBase
    {
        [Display(Name = "محصول جانبی")]
        [ForeignKey("Accessory")]
        public int AccessoryID { get; set; }
        public Product Accessory { get; set; }

        [Display(Name = "محصول")]
        //[ForeignKey("Product")]
        public int ProductID { get; set; }
        //public Product Product { get; set; }

    }

    public static class ProductAccessories
    {
        public static void Insert(List<ProductAccessory> productAccessory)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                db.ProductAccessories.AddRange(productAccessory);

                db.SaveChanges();
            }
        }

        public static List<JsonRelatedProduct> Get(int productID)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var query = from item in db.ProductAccessories
                            where item.ProductID == productID
                            select new JsonRelatedProduct
                            {
                                ID = item.ID,
                                Title = item.Accessory.Title,
                                RelationID = item.AccessoryID
                            };

                return query.ToList();
            }
        }

        public static List<ProductItem> GetByProductID(int productID)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                DateTime now = DateTime.Now;

                var accessories = (from item in db.ProductAccessories
                                   where item.ProductID == productID
                                   && item.Accessory.ProductStatus == ProductStatus.Approved
                                   && now >= item.Accessory.PublishDate
                                   && item.Accessory.IsInVisible == false
                                   select new ProductItem
                                   {
                                       ID = item.Accessory.ID,
                                       Title_Fa = item.Accessory.Title,
                                       Title_En = item.Accessory.Title_En,
                                       DisplayTitleType = item.Accessory.DisplayTitleType,
                                       GroupID = item.Accessory.GroupID,
                                       IsUnavailable = item.Accessory.IsUnavailable,

                                       HasVarients = item.Accessory.HasVarients,
                                       CommentCount = (from comment in db.ScoreComments
                                                       where comment.ProductID == item.AccessoryID && comment.ScoreCommentStatus == ScoreCommentStatus.Approved
                                                       select comment).Count(),
                                       ImageFile = (from img in db.ProductImages
                                                    where img.ProductID == item.AccessoryID && img.ProductImagePlace == ProductImagePlace.Home
                                                    select img.Filename).FirstOrDefault(),
                                       ProductScore = item.Accessory.ProductScore,
                                       SumScore = (from sum in db.ProductRates
                                                   where sum.ProductID == item.ProductID
                                                   select sum.Rate).Sum(),
                                       ScoreCount = (from sum in db.ProductRates
                                                     where sum.ProductID == item.ProductID
                                                     select sum.Rate).Count(),
                                   });

                return accessories.ToList();
            }
        }

        public static void DeleteProductAccessories(int productID)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var accessories = (from item in db.ProductAccessories
                                   where item.ProductID == productID
                                   select item);

                db.ProductAccessories.RemoveRange(accessories);

                db.SaveChanges();
            }
        }

    }
}
