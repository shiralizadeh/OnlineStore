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
    public class HomeBoxProduct : EntityBase
    {
        [ForeignKey("Product")]
        public int ProductID { get; set; }
        public Product Product { get; set; }

        [ForeignKey("HomeBox")]
        public int HomeBoxID { get; set; }
        public HomeBox HomeBox { get; set; }
    }

    public static class HomeBoxProducts
    {
        public static List<JsonHomeBoxProduct> Get(int homeBoxID)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var query = from item in db.HomeBoxProducts
                            where item.HomeBoxID == homeBoxID
                            select new JsonHomeBoxProduct
                            {
                                ID = item.ID,
                                Title = item.Product.Title,
                                ProductID = item.ProductID
                            };

                return query.ToList();
            }
        }

        public static List<ProductItem> GetProductItemByBoxID(int homeBoxID)
        {
            var now = DateTime.Now;

            using (var db = OnlineStoreDbContext.Entity)
            {
                var query = from boxItem in db.HomeBoxProducts
                            join product in db.Products on boxItem.ProductID equals product.ID
                            where
                            boxItem.HomeBoxID == homeBoxID &&
                            now >= product.PublishDate &&
                            product.IsInVisible == false &&
                            product.IsUnavailable == false &&
                            product.ProductStatus == ProductStatus.Approved
                            select new ProductItem
                            {
                                ID = product.ID,
                                GroupID = product.GroupID,
                                DisplayTitleType = product.DisplayTitleType,

                                Title_Fa = product.Title,
                                Title_En = product.Title_En,

                                IsUnavailable = product.IsUnavailable,
                                HasVarients = product.HasVarients,
                                CommentCount = (from comment in db.ScoreComments
                                                where comment.ProductID == product.ID && comment.ScoreCommentStatus == ScoreCommentStatus.Approved
                                                select comment).Count(),

                                ImageFile = (from img in db.ProductImages
                                             where img.ProductID == product.ID && img.ProductImagePlace == ProductImagePlace.Home
                                             select img.Filename).FirstOrDefault(),
                                ProductScore = product.ProductScore,
                                SumScore = (from sum in db.ProductRates
                                            where sum.ProductID == product.ID
                                            select sum.Rate).Sum(),
                                ScoreCount = (from sum in db.ProductRates
                                              where sum.ProductID == product.ID
                                              select sum.Rate).Count(),
                            };

                return query.ToList();
            }
        }

        public static List<ViewHomeBoxItem> GetHomeBoxItemsByBoxID(int homeBoxID)
        {
            var now = DateTime.Now;

            using (var db = OnlineStoreDbContext.Entity)
            {
                var query = from boxItem in db.HomeBoxItems
                            where boxItem.HomeBoxID == homeBoxID
                            orderby boxItem.OrderID
                            select new ViewHomeBoxItem
                            {
                                ID = boxItem.ID,
                                Title = boxItem.Title,
                                SubTitle = boxItem.SubTitle,
                                Filename = boxItem.Filename,
                                Link = boxItem.Link,
                            };

                return query.ToList();
            }
        }

        public static void Delete(int id)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var homeBoxItem = (from item in db.HomeBoxProducts
                                   where item.ID == id
                                   select item).Single();

                db.HomeBoxProducts.Remove(homeBoxItem);

                db.SaveChanges();
            }
        }

        public static void Insert(HomeBoxProduct homeBoxItem)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                db.HomeBoxProducts.Add(homeBoxItem);

                db.SaveChanges();
            }
        }

        public static bool checkRepeat(int homeBoxID, int productID)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                int count = db.HomeBoxProducts.Where(item => item.HomeBoxID == homeBoxID && item.ProductID == productID).Count();

                if (count > 0)
                {
                    return true;
                }

                return false;
            }
        }

        public static void Insert(List<HomeBoxProduct> HomeBoxProducts)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                db.HomeBoxProducts.AddRange(HomeBoxProducts);

                db.SaveChanges();
            }
        }

        public static void DeleteBoxItems(int homeBoxID)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var homeBoxItem = (from item in db.HomeBoxProducts
                                   where item.HomeBoxID == homeBoxID
                                   select item);

                db.HomeBoxProducts.RemoveRange(homeBoxItem);

                db.SaveChanges();
            }
        }


    }
}
