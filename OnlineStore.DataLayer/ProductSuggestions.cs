using OnlineStore.Models.Admin;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OnlineStore.EntityFramework;

namespace OnlineStore.DataLayer
{
    public class ProductSuggestion : EntityBase
    {
        [ForeignKey("Product")]
        public int ProductID { get; set; }
        public Product Product { get; set; }

        [MaxLength(128)]
        public string UserID { get; set; }

        [MaxLength(20)]
        public string IP { get; set; }

        [MaxLength(300)]
        public string FriendEmail { get; set; }

        public string Message { get; set; }
    }

    public static class ProductSuggestions
    {
        public static void Insert(ProductSuggestion productSuggestion)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                db.ProductSuggestions.Add(productSuggestion);

                db.SaveChanges();
            }
        }

        public static List<ViewProductSuggestion> Get(int pageIndex, int pageSize, string pageOrder, int? productID, string friendEmail, string message)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var query = from item in db.ProductSuggestions
                            select new ViewProductSuggestion
                            {
                                ID = item.ID,
                                UserID = item.UserID,
                                LastUpdate = item.LastUpdate,
                                ProductID = item.ProductID,
                                FriendEmail = item.FriendEmail,
                                Message = item.Message,
                                IP = item.IP
                            };

                if (productID.HasValue)
                    query = query.Where(item => item.ProductID == productID);

                if (!String.IsNullOrWhiteSpace(friendEmail))
                    query = query.Where(item => item.FriendEmail.Contains(friendEmail));

                if (!String.IsNullOrWhiteSpace(message))
                    query = query.Where(item => item.Message.Contains(message));

                if (!string.IsNullOrWhiteSpace(pageOrder))
                    query = query.OrderBy(pageOrder);

                query = query.Skip(pageIndex * pageSize).Take(pageSize);

                return query.ToList();
            }
        }

        public static int Count(int? productID, string friendEmail, string message)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var query = from item in db.ProductSuggestions
                            select item;

                if (productID.HasValue)
                    query = query.Where(item => item.ProductID == productID);

                if (!String.IsNullOrWhiteSpace(friendEmail))
                    query = query.Where(item => item.FriendEmail.Contains(friendEmail));

                if (!String.IsNullOrWhiteSpace(message))
                    query = query.Where(item => item.Message.Contains(message));

                return query.Count();
            }
        }

        public static void Delete(int id)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var suggest = (from item in db.ProductSuggestions
                               where item.ID == id
                               select item).Single();

                db.ProductSuggestions.Remove(suggest);

                db.SaveChanges();
            }
        }

    }
}
