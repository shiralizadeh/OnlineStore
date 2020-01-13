using OnlineStore.Models.Admin;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineStore.DataLayer
{
    public class ProductKeyword : EntityBase
    {
        [Display(Name = "محصول")]
        [ForeignKey("Product")]
        public int ProductID { get; set; }
        public Product Product { get; set; }

        [Display(Name = "کلیدواژه")]
        [ForeignKey("Keyword")]
        public int KeywordID { get; set; }
        public Keyword Keyword { get; set; }
    }

    public class ProductKeywords
    {
        public static List<EditProductKeyword> GetByProductID(int productID)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var query = from item in db.ProductKeywords
                            where item.ProductID == productID
                            select new EditProductKeyword
                            {
                                ID = item.ID,
                                KeywordID = item.KeywordID,
                                Title = item.Keyword.Title,
                            };

                return query.ToList();
            }
        }

        public static List<JsonKeywordProduct> GetByKeywordID(int keywordID)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var query = from item in db.ProductKeywords
                            where item.KeywordID == keywordID
                            select new JsonKeywordProduct
                            {
                                ID = item.ID,
                                ProductID = item.ProductID,
                                KeywordID = item.KeywordID,
                                Title = item.Product.Title

                            };

                return query.ToList();
            }
        }

        public static ProductKeyword GetByKeywordID_ProductID(int keywordID, int productID)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var query = from item in db.ProductKeywords
                            where item.KeywordID == keywordID && item.ProductID == productID
                            select item;

                return query.FirstOrDefault();
            }
        }

        public static void Insert(ProductKeyword productKeyword)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                db.ProductKeywords.Add(productKeyword);

                db.SaveChanges();
            }
        }

        public static void Insert(List<ProductKeyword> productKeywords)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                db.ProductKeywords.AddRange(productKeywords);

                db.SaveChanges();
            }
        }

        public static void Delete(int id)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var productKeyword = (from item in db.ProductKeywords
                                      where item.ID == id
                                      select item).Single();

                db.ProductKeywords.Remove(productKeyword);

                db.SaveChanges();
            }
        }

        public static void DeleteByKeywordID(int keywordID)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var productKeywords = (from item in db.ProductKeywords
                                       where item.KeywordID == keywordID
                                       select item).ToList();

                db.ProductKeywords.RemoveRange(productKeywords);

                db.SaveChanges();
            }
        }

        public static void Update(EditProductKeyword productKeyword)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var orgProductKeyword = db.ProductKeywords.Where(item => item.ID == productKeyword.ID).Single();

                orgProductKeyword.KeywordID = productKeyword.KeywordID;

                db.SaveChanges();
            }
        }
    }
}
