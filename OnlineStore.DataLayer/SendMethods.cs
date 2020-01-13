using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OnlineStore.EntityFramework;
using OnlineStore.Providers;

namespace OnlineStore.DataLayer
{
    public class SendMethod : EntityBase
    {
        [Display(Name = "عنوان")]
        [MaxLength(50)]
        public string Title { get; set; }

        [Display(Name = "عکس")]
        [MaxLength(50)]
        public string Filename { get; set; }

        [Display(Name = "توضیحات")]
        [MaxLength(500)]
        public string Description { get; set; }

        [Display(Name = "هزینه")]
        public int Price { get; set; }

        [Display(Name = "فعال")]
        public bool IsActive { get; set; }
    }

    public static class SendMethods
    {
        private static IQueryable<SendMethod> _cachedSendMethods
        {
            get
            {
                using (var db = OnlineStoreDbContext.Entity)
                    return db.SendMethods.ToCacheableList().AsQueryable();
            }
        }

        public static IList Get(int pageIndex, int pageSize, string pageOrder)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var query = from item in _cachedSendMethods
                            select new
                            {
                                item.ID,
                                item.Title,
                                Filename = StaticPaths.SendMethods + item.Filename,
                                Price = item.Price,
                                IsActive = item.IsActive,
                                LastUpdate = item.LastUpdate,
                            };

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
                var query = from item in _cachedSendMethods
                            select item;

                return query.Count();
            }
        }

        public static SendMethod GetByID(int id)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var sendMethod = _cachedSendMethods.Where(item => item.ID == id).Single();

                return sendMethod;
            }
        }

        public static List<SendMethod> GetActiveMethods()
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var query = from item in _cachedSendMethods
                            where item.IsActive
                            select new SendMethod
                            {
                                ID = item.ID,
                                Title = item.Title,
                            };

                return query.ToList();
            }
        }

        public static void Delete(int id)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var sendMethod = (from item in db.SendMethods
                                  where item.ID == id
                                  select item).Single();

                db.SendMethods.Remove(sendMethod);

                db.SaveChanges();
            }
        }

        public static void Insert(SendMethod sendMethod)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                db.SendMethods.Add(sendMethod);

                db.SaveChanges();
            }
        }

        public static void Update(SendMethod sendMethod)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var orgSendMethod = db.SendMethods.Where(item => item.ID == sendMethod.ID).Single();

                orgSendMethod.Title = sendMethod.Title;
                orgSendMethod.Filename = sendMethod.Filename;
                orgSendMethod.Description = sendMethod.Description;
                orgSendMethod.Price = sendMethod.Price;
                orgSendMethod.IsActive = sendMethod.IsActive;
                orgSendMethod.LastUpdate = sendMethod.LastUpdate;

                db.SaveChanges();
            }
        }
    }
}
