using OnlineStore.Providers;
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
    public class PaymentMethod : EntityBase
    {
        [Display(Name = "عنوان")]
        [MaxLength(50)]
        public string Title { get; set; }

        [Display(Name = "توضیحات")]
        [MaxLength(500)]
        public string Description { get; set; }

        [Display(Name = "عکس")]
        [MaxLength(50)]
        public string Filename { get; set; }

        [Display(Name = "فعال")]
        public bool IsActive { get; set; }
    }

    public static class PaymentMethods
    {
        private static IQueryable<PaymentMethod> _cachedPaymentMethods
        {
            get
            {
                using (var db = OnlineStoreDbContext.Entity)
                    return db.PaymentMethods.ToCacheableList().AsQueryable();
            }
        }

        public static IList Get(int pageIndex, int pageSize, string pageOrder)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var query = from item in _cachedPaymentMethods
                            select new
                            {
                                item.ID,
                                item.Title,
                                Filename = StaticPaths.PaymentMethods + item.Filename,
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
                var query = from item in _cachedPaymentMethods
                            select item;

                return query.Count();
            }
        }

        public static PaymentMethod GetByID(int id)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var paymentMethod = _cachedPaymentMethods.Where(item => item.ID == id).Single();

                return paymentMethod;
            }
        }

        public static List<PaymentMethod> GetActiveMethods()
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var query = from item in _cachedPaymentMethods
                            where item.IsActive
                            select new PaymentMethod
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
                var paymentMethod = (from item in db.PaymentMethods
                                     where item.ID == id
                                     select item).Single();

                db.PaymentMethods.Remove(paymentMethod);

                db.SaveChanges();
            }
        }

        public static void Insert(PaymentMethod paymentMethod)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                db.PaymentMethods.Add(paymentMethod);

                db.SaveChanges();
            }
        }

        public static void Update(PaymentMethod paymentMethod)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var orgPaymentMethod = db.PaymentMethods.Where(item => item.ID == paymentMethod.ID).Single();

                orgPaymentMethod.Title = paymentMethod.Title;
                orgPaymentMethod.Description = paymentMethod.Description;
                orgPaymentMethod.Filename = paymentMethod.Filename;
                orgPaymentMethod.IsActive = paymentMethod.IsActive;
                orgPaymentMethod.LastUpdate = paymentMethod.LastUpdate;

                db.SaveChanges();
            }
        }
    }
}
