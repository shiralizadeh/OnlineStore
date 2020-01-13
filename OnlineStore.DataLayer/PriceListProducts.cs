using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OnlineStore.EntityFramework;
using OnlineStore.Models.Enums;

namespace OnlineStore.DataLayer
{
    public class PriceListProduct : EntityBase
    {
        [Display(Name = "عنوان")]
        [MaxLength(100)]
        public string Title { get; set; }

        [Display(Name = "زیر عنوان")]
        [MaxLength(100)]
        public string SubTitle { get; set; }

        [Display(Name = "محصول")]
        [ForeignKey("Product")]
        public int? ProductID { get; set; }

        [Display(Name = "محصول")]
        public Product Product { get; set; }

        [Display(Name = "برند")]
        [ForeignKey("PriceListSection")]
        public int PriceListSectionID { get; set; }

        [Display(Name = "گروه")]
        public PriceListSection PriceListSection { get; set; }

        [Display(Name = "قیمت")]
        public int Price { get; set; }

        [Display(Name = "گارانتی")]
        [MaxLength(100)]
        public string Guarantee { get; set; }

        [Display(Name = "موجود")]
        public bool IsAvailable { get; set; }

        [Display(Name = "ترتیب نمایش")]
        public int OrderID { get; set; }

        [Display(Name = "استایل")]
        public PriceListProductType PriceListProductType { get; set; }
    }

    public static class PriceListProducts
    {
        public static IList Get(int pageIndex, int pageSize, string pageOrder, string title)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var query = from item in db.PriceListProducts
                            select new
                            {
                                item.ID,
                                item.Title,
                                item.SubTitle,
                                item.Price,
                                item.IsAvailable,
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
                var query = from item in db.PriceListProducts
                            select item;

                if (!string.IsNullOrWhiteSpace(title))
                    query = query.Where(item => item.Title.Contains(title));

                return query.Count();
            }
        }

        public static PriceListProduct GetByID(int id)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var priceListProduct = db.PriceListProducts.Where(item => item.ID == id).Single();

                return priceListProduct;
            }
        }

        public static void Delete(int id)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var priceListProduct = (from item in db.PriceListProducts
                                        where item.ID == id
                                        select item).Single();

                db.PriceListProducts.Remove(priceListProduct);

                db.SaveChanges();
            }
        }

        public static void Insert(PriceListProduct priceListProduct)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                db.PriceListProducts.Add(priceListProduct);

                db.SaveChanges();
            }
        }

        public static void Update(PriceListProduct priceListProduct)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var orgPriceListProduct = db.PriceListProducts.Where(item => item.ID == priceListProduct.ID).Single();

                orgPriceListProduct.Title = priceListProduct.Title;
                orgPriceListProduct.SubTitle = priceListProduct.SubTitle;
                orgPriceListProduct.ProductID = priceListProduct.ProductID;
                orgPriceListProduct.Price = priceListProduct.Price;
                orgPriceListProduct.Guarantee = priceListProduct.Guarantee;
                orgPriceListProduct.IsAvailable = priceListProduct.IsAvailable;
                orgPriceListProduct.OrderID = priceListProduct.OrderID;
                orgPriceListProduct.PriceListProductType = priceListProduct.PriceListProductType;
                orgPriceListProduct.LastUpdate = priceListProduct.LastUpdate;

                db.SaveChanges();
            }
        }

        public static void UpdateOrderID(int id, int orderID)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var orgPriceListProduct = db.PriceListProducts.Where(item => item.ID == id).Single();

                orgPriceListProduct.OrderID = orderID;
                orgPriceListProduct.LastUpdate = DateTime.Now;

                db.SaveChanges();
            }
        }

        public static void SaveChanges(int id, string value, PriceListFieldName priceListFieldName, out string oldValue)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                oldValue = String.Empty;

                var orgPriceListProduct = db.PriceListProducts.Where(item => item.ID == id).Single();

                switch (priceListFieldName)
                {
                    case PriceListFieldName.Title:
                        oldValue = orgPriceListProduct.Title;
                        orgPriceListProduct.Title = value;
                        break;
                    case PriceListFieldName.SubTitle:
                        oldValue = orgPriceListProduct.SubTitle;
                        orgPriceListProduct.SubTitle = value;
                        break;
                    case PriceListFieldName.Price:
                        oldValue = orgPriceListProduct.Price.ToString();
                        orgPriceListProduct.Price = Int32.Parse(value);
                        break;
                    case PriceListFieldName.IsAvailable:
                        oldValue = orgPriceListProduct.IsAvailable.ToString();
                        orgPriceListProduct.IsAvailable = Boolean.Parse(value);
                        break;
                    default:
                        break;
                }

                orgPriceListProduct.LastUpdate = DateTime.Now;

                db.SaveChanges();

            }
        }

    }
}
