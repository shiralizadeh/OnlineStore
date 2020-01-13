using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OnlineStore.EntityFramework;
using OnlineStore.Models.Public;
using OnlineStore.Providers;

namespace OnlineStore.DataLayer
{
    public class AttributeOption : EntityBase
    {
        [ForeignKey("Attribute")]
        public int AttributeID { get; set; }
        public Attribute Attribute { get; set; }

        [MaxLength(250)]
        [Display(Name = "عنوان")]
        public string Title { get; set; }

        [Display(Name = "ترتیب نمایش")]
        public int OrderID { get; set; }
    }

    public static class AttributeOptions
    {
        public static List<AttributeOption> GetByAttributeID(int attributeID)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var query = from item in db.AttributeOptions
                            where item.AttributeID == attributeID
                            select item;

                return query.ToList();
            }
        }

        public static int Count()
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var query = from item in db.AttributeOptions
                            select item;

                return query.Count();
            }
        }

        public static AttributeOption GetByID(int id)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var attributeOption = db.AttributeOptions.Where(item => item.ID == id).Single();

                return attributeOption;
            }
        }

        public static List<ViewAttributeOption> GetByIDs(List<int> list)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var query = from item in db.AttributeOptions
                            where list.Contains(item.ID)
                            orderby item.OrderID
                            select new ViewAttributeOption
                            {
                                ID = item.ID,
                                AttributeID = item.AttributeID,
                                Title = item.Title,
                            };

                return query.ToList();
            }
        }

        public static void Delete(int id)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var attributeOption = (from item in db.AttributeOptions
                                       where item.ID == id
                                       select item).Single();

                db.AttributeOptions.Remove(attributeOption);

                db.SaveChanges();
            }
        }

        public static void Insert(AttributeOption attributeOption)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                db.AttributeOptions.Add(attributeOption);

                db.SaveChanges();
            }
        }

        public static void Update(AttributeOption attributeOption)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var orgAttributeOption = db.AttributeOptions.Where(item => item.ID == attributeOption.ID).Single();

                orgAttributeOption.Title = attributeOption.Title;
                orgAttributeOption.OrderID = attributeOption.OrderID;
                orgAttributeOption.LastUpdate = attributeOption.LastUpdate;

                db.SaveChanges();
            }
        }
    }
}
