using OnlineStore.Models.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OnlineStore.EntityFramework;
using System.Collections;

namespace OnlineStore.DataLayer
{
    public class ProductRequest : EntityBase
    {
        [Display(Name = "محصول")]
        [ForeignKey("Product")]
        public int ProductID { get; set; }
        public Product Product { get; set; }

        [MaxLength(128)]
        [Display(Name = "کاربر")]
        public string UserID { get; set; }

        [MaxLength(300)]
        [Display(Name = "ایمیل")]
        public string Email { get; set; }

        [Display(Name = "شماره همراه")]
        [MaxLength(50)]
        public string Mobile { get; set; }

        [MaxLength(1000)]
        [Display(Name = "توضیحات")]
        public string Description { get; set; }

        [Display(Name = "وضعیت")]
        public ProductRequestStatus ProductRequestStatus { get; set; }

        [Display(Name = "تاریخ ثبت")]
        public DateTime DateTime { get; set; }
    }

    public static class ProductRequests
    {
        public static void Insert(ProductRequest request)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                db.ProductRequests.Add(request);

                db.SaveChanges();
            }
        }

        public static IList Get(int pageIndex, int pageSize, string pageOrder, int productID, ProductRequestStatus? productRequestStatus)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var query = from item in db.ProductRequests
                            select new
                            {
                                item.ID,
                                item.Email,
                                item.Mobile,
                                item.DateTime,
                                item.LastUpdate,
                                ProductTitle = item.Product.Title,
                                item.ProductID,
                                item.ProductRequestStatus,
                                item.Description
                            };

                if (productID != -1)
                {
                    query = query.Where(item => item.ProductID == productID);
                }
                if (productRequestStatus.HasValue)
                {
                    query = query.Where(item => item.ProductRequestStatus == productRequestStatus.Value);
                }

                if (!string.IsNullOrWhiteSpace(pageOrder))
                    query = query.OrderBy(pageOrder);

                query = query.Skip(pageIndex * pageSize).Take(pageSize);

                return query.ToList();
            }
        }

        public static int Count(int productID, ProductRequestStatus? productRequestStatus)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var query = from item in db.ProductRequests
                            select item;

                if (productID != -1)
                {
                    query = query.Where(item => item.ProductID == productID);
                }
                if (productRequestStatus.HasValue)
                {
                    query = query.Where(item => item.ProductRequestStatus == productRequestStatus.Value);
                }

                return query.Count();
            }
        }

        public static void Delete(int id)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var request = (from item in db.ProductRequests
                               where item.ID == id
                               select item).Single();

                db.ProductRequests.Remove(request);

                db.SaveChanges();
            }
        }

        public static ProductRequest GetByID(int id)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var request = db.ProductRequests.Where(item => item.ID == id).Single();

                return request;
            }
        }

        public static List<string> GetNewEmails(int productID)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var requests = from item in db.ProductRequests
                               where item.ProductID == productID
                               && item.ProductRequestStatus == ProductRequestStatus.NotChecked
                               group item by item.Email into emails
                               select emails.Key;

                return requests.ToList();
            }
        }

        public static List<string> GetNewMobiles(int productID)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var requests = from item in db.ProductRequests
                               where item.ProductID == productID
                               && item.ProductRequestStatus == ProductRequestStatus.NotChecked
                               group item by item.Mobile into mobiles
                               select mobiles.Key;

                return requests.ToList();
            }
        }

        public static void Update(ProductRequest request)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var orgComment = db.ProductRequests.Where(item => item.ID == request.ID).Single();

                orgComment.ProductRequestStatus = request.ProductRequestStatus;
                orgComment.LastUpdate = request.LastUpdate;

                db.SaveChanges();
            }
        }

        public static void UpdateStatus(string[] emails, string[] mobiles)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var orgRequests = db.ProductRequests.Where(item => emails.Contains(item.Email) || mobiles.Contains(item.Mobile)).ToList();

                foreach (var item in orgRequests)
                {
                    item.ProductRequestStatus = ProductRequestStatus.Answered;
                    item.LastUpdate = DateTime.Now;
                }

                db.SaveChanges();
            }
        }

    }

}
