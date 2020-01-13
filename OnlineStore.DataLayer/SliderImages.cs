using OnlineStore.Models.Enums;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OnlineStore.EntityFramework;
using OnlineStore.Providers;

namespace OnlineStore.DataLayer
{
    public class SliderImage : EntityBase
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

        [Display(Name = "محل قرار گیری")]
        public SliderType SliderType { get; set; }

        [Display(Name = "ترتیب نمایش")]
        public int OrderID { get; set; }
    }

    public static class SliderImages
    {
        public static IList Get(int pageIndex, int pageSize, string pageOrder, string title, SliderType? sliderType)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var now = DateTime.Now;

                var query = from item in db.SliderImages
                            select new
                            {
                                item.ID,
                                item.Title,
                                item.SubTitle,
                                Filename = StaticPaths.SliderImages + item.Filename,
                                StartDate = item.StartDate,
                                EndDate = item.EndDate,
                                SliderType = item.SliderType,
                                IsActive = item.IsActive,
                                IsOnline = (item.IsActive && now >= item.StartDate && now <= item.EndDate),
                                OrderID = item.OrderID,
                                LastUpdate = item.LastUpdate,
                            };

                if (!String.IsNullOrWhiteSpace(title))
                    query = query.Where(item => item.Title.Contains(title));

                if (sliderType.HasValue)
                    query = query.Where(item => item.SliderType == sliderType);

                //if (!String.IsNullOrWhiteSpace(pageOrder))
                //    query = query.OrderBy(pageOrder);
                query = query.OrderBy(a => a.SliderType).OrderByDescending(a => a.IsOnline);

                query = query.Skip(pageIndex * pageSize).Take(pageSize);

                return query.ToList();
            }
        }

        public static int Count(string title, SliderType? sliderType)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var query = from item in db.SliderImages
                            select item;

                if (!String.IsNullOrWhiteSpace(title))
                    query = query.Where(item => item.Title.Contains(title));

                if (sliderType.HasValue)
                    query = query.Where(item => item.SliderType == sliderType);

                return query.Count();
            }
        }

        public static SliderImage GetByID(int id)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var sliderImage = db.SliderImages.Where(item => item.ID == id).Single();

                return sliderImage;
            }
        }

        public static List<SliderImage> GetSlides(SliderType sliderType)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                DateTime now = DateTime.Now;

                var query = from item in db.SliderImages
                            where item.SliderType == sliderType
                            && item.IsActive
                            && now >= item.StartDate
                            && now <= item.EndDate
                            select item;

                query = query.OrderBy(item => item.OrderID);

                return query.Take(5).ToList();
            }
        }

        public static void Delete(int id)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var sliderImage = (from item in db.SliderImages
                                   where item.ID == id
                                   select item).Single();

                db.SliderImages.Remove(sliderImage);

                db.SaveChanges();
            }
        }

        public static void Insert(SliderImage sliderImage)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                db.SliderImages.Add(sliderImage);

                db.SaveChanges();
            }
        }

        public static void Update(SliderImage sliderImage)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var orgSliderImage = db.SliderImages.Where(item => item.ID == sliderImage.ID).Single();

                orgSliderImage.Title = sliderImage.Title;
                orgSliderImage.SubTitle = sliderImage.SubTitle;
                orgSliderImage.Filename = sliderImage.Filename;
                orgSliderImage.Link = sliderImage.Link;
                orgSliderImage.StartDate = sliderImage.StartDate;
                orgSliderImage.EndDate = sliderImage.EndDate;
                orgSliderImage.SliderType = sliderImage.SliderType;
                orgSliderImage.IsActive = sliderImage.IsActive;
                orgSliderImage.OrderID = sliderImage.OrderID;
                orgSliderImage.LastUpdate = sliderImage.LastUpdate;

                db.SaveChanges();
            }
        }
    }
}
