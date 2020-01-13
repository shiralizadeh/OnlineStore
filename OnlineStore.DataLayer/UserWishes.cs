using OnlineStore.Models.Enums;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OnlineStore.EntityFramework;
using OnlineStore.Models.Public;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineStore.DataLayer
{
    public class UserWishe : EntityBase
    {
        public string UserID { get; set; }

        [ForeignKey("Product")]
        public int ProductID { get; set; }
        public Product Product { get; set; }
    }

    public static class UserWishes
    {
        public static IList Get(int pageIndex, int pageSize, string pageOrder)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var query = from item in db.UserWishes
                            select new
                            {
                                item.ID,
                                LastUpdate = item.LastUpdate,
                            };

                if (!string.IsNullOrWhiteSpace(pageOrder))
                    query = query.OrderBy(pageOrder);

                query = query.Skip(pageIndex * pageSize).Take(pageSize);

                return query.ToList();
            }
        }

        public static int Count()
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var query = from item in db.UserWishes
                            select item;

                return query.Count();
            }
        }

        public static UserWishe GetByID(int id)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var userWishe = db.UserWishes.Where(item => item.ID == id).Single();

                return userWishe;
            }
        }

        public static List<ProductItem> GetByUserID(string userID)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var now = DateTime.Now.Date;

                var query = from item in db.UserWishes
                            where item.Product.ProductStatus == ProductStatus.Approved
                            && now >= item.Product.PublishDate
                            && item.Product.IsInVisible == false
                            && item.UserID == userID
                            select new ProductItem
                            {
                                ID = item.ProductID,
                                Title_Fa = item.Product.Title,
                                Title_En = item.Product.Title_En,
                                GroupID = item.Product.GroupID.Value,
                                DisplayTitleType = item.Product.DisplayTitleType,
                                HasVarients = item.Product.HasVarients,
                                IsUnavailable = item.Product.IsUnavailable,
                                CommentCount = (from comment in db.ProductComments
                                                where comment.ProductID == item.ProductID && comment.CommentStatus == CommentStatus.Approved
                                                select comment).Count(),

                                ImageFile = (from img in db.ProductImages
                                             where img.ProductID == item.ProductID && img.ProductImagePlace == ProductImagePlace.Home
                                             select img.Filename).FirstOrDefault(),
                                ProductScore = item.Product.ProductScore,
                                SumScore = (from sum in db.ProductRates
                                            where sum.ProductID == item.ProductID
                                            select sum.Rate).Sum(),
                                ScoreCount = (from sum in db.ProductRates
                                              where sum.ProductID == item.ProductID
                                              select sum.Rate).Count(),
                            };

                query = query.OrderByDescending(item => item.ID);

                return query.ToList();
            }
        }

        public static int CountByUserID(string userID)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var now = DateTime.Now.Date;

                var query = from item in db.UserWishes
                            where item.Product.ProductStatus == ProductStatus.Approved
                            && now >= item.Product.PublishDate
                            && item.Product.IsInVisible == false
                            && item.UserID == userID
                            select item;

                return query.Count();
            }
        }

        public static void Delete(int id)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var userWishe = (from item in db.UserWishes
                                 where item.ID == id
                                 select item).Single();

                db.UserWishes.Remove(userWishe);

                db.SaveChanges();
            }
        }

        public static bool Exists(string userID, int productID)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                return db.UserWishes.Any(item => item.UserID == userID && item.ProductID == productID);
            }
        }

        public static void Insert(UserWishe userWishe)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                db.UserWishes.Add(userWishe);

                db.SaveChanges();
            }
        }

        public static void Update(UserWishe userWishe)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var orgUserWishe = db.UserWishes.Where(item => item.ID == userWishe.ID).Single();

                orgUserWishe.ProductID = userWishe.ProductID;
                orgUserWishe.UserID = userWishe.UserID;
                orgUserWishe.LastUpdate = userWishe.LastUpdate;

                db.SaveChanges();
            }
        }
    }
}
