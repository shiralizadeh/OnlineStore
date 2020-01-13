using OnlineStore.EntityFramework;
using OnlineStore.Providers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Public = OnlineStore.Models.Public;

namespace OnlineStore.DataLayer
{
    public class CustomerComment : EntityBase
    {
        [Display(Name = "نام کاربر")]
        [MaxLength(100)]
        public string UserName { get; set; }

        [Display(Name = "متن")]
        [MaxLength(1000)]
        public string Text { get; set; }

        [Display(Name = "عنوان")]
        [MaxLength(100)]
        public string Title { get; set; }

        [Display(Name = "تصویر کاربر")]
        [MaxLength(100)]
        public string Image { get; set; }

        [Display(Name = "تاریخ")]
        public DateTime DateTime { get; set; }

        [Display(Name = "تاریخ")]
        [NotMapped]
        public string PersianDateTime
        {
            get
            {
                if (DateTime == new DateTime())
                    return "";

                return Utilities.ToPersianDate(DateTime);
            }
            set
            {
                if (String.IsNullOrWhiteSpace(value))
                    DateTime = DateTime.Now;

                DateTime = Utilities.ToEnglishDate(value);
            }
        }

        [Display(Name = "قابل نمایش")]
        public bool IsVisible { get; set; }
    }

    public static class CustomerComments
    {
        private static IQueryable<CustomerComment> _cachedCustomerComments
        {
            get
            {
                using (var db = OnlineStoreDbContext.Entity)
                    return db.CustomerComments.ToCacheableList().AsQueryable();
            }
        }

        public static IList Get(int pageIndex, int pageSize, string pageOrder, string userName, bool? isVisible)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var query = from item in _cachedCustomerComments
                            select new
                            {
                                item.ID,
                                item.UserName,
                                Image = StaticPaths.CustomerImages + item.Image,
                                IsVisible = item.IsVisible,
                                Title = item.Title,
                                Text = item.Text,
                                LastUpdate = item.LastUpdate,
                            };

                if (!String.IsNullOrWhiteSpace(userName))
                    query = query.Where(item => item.UserName.Contains(userName));

                if (isVisible.HasValue)
                    query = query.Where(item => item.IsVisible == isVisible);

                if (!String.IsNullOrWhiteSpace(pageOrder))
                    query = query.OrderBy(pageOrder);

                query = query.Skip(pageIndex * pageSize).Take(pageSize);

                return query.ToList();
            }
        }

        public static int Count(string userName, bool? isVisible)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var query = from item in _cachedCustomerComments
                            select item;

                if (!String.IsNullOrWhiteSpace(userName))
                    query = query.Where(item => item.UserName.Contains(userName));

                if (isVisible.HasValue)
                    query = query.Where(item => item.IsVisible == isVisible);

                return query.Count();
            }
        }

        public static CustomerComment GetByID(int id)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var customerComment = _cachedCustomerComments.Where(item => item.ID == id).Single();

                return customerComment;
            }
        }

        public static List<Public.ViewCustomerComment> ShowCustomerCommentsList()
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var query = from item in _cachedCustomerComments
                            where item.IsVisible
                            select new Public.ViewCustomerComment
                            {
                                UserName = item.UserName,
                                Image = item.Image,
                                Title = item.Title,
                                Text = item.Text,
                                PersianDateTime = item.PersianDateTime,
                                DateTime = item.DateTime
                            };

                query = query.OrderByDescending(item => item.DateTime);

                return query.ToList();
            }
        }

        public static void Delete(int id)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var customerComment = (from item in db.CustomerComments
                                       where item.ID == id
                                       select item).Single();

                db.CustomerComments.Remove(customerComment);

                db.SaveChanges();
            }
        }

        public static void Insert(CustomerComment customerComment)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                db.CustomerComments.Add(customerComment);

                db.SaveChanges();
            }
        }

        public static void Update(CustomerComment customerComment)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var orgCustomerComment = db.CustomerComments.Where(item => item.ID == customerComment.ID).Single();

                orgCustomerComment.UserName = customerComment.UserName;
                orgCustomerComment.Text = customerComment.Text;
                orgCustomerComment.Title = customerComment.Title;
                orgCustomerComment.Image = customerComment.Image;
                orgCustomerComment.IsVisible = customerComment.IsVisible;
                orgCustomerComment.DateTime = customerComment.DateTime;
                orgCustomerComment.LastUpdate = customerComment.LastUpdate;

                db.SaveChanges();
            }
        }

    }
}
