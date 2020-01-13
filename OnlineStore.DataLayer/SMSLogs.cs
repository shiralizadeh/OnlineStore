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
    public class SMSLog : EntityBase
    {
        [MaxLength(128)]
        [Display(Name = "کاربر")]
        public string UserID { get; set; }

        [MaxLength(20)]
        [Display(Name = "فرستنده")]
        public string From { get; set; }

        [MaxLength(20)]
        [Display(Name = "گیرنده")]
        public string To { get; set; }

        [MaxLength(500)]
        [Display(Name = "پیام")]
        public string Message { get; set; }

        [MaxLength(15)]
        [Display(Name = "IP")]
        public string IP { get; set; }

        [Display(Name = "نتیجه")]
        public long ResultCode { get; set; }
    }

    public static class SMSLogs
    {
        public static void Insert(SMSLog smsLog)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                db.SMSLogs.Add(smsLog);

                db.SaveChanges();
            }
        }

        public static List<SMSLog> Get(int pageIndex, int pageSize, string pageOrder, string to)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var query = from item in db.SMSLogs
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
                var query = from item in db.SMSLogs
                            select item;

                if (!string.IsNullOrWhiteSpace(to))
                    query = query.Where(item => item.To.Contains(to));

                return query.Count();
            }
        }
    }
}
