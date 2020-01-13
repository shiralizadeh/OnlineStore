using OnlineStore.Models.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OnlineStore.Models.Admin;
using Public = OnlineStore.Models.Public;
using OnlineStore.EntityFramework;

namespace OnlineStore.DataLayer
{
    public class ProductQuestion : EntityBase
    {
        [ForeignKey("Product")]
        [Display(Name = "محصول مربوطه")]
        public int ProductID { get; set; }
        public Product Product { get; set; }

        [Display(Name = "پرسش")]
        public string Question { get; set; }

        [Display(Name = "پاسخ")]
        public string Reply { get; set; }

        [MaxLength(128)]
        [Display(Name = "پرسشگر")]
        public string UserID { get; set; }

        [Display(Name = "پرسشگر")]
        public string UserName { get; set; }

        [Display(Name = "پرسشگر")]
        [NotMapped]
        public string UserFullName { get; set; }

        [Display(Name = "تاریخ ارسال پرسش")]
        public DateTime DateTime { get; set; }

        [Display(Name = "وضعیت")]
        public QuestionStatus QuestionStatus { get; set; }

        [Display(Name = "قابل نمایش")]
        public bool IsVisible { get; set; }
    }

    public static class ProductQuestions
    {
        public static void Insert(ProductQuestion question)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                db.ProductQuestions.Add(question);

                db.SaveChanges();
            }
        }

        public static List<ViewProductQuestion> Get(int pageIndex, int pageSize, string pageOrder, int? productID, string question, QuestionStatus? questionStatus)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var query = from item in db.ProductQuestions
                            select new ViewProductQuestion
                            {
                                ID = item.ID,
                                UserID = item.UserID,
                                UserName = item.UserName,
                                LastUpdate = item.LastUpdate,
                                DateTime = item.DateTime,
                                Question = item.Question,
                                QuestionStatus = item.QuestionStatus,
                                IsVisible = item.IsVisible,
                                ProductID = item.ProductID,
                                ProductTitle = item.Product.Title
                            };

                if (productID.HasValue)
                    query = query.Where(item => item.ProductID == productID);

                if (!String.IsNullOrWhiteSpace(question))
                    query = query.Where(item => item.Question.Contains(question));

                if (questionStatus.HasValue)
                    query = query.Where(item => item.QuestionStatus == questionStatus);

                if (!string.IsNullOrWhiteSpace(pageOrder))
                    query = query.OrderBy(pageOrder);

                query = query.Skip(pageIndex * pageSize).Take(pageSize);

                return query.ToList();
            }
        }

        public static int Count(int? productID, string question, QuestionStatus? questionStatus)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var query = from item in db.ProductQuestions
                            select item;

                if (productID.HasValue)
                    query = query.Where(item => item.ProductID == productID);

                if (!String.IsNullOrWhiteSpace(question))
                    query = query.Where(item => item.Question.Contains(question));

                if (questionStatus.HasValue)
                    query = query.Where(item => item.QuestionStatus == questionStatus);

                return query.Count();
            }
        }

        public static List<Public.ViewProductQuestion> ShowQuestionsList(int productID)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var query = from item in db.ProductQuestions
                            where item.ProductID == productID
                            && item.IsVisible
                            select new Public.ViewProductQuestion
                            {
                                UserID = item.UserID,
                                UserName = item.UserName,
                                Question = item.Question,
                                LastUpdate = item.LastUpdate,
                                Reply = item.Reply
                            };

                return query.ToList();
            }
        }

        public static ProductQuestion GetByID(int id)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var question = db.ProductQuestions.Where(item => item.ID == id).Single();

                return question;
            }
        }

        public static void Delete(int id)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var question = (from item in db.ProductQuestions
                                where item.ID == id
                                select item).Single();

                db.ProductQuestions.Remove(question);

                db.SaveChanges();
            }
        }

        public static void Update(ProductQuestion productQuestion)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var orgQuestion = db.ProductQuestions.Where(item => item.ID == productQuestion.ID).Single();

                orgQuestion.Question = productQuestion.Question;
                orgQuestion.Reply = productQuestion.Reply;
                orgQuestion.QuestionStatus = productQuestion.QuestionStatus;
                orgQuestion.LastUpdate = productQuestion.LastUpdate;
                orgQuestion.IsVisible = productQuestion.IsVisible;

                db.SaveChanges();
            }
        }

        public static void UpdateQuestions(List<int> ids)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var listQuestions = db.ProductQuestions.Where(item => ids.Contains(item.ID) && !item.IsVisible);

                foreach (var item in listQuestions)
                {
                    item.IsVisible = true;
                }

                db.SaveChanges();
            }
        }

    }
}
