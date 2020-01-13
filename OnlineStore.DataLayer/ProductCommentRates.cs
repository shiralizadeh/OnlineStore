using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OnlineStore.Models.User;

namespace OnlineStore.DataLayer
{
    public class ProductCommentRate : EntityBase
    {
        [ForeignKey("ScoreComment")]
        public int ScoreCommentID { get; set; }
        public ScoreComment ScoreComment { get; set; }

        public string UserID { get; set; }
        public bool IsLike { get; set; }
    }

    public static class ProductCommentRates
    {
        public static ProductCommentRate GetByUserID_CommentID(string userID, int commentID)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var comment = db.ProductCommentRates.Where(item => item.UserID == userID &&
                                                                   item.ScoreCommentID == commentID).FirstOrDefault();

                return comment;
            }
        }

        public static List<ViewProductCommentRate> GetByUserID(string userID)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var query = from item in db.ProductCommentRates
                            where item.UserID == userID
                            select new ViewProductCommentRate
                            {
                                ID = item.ID,
                                CommentText = item.ScoreComment.Text,
                                Title_En = item.ScoreComment.Product.Title_En,
                                Title_Fa = item.ScoreComment.Product.Title,
                                DisplayTitleType = item.ScoreComment.Product.DisplayTitleType,
                                IsLike = item.IsLike,
                                LastUpdate = item.LastUpdate
                            };

                return query.ToList();
            }
        }

        public static int CountByUserID(string userID)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var query = from item in db.ProductCommentRates
                            where item.UserID == userID
                            select item;

                return query.Count();
            }
        }

        public static void Insert(ProductCommentRate productCommentRate)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                db.ProductCommentRates.Add(productCommentRate);

                db.SaveChanges();
            }
        }

        public static int CountRates(bool isLike, int id)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                return db.ProductCommentRates.Where(item => item.IsLike == isLike &&
                                                            item.ScoreCommentID == id).Count();
            }
        }

        public static void Update(ProductCommentRate comment)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var orgComment = db.ProductCommentRates.Where(item => item.ID == comment.ID).Single();

                orgComment.IsLike = comment.IsLike;

                db.SaveChanges();
            }
        }
    }
}
