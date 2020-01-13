using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OnlineStore.EntityFramework;

namespace OnlineStore.DataLayer
{
    public class NewsLetterMember : EntityBase
    {
        [Display(Name = "پست الکترونیک")]
        [MaxLength(300)]
        public string Email { get; set; }
    }

    public static class NewsLetterMembers
    {
        public static void Insert(NewsLetterMember member)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                db.NewsLetterMembers.Add(member);

                db.SaveChanges();
            }
        }

        public static List<NewsLetterMember> Get(int pageIndex, int pageSize, string pageOrder)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var query = from item in db.NewsLetterMembers
                            select item;

                if (!string.IsNullOrWhiteSpace(pageOrder))
                    query = query.OrderBy(pageOrder);

                query = query.Skip(pageIndex * pageSize).Take(pageSize);

                return query.ToList();
            }
        }

        public static int Count()
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var query = from item in db.NewsLetterMembers
                            select item;

                return query.Count();
            }
        }

        public static void Delete(int id)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var member = (from item in db.NewsLetterMembers
                              where item.ID == id
                              select item).Single();

                db.NewsLetterMembers.Remove(member);

                db.SaveChanges();
            }
        }
    }
}
