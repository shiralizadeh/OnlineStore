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
    public class ProductVisit : EntityBase
    {
        [MaxLength(128)]
        [Display(Name = "کاربر")]
        public string UserID { get; set; }

        [MaxLength(15)]
        [Display(Name = "IP")]
        public string IP { get; set; }

        [ForeignKey("Product")]
        public int ProductID { get; set; }
        public Product Product { get; set; }

    }

    public static class ProductVisits
    {
        public static void Insert(ProductVisit visit)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                db.ProductVisits.Add(visit);

                db.SaveChanges();
            }
        }

        public static IList Get(int pageIndex, int pageSize, string pageOrder, int productID, DateTime? fromDate, DateTime? toDate, List<int> groups)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var query = from item in db.ProductVisits
                            select item;

                if (productID != -1)
                {
                    query = query.Where(item => item.ProductID == productID);
                }

                if (fromDate.HasValue)
                {
                    query = query.Where(item => item.LastUpdate >= fromDate);
                }

                if (toDate.HasValue)
                {
                    query = query.Where(item => item.LastUpdate <= toDate);
                }

                if (groups != null && groups.Count > 0)
                {
                    query = query.Where(item => db.ProductGroups.Any(grp => grp.ProductID == item.ProductID && groups.Contains(grp.GroupID)));
                }

                var result = from item in query
                             group item by item.ProductID into list
                             orderby list.Key
                             select new
                             {
                                 ProductTitle = list.Select(s => s.Product.Title).Distinct(),
                                 VisitsCount = list.Count(),
                                 VisitsByIP = (from c in list
                                               group c by c.IP into visits
                                               select visits
                                               ).Count(),

                             };

                result = result.OrderByDescending(item => item.VisitsCount);
                result = result.Skip(pageIndex * pageSize).Take(pageSize);

                return result.ToList();
            }
        }

        public static int Count(int productID, DateTime? fromDate, DateTime? toDate, List<int> groups)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var query = from item in db.ProductVisits
                            select item;

                if (productID != -1)
                {
                    query = query.Where(item => item.ProductID == productID);
                }

                if (fromDate.HasValue)
                {
                    query = query.Where(item => item.LastUpdate >= fromDate);
                }

                if (toDate.HasValue)
                {
                    query = query.Where(item => item.LastUpdate <= toDate);
                }

                if (groups != null && groups.Count > 0)
                {
                    query = query.Where(item => db.ProductGroups.Any(grp => grp.ProductID == item.ProductID && groups.Contains(grp.GroupID)));
                }

                var result = from item in query
                             group item by item.ProductID into list
                             orderby list.Key
                             select list;

                return result.Count();
            }
        }

    }
}
