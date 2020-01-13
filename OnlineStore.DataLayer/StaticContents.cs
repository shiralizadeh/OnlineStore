using OnlineStore.Models.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OnlineStore.EntityFramework;

namespace OnlineStore.DataLayer
{
    public class StaticContent : EntityBase
    {
        [Display(Name = "کد")]
        [MaxLength(25)]
        public string Name { get; set; }

        [Display(Name = "عنوان")]
        [MaxLength(50)]
        public string Title { get; set; }

        [Display(Name = "متن")]
        public string Content { get; set; }

        [Display(Name = "نوع")]
        public StaticContentType StaticContentType { get; set; }

        public bool IsReadOnly { get; set; }
    }
    public static class StaticContents
    {
        private static IQueryable<StaticContent> _cachedStaticContents
        {
            get
            {
                using (var db = OnlineStoreDbContext.Entity)
                    return db.StaticContents.ToCacheableList().AsQueryable();
            }
        }

        public static void Update(StaticContent staticContent)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var orgstaticContent = db.StaticContents.Where(item => item.ID == staticContent.ID).Single();

                orgstaticContent.Title = staticContent.Title;
                orgstaticContent.Content = staticContent.Content;
                orgstaticContent.StaticContentType = staticContent.StaticContentType;
                orgstaticContent.LastUpdate = staticContent.LastUpdate;

                db.SaveChanges();
            }

        }

        public static StaticContent GetByID(int id)
        {
            return _cachedStaticContents.SingleOrDefault(item => item.ID == id);
        }

        public static StaticContent GetByName(string name)
        {
            var result = _cachedStaticContents.FirstOrDefault(item => item.Name == name);

            if (result == null)
                return new StaticContent() { Content = "نا مشخص" };
            else
                return result;
        }

        public static string GetContentByName(string name)
        {
            var result = _cachedStaticContents.FirstOrDefault(item => item.Name == name);

            if (result == null)
                result = new StaticContent() { Content = "نا مشخص" };

            return result.Content;
        }

        public static List<StaticContent> Get(int pageIndex, int pageSize, string pageOrder)
        {
            var query = from item in _cachedStaticContents
                        where !item.IsReadOnly
                        select item;

            if (!string.IsNullOrWhiteSpace(pageOrder))
                query = query.OrderBy(pageOrder);

            query = query.Skip(pageIndex * pageSize).Take(pageSize);

            return query.ToList();
        }

        public static int Count()
        {
            var query = from item in _cachedStaticContents
                        where !item.IsReadOnly
                        select item;

            return query.Count();
        }

    }
}
