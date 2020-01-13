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
    public class AttrGroupGroup : EntityBase
    {
        [Display(Name = "گروه ویژگی")]
        [ForeignKey("AttrGroup")]
        public int AttrGroupID { get; set; }
        public AttrGroup AttrGroup { get; set; }

        [Display(Name = "گروه")]
        [ForeignKey("Group")]
        public int GroupID { get; set; }
        public Group Group { get; set; }
    }

    public static class AttrGroupGroups
    {
        public static IList Get(int pageIndex, int pageSize, string pageOrder)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var query = from item in db.AttrGroupGroups
                            select new
                            {
                                item.ID,
                                item.Group.Title,
                                item.LastUpdate,
                            };

                if (!string.IsNullOrWhiteSpace(pageOrder))
                    query = query.OrderBy(pageOrder);

                query = query.Skip(pageIndex * pageSize).Take(pageSize);

                return query.ToList();
            }
        }

        public static List<AttrGroupGroup> GetByAttrGroupID(int attrGroupID)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var query = from item in db.AttrGroupGroups
                            where item.AttrGroupID == attrGroupID
                            select item;

                return query.ToList();
            }
        }

        public static int Count()
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var query = from item in db.AttrGroupGroups
                            select item;

                return query.Count();
            }
        }

        public static AttrGroupGroup GetByID(int id)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var attrGroupGroup = db.AttrGroupGroups.Where(item => item.ID == id).Single();

                return attrGroupGroup;
            }
        }

        public static void Delete(int id)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var attrGroupGroup = (from item in db.AttrGroupGroups
                                      where item.ID == id
                                      select item).Single();

                db.AttrGroupGroups.Remove(attrGroupGroup);

                db.SaveChanges();
            }
        }

        public static void Insert(AttrGroupGroup attrGroupGroup)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                if (!db.Groups.Any(item => item.ParentID == attrGroupGroup.GroupID))
                {
                    db.AttrGroupGroups.Add(attrGroupGroup);

                    db.SaveChanges();
                }
            }
        }

        public static void Update(AttrGroupGroup attrGroupGroup)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var orgAttrGroupGroup = db.AttrGroupGroups.Where(item => item.ID == attrGroupGroup.ID).Single();

                orgAttrGroupGroup.AttrGroupID = attrGroupGroup.AttrGroupID;
                orgAttrGroupGroup.GroupID = attrGroupGroup.GroupID;
                orgAttrGroupGroup.LastUpdate = attrGroupGroup.LastUpdate;

                db.SaveChanges();
            }
        }
    }
}
