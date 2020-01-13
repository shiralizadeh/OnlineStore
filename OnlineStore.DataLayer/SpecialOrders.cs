using OnlineStore.Identity;
using OnlineStore.Models.Admin;
using OnlineStore.Models.Enums;
using OnlineStore.EntityFramework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

namespace OnlineStore.DataLayer
{
    public class SpecialOrder : EntityBase
    {
        [MaxLength(128)]
        public string UserID { get; set; }

        [Display(Name = "سفارش")]
        public string Description { get; set; }

        [Display(Name = "وضعیت")]
        public SpecialOrderStatus SpecialOrderStatus { get; set; }
    }

    public static class SpecialOrders
    {
        public static IList Get(int pageIndex, int pageSize, string pageOrder)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var query = from item in db.SpecialOrders
                            select new
                            {
                                item.ID,
                                item.UserID,
                                item.Description,
                                item.SpecialOrderStatus,
                                item.LastUpdate,
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
                var query = from item in db.SpecialOrders
                            select item;

                return query.Count();
            }
        }

        public static SpecialOrder GetByID(int id)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var specialOrder = db.SpecialOrders.Where(item => item.ID == id).Single();

                return specialOrder;
            }
        }

        public static void Delete(int id)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var specialOrder = (from item in db.SpecialOrders
                                where item.ID == id
                                select item).Single();

                db.SpecialOrders.Remove(specialOrder);

                db.SaveChanges();
            }
        }

        public static void Insert(SpecialOrder specialOrder)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                db.SpecialOrders.Add(specialOrder);

                db.SaveChanges();
            }
        }

        public static void Update(SpecialOrder specialOrder)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var orgSpecialOrder = db.SpecialOrders.Where(item => item.ID == specialOrder.ID).Single();

                orgSpecialOrder.Description = specialOrder.Description;
                orgSpecialOrder.SpecialOrderStatus = specialOrder.SpecialOrderStatus;
                orgSpecialOrder.LastUpdate = specialOrder.LastUpdate;

                db.SaveChanges();
            }
        }
    }
}
