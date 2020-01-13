using OnlineStore.Models.Enums;
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
    public class AttributeGroup : EntityBase
    {
        [Display(Name = "ویژگی")]
        [ForeignKey("Attribute")]
        public int AttributeID { get; set; }
        public Attribute Attribute { get; set; }

        [Display(Name = "گروه")]
        [ForeignKey("Group")]
        public int GroupID { get; set; }
        public Group Group { get; set; }
    }

    public static class AttributeGroups
    {
        public static IList Get(int pageIndex, int pageSize, string pageOrder)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var query = from item in db.AttributeGroups
                            select new
                            {
                                item.ID,
                                item.AttributeID,
                                item.GroupID,
                                item.LastUpdate,
                                item.Attribute.Title,
                            };

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
                var query = from item in db.AttributeGroups
                            select new
                            {
                                item.ID,
                                item.AttributeID,
                                item.GroupID,
                                item.LastUpdate,
                                item.Attribute.Title,
                            };

                return query.Count();
            }
        }

        public static AttributeGroup GetByID(int id)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var attributeGroup = db.AttributeGroups.Where(item => item.ID == id).Single();

                return attributeGroup;
            }
        }

        public static List<AttributeGroup> GetByAttributeID(int attributeID)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var query = from item in db.AttributeGroups
                            where item.AttributeID == attributeID
                            select item;

                return query.ToList();
            }
        }

        public static void Delete(int id)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var attributeGroup = (from item in db.AttributeGroups
                                      where item.ID == id
                                      select item).Single();

                db.AttributeGroups.Remove(attributeGroup);

                db.SaveChanges();
            }
        }

        public static void Insert(AttributeGroup attributeGroup)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                if (!db.Groups.Any(item => item.ParentID == attributeGroup.GroupID))
                {
                    db.AttributeGroups.Add(attributeGroup);

                    db.SaveChanges();
                }
            }
        }

        public static void Update(AttributeGroup attributeGroup)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var orgAttributeGroup = db.AttributeGroups.Where(item => item.ID == attributeGroup.ID).Single();

                orgAttributeGroup.GroupID = attributeGroup.GroupID;
                orgAttributeGroup.AttributeID = attributeGroup.AttributeID;
                orgAttributeGroup.LastUpdate = attributeGroup.LastUpdate;

                db.SaveChanges();
            }
        }

    }

}
