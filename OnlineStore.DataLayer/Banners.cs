using OnlineStore.Providers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using OnlineStore.EntityFramework;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineStore.DataLayer
{
    public class Banner : EntityBase
    {
        public Guid Key { get; set; }

        [Display(Name = "عنوان")]
        [MaxLength(100)]
        public string Title { get; set; }

        [Display(Name = "تصویر")]
        [MaxLength(200)]
        public string Filename { get; set; }

        [Display(Name = "تاریخ شروع")]
        public DateTime StartDate { get; set; }

        [Display(Name = "تاریخ شروع")]
        [NotMapped]
        public string PersianStartDate
        {
            get
            {
                if (StartDate == new DateTime())
                    return "";

                return Utilities.ToPersianDate(StartDate);
            }
            set
            {
                if (String.IsNullOrWhiteSpace(value))
                    StartDate = DateTime.Now;

                StartDate = Utilities.ToEnglishDate(value);
            }
        }

        [Display(Name = "تاریخ پایان")]
        public DateTime EndDate { get; set; }

        [Display(Name = "تاریخ پایان")]
        [NotMapped]
        public string PersianEndDate
        {
            get
            {
                if (EndDate == new DateTime())
                    return "";

                return Utilities.ToPersianDate(EndDate);
            }
            set
            {
                if (String.IsNullOrWhiteSpace(value))
                    EndDate = DateTime.Now;

                EndDate = Utilities.ToEnglishDate(value);
            }
        }

        [Display(Name = "فعال")]
        public bool IsActive { get; set; }

        [Display(Name = "محل قرار گیری")]
        public string BannerType { get; set; }

        [Display(Name = "لینک")]
        public string Link { get; set; }

        [Display(Name = "تعداد کلیک")]
        public int ClickCount { get; set; }

        [Display(Name = "ترتیب نمایش")]
        public int OrderID { get; set; }

    }

    public static class Banners
    {
        public static IList Get(int pageIndex, int pageSize, string pageOrder)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var query = from item in db.Banners
                            select new
                            {
                                item.ID,
                                item.Title,
                                Filename = StaticPaths.BannerImages + item.Filename,
                                StartDate = item.StartDate,
                                EndDate = item.EndDate,
                                BannerType = item.BannerType,
                                ClickCount = item.ClickCount,
                                Link = item.Link,
                                IsActive = item.IsActive,
                                OrderID = item.OrderID,
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
                var query = from item in db.Banners
                            select item;

                return query.Count();
            }
        }

        public static Banner GetByID(int id)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var banner = db.Banners.Where(item => item.ID == id).Single();

                return banner;
            }
        }

        public static Banner GetByGuid(Guid guid)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var banner = db.Banners.Where(item => item.Key == guid).Single();

                return banner;
            }
        }

        public static List<Banner> ShowPageBanners(string page)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                DateTime now = DateTime.Now;

                var query = from item in db.Banners
                            where item.BannerType.Contains(page)
                            && item.IsActive
                            && now >= item.StartDate
                            && now <= item.EndDate
                            select item;

                query = query.OrderBy(item => item.OrderID);

                return query.ToList();
            }
        }

        public static void Delete(int id)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var banner = (from item in db.Banners
                              where item.ID == id
                              select item).Single();

                db.Banners.Remove(banner);

                db.SaveChanges();
            }
        }

        public static void Insert(Banner banner)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                db.Banners.Add(banner);

                db.SaveChanges();
            }
        }

        public static void Update(Banner banner)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var orgBanner = db.Banners.Where(item => item.ID == banner.ID).Single();

                //orgBanner.Key = banner.Key;
                orgBanner.Title = banner.Title;
                orgBanner.Filename = banner.Filename;
                orgBanner.StartDate = banner.StartDate;
                orgBanner.EndDate = banner.EndDate;
                orgBanner.Link = banner.Link;
                orgBanner.IsActive = banner.IsActive;
                orgBanner.OrderID = banner.OrderID;
                orgBanner.LastUpdate = banner.LastUpdate;

                db.SaveChanges();
            }
        }

        public static bool AddClick(Guid key)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var orgBanner = db.Banners.Where(item => item.Key == key).SingleOrDefault();

                if (orgBanner != null)
                {
                    orgBanner.ClickCount++;

                    db.SaveChanges();

                    return true;
                }

                return false;
            }
        }
    }
}
