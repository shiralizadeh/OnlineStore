using OnlineStore.Models.Enums;
using System;
using System.Collections;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using OnlineStore.EntityFramework;
using OnlineStore.Providers;
using OnlineStore.Models.Admin;
using OnlineStore.Models.User;
using OnlineStore.Models.Public;
using System.Collections.Generic;
using System.Drawing;

namespace OnlineStore.DataLayer
{
    public partial class Article : EntityBase
    {

        [Display(Name = "گروه مطلب")]
        [ForeignKey("Group")]
        public int? GroupID { get; set; }

        [Display(Name = "گروه مطلب")]
        public virtual Group Group { get; set; }

        [Display(Name = "کاربر")]
        [MaxLength(128)]
        public string UserID { get; set; }

        [Display(Name = "عنوان")]
        [MaxLength(300)]
        public string Title { get; set; }

        [Display(Name = "تصویر")]
        [MaxLength(100)]
        public string Image { get; set; }

        [Display(Name = "خلاصه")]
        [MaxLength(500)]
        public string Summary { get; set; }

        [Display(Name = "متن")]
        public string Text { get; set; }

        [Display(Name = "تاریخ انتشار")]
        public DateTime PublishDate { get; set; }

        [Display(Name = "تاریخ انتشار")]
        [NotMapped]
        public string PersianPublishDate
        {
            get
            {
                if (PublishDate == new DateTime())
                    return "";

                return Utilities.ToPersianDate(PublishDate);
            }
            set
            {
                if (String.IsNullOrWhiteSpace(value))
                    PublishDate = DateTime.Now;

                PublishDate = Utilities.ToEnglishDate(value);
            }
        }

        [Display(Name = "نوع مطلب")]
        public ArticleType ArticleType { get; set; }

        [Display(Name = "امتیاز مطلب")]
        public float ArticleScore { get; set; }

        [Display(Name = "وضعیت")]
        public ArticleStatus ArticleStatus { get; set; }

        [Display(Name = "قابل نمایش")]
        public bool IsVisible { get; set; }

        [Display(Name = "تعداد بازدید")]
        public int VisitCount { get; set; }

        [Display(Name = "خبر ویژه")]
        public bool IsLatestNews { get; set; }

        [Display(Name = "ترتیب نمایش")]
        public int OrderID { get; set; }


    }

    public static class Articles
    {
        public static List<ViewArticle> Get(int pageIndex,
                                            int pageSize,
                                            string pageOrder,
                                            ArticleType articleType,
                                            int groupID,
                                            string userName,
                                            string title,
                                            DateTime? fromDate,
                                            DateTime? toDate,
                                            ArticleStatus? articleStatus
                                            )
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var query = from item in db.Articles
                            where item.ArticleType == articleType
                            select new ViewArticle
                            {
                                ID = item.ID,
                                Title = item.Title,
                                LastUpdate = item.LastUpdate,
                                ArticleStatus = item.ArticleStatus,
                                PublishDate = item.PublishDate,
                                GroupName = item.Group.Title,
                                UserID = item.UserID,
                                IsVisible = item.IsVisible,
                                GroupID = item.GroupID.HasValue ? item.GroupID.Value : 0,
                                IsLatestNews = item.IsLatestNews,
                            };

                if (groupID != -1)
                    query = query.Where(item => item.GroupID == groupID);

                if (!String.IsNullOrWhiteSpace(title))
                    query = query.Where(item => item.Title.Contains(title));

                if (fromDate.HasValue)
                    query = query.Where(item => item.PublishDate >= fromDate);

                if (toDate.HasValue)
                    query = query.Where(item => item.PublishDate <= toDate);

                if (articleStatus.HasValue)
                    query = query.Where(item => item.ArticleStatus == articleStatus);

                if (!String.IsNullOrWhiteSpace(pageOrder))
                    query = query.OrderBy(pageOrder);

                query = query.Skip(pageIndex * pageSize).Take(pageSize);

                return query.ToList();
            }
        }

        public static DateTime LatestDateByGroupID(int groupID)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var list = from item in db.Articles
                           where item.GroupID == groupID
                           orderby item.LastUpdate descending
                           select item.LastUpdate;

                return list.First();
            }
        }

        public static int Count(ArticleType articleType,
                                int groupID,
                                string userName,
                                string title,
                                DateTime? fromDate,
                                DateTime? toDate,
                                ArticleStatus? articleStatus
                               )
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var query = from item in db.Articles
                            where item.ArticleType == articleType
                            select item;

                if (groupID != -1)
                    query = query.Where(item => item.GroupID == groupID);

                if (!String.IsNullOrWhiteSpace(title))
                    query = query.Where(item => item.Title.Contains(title));

                if (fromDate.HasValue)
                    query = query.Where(item => item.PublishDate >= fromDate);

                if (toDate.HasValue)
                    query = query.Where(item => item.PublishDate <= toDate);

                if (articleStatus.HasValue)
                    query = query.Where(item => item.ArticleStatus == articleStatus);

                return query.Count();
            }
        }

        public static Article GetByID(int id)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var article = db.Articles.Where(item => item.ID == id).Single();

                return article;
            }
        }

        public static ViewArticle GetTitleByID(int id)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var article = from item in db.Articles
                              where item.ID == id
                              select new ViewArticle
                              {
                                  Title = item.Title
                              };

                return article.SingleOrDefault();
            }
        }

        public static List<Article> GetByGroupID(int groupID)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var query = from item in db.Articles
                            orderby item.LastUpdate descending
                            where item.GroupID == groupID
                            select item;

                return query.ToList();
            }
        }

        public static void Delete(int id)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var article = (from item in db.Articles
                               where item.ID == id
                               select item).Single();

                db.Articles.Remove(article);

                db.SaveChanges();
            }
        }

        public static void Insert(Article article)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                db.Articles.Add(article);

                db.SaveChanges();
            }
        }

        public static void Update(Article article)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var orgarticle = db.Articles.Where(item => item.ID == article.ID).Single();

                orgarticle.GroupID = article.GroupID;
                orgarticle.Title = article.Title;
                orgarticle.Image = article.Image;
                orgarticle.Summary = article.Summary;
                orgarticle.Text = article.Text;
                orgarticle.PublishDate = article.PublishDate;
                orgarticle.ArticleStatus = article.ArticleStatus;
                orgarticle.ArticleScore = article.ArticleScore;
                orgarticle.IsVisible = article.IsVisible;
                orgarticle.IsLatestNews = article.IsLatestNews;
                orgarticle.OrderID = article.OrderID;
                orgarticle.LastUpdate = article.LastUpdate;

                db.SaveChanges();
            }
        }

        public static List<BlogPost> GetBlogList(int pageIndex, int pageSize, ArticleType articleType, DateTime publishDate, int? groupID)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var query = from item in db.Articles
                            where item.ArticleType == articleType &&
                            item.ArticleStatus == ArticleStatus.Approved &&
                            item.IsVisible &&
                            item.PublishDate <= publishDate
                            select new BlogPost
                            {
                                ID = item.ID,
                                Title = item.Title,
                                Summary = item.Summary,
                                LastUpdate = item.LastUpdate,
                                GroupName = item.GroupID.HasValue ? item.Group.Title : String.Empty,
                                GroupID = item.GroupID.HasValue ? item.GroupID.Value : 0,
                                UserID = item.UserID,
                                Image = item.Image,
                                VisitCount = item.VisitCount,
                                CommentsCount = db.ArticleComments.Where(
                                                                    comment => comment.ArticleID == item.ID &&
                                                                    comment.CommentStatus == ArticleCommentStatus.Approved
                                                                    ).Count()
                            };

                if (groupID.HasValue)
                    query = query.Where(item => item.GroupID == groupID);

                query = query.OrderByDescending(item => item.LastUpdate);

                query = query.Skip(pageIndex * pageSize).Take(pageSize);

                var list = query.ToList();

                return list;
            }
        }

        public static int CountBlogList(ArticleType articleType, DateTime publishDate, int? groupID)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var query = from item in db.Articles
                            where item.ArticleType == articleType &&
                            item.ArticleStatus == ArticleStatus.Approved &&
                            item.IsVisible &&
                            item.PublishDate <= publishDate
                            select item;

                if (groupID.HasValue && groupID.Value != 0)
                    query = query.Where(item => item.GroupID == groupID);

                return query.Count();
            }
        }

        public static BlogPost GetBlogByID(int id)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                DateTime now = DateTime.Now;

                var query = from item in db.Articles
                            where item.ID == id
                            select new BlogPost
                            {
                                ID = item.ID,
                                Title = item.Title,
                                LastUpdate = item.LastUpdate,
                                PublishDate = item.PublishDate,
                                GroupName = item.GroupID.HasValue ? item.Group.Title : String.Empty,
                                GroupID = item.GroupID.HasValue ? item.GroupID.Value : 0,
                                UserID = item.UserID,
                                Image = item.Image,
                                Text = item.Text,
                                VisitCount = item.VisitCount,
                                ArticleScore = item.ArticleScore,
                                CommentsCount = db.ArticleComments.Where(
                                                                    comment => comment.ArticleID == item.ID &&
                                                                    comment.CommentStatus == ArticleCommentStatus.Approved
                                                                    ).Count(),
                                SumScore = (from sum in db.ArticleRates
                                            where sum.ArticleID == item.ID
                                            select sum.Rate).Sum(),
                                ScoreCount = (from sum in db.ArticleRates
                                              where sum.ArticleID == item.ID
                                              select sum.Rate).Count(),

                            };

                return query.SingleOrDefault();
            }
        }

        public static List<JsonBlogSearch> SimpleSearch(string key, ArticleStatus? articleStatus = null, Size? imageSize = null)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                DateTime now = DateTime.Now;

                var query = from item in db.Articles
                            where (item.ArticleStatus == ArticleStatus.Approved || !articleStatus.HasValue)
                            && now >= item.PublishDate
                            && item.IsVisible
                            && (item.Title.Contains(key))
                            orderby item.LastUpdate descending
                            select new JsonBlogSearch
                            {
                                ID = item.ID,
                                Title = item.Title,
                                Image = item.Image,
                                GroupTitle = item.GroupID.HasValue ? item.Group.Title : String.Empty,
                                Summary = item.Summary
                            };

                var result = query.Take(20).ToList();

                foreach (var item in result)
                {
                    item.Image = UrlProvider.GetPostImage(item.Image, !imageSize.HasValue ? StaticValues.SearchImageSize : imageSize.Value);
                }

                return result;
            }
        }

        public static List<RecentPost> GetLatestPosts(int? groupID = null)
        {
            DateTime now = DateTime.Now;

            using (var db = OnlineStoreDbContext.Entity)
            {
                var query = from item in db.Articles
                            where item.ArticleType == ArticleType.Blog &&
                            item.ArticleStatus == ArticleStatus.Approved &&
                            item.IsVisible &&
                            item.IsLatestNews &&
                            item.PublishDate <= now
                            orderby item.LastUpdate descending
                            select new RecentPost
                            {
                                ID = item.ID,
                                Title = item.Title,
                                Summary = item.Summary,
                                Image = item.Image,
                                LastUpdate = item.LastUpdate,
                                GroupID = item.GroupID.HasValue ? item.GroupID.Value : 0,
                                OrderID = item.OrderID,
                                GroupTitle = item.Group.Title
                            };

                if (groupID.HasValue)
                {
                    query = query.Where(item => item.GroupID == groupID);
                }

                var list = query.Take(5).ToList();

                return list;
            }
        }

        public static DateTime LatestLastUpdate(int? groupID = null)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var now = DateTime.Now;

                var post = from item in db.Articles
                           where item.ArticleType == ArticleType.Blog &&
                           item.ArticleStatus == ArticleStatus.Approved &&
                           item.IsVisible &&
                           item.IsLatestNews &&
                           item.PublishDate <= now &&
                           (!groupID.HasValue || (groupID.HasValue && item.GroupID == groupID.Value))
                           orderby item.LastUpdate descending
                           select item.LastUpdate;
                return post.First();
            }
        }

        public static void AddVisits(int id)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var orgArticle = db.Articles.Where(item => item.ID == id).Single();

                orgArticle.VisitCount++;

                db.SaveChanges();
            }
        }

        public static List<RSSProductsBlog> GetLatest()
        {
            DateTime now = DateTime.Now;

            using (var db = OnlineStoreDbContext.Entity)
            {
                var query = from item in db.Articles
                            where item.ArticleType == ArticleType.Blog &&
                            item.ArticleStatus == ArticleStatus.Approved &&
                            item.IsVisible &&
                            item.IsLatestNews &&
                            item.PublishDate <= now
                            orderby item.LastUpdate descending
                            select new RSSProductsBlog
                            {
                                ID = item.ID,
                                Title_Fa = item.Title,
                                Summary = item.Summary,
                                Image = item.Image,
                                Date = item.LastUpdate,
                                GroupID = item.GroupID.HasValue ? item.GroupID.Value : 0,
                                GroupTitle = item.Group.Title,
                                Type = RSSRowType.Blog
                            };

                var list = query.Take(10).ToList();

                return list;
            }
        }

        #region User

        public static List<UserViewArticle> GetByUserID(ArticleType articleType, string userID)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var query = from item in db.Articles
                            where item.ArticleType == articleType && item.UserID == userID
                            select new UserViewArticle
                            {
                                ID = item.ID,
                                Title = item.Title,
                                LastUpdate = item.LastUpdate,
                                ArticleStatus = item.ArticleStatus,
                                PublishDate = item.PublishDate,
                                GroupName = item.Group.Title,
                                VisitCount = item.VisitCount

                            };

                return query.ToList();
            }
        }

        public static int CountByUserID(ArticleType articleType, string userID)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var query = from item in db.Articles
                            where item.ArticleType == articleType && item.UserID == userID
                            select item;

                return query.Count();
            }
        }

        public static void UpdateByUser(Article article)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var orgarticle = db.Articles.Where(item => item.ID == article.ID).Single();

                orgarticle.GroupID = article.GroupID;
                orgarticle.Title = article.Title;
                orgarticle.Image = article.Image;
                orgarticle.Summary = article.Summary;
                orgarticle.Text = article.Text;
                orgarticle.ArticleStatus = article.ArticleStatus;
                orgarticle.LastUpdate = article.LastUpdate;

                db.SaveChanges();
            }
        }

        public static bool ExistsByGroupID(int groupID)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var list = from item in db.Articles
                           where item.GroupID == groupID
                           select item;

                return list.Any();
            }
        }

        #endregion User
    }
}
