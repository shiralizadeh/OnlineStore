using OnlineStore.Models.Public;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineStore.DataLayer
{
    public class ProductGroup : EntityBase
    {
        [ForeignKey("Product")]
        public int ProductID { get; set; }
        public Product Product { get; set; }

        [ForeignKey("Group")]
        public int GroupID { get; set; }
        public Group Group { get; set; }
    }

    public static class ProductGroups
    {
        public static List<ProductGroup> GetByProductID(int productID)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var query = from item in db.ProductGroups
                            where item.ProductID == productID
                            select item;

                return query.ToList();
            }
        }

        public static List<ViewProductGroup> GetViewByProductID(int productID)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var query = from item in db.ProductGroups
                            where item.ProductID == productID
                            select new ViewProductGroup
                            {
                                ID = item.ID,
                                GroupID = item.GroupID,
                                Title = item.Group.Title,
                                TitleEn = item.Group.TitleEn,
                            };

                return query.ToList();
            }
        }

        public static int Count()
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var query = from item in db.ProductGroups
                            select item;

                return query.Count();
            }
        }

        public static ProductGroup GetByID(int id)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var productGroup = db.ProductGroups.Where(item => item.ID == id).Single();

                return productGroup;
            }
        }

        public static void Delete(int id)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var productGroup = (from item in db.ProductGroups
                                    where item.ID == id
                                    select item).Single();

                db.ProductGroups.Remove(productGroup);

                db.SaveChanges();
            }
        }

        public static void Insert(ProductGroup productGroup)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                if (!db.Groups.Any(item => item.ParentID == productGroup.GroupID))
                {
                    db.ProductGroups.Add(productGroup);

                    db.SaveChanges();
                }
            }
        }

        public static void Update(ProductGroup productGroup)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var orgProductGroup = db.ProductGroups.Where(item => item.ID == productGroup.ID).Single();

                orgProductGroup.ProductID = productGroup.ProductID;
                orgProductGroup.GroupID = productGroup.GroupID;
                orgProductGroup.LastUpdate = productGroup.LastUpdate;

                db.SaveChanges();
            }
        }

        public static List<int> GetGroupsParents(int productID)
        {
            List<int> all = new List<int>();

            using (var db = OnlineStoreDbContext.Entity)
            {
                var query = from item in db.ProductGroups
                            where item.ProductID == productID
                            select item.GroupID;

                List<int> groups = query.ToList();

                all.AddRange(groups);

                foreach (var item in groups)
                {
                    int? groupID = item;

                    while (groupID.HasValue)
                    {
                        int? p = db.Groups.First(s => s.ID == groupID).ParentID;

                        groupID = p.Value;

                        if (p.HasValue)
                        {
                            all.Add(p.Value);
                        }
                    }
                }
            }

            return all;
        }

        public static List<ViewProductGroup> GetParents(int productID)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var query = from item in db.ProductGroups
                            where item.ProductID == productID
                            select item.GroupID;

                var result = new List<ViewProductGroup>();

                foreach (var item in query.ToList())
                {
                    var grp = db.Groups.First(s => s.ID == item);

                    result.Add(new ViewProductGroup()
                    {
                        GroupID = grp.ID,
                        Title = grp.Title,
                        TitleEn = grp.TitleEn,
                        ParentID = grp.ParentID
                    });

                    while (grp.ParentID.HasValue)
                    {
                        var p = db.Groups.First(s => s.ID == grp.ParentID.Value);

                        result.Add(new ViewProductGroup { GroupID = p.ID, Title = p.Title, TitleEn = p.TitleEn });

                        grp = p;
                    }
                }
                return result;
            }
        }
    }
}
