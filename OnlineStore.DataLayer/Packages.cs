using OnlineStore.Models.Enums;
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
using OnlineStore.Models.Public;

namespace OnlineStore.DataLayer
{
    public class Package : EntityBase
    {
        [MaxLength(300)]
        [Display(Name = "عنوان")]
        public string Title { get; set; }

        [Display(Name = "متن")]
        public string Text { get; set; }

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

        [Display(Name = "وضعیت")]
        public PackageStatus PackageStatus { get; set; }

        [Display(Name = "ترتیب نمایش")]
        public int OrderID { get; set; }

        [Display(Name = "امتیاز بسته")]
        public float PackageScore { get; set; }

        public DateTime CreatedDate { get; set; }
    }

    public static class Packages
    {
        public static IList Get(int pageIndex, int pageSize, string pageOrder, string title, DateTime? startDate, DateTime? endDate)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var query = from item in db.Packages
                            select new
                            {
                                item.ID,
                                item.Title,
                                item.StartDate,
                                item.EndDate,
                                item.PackageStatus,
                                item.LastUpdate,
                                item.OrderID
                            };

                if (!string.IsNullOrWhiteSpace(title))
                    query = query.Where(item => item.Title.Contains(title));

                if (startDate.HasValue)
                    query = query.Where(item => item.StartDate >= startDate);

                if (endDate.HasValue)
                    query = query.Where(item => item.EndDate <= endDate);

                if (!string.IsNullOrWhiteSpace(pageOrder))
                    query = query.OrderBy(pageOrder);

                query = query.Skip(pageIndex * pageSize).Take(pageSize);

                return query.ToList();
            }
        }

        public static int Count(string title, DateTime? startDate, DateTime? endDate)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var query = from item in db.Packages
                            select item;

                if (!string.IsNullOrWhiteSpace(title))
                    query = query.Where(item => item.Title.Contains(title));

                if (startDate.HasValue)
                    query = query.Where(item => item.StartDate <= startDate);

                if (endDate.HasValue)
                    query = query.Where(item => item.EndDate >= endDate);

                return query.Count();
            }
        }

        public static Package GetByID(int id)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var package = db.Packages.Where(item => item.ID == id).Single();

                return package;
            }
        }

        public static void Insert(Package package)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                db.Packages.Add(package);

                db.SaveChanges();
            }
        }

        public static void Delete(int id)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var package = (from item in db.Packages
                               where item.ID == id
                               select item).Single();

                db.Packages.Remove(package);

                db.SaveChanges();
            }
        }

        public static void Update(Package package)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var orgPackage = db.Packages.Where(item => item.ID == package.ID).Single();

                orgPackage.Title = package.Title;
                orgPackage.Text = package.Text;
                orgPackage.StartDate = package.StartDate;
                orgPackage.EndDate = package.EndDate;
                orgPackage.PackageStatus = package.PackageStatus;
                orgPackage.LastUpdate = package.LastUpdate;

                db.SaveChanges();
            }
        }

        public static PackageDetail GetDetails(int id, bool isAdmin = false)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var now = DateTime.Now;

                var package = from item in db.Packages
                              where item.ID == id
                              && ((item.StartDate <= now
                              && item.EndDate >= now
                              && item.PackageStatus == PackageStatus.Approved) || isAdmin)
                              select new PackageDetail
                              {
                                  ID = item.ID,
                                  Title = item.Title,
                                  Text = item.Text,
                                  PackageScore = item.PackageScore
                              };

                return package.FirstOrDefault();
            }
        }

        public static List<PackageItem> GetList(int pageIndex, int pageSize, string pageOrder)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var now = DateTime.Now;
                var query = from item in db.Packages
                            where item.StartDate <= now &&
                                  item.EndDate >= now &&
                                  item.PackageStatus == PackageStatus.Approved
                            select new PackageItem
                            {
                                ID = item.ID,
                                Title = item.Title,
                                PackageScore = item.PackageScore,
                                ImageFile = (from image in db.PackageImages
                                             where image.PackageID == item.ID &&
                                                   image.PackageImagePlace == ProductImagePlace.Home
                                             select image.Filename
                                            ).FirstOrDefault(),
                                OrderID = item.OrderID
                            };


                if (!string.IsNullOrWhiteSpace(pageOrder))
                    query = query.OrderBy(pageOrder);

                query = query.Skip(pageIndex * pageSize).Take(pageSize);

                var result = query.ToList();

                foreach (var item in result)
                {
                    var Prices = PackageProducts.GetByPackageID(item.ID);

                    item.OldPrice = Prices.Sum(p => p.OldPrice);
                    item.NewPrice = Prices.Sum(p => p.NewPrice);
                    item.ImageFile = UrlProvider.GetPackageImage(item.ImageFile, StaticValues.DefaultProductImageSize);
                    item.Url = UrlProvider.GetPackageUrl(item.Title, item.ID);
                }

                return result;
            }
        }

        public static int CountList()
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var now = DateTime.Now;
                var query = from item in db.Packages
                            where item.StartDate <= now &&
                                  item.EndDate >= now &&
                                  item.PackageStatus == PackageStatus.Approved
                            orderby item.OrderID
                            select item;

                return query.Count();
            }
        }

    }
}
