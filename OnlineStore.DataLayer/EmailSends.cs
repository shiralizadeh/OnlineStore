using OnlineStore.Models.Enums;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OnlineStore.EntityFramework;
using OnlineStore.Models.Public;

namespace OnlineStore.DataLayer
{
    public class EmailSend : EntityBase
    {
        [ForeignKey("From")]
        [Display(Name = "فرستنده")]
        public int FromID { get; set; }
        [Display(Name = "فرستنده")]
        public Email From { get; set; }


        [Display(Name = "گیرنده")]
        [MaxLength(300)]
        public string To { get; set; }

        [Display(Name = "موضوع")]
        [MaxLength(200)]
        public string Subject { get; set; }


        [Display(Name = "متن پیام")]
        public string Text { get; set; }


        [Display(Name = "تاریخ ارسال")]
        public DateTime? SentDate { get; set; }


        [Display(Name = "وضعیت")]
        public EmailSendStatus EmailSendStatus { get; set; }


        [Display(Name = "اولویت")]
        public Priority Priority { get; set; }
    }

    public static class EmailSends
    {
        public static void InsertGroup(List<EmailSend> emailSends)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                db.EmailSends.AddRange(emailSends);

                db.SaveChanges();
            }
        }

        public static void Update(EmailSend emailSend)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var orgEmailSend = db.EmailSends.Where(item => item.ID == emailSend.ID).Single();

                orgEmailSend.From = emailSend.From;
                orgEmailSend.To = emailSend.To;
                orgEmailSend.Subject = emailSend.Subject;
                orgEmailSend.Text = emailSend.Text;
                orgEmailSend.EmailSendStatus = emailSend.EmailSendStatus;
                orgEmailSend.Priority = emailSend.Priority;
                orgEmailSend.LastUpdate = emailSend.LastUpdate;

                db.SaveChanges();
            }
        }

        public static void UpdateStatus(EmailSend emailSend)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var orgEmailSend = db.EmailSends.Where(item => item.ID == emailSend.ID).Single();

                orgEmailSend.EmailSendStatus = emailSend.EmailSendStatus;
                orgEmailSend.LastUpdate = emailSend.LastUpdate;

                db.SaveChanges();
            }
        }


        public static void Delete(int id)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var emailSend = (from item in db.EmailSends
                                 where item.ID == id
                                 select item).Single();

                db.EmailSends.Remove(emailSend);

                db.SaveChanges();
            }
        }

        public static IList Get(int pageIndex, int pageSize, string pageOrder)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var query = from item in db.EmailSends
                            select new
                            {
                                item.ID,
                                FromTitle = item.From.EmailAddress,
                                item.To,
                                item.Subject,
                                item.EmailSendStatus,
                                item.Priority,
                                item.LastUpdate
                            };

                if (!String.IsNullOrWhiteSpace(pageOrder))
                    query = query.OrderBy(pageOrder);

                query = query.Skip(pageIndex * pageSize).Take(pageSize);

                return query.ToList();
            }

        }

        public static EmailSend GetByID(int id)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var email = db.EmailSends.Where(item => item.ID == id).Single();

                return email;
            }
        }

        public static int Count()
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var query = from item in db.EmailSends
                            select item;

                return query.Count();
            }
        }

        public static List<ViewEmailSend> GetEmailsList(int count)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var query = from item in db.EmailSends
                            where item.EmailSendStatus == EmailSendStatus.NotChecked
                            orderby item.Priority descending, item.LastUpdate
                            select new ViewEmailSend
                            {
                                ID = item.ID,
                                To = item.To,
                                Subject = item.Subject,
                                Text = item.Text,
                                FromID = item.FromID
                            };

                var result = query.Take(count).ToList();

                foreach (var item in result)
                {
                    var from = Emails.GetByID(item.FromID);
                    item.From = new ViewEmail
                    {
                        ID = from.ID,
                        EmailAddress = from.EmailAddress,
                        Password = from.Password,
                        Title = from.Title
                    };
                }

                return result;
            }

        }

    }
}
