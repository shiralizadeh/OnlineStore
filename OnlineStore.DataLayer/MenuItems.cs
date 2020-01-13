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

namespace OnlineStore.DataLayer
{
    public class MenuItem : EntityBase
    {
        [ForeignKey("Parent")]
        [Display(Name = "پدر")]
        public int? ParentID { get; set; }

        [Display(Name = "پدر")]
        public MenuItem Parent { get; set; }

        [Display(Name = "عنوان")]
        public string Title { get; set; }

        [Display(Name = "محتوا")]
        public string Content { get; set; }

        [Display(Name = "لینک")]
        [MaxLength(500)]
        public string Link { get; set; }

        [Display(Name = "نوع آیتم")]
        public MenuItemType MenuItemType { get; set; }

        [Display(Name = "قابل نمایش")]
        public bool IsVisible { get; set; }

        [Display(Name = "نمایش در بردکرامب")]
        public bool ShowInBreadCrumb { get; set; }

        [Display(Name = "کلاس آیکن")]
        [MaxLength(50)]
        public string IconClass { get; set; }

        [Display(Name = "ترتیب نمایش")]
        public int OrderID { get; set; }
    }

    public static class MenuItems
    {
        public static IList Get(int pageIndex, int pageSize, string pageOrder, MenuItemType? menuItemType, string title, List<int> childs, int groupID)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var query = from item in db.MenuItems
                            orderby item.OrderID
                            select new
                            {
                                item.ID,
                                item.Title,
                                item.MenuItemType,
                                ParentTitle = item.ParentID.HasValue ? item.Parent.Title : String.Empty,
                                item.OrderID,
                                item.IsVisible,
                                item.LastUpdate
                            };

                if (groupID != -1)
                {
                    query = query.Where(item => childs.Contains(item.ID));
                }

                if (menuItemType != null)
                    query = query.Where(item => item.MenuItemType == menuItemType);

                if (!string.IsNullOrWhiteSpace(title))
                    query = query.Where(item => item.Title.Contains(title));

                if (!string.IsNullOrWhiteSpace(pageOrder))
                    query = query.OrderBy(pageOrder);

                query = query.Skip(pageIndex * pageSize).Take(pageSize);

                return query.ToList();
            }
        }

        public static int Count(MenuItemType? menuItemType, string title, List<int> childs, int groupID)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var query = from item in db.MenuItems
                            select item;

                if (groupID != -1)
                {
                    query = query.Where(item => childs.Contains(item.ID));
                }

                if (menuItemType != null)
                    query = query.Where(item => item.MenuItemType == menuItemType);

                if (!string.IsNullOrWhiteSpace(title))
                    query = query.Where(item => item.Title.Contains(title));

                return query.Count();
            }
        }

        public static List<MenuItem> GetByMenuItemType(MenuItemType menuItemType)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var query = from item in db.MenuItems
                            where item.MenuItemType == menuItemType
                            select item;

                return query.ToList();
            }
        }

        public static void Insert(MenuItem item)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                db.MenuItems.Add(item);

                db.SaveChanges();
            }
        }

        public static void Update(MenuItem menuItem)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var orgItem = db.MenuItems.Where(item => item.ID == menuItem.ID).Single();

                orgItem.Title = menuItem.Title;
                orgItem.ParentID = menuItem.ParentID;
                orgItem.Content = menuItem.Content;
                orgItem.Link = menuItem.Link;
                orgItem.IsVisible = menuItem.IsVisible;
                orgItem.MenuItemType = menuItem.MenuItemType;
                orgItem.ShowInBreadCrumb = menuItem.ShowInBreadCrumb;
                orgItem.IconClass = menuItem.IconClass;
                orgItem.OrderID = menuItem.OrderID;
                orgItem.LastUpdate = menuItem.LastUpdate;

                db.SaveChanges();
            }
        }

        public static void Delete(int id)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var menuItem = (from item in db.MenuItems
                                where item.ID == id
                                select item).Single();

                db.MenuItems.Remove(menuItem);

                db.SaveChanges();
            }
        }

        public static List<MenuItem> GetRoot(bool? isVisible = null)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var query = from item in db.MenuItems
                            where item.ParentID == null
                            select item;

                if (isVisible.HasValue)
                    query = query.Where(item => item.IsVisible == isVisible);

                query = query.OrderBy(item => item.OrderID);

                return query.ToList();
            }
        }

        public static List<MenuItem> GetAll(bool? isVisible = null)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var query = from item in db.MenuItems
                            select item;

                if (isVisible.HasValue)
                    query = query.Where(item => item.IsVisible == isVisible);

                return query.ToList();
            }
        }

        public static DateTime LastestDate()
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var query = from item in db.MenuItems
                            where item.MenuItemType == MenuItemType.Page
                            orderby item.LastUpdate descending
                            select item.LastUpdate;

                return query.First();
            }
        }

        public static List<MenuItem> GetByParentID(int? parentID, bool? isVisible = null)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var query = from item in db.MenuItems
                            where item.ParentID == parentID
                            select item;

                if (isVisible.HasValue)
                    query = query.Where(item => item.IsVisible == isVisible.Value);

                return query.ToList();
            }
        }

        public static MenuItem GetByID(int id)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var menuItem = db.MenuItems.Where(item => item.ID == id).Single();

                return menuItem;
            }
        }

        public static List<MenuItem> GetParentsRecursive(MenuItem menuItem)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var result = new List<MenuItem>();

                MenuItem parent = null;

                if (menuItem.ParentID.HasValue)
                {
                    parent = db.MenuItems.Where(item => item.ID == menuItem.ParentID.Value).Single();
                }

                while (parent != null)
                {
                    if (parent.ShowInBreadCrumb)
                    {
                        result.Add(parent);
                    }

                    parent = parent.Parent;
                }

                result.Add(menuItem);

                return result;
            }
        }

        public static List<MenuItem> GetChildsRecursive(int groupID)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var result = new List<MenuItem>();

                var query = from item in db.MenuItems
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

        private static void GetRecursive(List<MenuItem> result, int groupID)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var query = from item in db.MenuItems
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

        public static bool HasChild(int groupID)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                return db.MenuItems.Any(item => item.ParentID == groupID);
            }
        }


    }
}
