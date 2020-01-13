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
    public class Log : EntityBase
    {
        [MaxLength(15)]
        [Display(Name = "IP")]
        public string IP { get; set; }

        [Display(Name = "عنوان")]
        public string Title { get; set; }

        [Display(Name = "پیام")]
        public string Message { get; set; }

        public LogType Type { get; set; }
    }

    public static class Logs
    {
        public static void Insert(Log log)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                db.Logs.Add(log);

                db.SaveChanges();
            }
        }

        public static void Alert(string ip, string title = "", string message = "", LogType type = LogType.Info)
        {
            Logs.Insert(new Log()
            {
                IP = ip,
                Title = title,
                Message = message,
                LastUpdate = DateTime.Now,
                Type = type
            });
        }

        public static List<Log> Get(int pageIndex, int pageSize, string pageOrder)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var query = from item in db.Logs
                            select item;

                if (!string.IsNullOrWhiteSpace(pageOrder))
                    query = query.OrderBy(pageOrder);

                query = query.Skip(pageIndex * pageSize).Take(pageSize);

                return query.ToList();
            }
        }

        public static int Count()
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var query = from item in db.Logs
                            select item;

                return query.Count();
            }
        }
    }
}
