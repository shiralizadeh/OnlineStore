using OnlineStore.Models.Admin;
using OnlineStore.Models.Public;
using OnlineStore.Providers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineStore.DataLayer
{
    public class ProductMark : EntityBase
    {
        [ForeignKey("Product")]
        public int ProductID { get; set; }
        public Product Product { get; set; }

        [Display(Name = "عنوان")]
        [MaxLength(50)]
        public string Title { get; set; }

        [Display(Name = "رنگ")]
        [MaxLength(10)]
        public string Color { get; set; }

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

    public static class ProductMarks
    {
        public static List<EditProductMark> GetByProductID(int productID)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var query = from item in db.ProductMarks
                            where item.ProductID == productID
                            select new EditProductMark
                            {
                                ID = item.ID,
                                Title = item.Title,
                                Color = item.Color,
                                StartDate = item.StartDate,
                                EndDate = item.EndDate,
                            };

                return query.ToList();
            }
        }

        public static List<MarkItem> GetMarksByProductID(int productID)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var now = DateTime.Now.Date;

                var query = from item in db.ProductMarks
                            where item.ProductID == productID
                            && item.StartDate <= now
                            && item.EndDate >= now
                            select new MarkItem
                            {
                                Title = item.Title,
                                Color = item.Color,
                            };

                return query.ToList();
            }
        }

        public static int Count()
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var query = from item in db.ProductMarks
                            select item;

                return query.Count();
            }
        }

        public static ProductMark GetByID(int id)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var productMark = db.ProductMarks.Where(item => item.ID == id).Single();

                return productMark;
            }
        }

        public static void Delete(int id)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var productMark = (from item in db.ProductMarks
                                   where item.ID == id
                                   select item).Single();

                db.ProductMarks.Remove(productMark);

                db.SaveChanges();
            }
        }

        public static void Insert(ProductMark productMark)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                db.ProductMarks.Add(productMark);

                db.SaveChanges();
            }
        }

        public static void Update(EditProductMark productMark)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var orgProductMark = db.ProductMarks.Where(item => item.ID == productMark.ID).Single();

                //orgProductMark.ProductID = productMark.ProductID;
                orgProductMark.Title = productMark.Title;
                orgProductMark.Color = productMark.Color;
                orgProductMark.StartDate = productMark.StartDate;
                orgProductMark.EndDate = productMark.EndDate;
                //orgProductMark.LastUpdate = productMark.LastUpdate;

                db.SaveChanges();
            }
        }
    }
}
