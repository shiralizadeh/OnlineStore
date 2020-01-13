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
using OnlineStore.Models.Public;

namespace OnlineStore.DataLayer
{
    public class ProducerGroup : EntityBase
    {
        [Display(Name = "گروه")]
        [ForeignKey("Group")]
        public int GroupID { get; set; }
        public Group Group { get; set; }

        [Display(Name = "تولید کننده")]
        [ForeignKey("Producer")]
        public int ProducerID { get; set; }
        public Producer Producer { get; set; }
    }

    public static class ProducerGroups
    {
        public static IList Get(int pageIndex, int pageSize, string pageOrder)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var query = from item in db.ProducerGroups
                            select new
                            {
                                item.ID,
                                item.ProducerID,
                                item.GroupID,
                                item.LastUpdate,
                            };

                if (!string.IsNullOrWhiteSpace(pageOrder))
                    query = query.OrderBy(pageOrder);

                query = query.Skip(pageIndex * pageSize).Take(pageSize);

                return query.ToList();
            }
        }

        public static List<ProducerGroup> GetByGroupIDs(List<int> groupIDs)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var query = from item in db.ProducerGroups
                            where groupIDs.Contains(item.GroupID)
                            select item;

                return query.ToList();
            }
        }

        public static List<ProducerGroup> GetByProducerID(int producerID)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var query = from item in db.ProducerGroups
                            where item.ProducerID == producerID
                            select item;

                return query.ToList();
            }
        }

        public static List<ProducerGroup> Get()
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var query = from item in db.ProducerGroups
                            select item;

                return query.ToList();
            }
        }

        public static int Count()
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var query = from item in db.ProducerGroups
                            select item;

                return query.Count();
            }
        }

        public static ProducerGroup GetByID(int id)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var producerGroup = db.ProducerGroups.Where(item => item.ID == id).Single();

                return producerGroup;
            }
        }

        public static void Delete(int id)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var producerGroup = (from item in db.ProducerGroups
                                     where item.ID == id
                                     select item).Single();

                db.ProducerGroups.Remove(producerGroup);

                db.SaveChanges();
            }
        }

        public static void Insert(ProducerGroup producerGroup)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                if (!db.Groups.Any(item => item.ParentID == producerGroup.GroupID))
                {
                    db.ProducerGroups.Add(producerGroup);

                    db.SaveChanges();
                }
            }
        }

        public static void Update(ProducerGroup producerGroup)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var orgProducerGroup = db.ProducerGroups.Where(item => item.ID == producerGroup.ID).Single();

                orgProducerGroup.ProducerID = producerGroup.ProducerID;
                orgProducerGroup.GroupID = producerGroup.GroupID;
                orgProducerGroup.LastUpdate = producerGroup.LastUpdate;

                db.SaveChanges();
            }
        }
    }
}