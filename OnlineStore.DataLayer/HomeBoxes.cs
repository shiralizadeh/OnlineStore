using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OnlineStore.EntityFramework;
using System.Collections;
using System.ComponentModel.DataAnnotations;
using OnlineStore.Models.Public;
using OnlineStore.Models.Enums;

namespace OnlineStore.DataLayer
{
    public class HomeBox : EntityBase
    {
        [Display(Name = "عنوان جعبه")]
        public string Title { get; set; }

        [Display(Name = "ترتیب نمایش")]
        public int OrderID { get; set; }

        [Display(Name = "محل قرار گیری")]
        public HomeBoxType HomeBoxType { get; set; }

        [Display(Name = "قابل نمایش")]
        public bool IsVisible { get; set; }
    }

    public static class HomeBoxes
    {
        private static IQueryable<HomeBox> _cachedHomeBoxes
        {
            get
            {
                using (var db = OnlineStoreDbContext.Entity)
                    return db.HomeBoxes.ToCacheableList().AsQueryable();
            }
        }

        public static IList Get(int pageIndex, int pageSize, string pageOrder)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var query = from item in _cachedHomeBoxes
                            select new
                            {
                                item.ID,
                                item.Title,
                                IsVisible = item.IsVisible,
                                LastUpdate = item.LastUpdate,
                                OrderID = item.OrderID,
                                Url = item.HomeBoxType == HomeBoxType.Group ?
                                "/Admin/HomeBoxItems/index?HomeBoxID=" + item.ID :
                                "/Admin/HomeBoxProducts/index?HomeBoxID=" + item.ID
                            };

                if (!string.IsNullOrWhiteSpace(pageOrder))
                    query = query.OrderBy(pageOrder);
                else
                    query = query.OrderBy(item => item.OrderID);

                query = query.Skip(pageIndex * pageSize).Take(pageSize);

                return query.ToList();
            }
        }

        public static int Count()
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var query = from item in _cachedHomeBoxes
                            select item;

                return query.Count();
            }
        }

        public static HomeBox GetByID(int id)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var homeBox = _cachedHomeBoxes.Where(item => item.ID == id).Single();

                return homeBox;
            }
        }

        public static List<ViewHomeBox> Get(HomeBoxType? homeBoxType = null)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                DateTime now = DateTime.Now;

                var boxes = from box in _cachedHomeBoxes
                            where box.IsVisible
                            orderby box.OrderID
                            select new ViewHomeBox
                            {
                                ID = box.ID,
                                Title = box.Title,
                                HomeBoxType = box.HomeBoxType,
                                OrderID = box.OrderID,
                            };

                if (homeBoxType != null)
                    boxes = boxes.Where(box => box.HomeBoxType == homeBoxType);

                return boxes.ToList();
            }
        }

        public static void Delete(int id)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var homeBox = (from item in db.HomeBoxes
                               where item.ID == id
                               select item).Single();

                db.HomeBoxes.Remove(homeBox);

                db.SaveChanges();
            }
        }

        public static void Insert(HomeBox homeBox)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                db.HomeBoxes.Add(homeBox);

                db.SaveChanges();
            }
        }

        public static void Update(HomeBox homeBox)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var orghomeBox = db.HomeBoxes.Where(item => item.ID == homeBox.ID).Single();

                orghomeBox.Title = homeBox.Title;
                orghomeBox.IsVisible = homeBox.IsVisible;
                orghomeBox.HomeBoxType = homeBox.HomeBoxType;
                orghomeBox.OrderID = homeBox.OrderID;
                orghomeBox.LastUpdate = homeBox.LastUpdate;

                db.SaveChanges();
            }
        }

    }

}
