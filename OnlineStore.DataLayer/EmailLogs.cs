using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OnlineStore.Models.Enums;
using OnlineStore.EntityFramework;

namespace OnlineStore.DataLayer
{
    public class EmailLog : EntityBase
    {
        [MaxLength(128)]
        [Display(Name = "کاربر")]
        public string UserID { get; set; }

        [MaxLength(300)]
        [Display(Name = "فرستنده")]
        public string From { get; set; }

        [MaxLength(300)]
        [Display(Name = "گیرنده")]
        public string To { get; set; }

        [Display(Name = "پیام")]
        public string Message { get; set; }

        [MaxLength(15)]
        [Display(Name = "IP")]
        public string IP { get; set; }

        [MaxLength(128)]
        public string Key { get; set; }

        public EmailStatus Status { get; set; }

    }

    public static class EmailLogs
    {
        public static void Insert(EmailLog emailLog)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                db.EmailLogs.Add(emailLog);

                db.SaveChanges();
            }
        }

        public static List<EmailLog> Get(int pageIndex, int pageSize, string pageOrder, string to)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var query = from item in db.EmailLogs
                            select item;

                if (!string.IsNullOrWhiteSpace(to))
                    query = query.Where(item => item.To.Contains(to));

                if (!string.IsNullOrWhiteSpace(pageOrder))
                    query = query.OrderBy(pageOrder);

                query = query.Skip(pageIndex * pageSize).Take(pageSize);

                return query.ToList();
            }
        }

        public static int Count(string to)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var query = from item in db.EmailLogs
                            select item;

                if (!string.IsNullOrWhiteSpace(to))
                    query = query.Where(item => item.To.Contains(to));

                return query.Count();
            }
        }
    }

}
