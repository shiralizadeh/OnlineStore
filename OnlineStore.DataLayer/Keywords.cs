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
    public class Keyword : EntityBase
    {
        [Display(Name = "کلیدواژه")]
        public string Title { get; set; }

        [Display(Name = "فعال")]
        public bool IsActive { get; set; }
    }

    public static class Keywords
    {
        public static IList Get(int pageIndex, int pageSize, string pageOrder, string title)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var query = from item in db.Keywords
                            select new
                            {
                                item.ID,
                                item.Title,
                                item.IsActive,
                                item.LastUpdate
                            };

                if (!string.IsNullOrWhiteSpace(title))
                    query = query.Where(item => item.Title.Contains(title));

                if (!string.IsNullOrWhiteSpace(pageOrder))
                    query = query.OrderBy(pageOrder);

                query = query.Skip(pageIndex * pageSize).Take(pageSize);

                return query.ToList();
            }
        }

        public static int Count(string title)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var query = from item in db.Keywords
                            select item;

                if (!string.IsNullOrWhiteSpace(title))
                    query = query.Where(item => item.Title.Contains(title));

                return query.Count();
            }
        }

        public static Keyword GetByID(int id)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var keyword = db.Keywords.Where(item => item.ID == id).Single();

                return keyword;
            }
        }

        public static void Insert(Keyword keyword)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                db.Keywords.Add(keyword);

                db.SaveChanges();
            }
        }

        public static void Delete(int id)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var keyword = (from item in db.Keywords
                               where item.ID == id
                               select item).Single();

                db.Keywords.Remove(keyword);

                db.SaveChanges();
            }
        }

        public static void Update(Keyword keyword)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var orgKeyword = db.Keywords.Where(item => item.ID == keyword.ID).Single();

                orgKeyword.Title = keyword.Title;
                orgKeyword.IsActive = keyword.IsActive;

                db.SaveChanges();
            }
        }

        public static List<Keyword> Search(string key, bool? isActive = null)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var query = from item in db.Keywords
                            where item.Title.Contains(key)
                            select item;

                if (isActive.HasValue)
                {
                    query = query.Where(item => item.IsActive == isActive);
                }

                return query.ToList();
            }
        }

        public static Keyword GetByTitle(string key)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var query = from item in db.Keywords
                            where item.Title == key
                            select item;

                var keyword = query.FirstOrDefault();

                return keyword;
            }
        }

    }
}
