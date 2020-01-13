using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OnlineStore.EntityFramework;
using OnlineStore.Models.Admin;
using OnlineStore.Models.Public;
using OnlineStore.Models.Enums;
using OnlineStore.Providers;

namespace OnlineStore.DataLayer
{
    public class ProductGift : EntityBase
    {
        [Display(Name = "محصول")]
        [ForeignKey("Product")]
        public int? ProductID { get; set; }
        public Product Product { get; set; }

        [Display(Name = "محصول هدیه")]
        [ForeignKey("Gift")]
        public int GiftID { get; set; }
        public Product Gift { get; set; }

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

    }

    public static class ProductGifts
    {
        public static void Insert(List<ProductGift> gifts)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                db.ProductGifts.AddRange(gifts);

                db.SaveChanges();
            }
        }

        public static List<JsonProductGift> Get(int productID)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var query = from item in db.ProductGifts
                            where item.ProductID == productID
                            select new JsonProductGift
                            {
                                ID = item.ID,
                                Title = item.Gift.Title,
                                GiftID = item.GiftID,
                                StartDate = item.StartDate,
                                EndDate = item.EndDate
                            };

                return query.ToList();
            }
        }

        public static List<ProductItem> GetByProductID(int productID)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                DateTime now = DateTime.Now;

                var gifts = (from item in db.Products
                             where item.ProductStatus == ProductStatus.Approved
                             && now >= item.PublishDate
                             && item.IsInVisible == false
                             && db.ProductGifts.Any(gft => gft.GiftID == item.ID &&
                                                           gft.ProductID == productID &&
                                                           gft.StartDate <= now &&
                                                           gft.EndDate >= now)
                             select new ProductItem
                             {
                                 ID = item.ID,
                                 Title_Fa = item.Title,
                                 Title_En = item.Title_En,
                                 DisplayTitleType = item.DisplayTitleType,
                                 GroupID = item.GroupID,
                             });

                return gifts.ToList();
            }
        }

        public static void DeleteGifts(int productID)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var gifts = (from item in db.ProductGifts
                             where item.ProductID == productID
                             select item);

                db.ProductGifts.RemoveRange(gifts);

                db.SaveChanges();
            }
        }

        public static int CountByProductID(int productID)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var query = from item in db.ProductGifts
                            where item.ProductID == productID || item.GiftID == productID
                            select item;

                return query.Count();
            }
        }

    }
}
