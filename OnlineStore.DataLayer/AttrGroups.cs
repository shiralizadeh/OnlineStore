using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OnlineStore.EntityFramework;
using OnlineStore.Models.Admin;

namespace OnlineStore.DataLayer
{
    public class AttrGroup : EntityBase
    {
        [Display(Name = "عنوان")]
        [MaxLength(100)]
        public string Title { get; set; }

        [Display(Name = "ترتیب نمایش")]
        public int OrderID { get; set; }
    }

    public static class AttrGroups
    {
        public static IList Get(int pageIndex, int pageSize, string pageOrder, string title, List<int> groups)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var query = from item in db.AttrGroups
                            select new ViewAttrGroup
                            {
                                ID = item.ID,
                                Title = item.Title,
                                LastUpdate = item.LastUpdate,
                                OrderID = item.OrderID
                            };

                if (groups != null && groups.Count > 0)
                {
                    query = query.Where(item => db.AttrGroupGroups.Any(group => group.AttrGroupID == item.ID && groups.Contains(group.GroupID)));
                }

                if (!string.IsNullOrWhiteSpace(title))
                    query = query.Where(item => item.Title.Contains(title));

                if (!string.IsNullOrWhiteSpace(pageOrder))
                    query = query.OrderBy(pageOrder);

                query = query.Skip(pageIndex * pageSize).Take(pageSize);

                var result = query.ToList();
                foreach (var item in result)
                {
                    var groupIDs = AttrGroupGroups.GetByAttrGroupID(item.ID).Select(attrg => attrg.GroupID).ToList();

                    if (groupIDs.Count > 0)
                        item.GroupsTitle = Groups.GetByIDs(groupIDs).Select(group => group.Title).Aggregate((a, b) => b + ", " + a);
                }

                return result;
            }
        }

        public static List<AttrGroup> GetByGroupID(int groupID)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var query = from attrGroup in db.AttrGroups
                            where db.AttrGroupGroups.Any(attrGroupGroup => attrGroupGroup.AttrGroupID == attrGroup.ID)
                            select attrGroup;

                query = query.OrderBy(item => item.OrderID);

                return query.ToList();
            }
        }

        public static List<AttrGroup> GetByGroupIDs(List<int> groupIDs)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var query = from attrGroup in db.AttrGroups
                            where db.AttrGroupGroups.Any(attrGroupGroup => attrGroupGroup.AttrGroupID == attrGroup.ID && groupIDs.Contains(attrGroupGroup.GroupID))
                            select attrGroup;

                query = query.OrderBy(item => item.OrderID);

                return query.ToList();
            }
        }

        public static int Count(string title, List<int> groups)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var query = from item in db.AttrGroups
                            select item;

                if (groups != null && groups.Count > 0)
                {
                    query = query.Where(item => db.AttrGroupGroups.Any(group => group.AttrGroupID == item.ID && groups.Contains(group.GroupID)));
                }

                if (!string.IsNullOrWhiteSpace(title))
                    query = query.Where(item => item.Title.Contains(title));

                return query.Count();
            }
        }

        public static AttrGroup GetByID(int id)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var attrGroup = db.AttrGroups.Where(item => item.ID == id).Single();

                return attrGroup;
            }
        }

        public static void Delete(int id)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var attrGroup = (from item in db.AttrGroups
                                 where item.ID == id
                                 select item).Single();

                db.AttrGroups.Remove(attrGroup);

                db.SaveChanges();
            }
        }

        public static void Insert(AttrGroup attrGroup)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                db.AttrGroups.Add(attrGroup);

                db.SaveChanges();
            }
        }

        public static void Update(AttrGroup attrGroup)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var orgAttrGroup = db.AttrGroups.Where(item => item.ID == attrGroup.ID).Single();

                orgAttrGroup.Title = attrGroup.Title;
                orgAttrGroup.OrderID = attrGroup.OrderID;
                orgAttrGroup.LastUpdate = attrGroup.LastUpdate;

                db.SaveChanges();
            }
        }

        public static List<AttrGroup> GetAll()
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var query = from item in db.AttrGroups
                            select item;

                query = query.OrderBy(item => item.OrderID);

                return query.ToList();
            }
        }
    }
}
