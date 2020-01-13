using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OnlineStore.EntityFramework;
using OnlineStore.Providers;
using OnlineStore.Models.Admin;
using OnlineStore.Models.Public;
using System.Drawing;

namespace OnlineStore.DataLayer
{
    public class Producer : EntityBase
    {
        [Display(Name = "عنوان")]
        [MaxLength(50)]
        public string Title { get; set; }

        [Display(Name = "عنوان انگلیسی")]
        [MaxLength(50)]
        public string TitleEn { get; set; }

        [Display(Name = "تصویر لوگو")]
        [MaxLength(200)]
        public string Filename { get; set; }

        [Display(Name = "ترتیب نمایش")]
        public int OrderID { get; set; }

        [Display(Name = "وزن نمایش")]
        public int Weight { get; set; }

        [Display(Name = "متن")]
        public string Text { get; set; }

        [Display(Name = "قابل نمایش")]
        public bool IsVisible { get; set; }

    }

    public static class Producers
    {
        public static IList Get(int pageIndex, int pageSize, string pageOrder, string title)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var query = from item in db.Producers
                            select new Models.Admin.ViewProducer
                            {
                                ID = item.ID,
                                Title = item.Title,
                                TitleEn = item.TitleEn,
                                Filename = StaticPaths.ProducerImages + item.Filename,
                                OrderID = item.OrderID,
                                IsVisible = item.IsVisible,
                                LastUpdate = item.LastUpdate

                            };

                if (!string.IsNullOrWhiteSpace(title))
                    query = query.Where(item => item.Title.Contains(title) || item.TitleEn.Contains(title));

                if (!string.IsNullOrWhiteSpace(pageOrder))
                    query = query.OrderBy(pageOrder);

                query = query.Skip(pageIndex * pageSize).Take(pageSize);

                var result = query.ToList();
                foreach (var item in result)
                {
                    var groupIDs = ProducerGroups.GetByProducerID(item.ID).Select(prog => prog.GroupID).ToList();

                    if (groupIDs.Count > 0)
                        item.GroupsTitle = Groups.GetByIDs(groupIDs).Select(group => group.Title).Aggregate((a, b) => b + ", " + a);
                }

                return result;
            }
        }

        public static List<Producer> GetByGroupIDs(List<int> groupIDs)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var query = from item in db.Producers
                            where db.ProducerGroups.Any(prog => prog.ProducerID == item.ID && groupIDs.Contains(prog.GroupID))
                            select item;

                query = query.OrderBy(item => item.OrderID);

                return query.ToList();
            }
        }

        public static List<Producer> GetByIDs(List<int> ids)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var query = from item in db.Producers
                            where ids.Contains(item.ID)
                            select item;

                return query.ToList();
            }
        }

        public static List<Producer> GetByProducerName(List<string> producers)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var query = from item in db.Producers
                            where producers.Contains(item.TitleEn)
                            select item;

                return query.ToList();
            }
        }

        public static List<ProducerItem> GetProducerItemByGroupIDs(List<int> groupIDs)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var query = from item in db.Producers
                            where db.ProducerGroups.Any(prog => prog.ProducerID == item.ID && groupIDs.Contains(prog.GroupID)) && item.ID != 8
                            orderby db.Products.Count(p => p.ProducerID == item.ID) descending
                            select new ProducerItem
                            {
                                ID = item.ID,
                                Title = item.Title,
                                TitleEn = item.TitleEn,
                            };

                return query.ToList();
            }
        }

        public static List<Producer> Get()
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var query = from item in db.Producers
                            where item.ID != 8
                            && item.IsVisible
                            && item.Filename != null
                            select item;

                query = query.OrderBy(item => item.OrderID);

                return query.ToList();
            }
        }

        public static List<OnlineStore.Models.Public.ViewProducer> SimpleSearch(string key, List<int> groupIDs, Size? imageSize = null)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var query = from item in db.Producers
                            where item.ID != 8
                            && item.IsVisible
                            && (item.Title.Contains(key) || item.TitleEn.Contains(key))
                            orderby item.OrderID
                            select new OnlineStore.Models.Public.ViewProducer
                            {
                                ID = item.ID,
                                Title = item.Title,
                                TitleEn = item.TitleEn,
                                Filename = item.Filename
                            };

                if (groupIDs.Count > 0)
                {
                    query = query.Where(item => db.ProducerGroups.Any(
                                                                    group => groupIDs.Contains(group.GroupID) &&
                                                                    group.ProducerID == item.ID
                                                                    ));
                }

                var list = query.Take(20).ToList();

                foreach (var item in list)
                {
                    item.Filename = UrlProvider.GetProducerImage(item.Filename,
                                                !imageSize.HasValue ?
                                                StaticValues.SearchImageSize :
                                                imageSize.Value);
                }

                return list;
            }
        }

        public static List<Producer> GetAll()
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var query = from item in db.Producers
                            select item;

                query = query.OrderBy(item => item.Title);

                return query.ToList();
            }
        }

        public static int Count(string title)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var query = from item in db.Producers
                            select item;

                if (!string.IsNullOrWhiteSpace(title))
                    query = query.Where(item => item.Title.Contains(title) || item.TitleEn.Contains(title));

                return query.Count();
            }
        }

        public static Producer GetByID(int id)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var producer = db.Producers.Where(item => item.ID == id).Single();

                return producer;
            }
        }

        public static void Delete(int id)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var producer = (from item in db.Producers
                                where item.ID == id
                                select item).Single();

                db.Producers.Remove(producer);

                db.SaveChanges();
            }
        }

        public static void Insert(Producer producer)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                db.Producers.Add(producer);

                db.SaveChanges();
            }
        }

        public static void Update(Producer producer)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var orgProducer = db.Producers.Where(item => item.ID == producer.ID).Single();

                orgProducer.Title = producer.Title;
                orgProducer.TitleEn = producer.TitleEn;
                orgProducer.Filename = producer.Filename;
                orgProducer.OrderID = producer.OrderID;
                orgProducer.Weight = producer.Weight;
                orgProducer.Text = producer.Text;
                orgProducer.IsVisible = producer.IsVisible;
                orgProducer.LastUpdate = producer.LastUpdate;

                db.SaveChanges();
            }
        }
    }
}
