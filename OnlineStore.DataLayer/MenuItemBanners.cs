using OnlineStore.Models.Admin;
using OnlineStore.Models.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineStore.DataLayer
{
    public class MenuItemBanner : EntityBase
    {
        [MaxLength(200)]
        public string Filename { get; set; }

        public MenuItemBannerType MenuItemBannerType { get; set; }

        [MaxLength(300)]
        public string Link { get; set; }

        [ForeignKey("MenuItem")]
        public int MenuItemID { get; set; }
        public MenuItem MenuItem { get; set; }

        public Guid Key { get; set; }

        [Display(Name = "تعداد کلیک")]
        public int ClickCount { get; set; }
    }

    public static class MenuItemBanners
    {
        public static List<EditMenuItemBanner> GetByMenuItemID(int menuItemID)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var query = from item in db.MenuItemBanners
                            where item.MenuItemID == menuItemID
                            select new EditMenuItemBanner
                            {
                                ID = item.ID,
                                Filename = item.Filename,
                                MenuItemBannerType = item.MenuItemBannerType,
                                Link = item.Link,
                                Key = item.Key
                            };

                return query.ToList();
            }
        }

        public static MenuItemBanner GetByID(int id)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var menuItemBanner = db.MenuItemBanners.Where(item => item.ID == id).Single();

                return menuItemBanner;
            }
        }

        public static void Delete(int id)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var menuItemBanner = (from item in db.MenuItemBanners
                                      where item.ID == id
                                      select item).Single();

                db.MenuItemBanners.Remove(menuItemBanner);

                db.SaveChanges();
            }
        }

        public static void Insert(MenuItemBanner menuItemBanner)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                db.MenuItemBanners.Add(menuItemBanner);

                db.SaveChanges();
            }
        }

        public static void UpdateMenuItemBannerType(int id, MenuItemBannerType menuItemBannerType)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var orgMenuItemBanner = db.MenuItemBanners.Where(item => item.ID == id).Single();

                orgMenuItemBanner.MenuItemBannerType = menuItemBannerType;

                db.SaveChanges();
            }
        }

        public static MenuItemBanner GetByGuid(Guid guid)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var banner = db.MenuItemBanners.Where(item => item.Key == guid).Single();

                return banner;
            }
        }

        public static void AddClick(Guid key)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var orgBanner = db.MenuItemBanners.Where(item => item.Key == key).Single();

                orgBanner.ClickCount++;

                db.SaveChanges();
            }
        }


    }

}
