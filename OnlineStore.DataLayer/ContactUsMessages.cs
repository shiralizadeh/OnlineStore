using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OnlineStore.Models.Enums;
using System.ComponentModel.DataAnnotations;
using OnlineStore.EntityFramework;

namespace OnlineStore.DataLayer
{
    public class ContactUsMessage : EntityBase
    {
        [MaxLength(100)]
        [Display(Name = "نام و نام خانوادگی")]
        public string FullName { get; set; }

        [MaxLength(300)]
        [Display(Name = "پست الکترونیک")]
        public string Email { get; set; }

        [MaxLength(300)]
        [Display(Name = "موضوع")]
        public string Subject { get; set; }

        [MaxLength(1000)]
        [Display(Name = "پیام")]
        public string Message { get; set; }

        [Display(Name = "وضعیت")]
        public ContactUsMessageStatus ContactUsMessageStatus { get; set; }

        [MaxLength(128)]
        public string Key { get; set; }

        [Display(Name = "متن پاسخ")]
        public string Answer { get; set; }

    }

    public static class ContactUsMessages
    {
        public static void Insert(ContactUsMessage msg)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                db.ContactUsMessages.Add(msg);

                db.SaveChanges();
            }
        }

        public static List<ContactUsMessage> Get(int pageIndex,
                                                 int pageSize,
                                                 string pageOrder,
                                                 string fullName,
                                                 string email,
                                                 ContactUsMessageStatus? contactUsMessageStatus)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var query = from item in db.ContactUsMessages
                            select item;

                if (!String.IsNullOrWhiteSpace(fullName))
                    query = query.Where(item => item.FullName.Contains(fullName));

                if (!String.IsNullOrWhiteSpace(email))
                    query = query.Where(item => item.Email.Contains(email));

                if (contactUsMessageStatus.HasValue)
                    query = query.Where(item => item.ContactUsMessageStatus == contactUsMessageStatus);

                if (!String.IsNullOrWhiteSpace(pageOrder))
                    query = query.OrderBy(pageOrder);

                query = query.Skip(pageIndex * pageSize).Take(pageSize);

                return query.ToList();
            }
        }

        public static int Count(string fullName, string email, ContactUsMessageStatus? contactUsMessageStatus)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var query = from item in db.ContactUsMessages
                            select item;

                if (!String.IsNullOrWhiteSpace(fullName))
                    query = query.Where(item => item.FullName.Contains(fullName));

                if (!String.IsNullOrWhiteSpace(email))
                    query = query.Where(item => item.Email.Contains(email));

                if (contactUsMessageStatus.HasValue)
                    query = query.Where(item => item.ContactUsMessageStatus == contactUsMessageStatus);

                return query.Count();
            }
        }

        public static ContactUsMessage GetByID(int id)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var message = db.ContactUsMessages.Where(item => item.ID == id).Single();

                return message;
            }
        }

        public static void Delete(int id)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var message = (from item in db.ContactUsMessages
                               where item.ID == id
                               select item).Single();

                db.ContactUsMessages.Remove(message);

                db.SaveChanges();
            }
        }

        public static void Update(ContactUsMessage message)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var orgComment = db.ContactUsMessages.Where(item => item.ID == message.ID).Single();

                orgComment.ContactUsMessageStatus = message.ContactUsMessageStatus;
                orgComment.Answer = message.Answer;
                orgComment.LastUpdate = message.LastUpdate;

                db.SaveChanges();
            }
        }
    }
}
