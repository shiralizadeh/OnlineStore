using OnlineStore.Providers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OnlineStore.EntityFramework;

namespace OnlineStore.DataLayer
{
    public class HomeBoxItem : EntityBase
    {
        [Display(Name = "عنوان")]
        [MaxLength(50)]
        public string Title { get; set; }

        [Display(Name = "زیر عنوان")]
        [MaxLength(50)]
        public string SubTitle { get; set; }

        [Display(Name = "عکس")]
        [MaxLength(200)]
        public string Filename { get; set; }

        [Display(Name = "لینک")]
        [MaxLength(200)]
        public string Link { get; set; }

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

        [ForeignKey("HomeBox")]
        public int HomeBoxID { get; set; }
        public HomeBox HomeBox { get; set; }

        [Display(Name = "ترتیب نمایش")]
        public int OrderID { get; set; }

    }

    public static class HomeBoxItems
    {
        public static IList Get(int pageIndex, int pageSize, string pageOrder, string title, int homeBoxID)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var query = from item in db.HomeBoxItems
                            where item.HomeBoxID == homeBoxID
                            select new
                            {
                                item.ID,
                                item.Title,
                                Filename = StaticPaths.SliderImages + item.Filename,
                                StartDate = item.StartDate,
                                EndDate = item.EndDate,
                                IsActive = item.IsActive,
                                OrderID = item.OrderID,
                                LastUpdate = item.LastUpdate,
                            };

                if (!String.IsNullOrWhiteSpace(title))
                    query = query.Where(item => item.Title.Contains(title));

                if (!String.IsNullOrWhiteSpace(pageOrder))
                    query = query.OrderBy(pageOrder);

                query = query.Skip(pageIndex * pageSize).Take(pageSize);

                return query.ToList();
            }
        }

        public static int Count(string title, int homeBoxID)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var query = from item in db.HomeBoxItems
                            where item.HomeBoxID == homeBoxID
                            select item;

                if (!String.IsNullOrWhiteSpace(title))
                    query = query.Where(item => item.Title.Contains(title));

                return query.Count();
            }
        }

        public static void Delete(int id)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var homeBoxItem = (from item in db.HomeBoxItems
                                   where item.ID == id
                                   select item).Single();

                db.HomeBoxItems.Remove(homeBoxItem);

                db.SaveChanges();
            }
        }

        public static void Insert(HomeBoxItem homeBoxItem)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                db.HomeBoxItems.Add(homeBoxItem);

                db.SaveChanges();
            }
        }

        public static HomeBoxItem GetByID(int id)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var item = db.HomeBoxItems.Where(i => i.ID == id);

                return item.Single();
            }
        }

        public static void Update(HomeBoxItem homeBoxItem)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var orghomeBoxItem = db.HomeBoxItems.Where(item => item.ID == homeBoxItem.ID).Single();

                orghomeBoxItem.Title = homeBoxItem.Title;
                orghomeBoxItem.SubTitle = homeBoxItem.SubTitle;
                orghomeBoxItem.Filename = homeBoxItem.Filename;
                orghomeBoxItem.StartDate = homeBoxItem.StartDate;
                orghomeBoxItem.EndDate = homeBoxItem.EndDate;
                orghomeBoxItem.Link = homeBoxItem.Link;
                orghomeBoxItem.IsActive = homeBoxItem.IsActive;
                orghomeBoxItem.OrderID = homeBoxItem.OrderID;
                orghomeBoxItem.LastUpdate = homeBoxItem.LastUpdate;
            }
        }
    }

}
