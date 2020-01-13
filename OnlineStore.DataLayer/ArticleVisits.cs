using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineStore.DataLayer
{
    public class ArticleVisit : EntityBase
    {
        [MaxLength(128)]
        [Display(Name = "کاربر")]
        public string UserID { get; set; }

        [MaxLength(15)]
        [Display(Name = "IP")]
        public string IP { get; set; }

        [ForeignKey("Article")]
        public int ArticleID { get; set; }
        public Article Article { get; set; }
    }

    public static class ArticleVisits
    {
        public static void Insert(ArticleVisit visit)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                db.ArticleVisits.Add(visit);

                db.SaveChanges();
            }
        }

        public static IList Get(int pageIndex, int pageSize, string pageOrder, int articleID, DateTime? fromDate, DateTime? toDate, int? groupID)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var query = from item in db.ArticleVisits
                            select item;

                if (articleID != -1)
                {
                    query = query.Where(item => item.ArticleID == articleID);
                }

                if (fromDate.HasValue)
                {
                    query = query.Where(item => item.LastUpdate >= fromDate);
                }

                if (toDate.HasValue)
                {
                    query = query.Where(item => item.LastUpdate <= toDate);
                }

                if (groupID != null)
                {
                    query = query.Where(item => item.Article.GroupID == groupID);
                }

                var result = from item in query
                             group item by item.ArticleID into list
                             orderby list.Key
                             select new
                             {
                                 ArticleTitle = list.Select(s => s.Article.Title).Distinct(),
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

        public static int Count(int articleID, DateTime? fromDate, DateTime? toDate, int? groupID)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var query = from item in db.ArticleVisits
                            select item;

                if (articleID != -1)
                {
                    query = query.Where(item => item.ArticleID == articleID);
                }

                if (fromDate.HasValue)
                {
                    query = query.Where(item => item.LastUpdate >= fromDate);
                }

                if (toDate.HasValue)
                {
                    query = query.Where(item => item.LastUpdate <= toDate);
                }

                if (groupID != null)
                {
                    query = query.Where(item => item.Article.GroupID == groupID);
                }

                var result = from item in query
                             group item by item.ArticleID into list
                             orderby list.Key
                             select list;

                return result.Count();
            }
        }

    }
}
