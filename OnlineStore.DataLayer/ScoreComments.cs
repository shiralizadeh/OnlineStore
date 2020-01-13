using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OnlineStore.Models.Enums;
using OnlineStore.Models.User;
using OnlineStore.EntityFramework;
using Public = OnlineStore.Models.Public;
using Admin = OnlineStore.Models.Admin;
using System.ComponentModel.DataAnnotations;

namespace OnlineStore.DataLayer
{
    public class ScoreComment : EntityBase
    {
        [ForeignKey("Product")]
        public int ProductID { get; set; }
        public Product Product { get; set; }

        public string UserID { get; set; }

        [Display(Name = "توضیحات")]
        public string Text { get; set; }
        public ScoreCommentStatus ScoreCommentStatus { get; set; }
    }

    public static class ScoreComments
    {
        public static void Insert(ScoreComment scoreComment)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                db.ScoreComments.Add(scoreComment);

                db.SaveChanges();
            }
        }

        public static JsonUserScore GetUserScores(int productID, string userID)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var score = from item in db.ScoreComments
                            where item.ProductID == productID && item.UserID == userID
                            select new JsonUserScore
                            {
                                Text = item.Text,
                                ScoreValues = (from rate in db.ScoreParameterValues
                                               where rate.ScoreCommentID == item.ID
                                               select new Public.ScoreValue
                                               {
                                                   Rate = rate.Rate,
                                                   ScoreParameterID = rate.ScoreParameterID

                                               }).ToList()
                            };

                return score.FirstOrDefault();
            }
        }

        public static List<ViewScoreComment> GetUserComments(string userID)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var comment = from item in db.ScoreComments
                              where item.UserID == userID
                              select new ViewScoreComment
                              {
                                  ID = item.ID,
                                  Title_En = item.Product.Title_En,
                                  Title_Fa = item.Product.Title,
                                  DisplayTitleType = item.Product.DisplayTitleType,
                                  Text = item.Text,
                                  ScoreCommentStatus = item.ScoreCommentStatus,
                                  LikeCount = (from like in db.ProductCommentRates
                                               where like.ScoreCommentID == item.ID && like.IsLike
                                               select like.ID).Count(),
                                  DisLikeCount = (from disLike in db.ProductCommentRates
                                                  where disLike.ScoreCommentID == item.ID && !disLike.IsLike
                                                  select disLike.ID).Count()
                              };

                return comment.ToList();
            }
        }

        /// <summary>
        /// نمایش لیست امتیازدهندگان
        /// </summary>
        /// <param name="productID">کد محصول</param>
        /// <returns>امتیازدهندگان + نتیجه امتیازات</returns>
        public static List<Public.ViewScoreComment> GetScoreComments(int productID)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var query = (from item in db.ScoreComments
                             where item.ProductID == productID && item.ScoreCommentStatus == ScoreCommentStatus.Approved
                             select new Public.ViewScoreComment
                             {
                                 ID = item.ID,
                                 ProductID = item.ProductID,
                                 UserID = item.UserID,
                                 Text = item.Text,
                                 LastUpdate = item.LastUpdate,
                                 ScoreValues = (from rate in db.ScoreParameterValues
                                                where rate.ScoreCommentID == item.ID
                                                select new Public.ScoreValue
                                                {
                                                    Rate = rate.Rate,
                                                    ScoreParameterID = rate.ScoreParameterID,
                                                    Title = rate.ScoreParameter.Title,
                                                }).OrderBy(order => order.ScoreParameterID).ToList(),

                                 LikesCount = (from like in db.ProductCommentRates
                                               where like.ScoreCommentID == item.ID && like.IsLike
                                               select like).Count(),

                                 DisLikesCount = (from disLike in db.ProductCommentRates
                                                  where disLike.ScoreCommentID == item.ID && !disLike.IsLike
                                                  select disLike).Count()
                             });

                query = query.OrderByDescending(item => item.LastUpdate);

                return query.ToList();
            }
        }

        public static List<Admin.ViewScoreComment> Get(int pageIndex, int pageSize, string pageOrder, int? productID, ScoreCommentStatus? scoreCommentStatus)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var query = from item in db.ScoreComments
                            select new Admin.ViewScoreComment
                            {
                                ID = item.ID,
                                UserID = item.UserID,
                                LastUpdate = item.LastUpdate,
                                Text = item.Text,
                                ScoreCommentStatus = item.ScoreCommentStatus,
                                ProductID = item.ProductID
                            };

                if (productID.HasValue)
                    query = query.Where(item => item.ProductID == productID);

                if (scoreCommentStatus.HasValue)
                    query = query.Where(item => item.ScoreCommentStatus == scoreCommentStatus);

                if (!string.IsNullOrWhiteSpace(pageOrder))
                    query = query.OrderBy(pageOrder);

                query = query.Skip(pageIndex * pageSize).Take(pageSize);

                return query.ToList();
            }
        }

        public static int Count(int? productID, ScoreCommentStatus? scoreCommentStatus)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var query = from item in db.ScoreComments
                            select item;

                if (productID.HasValue)
                    query = query.Where(item => item.ProductID == productID);

                if (scoreCommentStatus.HasValue)
                    query = query.Where(item => item.ScoreCommentStatus == scoreCommentStatus);

                return query.Count();
            }
        }

        public static int CountByUserID(string userID)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var query = from item in db.ScoreComments
                            where item.UserID == userID
                            select item;

                return query.Count();
            }
        }

        public static Admin.EditScoreComment GetByID(int id)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var query = (from item in db.ScoreComments
                             where item.ID == id
                             select new Admin.EditScoreComment
                             {
                                 ID = item.ID,
                                 ProductID = item.ProductID,
                                 UserID = item.UserID,
                                 ProductTitle = item.Product.Title,
                                 Text = item.Text,
                                 LastUpdate = item.LastUpdate,
                                 ScoreCommentStatus = item.ScoreCommentStatus,
                                 ScoreValues = (from rate in db.ScoreParameterValues
                                                where rate.ScoreCommentID == id
                                                select new Public.ScoreValue
                                                {
                                                    Rate = rate.Rate,
                                                    ScoreParameterID = rate.ScoreParameterID,
                                                    Title = rate.ScoreParameter.Title
                                                }).ToList()
                             });

                return query.SingleOrDefault();
            }
        }

        public static void Delete(int id)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var comment = (from item in db.ScoreComments
                               where item.ID == id
                               select item).Single();

                db.ScoreComments.Remove(comment);

                db.SaveChanges();
            }
        }

        public static void Update(ScoreComment scoreComment)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var orgComment = db.ScoreComments.Where(item => item.ID == scoreComment.ID).Single();

                orgComment.Text = scoreComment.Text;
                orgComment.ScoreCommentStatus = scoreComment.ScoreCommentStatus;
                orgComment.LastUpdate = scoreComment.LastUpdate;

                db.SaveChanges();
            }
        }

        public static void UpdateByUser(ScoreComment scoreComment)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var orgComment = db.ScoreComments.Where(item => item.ID == scoreComment.ID).Single();

                orgComment.Text = scoreComment.Text;
                orgComment.LastUpdate = scoreComment.LastUpdate;

                db.SaveChanges();
            }
        }

        public static void Confirm(List<int> ids)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var list = db.ScoreComments.Where(item => ids.Contains(item.ID) && item.ScoreCommentStatus != ScoreCommentStatus.Approved);

                foreach (var item in list)
                {
                    item.ScoreCommentStatus = ScoreCommentStatus.Approved;
                }

                db.SaveChanges();
            }
        }

        public static List<Public.ViewScoreComment> GetLatestComments(int productID)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var query = (from item in db.ScoreComments
                             where item.ScoreCommentStatus == ScoreCommentStatus.Approved
                             && item.ProductID == productID
                             select new Public.ViewScoreComment
                             {
                                 ID = item.ID,
                                 ProductID = item.ProductID,
                                 LastUpdate = item.LastUpdate,
                                 Text = item.Text
                             });

                query = query.OrderByDescending(item => item.LastUpdate);

                var result = query.Take(10).ToList();

                return result;
            }
        }

        public static DateTime LatestLastUpdate(int productID)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var query = from item in db.ScoreComments
                            where item.ScoreCommentStatus == ScoreCommentStatus.Approved
                            && item.ProductID == productID
                            orderby item.LastUpdate descending
                            select item.LastUpdate;

                return query.FirstOrDefault();
            }
        }
    }
}
