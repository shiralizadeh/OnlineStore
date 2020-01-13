using OnlineStore.Models.Enums;
using OnlineStore.Models.Public;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OnlineStore.EntityFramework;
using OnlineStore.Models;

namespace OnlineStore.DataLayer
{
    public class Group : EntityBase
    {
        [Display(Name = "عنوان")]
        [MaxLength(50)]
        public string Title { get; set; }

        [Display(Name = "عنوان انگلیسی")]
        [MaxLength(50)]
        public string TitleEn { get; set; }

        [NotMapped]
        public string UrlPerfix
        {
            get
            {
                return TitleEn;
            }
        }

        [Display(Name = "پیشوند فارسی")]
        public string Perfix { get; set; }

        [Display(Name = "پیشوند انگلیسی")]
        public string Perfix_En { get; set; }

        [Display(Name = "پدر")]
        [ForeignKey("Parent")]
        public int? ParentID { get; set; }
        public Group Parent { get; set; }

        [Display(Name = "نوع گروه")]
        public GroupType GroupType { get; set; }

        [Display(Name = "عکس گروه")]
        [MaxLength(200)]
        public string Image { get; set; }

        [Display(Name = "عکس دکمه")]
        [MaxLength(200)]
        public string ButtonImage { get; set; }

        [Display(Name = "توضیحات")]
        [MaxLength(1000)]
        public string Text { get; set; }
    }

    public static class Groups
    {
        private static IQueryable<Group> _cachedGroups
        {
            get
            {
                using (var db = OnlineStoreDbContext.Entity)
                    return db.Groups.ToCacheableList().AsQueryable();
            }
        }

        public static IList Get(int pageIndex, int pageSize, string pageOrder, GroupType groupType, string title)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var query = from item in db.Groups
                            where item.GroupType == groupType
                            select new
                            {
                                item.ID,
                                ParentGroupTitle = item.Parent.Title,
                                item.Title,
                                item.TitleEn,
                                item.Perfix,
                                item.Perfix_En,
                                item.LastUpdate,
                            };

                if (!string.IsNullOrWhiteSpace(title))
                    query = query.Where(item => item.Title.Contains(title) || item.TitleEn.Contains(title));

                if (!string.IsNullOrWhiteSpace(pageOrder))
                    query = query.OrderBy(pageOrder);

                query = query.Skip(pageIndex * pageSize).Take(pageSize);

                return query.ToList();
            }
        }

        public static List<Group> GetRoot(GroupType groupType)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var query = from item in _cachedGroups
                            where item.ParentID == null && item.GroupType == groupType
                            select item;

                return query.ToList();
            }
        }

        public static List<ViewArticleGroup> GetAll(GroupType groupType)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var query = from item in _cachedGroups
                            where item.GroupType == groupType
                            select new ViewArticleGroup
                            {
                                ID = item.ID,
                                Title = item.Title,
                                TitleEn = item.TitleEn,
                            };

                return query.ToList();
            }
        }

        public static List<Group> GetByGroupType(GroupType groupType)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var query = from item in _cachedGroups
                            where item.GroupType == groupType
                            select item;

                return query.ToList();
            }
        }

        public static List<Group> Get(GroupType groupType)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var query = from item in _cachedGroups
                            where item.GroupType == groupType
                            select item;

                return query.ToList();
            }
        }

        public static List<Group> GetByParentID(int parentID)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var query = from item in _cachedGroups
                            where item.ParentID == parentID
                            select item;

                return query.ToList();
            }
        }

        public static List<Group> GetChildsRecursive(int groupID)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var result = new List<Group>();

                var query = from item in _cachedGroups
                            where item.ParentID == groupID
                            select item;

                foreach (var item in query)
                {
                    if (HasChild(item.ID))
                        GetRecursive(result, item.ID);
                    else
                        result.Add(item);
                }

                return result;
            }
        }

        public static List<Group> GetParentsRecursive(Group group)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var result = new List<Group>();

                var parent = db.Groups.Where(item => item.ID == group.ParentID.Value).Single();

                while (parent != null)
                {
                    result.Add(parent);

                    if (parent.ParentID.HasValue)
                        parent = Groups.GetByID(parent.ParentID.Value);
                    else
                        parent = null;
                }

                result.Reverse();

                result.Add(group);

                return result;
            }
        }

        private static void GetRecursive(List<Group> result, int groupID)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var query = from item in _cachedGroups
                            where item.ParentID == groupID
                            select item;

                foreach (var item in query)
                {
                    if (HasChild(item.ID))
                        GetRecursive(result, item.ID);
                    else
                        result.Add(item);
                }
            }
        }

        public static List<ViewProductGroup> GetRelatedGroupsByProducer(int producerID)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var now = DateTime.Now;

                var query = from item in _cachedGroups
                            where item.GroupType == GroupType.Products
                            && db.ProducerGroups.Any(grp => grp.GroupID == item.ID && grp.ProducerID == producerID)
                            && db.Products.Any(p => p.ProductStatus == ProductStatus.Approved
                                               && p.PublishDate <= now
                                               && !p.IsInVisible
                                               && p.ProducerID == producerID
                                               && p.GroupID == item.ID
                                               )
                            select new ViewProductGroup
                            {
                                ID = item.ID,
                                Perfix = item.Perfix,
                                Title = item.Title,
                                TitleEn = item.TitleEn
                            };

                return query.ToList();
            }
        }

        public static List<Group> SimpleSearch(string key)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var query = from item in _cachedGroups
                            where (item.Title.Contains(key) || item.TitleEn.Contains(key))
                            && item.GroupType == GroupType.Products
                            orderby item.LastUpdate
                            select item;

                return query.Take(20).ToList();

            }
        }

        public static bool HasChild(int groupID)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                return db.Groups.Any(item => item.ParentID == groupID);
            }
        }

        public static int Count(GroupType groupType, string title)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var query = from item in _cachedGroups
                            where item.GroupType == groupType
                            select item;

                if (!string.IsNullOrWhiteSpace(title))
                    query = query.Where(item => item.Title.Contains(title) || item.TitleEn.Contains(title));

                return query.Count();
            }
        }

        public static Group GetByID(int id)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var group = db.Groups.Where(item => item.ID == id).Single();

                return group;
            }
        }

        public static Group GetRandom()
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var query = from item in _cachedGroups
                            where !_cachedGroups.Any(a => a.ParentID == item.ID)
                            select item;

                Random rand = new Random();
                int toSkip = rand.Next(0, query.Count());

                return query.Skip(toSkip).Take(1).First();
            }
        }

        public static DateTime LastestDate(GroupType groupType)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var list = from item in _cachedGroups
                           where item.GroupType == groupType
                           orderby item.LastUpdate descending
                           select item.LastUpdate;

                return list.First();
            }
        }

        public static List<Group> GetByIDs(List<int> ids)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var query = db.Groups.Where(item => ids.Contains(item.ID)).ToList();

                return query;
            }
        }

        public static Group GetByTitle(string title, GroupType groupType)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var group = db.Groups.Where(item => item.GroupType == groupType && (item.Title == title || item.TitleEn == title)).SingleOrDefault();

                return group;
            }
        }

        public static void Delete(int id)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var group = (from item in db.Groups
                             where item.ID == id
                             select item).Single();

                db.Groups.Remove(group);

                db.SaveChanges();
            }
        }

        public static void Insert(Group group)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                db.Groups.Add(group);

                db.SaveChanges();
            }
        }

        public static void Update(Group group)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var orgGroup = db.Groups.Where(item => item.ID == group.ID).Single();

                orgGroup.Title = group.Title;
                orgGroup.TitleEn = group.TitleEn;

                orgGroup.Perfix = group.Perfix;
                orgGroup.Perfix_En = group.Perfix_En;

                orgGroup.ParentID = group.ParentID;
                orgGroup.GroupType = group.GroupType;
                orgGroup.Text = group.Text;

                orgGroup.Image = group.Image;
                orgGroup.ButtonImage = group.ButtonImage;

                orgGroup.LastUpdate = group.LastUpdate;

                db.SaveChanges();
            }
        }
    }
}
