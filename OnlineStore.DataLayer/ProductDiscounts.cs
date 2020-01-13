using OnlineStore.Models.Enums;
using System;
using System.Collections;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using OnlineStore.EntityFramework;
using System.ComponentModel.DataAnnotations;
using OnlineStore.Models.Admin;
using System.Collections.Generic;
using OnlineStore.Providers;
using OnlineStore.Identity;
using OnlineStore.Models.Public;

namespace OnlineStore.DataLayer
{
    public class ProductDiscount : EntityBase
    {
        [Display(Name = "عنوان")]
        [MaxLength(50)]
        public string Title { get; set; }

        [Display(Name = "محصول")]
        [ForeignKey("Product")]
        public int? ProductID { get; set; }
        public Product Product { get; set; }

        [Display(Name = "نوع کالا")]
        [ForeignKey("ProductVarient")]
        public int? ProductVarientID { get; set; }
        public ProductVarient ProductVarient { get; set; }

        [Display(Name = "گروه")]
        [ForeignKey("Group")]
        public int? GroupID { get; set; }
        public Group Group { get; set; }

        [Display(Name = "نقش")]
        public string RoleID { get; set; }

        [Display(Name = "نوع تخفیف")]
        public DiscountType DiscountType { get; set; }

        [Display(Name = "درصد تخفیف")]
        public float Percent { get; set; }

        [Display(Name = "قیمت تخفیف")]
        public float Price { get; set; }

        [Display(Name = "قیمت بعد از تخفیف")]
        public float Price_01 { get; set; }

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
        public ProductDiscountStatus ProductDiscountStatus { get; set; }
    }

    public static class ProductDiscounts
    {
        public static List<JsonProductDiscount> Get(int pageIndex, int pageSize, string pageOrder, string title, DateTime? fromDate, DateTime? toDate)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var query = from item in db.ProductDiscounts
                            select item;

                if (!String.IsNullOrWhiteSpace(title))
                    query = query.Where(item =>
                        item.Title.Contains(title) ||
                        item.Product.Title.Contains(title) ||
                        item.Product.Title_En.Contains(title)
                    );

                if (fromDate.HasValue)
                    query = query.Where(item => item.StartDate >= fromDate);

                if (toDate.HasValue)
                    query = query.Where(item => item.EndDate <= toDate);

                //if (!string.IsNullOrWhiteSpace(pageOrder))
                //query = query.OrderBy(pageOrder);
                query = query.OrderByDescending(a => a.ProductDiscountStatus);

                query = query.Skip(pageIndex * pageSize).Take(pageSize);

                var result = from item in query
                             select new JsonProductDiscount
                             {
                                 ID = item.ID,
                                 ProductID = item.ProductID,
                                 GroupID = item.GroupID,
                                 RoleID = item.RoleID,
                                 Title = item.Title,
                                 StartDate = item.StartDate,
                                 EndDate = item.EndDate,
                                 Percent = item.Percent,
                                 ProductDiscountStatus = item.ProductDiscountStatus,
                                 LastUpdate = item.LastUpdate,
                             };

                return result.ToList();
            }
        }

        internal static void DeleteByProductVarientID(int productVarientID)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                foreach (var item in GetByProductVarientID(productVarientID))
                {
                    Delete(item.ID);
                }
            }
        }

        public static List<ProductDiscount> GetByProductVarientID(int productVarientID)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var query = from item in db.ProductDiscounts
                            where item.ProductVarientID == productVarientID
                            select item;

                return query.ToList();
            }
        }

        public static int Count(string title, DateTime? fromDate, DateTime? toDate)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var query = from item in db.ProductDiscounts
                            select item;

                if (!String.IsNullOrWhiteSpace(title))
                    query = query.Where(item => item.Title.Contains(title));

                if (fromDate.HasValue)
                    query = query.Where(item => item.StartDate >= fromDate);

                if (toDate.HasValue)
                    query = query.Where(item => item.EndDate <= toDate);

                return query.Count();
            }
        }

        public static ProductDiscount GetByID(int id)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var productDiscount = db.ProductDiscounts.Where(item => item.ID == id).Single();

                return productDiscount;
            }
        }

        public static List<EditProductDiscount> GetAllByProductID(int productID)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var now = DateTime.Now;

                var query = from item in db.ProductDiscounts
                            where
                            item.ProductID == productID
                            && item.StartDate <= now
                            && item.EndDate >= now
                            && item.ProductDiscountStatus == ProductDiscountStatus.Approved
                            select new EditProductDiscount
                            {
                                ID = item.ID,
                                Title = item.Title,
                                DiscountType = item.DiscountType,
                                Price = item.Price,
                                Percent = item.Percent,
                                StartDate = item.StartDate,
                                EndDate = item.EndDate
                            };

                return query.ToList();
            }
        }

        public static List<ProductDiscount> GetByProductID(int productID)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var now = DateTime.Now;

                var query = from item in db.ProductDiscounts
                            where
                            item.ProductID == productID
                            && item.ProductVarientID == null
                            && item.StartDate <= now
                            && item.EndDate >= now
                            && item.ProductDiscountStatus == ProductDiscountStatus.Approved
                            select item;

                return query.ToList();
            }
        }

        public static List<ProductDiscount> GetByVarientID(int productID, int varientID)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var now = DateTime.Now;

                var query = from item in db.ProductDiscounts
                            where
                            item.ProductID == productID
                            && item.ProductVarientID == varientID
                            && item.StartDate <= now
                            && item.EndDate >= now
                            && item.ProductDiscountStatus == ProductDiscountStatus.Approved
                            select item;

                return query.ToList();
            }
        }

        public static bool HasVarientDiscount(int productID)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var now = DateTime.Now;

                var query = from item in db.ProductDiscounts
                            where
                            item.ProductID == productID
                            && item.StartDate <= now
                            && item.EndDate >= now
                            && item.ProductDiscountStatus == ProductDiscountStatus.Approved
                            select item;

                return query.Any();
            }
        }

        public static ProductDiscountItem GetProductDiscount(int productID, string userID, int? productVarientID = null)
        {
            List<ProductDiscount> productDiscounts = null;

            if (productVarientID.HasValue)
                productDiscounts = ProductDiscounts.GetByVarientID(productID, productVarientID.Value);
            else
                productDiscounts = ProductDiscounts.GetByProductID(productID);

            List<ProductDiscount> roleDiscounts = new List<ProductDiscount>();
            if (!String.IsNullOrWhiteSpace(userID))
            {
                var roles = UserRoles.GetByUserID(userID);
                foreach (var item in roles)
                {
                    roleDiscounts.AddRange(ProductDiscounts.GetByRoleID(item.RoleId));
                }
            }

            var groups = ProductGroups.GetByProductID(productID);
            List<ProductDiscount> groupDiscounts = new List<ProductDiscount>();
            foreach (var item in groups)
            {
                groupDiscounts.AddRange(ProductDiscounts.GetByGroupID(item.GroupID));
            }

            var productDiscountItem = new ProductDiscountItem();
            var productDiscount = new ProductDiscount();

            if (roleDiscounts.Count > 0)
            {
                productDiscount = roleDiscounts.Last();
            }

            if (groupDiscounts.Count > 0)
            {
                productDiscount = groupDiscounts.Last();
            }

            if (productDiscounts.Count > 0)
            {
                productDiscount = productDiscounts.Last();
            }

            if (productDiscount != null)
            {
                productDiscountItem.ID = productDiscount.ID;
                productDiscountItem.DiscountType = productDiscount.DiscountType;
                switch (productDiscountItem.DiscountType)
                {
                    case DiscountType.Percent:
                        productDiscountItem.Value = productDiscount.Percent;
                        break;
                    case DiscountType.PriceAfter:
                        productDiscountItem.Value = productDiscount.Price;
                        break;
                    case DiscountType.PriceBefore:
                        productDiscountItem.Value = productDiscount.Price;
                        break;
                    case DiscountType.PriceBeforeAfter:
                        productDiscountItem.Value = productDiscount.Price;
                        productDiscountItem.Price_01 = productDiscount.Price_01;
                        break;
                    default:
                        productDiscountItem.Value = 0;
                        break;
                }
            }
            else
                productDiscountItem.Value = 0;

            return productDiscountItem;
        }

        public static List<ProductDiscount> GetByRoleID(string roleID)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var now = DateTime.Now;

                var query = from item in db.ProductDiscounts
                            where item.RoleID == roleID
                            && item.StartDate <= now
                            && item.EndDate >= now
                            select item;

                return query.ToList();
            }
        }

        public static List<ProductDiscount> GetByGroupID(int groupID)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var now = DateTime.Now;

                var query = from item in db.ProductDiscounts
                            where item.GroupID == groupID
                            && item.StartDate <= now
                            && item.EndDate >= now
                            select item;

                return query.ToList();
            }
        }

        public static void Delete(int id)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var productDiscount = (from item in db.ProductDiscounts
                                       where item.ID == id
                                       select item).Single();

                db.ProductDiscounts.Remove(productDiscount);

                db.SaveChanges();
            }
        }

        public static void Insert(ProductDiscount productDiscount)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                db.ProductDiscounts.Add(productDiscount);

                db.SaveChanges();
            }
        }

        public static void Update(ProductDiscount productDiscount)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var orgProductDiscount = db.ProductDiscounts.Where(item => item.ID == productDiscount.ID).Single();

                orgProductDiscount.ProductID = productDiscount.ProductID;
                orgProductDiscount.GroupID = productDiscount.GroupID;
                orgProductDiscount.RoleID = productDiscount.RoleID;

                orgProductDiscount.DiscountType = productDiscount.DiscountType;
                orgProductDiscount.Percent = productDiscount.Percent;
                orgProductDiscount.Price = productDiscount.Price;

                orgProductDiscount.Title = productDiscount.Title;
                orgProductDiscount.StartDate = productDiscount.StartDate;
                orgProductDiscount.EndDate = productDiscount.EndDate;
                orgProductDiscount.ProductDiscountStatus = productDiscount.ProductDiscountStatus;
                orgProductDiscount.LastUpdate = productDiscount.LastUpdate;

                db.SaveChanges();
            }
        }
    }
}
