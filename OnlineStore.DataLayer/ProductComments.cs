using OnlineStore.Models.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OnlineStore.Models.Admin;
using OnlineStore.EntityFramework;
using Public = OnlineStore.Models.Public;

namespace OnlineStore.DataLayer
{
    public class ProductComment : EntityBase
    {
        [Display(Name = "محصول مربوطه")]
        [ForeignKey("Product")]
        public int ProductID { get; set; }

        [Display(Name = "محصول مربوطه")]
        public Product Product { get; set; }

        [Display(Name = "پاسخ به")]
        [ForeignKey("ReplyTo")]
        public int? ReplyToID { get; set; }

        [Display(Name = "پاسخ به")]
        public ProductComment ReplyTo { get; set; }

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

        [Display(Name = "وضعیت")]
        public CommentStatus CommentStatus { get; set; }
    }

    public static class ProductComments
    {
        public static void Insert(ProductComment comment)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                db.ProductComments.Add(comment);

                db.SaveChanges();
            }
        }

        public static List<ViewProductComments> Get(int pageIndex, int pageSize, string pageOrder, int? productID, string email, CommentStatus? commentStatus)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var query = from item in db.ProductComments
                            select new ViewProductComments
                            {
                                ID = item.ID,
                                Subject = item.Subject,
                                UserID = item.UserID,
                                UserName = item.UserName,
                                Email = item.Email,
                                LastUpdate = item.LastUpdate,
                                CommentStatus = item.CommentStatus,
                                ProductID = item.ProductID
                            };

                if (productID.HasValue)
                    query = query.Where(item => item.ProductID == productID);

                if (!String.IsNullOrWhiteSpace(email))
                    query = query.Where(item => item.Email.Contains(email));

                if (commentStatus.HasValue)
                    query = query.Where(item => item.CommentStatus == commentStatus);

                if (!string.IsNullOrWhiteSpace(pageOrder))
                    query = query.OrderBy(pageOrder);

                query = query.Skip(pageIndex * pageSize).Take(pageSize);

                return query.ToList();
            }
        }

        public static int Count(int? productID, string email, CommentStatus? commentStatus)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var query = from item in db.ProductComments
                            select item;

                if (productID.HasValue)
                    query = query.Where(item => item.ProductID == productID);

                if (!String.IsNullOrWhiteSpace(email))
                    query = query.Where(item => item.Email.Contains(email));

                if (commentStatus.HasValue)
                    query = query.Where(item => item.CommentStatus == commentStatus);

                return query.Count();
            }
        }

        public static int GetCommentCount(int productID)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var query = from item in db.ProductComments
                            where item.ProductID == productID
                                  && item.CommentStatus == CommentStatus.Approved
                            select item;

                return query.Count();
            }
        }

        public static List<Public.ViewProductComment> ShowCommentsList(int productID)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var query = from item in db.ProductComments
                            where item.ProductID == productID
                                  && item.CommentStatus == CommentStatus.Approved
                            select new Public.ViewProductComment
                            {
                                UserID = item.UserID,
                                UserName = item.UserName,
                                Text = item.Text,
                                LastUpdate = item.LastUpdate
                            };

                return query.ToList();
            }
        }

        public static ProductComment GetByID(int id)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var comment = db.ProductComments.Where(item => item.ID == id).Single();

                return comment;
            }
        }

        public static void Delete(int id)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var comment = (from item in db.ProductComments
                               where item.ID == id
                               select item).Single();

                db.ProductComments.Remove(comment);

                db.SaveChanges();
            }
        }

        public static void Update(ProductComment comment)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var orgComment = db.ProductComments.Where(item => item.ID == comment.ID).Single();

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
                var list = db.ProductComments.Where(item => ids.Contains(item.ID) && item.CommentStatus != CommentStatus.Approved);

                foreach (var item in list)
                {
                    item.CommentStatus = CommentStatus.Approved;
                }

                db.SaveChanges();
            }
        }

    }

}
