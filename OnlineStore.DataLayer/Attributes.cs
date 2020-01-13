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
using OnlineStore.Models.Public;
using OnlineStore.Providers;

namespace OnlineStore.DataLayer
{
    public class Attribute : EntityBase
    {
        [Display(Name = "گروه ویژگی")]
        [ForeignKey("AttrGroup")]
        public int AttrGroupID { get; set; }
        public AttrGroup AttrGroup { get; set; }

        [Display(Name = "عنوان")]
        [MaxLength(250)]
        public string Title { get; set; }

        [Display(Name = "نوع")]
        public AttributeType AttributeType { get; set; }

        [Display(Name = "پسوند")]
        public string Posfix { get; set; }

        [Display(Name = "پیشوند")]
        public string Perfix { get; set; }

        [Display(Name = "قابل جستجو")]
        public bool IsSearchable { get; set; }

        [Display(Name = "ترتیب نمایش")]
        public int OrderID { get; set; }

        [Display(Name = "ویژگی وابسته")]
        [ForeignKey("Dependent")]
        public int? DependentID { get; set; }

        [Display(Name = "ویژگی وابسته")]
        public Attribute Dependent { get; set; }

        public int? DependentOptionID { get; set; }
    }

    public static class Attributes
    {
        public static List<GroupOption> GroupOptions
        {
            get
            {
                return (List<GroupOption>)StaticValues.GroupOptions;
            }
        }

        public static IList Get(int pageIndex, int pageSize, string pageOrder, string title, int attrGroupID, List<int> groups, AttributeType? attributeType)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var query = from item in db.Attributes
                            select new ViewAttribute
                            {
                                ID = item.ID,
                                Title = item.Title,
                                AttrGroupID = item.AttrGroupID,
                                AttrGroupTitle = item.AttrGroup.Title,
                                AttributeType = item.AttributeType,
                                IsSearchable = item.IsSearchable,
                                LastUpdate = item.LastUpdate,
                                OrderID = item.OrderID
                            };
                if (attributeType != null)
                {
                    query = query.Where(item => item.AttributeType == attributeType);
                }

                if (groups != null && groups.Count > 0)
                {
                    query = query.Where(item => db.AttributeGroups.Any(group => group.AttributeID == item.ID && groups.Contains(group.GroupID)));
                }

                if (!string.IsNullOrWhiteSpace(title))
                    query = query.Where(item => item.Title.Contains(title));

                if (attrGroupID != -1)
                    query = query.Where(item => item.AttrGroupID == attrGroupID);

                if (!string.IsNullOrWhiteSpace(pageOrder))
                    query = query.OrderBy(pageOrder);

                query = query.Skip(pageIndex * pageSize).Take(pageSize);

                var result = query.ToList();
                foreach (var item in result)
                {
                    var groupIDs = AttributeGroups.GetByAttributeID(item.ID).Select(attrg => attrg.GroupID).ToList();

                    if (groupIDs.Count > 0)
                        item.GroupsTitle = Groups.GetByIDs(groupIDs).Select(group => group.Title).Aggregate((a, b) => b + ", " + a);
                }

                return result;
            }
        }

        public static int Count(string title, int attrGroupID, List<int> groups, AttributeType? attributeType)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var query = from item in db.Attributes
                            select item;

                if (attributeType != null)
                {
                    query = query.Where(item => item.AttributeType == attributeType);
                }

                if (groups != null && groups.Count > 0)
                {
                    query = query.Where(item => db.AttributeGroups.Any(group => group.AttributeID == item.ID && groups.Contains(group.GroupID)));
                }

                if (!string.IsNullOrWhiteSpace(title))
                    query = query.Where(item => item.Title.Contains(title));

                if (attrGroupID != -1)
                    query = query.Where(item => item.AttrGroupID == attrGroupID);

                return query.Count();
            }
        }

        public static List<ViewAttribute> GetByGroupIDs(List<int> groups, bool? isSearchable = null)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var output = new List<ViewAttribute>();
                var attributes = (from attr in db.Attributes
                                  where db.AttributeGroups.Any(attrg => attrg.AttributeID == attr.ID && groups.Contains(attrg.GroupID))
                                  select new ViewAttribute
                                  {
                                      ID = attr.ID,
                                      Title = attr.Title,
                                      AttrGroupID = attr.AttrGroupID,
                                      AttrGroupTitle = attr.AttrGroup.Title,
                                      AttributeType = attr.AttributeType,
                                      Perfix = attr.Perfix,
                                      Posfix = attr.Posfix,
                                      IsSearchable = attr.IsSearchable,
                                      OrderID = attr.OrderID,
                                      GroupOrderID = attr.AttrGroup.OrderID,
                                      DependentID = attr.DependentID,
                                      DependentOptionID = attr.DependentOptionID
                                  });

                attributes = attributes.OrderBy(item => item.OrderID);

                if (isSearchable.HasValue)
                    attributes = attributes.Where(attr => attr.IsSearchable == isSearchable);

                output = attributes.ToList();

                foreach (var item in output)
                {
                    item.Options = AttributeOptions.GetByAttributeID(item.ID)
                                                   .OrderBy(ao => ao.OrderID)
                                                   .Select(ao =>
                                                       new ViewOption()
                                                       {
                                                           ID = ao.ID,
                                                           Title = ao.Title,
                                                           //Count = db.AttributeValues.Count(av => av.AttributeOptionID == ao.ID)
                                                       })
                                                   .ToList();
                }

                return output;
            }
        }

        public static List<ViewAttribute> GetDependentByGroupIDs(List<int> groups)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var output = new List<ViewAttribute>();
                var attributes = (from attr in db.Attributes
                                  where db.AttributeGroups.Any(attrg => attrg.AttributeID == attr.ID && groups.Contains(attrg.GroupID)) &&
                                        (attr.AttributeType == AttributeType.Check ||
                                         attr.AttributeType == AttributeType.SingleItem ||
                                         attr.AttributeType == AttributeType.MultipleItem)
                                  select new ViewAttribute
                                  {
                                      ID = attr.ID,
                                      Title = attr.Title,
                                      AttrGroupID = attr.AttrGroupID,
                                      AttrGroupTitle = attr.AttrGroup.Title,
                                      AttributeType = attr.AttributeType,
                                      Perfix = attr.Perfix,
                                      Posfix = attr.Posfix,
                                      IsSearchable = attr.IsSearchable,
                                      OrderID = attr.OrderID,
                                      GroupOrderID = attr.AttrGroup.OrderID
                                  });

                attributes = attributes.OrderBy(item => item.OrderID);

                output = attributes.ToList();

                return output;
            }
        }

        public static Attribute GetByID(int id)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var attribute = db.Attributes.Where(item => item.ID == id).Single();

                return attribute;
            }
        }

        public static List<Attribute> GetByIDs(List<int> ids)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var result = db.Attributes.Where(item => ids.Contains(item.ID));

                result = result.OrderBy(item => item.OrderID);

                return result.ToList();
            }
        }

        public static void Delete(int id)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var attribute = (from item in db.Attributes
                                 where item.ID == id
                                 select item).Single();

                db.Attributes.Remove(attribute);

                db.SaveChanges();
            }
        }

        public static void Insert(Attribute attribute)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                db.Attributes.Add(attribute);

                db.SaveChanges();
            }
        }

        public static void Update(Attribute attribute)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var orgAttribute = db.Attributes.Where(item => item.ID == attribute.ID).Single();

                orgAttribute.Title = attribute.Title;
                orgAttribute.AttrGroupID = attribute.AttrGroupID;
                orgAttribute.AttributeType = attribute.AttributeType;
                orgAttribute.Posfix = attribute.Posfix;
                orgAttribute.Perfix = attribute.Perfix;
                orgAttribute.IsSearchable = attribute.IsSearchable;
                orgAttribute.OrderID = attribute.OrderID;
                orgAttribute.DependentID = attribute.DependentID;
                orgAttribute.DependentOptionID = attribute.DependentOptionID;
                orgAttribute.LastUpdate = attribute.LastUpdate;

                db.SaveChanges();
            }
        }

    }

}
