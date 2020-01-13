using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OnlineStore.EntityFramework;
using OnlineStore.Models.Admin;
using OnlineStore.Models.Enums;

namespace OnlineStore.DataLayer
{
    public class PriceListSection : EntityBase
    {
        [Display(Name = "عنوان گروه")]
        [MaxLength(50)]
        public string Title { get; set; }

        [Display(Name = "تصویر")]
        [MaxLength(200)]
        public string Image { get; set; }

        [Display(Name = "ترتیب نمایش")]
        public int OrderID { get; set; }

        [Display(Name = "ستون نمایش")]
        public int ColumnID { get; set; }

        [Display(Name = "نوع گروه")]
        public PriceListSectionType PriceListSectionType { get; set; }
    }

    public static class PriceListSections
    {
        public static IList Get(int pageIndex, int pageSize, string pageOrder, string title)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var query = from item in db.PriceListSections
                            select new
                            {
                                item.ID,
                                item.Title,
                                item.OrderID,
                                item.LastUpdate
                            };

                if (!string.IsNullOrWhiteSpace(title))
                    query = query.Where(item => item.Title.Contains(title));

                if (!string.IsNullOrWhiteSpace(pageOrder))
                    query = query.OrderBy(pageOrder);

                query = query.Skip(pageIndex * pageSize).Take(pageSize);

                var result = query.ToList();

                return result;
            }
        }

        public static int Count(string title)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var query = from item in db.PriceListSections
                            select item;

                if (!string.IsNullOrWhiteSpace(title))
                    query = query.Where(item => item.Title.Contains(title));

                return query.Count();
            }
        }

        public static PriceListSection GetByID(int id)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var priceListSection = db.PriceListSections.Where(item => item.ID == id).Single();

                return priceListSection;
            }
        }

        public static void Delete(int id)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var priceListSection = (from item in db.PriceListSections
                                        where item.ID == id
                                        select item).Single();

                db.PriceListSections.Remove(priceListSection);

                db.SaveChanges();
            }
        }

        public static void Insert(PriceListSection priceListSection)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                db.PriceListSections.Add(priceListSection);

                db.SaveChanges();
            }
        }

        public static void Update(PriceListSection priceListSection)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var orgPriceListSection = db.PriceListSections.Where(item => item.ID == priceListSection.ID).Single();

                orgPriceListSection.Title = priceListSection.Title;
                orgPriceListSection.Image = priceListSection.Image;
                orgPriceListSection.PriceListSectionType = priceListSection.PriceListSectionType;
                orgPriceListSection.OrderID = priceListSection.OrderID;
                orgPriceListSection.ColumnID = priceListSection.ColumnID;
                orgPriceListSection.LastUpdate = priceListSection.LastUpdate;

                db.SaveChanges();
            }
        }

        public static List<PriceListSection> Get()
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var query = from item in db.PriceListSections
                            select item;

                var result = query.ToList();

                return result;
            }
        }

        public static List<ViewPriceList> GetWithProducts(PriceListSectionType priceListSectionType)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var query = from item in db.PriceListSections
                            where item.PriceListSectionType == priceListSectionType
                            orderby item.OrderID
                            select new ViewPriceList
                            {
                                SectionID = item.ID,
                                SectionTitle = item.Title,
                                SectionImage = item.Image,
                                ColumnID = item.ColumnID,
                                PriceListProducts = (from product in db.PriceListProducts
                                                     where product.PriceListSectionID == item.ID
                                                     orderby product.OrderID
                                                     select new ViewPriceListProduct
                                                     {
                                                         ID = product.ID,
                                                         Title = product.Title,
                                                         SubTitle = product.SubTitle,
                                                         Price = product.Price,
                                                         Guarantee = product.Guarantee,
                                                         IsAvailable = product.IsAvailable,
                                                         PriceListProductType = product.PriceListProductType

                                                     }).ToList()
                            };

                var result = query.ToList();

                return result;
            }
        }

        public static void SortSections(int id, int orderID, int columnID)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var orgPriceListSection = db.PriceListSections.Where(item => item.ID == id).Single();

                orgPriceListSection.OrderID = orderID;
                orgPriceListSection.ColumnID = columnID;
                orgPriceListSection.LastUpdate = DateTime.Now;

                db.SaveChanges();
            }
        }
    }

}
