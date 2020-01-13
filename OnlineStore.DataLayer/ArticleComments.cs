using OnlineStore.Models.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Collections;
using OnlineStore.EntityFramework;
using System.Collections.Generic;
using OnlineStore.Models.Admin;
using OnlineStore.Models.Public;
using System;

namespace OnlineStore.DataLayer
{
    public partial class ArticleComment : EntityBase
    {
        [Display(Name = "مطلب مربوطه")]
        [ForeignKey("Article")]
        public int ArticleID { get; set; }

        [Display(Name = "مطلب مربوطه")]
        public Article Article { get; set; }

        [Display(Name = "پاسخ به")]
        [ForeignKey("ReplyTo")]
        public int? ReplyToID { get; set; }

        [Display(Name = "پاسخ به")]
        public ArticleComment ReplyTo { get; set; }

        [Display(Name = "کد کاربر")]
        [MaxLength(128)]
        public string UserID { get; set; }

        [Display(Name = "نام کاربر")]
        [MaxLength(100)]
        public string UserName { get; set; }

        [Display(Name = "ایمیل کاربر")]
        [MaxLength(100)]
        public string Email { get; set; }

        [Display(Name = "موضوع")]
        [MaxLength(300)]
        public string Subject { get; set; }

        [Display(Name = "متن نظر")]
        public string Text { get; set; }

        [Display(Name = "وضعیت نظر")]
        public ArticleCommentStatus CommentStatus { get; set; }
    }

    public static class ArticleComments
    {
        public static void Insert(ArticleComment comment)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                db.ArticleComments.Add(comment);

                db.SaveChanges();
            }
        }

        public static List<ViewArticleComments> Get(int pageIndex, int pageSize, string pageOrder, int? articleID, string email, ArticleCommentStatus? articleCommentStatus)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var query = from item in db.ArticleComments
                            select new ViewArticleComments
                            {
                                ID = item.ID,
                                Subject = item.Subject,
                                UserID = item.UserID,
                                UserName = item.UserName,
                                Email = item.Email,
                                LastUpdate = item.LastUpdate,
                                CommentStatus = item.CommentStatus,
                                ArticleID = item.ArticleID
                            };

                if (articleID.HasValue)
                    query = query.Where(item => item.ArticleID == articleID);

                if (!String.IsNullOrWhiteSpace(email))
                    query = query.Where(item => item.Email.Contains(email));

                if (articleCommentStatus.HasValue)
                    query = query.Where(item => item.CommentStatus == articleCommentStatus);

                if (!String.IsNullOrWhiteSpace(pageOrder))
                    query = query.OrderBy(pageOrder);

                query = query.Skip(pageIndex * pageSize).Take(pageSize);

                return query.ToList();
            }
        }

        public static List<RecentComment> GetLatestComments(ArticleType articleType, int count, int? groupID = null)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var query = from item in db.ArticleComments
                            where item.CommentStatus == ArticleCommentStatus.Approved &&
                            item.Article.ArticleType == articleType
                            select new RecentComment
                            {
                                ID = item.ID,
                                UserID = item.UserID,
                                UserName = item.UserName,
                                LastUpdate = item.LastUpdate,
                                ArticleID = item.ArticleID,
                                ArticleTitle = item.Article.Title,
                                Text = item.Text,
                                Subject = item.Subject,
                                GroupID = item.Article.GroupID.HasValue ? item.Article.GroupID.Value : 0
                            };

                if (groupID.HasValue)
                {
                    query = query.Where(item => item.GroupID == groupID.Value);
                }

                query = query.OrderByDescending(item => item.LastUpdate).Take(count);

                return query.ToList();
            }
        }

        public static int Count(int? articleID, string email, ArticleCommentStatus? articleCommentStatus)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var query = from item in db.ArticleComments
                            select item;

                if (articleID.HasValue)
                    query = query.Where(item => item.ArticleID == articleID);

                if (!String.IsNullOrWhiteSpace(email))
                    query = query.Where(item => item.Email.Contains(email));

                if (articleCommentStatus.HasValue)
                    query = query.Where(item => item.CommentStatus == articleCommentStatus);

                return query.Count();
            }
        }

        public static ArticleComment GetByID(int id)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var comment = db.ArticleComments.Where(item => item.ID == id).Single();

                return comment;
            }
        }

        public static void Delete(int id)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var comment = (from item in db.ArticleComments
                               where item.ID == id
                               select item).Single();

                db.ArticleComments.Remove(comment);

                db.SaveChanges();
            }
        }

        public static void Update(ArticleComment comment)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var orgComment = db.ArticleComments.Where(item => item.ID == comment.ID).Single();

                orgComment.CommentStatus = comment.CommentStatus;
                orgComment.Subject = comment.Subject;
                orgComment.Text = comment.Text;

                db.SaveChanges();
            }
        }

        public static void Confirm(List<int> ids)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var list = db.ArticleComments.Where(item => ids.Contains(item.ID) && item.CommentStatus != ArticleCommentStatus.Approved);

                foreach (var item in list)
                {
                    item.CommentStatus = ArticleCommentStatus.Approved;
                }

                db.SaveChanges();
            }
        }

        public static List<ViewArticleComment> GetByArticleID(int articleID)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var query = (from comments in db.ArticleComments
                             where comments.ArticleID == articleID
                             && comments.CommentStatus == ArticleCommentStatus.Approved
                             select new ViewArticleComment
                             {
                                 ID = comments.ID,
                                 Subject = comments.Subject,
                                 Text = comments.Text,
                                 LastUpdate = comments.LastUpdate,
                                 UserID = comments.UserID,
                                 UserName = comments.UserName

                             }).ToList();

                return query.ToList();
            }
        }
    }
}
