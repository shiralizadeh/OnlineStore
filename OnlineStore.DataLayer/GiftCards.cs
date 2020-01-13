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
using OnlineStore.Models.Enums;

namespace OnlineStore.DataLayer
{
    public class GiftCard : EntityBase
    {
        [Display(Name = "کد تخفیف")]
        [MaxLength(50)]
        public string Serial { get; set; }

        [Display(Name = "درصد تخفیف")]
        public int? Percent { get; set; }

        [Display(Name = "قیمت تخفیف")]
        public int? Price { get; set; }

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

        [Display(Name = "آیا استفاده شده")]
        public bool IsUsed { get; set; }

        [Display(Name = "کاربر استفاده کننده")]
        [MaxLength(128)]
        public string UsedUserID { get; set; }

        [Display(Name = "تاریخ استفاده")]
        public DateTime? UsedDateTime { get; set; }

        [Display(Name = "استفاده نامحدود")]
        public bool IsUnlimit { get; set; }

        [Display(Name = "نوع تخفیف")]
        public GiftCardType GiftCardType { get; set; }

        [Display(Name = "تعداد کالاهای سبد")]
        public int? Count { get; set; }

        [Display(Name = "حداقل قیمت خرید")]
        public int? MinimumPrice { get; set; }

        [Display(Name = "شامل تمامی محصولات")]
        public bool AllProducts { get; set; }

        [Display(Name = "گروه تخفیف")]
        public int? GroupID { get; set; }

    }

    public static class GiftCards
    {
        private static IQueryable<GiftCard> _cachedGiftCards
        {
            get
            {
                using (var db = OnlineStoreDbContext.Entity)
                    return db.GiftCards.ToCacheableList().AsQueryable();
            }
        }

        public static IList Get(int pageIndex, int pageSize, string pageOrder, string serial, DateTime? fromDate, DateTime? toDate, bool? isUsed)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var query = from item in _cachedGiftCards
                            select new
                            {
                                item.ID,
                                item.Serial,
                                item.StartDate,
                                item.EndDate,
                                item.IsUnlimit,
                                item.IsUsed,
                                item.LastUpdate,
                            };

                if (!String.IsNullOrWhiteSpace(serial))
                    query = query.Where(item => item.Serial.Contains(serial));

                if (fromDate.HasValue)
                    query = query.Where(item => item.StartDate >= fromDate);

                if (toDate.HasValue)
                    query = query.Where(item => item.EndDate <= toDate);

                if (isUsed.HasValue)
                    query = query.Where(item => item.IsUsed == isUsed);

                if (!string.IsNullOrWhiteSpace(pageOrder))
                    query = query.OrderBy(pageOrder);

                query = query.Skip(pageIndex * pageSize).Take(pageSize);

                return query.ToList();
            }
        }

        public static int Count(string serial, DateTime? fromDate, DateTime? toDate, bool? isUsed)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var query = from item in _cachedGiftCards
                            select item;

                if (!String.IsNullOrWhiteSpace(serial))
                    query = query.Where(item => item.Serial.Contains(serial));

                if (fromDate.HasValue)
                    query = query.Where(item => item.StartDate >= fromDate);

                if (toDate.HasValue)
                    query = query.Where(item => item.EndDate <= toDate);

                if (isUsed.HasValue)
                    query = query.Where(item => item.IsUsed == isUsed);

                return query.Count();
            }
        }

        public static GiftCard GetByID(int id)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var giftCard = _cachedGiftCards.Where(item => item.ID == id).Single();

                return giftCard;
            }
        }

        public static void Delete(int id)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var giftCard = (from item in db.GiftCards
                                where item.ID == id
                                select item).Single();

                db.GiftCards.Remove(giftCard);

                db.SaveChanges();
            }
        }

        public static void Insert(GiftCard giftCard)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                db.GiftCards.Add(giftCard);

                db.SaveChanges();
            }
        }

        public static void Update(GiftCard giftCard)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var orgGiftCard = db.GiftCards.Where(item => item.ID == giftCard.ID).Single();

                orgGiftCard.Serial = giftCard.Serial;
                orgGiftCard.Percent = giftCard.Percent;
                orgGiftCard.Price = giftCard.Price;
                orgGiftCard.Count = giftCard.Count;
                orgGiftCard.MinimumPrice = giftCard.MinimumPrice;
                orgGiftCard.AllProducts = giftCard.AllProducts;
                orgGiftCard.GroupID = giftCard.GroupID;
                orgGiftCard.Percent = giftCard.Percent;
                orgGiftCard.StartDate = giftCard.StartDate;
                orgGiftCard.EndDate = giftCard.EndDate;
                orgGiftCard.IsUsed = giftCard.IsUsed;
                orgGiftCard.UsedUserID = giftCard.UsedUserID;
                orgGiftCard.UsedDateTime = giftCard.UsedDateTime;
                orgGiftCard.IsUnlimit = giftCard.IsUnlimit;
                orgGiftCard.LastUpdate = giftCard.LastUpdate;

                db.SaveChanges();
            }
        }
    }
}
