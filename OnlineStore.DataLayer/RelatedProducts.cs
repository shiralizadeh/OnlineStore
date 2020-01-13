using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OnlineStore.EntityFramework;
using OnlineStore.Models.Admin;
using OnlineStore.Models.Public;
using OnlineStore.Models.Enums;

namespace OnlineStore.DataLayer
{
    public class RelatedProduct : EntityBase
    {
        [Display(Name = "محصول مرتبط")]
        [ForeignKey("Relation")]
        public int RelationID { get; set; }
        public Product Relation { get; set; }

        [Display(Name = "محصول")]
        [ForeignKey("Product")]
        public int? ProductID { get; set; }
        public Product Product { get; set; }

    }

    public static class RelatedProducts
    {
        public static void Insert(List<RelatedProduct> relatedProduct)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                db.RelatedProducts.AddRange(relatedProduct);

                db.SaveChanges();
            }
        }

        public static List<JsonRelatedProduct> Get(int productID)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var query = from item in db.RelatedProducts
                            where item.ProductID == productID
                            select new JsonRelatedProduct
                            {
                                ID = item.ID,
                                Title = item.Relation.Title,
                                RelationID = item.RelationID,

                            };

                return query.ToList();
            }
        }

        public static List<ProductItem> GetByProductID(int productID)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                DateTime now = DateTime.Now;

                var relatedProducts = (from item in db.Products
                                       where
                                       item.ProductStatus == ProductStatus.Approved
                                       && now >= item.PublishDate
                                       && !item.IsInVisible
                                       && db.RelatedProducts.Any(rel => rel.ProductID == productID && rel.RelationID == item.ID)
                                       select new ProductItem
                                       {
                                           ID = item.ID,
                                           Title_Fa = item.Title,
                                           Title_En = item.Title_En,
                                           DisplayTitleType = item.DisplayTitleType,
                                           GroupID = item.GroupID,
                                           HasVarients = item.HasVarients,
                                           IsUnavailable = item.IsUnavailable,
                                           CommentCount = (from comment in db.ScoreComments
                                                           where comment.ProductID == item.ID && comment.ScoreCommentStatus == ScoreCommentStatus.Approved
                                                           select comment).Count(),
                                           ImageFile = (from img in db.ProductImages
                                                        where img.ProductID == item.ID && img.ProductImagePlace == ProductImagePlace.Home
                                                        select img.Filename).FirstOrDefault(),
                                           ProductScore = item.ProductScore,
                                           SumScore = (from sum in db.ProductRates
                                                       where sum.ProductID == item.ID
                                                       select sum.Rate).Sum(),
                                           ScoreCount = (from sum in db.ProductRates
                                                         where sum.ProductID == item.ID
                                                         select sum.Rate).Count(),
                                           PriceStatus = item.PriceStatus
                                       });

                return relatedProducts.ToList();
            }
        }

        public static void DeleteRelatedProducts(int productID)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var relatedProducts = (from item in db.RelatedProducts
                                       where item.ProductID == productID
                                       select item);

                db.RelatedProducts.RemoveRange(relatedProducts);

                db.SaveChanges();
            }
        }

    }
}
