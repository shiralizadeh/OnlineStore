using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OnlineStore.EntityFramework;

namespace OnlineStore.DataLayer
{
    public class Email : EntityBase
    {
        [MaxLength(100)]
        [Display(Name = "عنوان")]
        public string Title { get; set; }

        [MaxLength(300)]
        [Display(Name = "ایمیل")]
        public string EmailAddress { get; set; }

        [MaxLength(20)]
        [Display(Name = "کلمه عبور")]
        public string Password { get; set; }
    }

    public static class Emails
    {
        private static IQueryable<Email> _cachedEmails
        {
            get
            {
                using (var db = OnlineStoreDbContext.Entity)
                    return db.Emails.ToCacheableList().AsQueryable();
            }
        }

        public static IList Get(int pageIndex, int pageSize, string pageOrder)
        {
            var query = from item in _cachedEmails
                        select item;

            if (!String.IsNullOrWhiteSpace(pageOrder))
                query = query.OrderBy(pageOrder);

            query = query.Skip(pageIndex * pageSize).Take(pageSize);

            return query.ToList();
        }

        public static List<Email> GetList()
        {
            var query = from item in _cachedEmails
                        select item;

            return query.ToList();
        }

        public static Email GetByID(int id)
        {
            var email = _cachedEmails.Where(item => item.ID == id).Single();

            return email;
        }

        public static void Insert(Email email)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                db.Emails.Add(email);

                db.SaveChanges();
            }
        }

        public static void Update(Email email)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var orgEmail = db.Emails.Where(item => item.ID == email.ID).Single();

                orgEmail.EmailAddress = email.EmailAddress;
                orgEmail.Title = email.Title;
                orgEmail.Password = email.Password;
                orgEmail.LastUpdate = email.LastUpdate;

                db.SaveChanges();
            }
        }

        public static void Delete(int id)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var email = (from item in db.Emails
                             where item.ID == id
                             select item).Single();

                db.Emails.Remove(email);

                db.SaveChanges();
            }
        }

        public static int Count()
        {
            var query = from item in _cachedEmails
                        select item;

            return query.Count();
        }

    }
}
