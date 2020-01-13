using OnlineStore.Models.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OnlineStore.EntityFramework;
using System.Collections;
using OnlineStore.Providers;
using OnlineStore.Models;
using OnlineStore.Models.Public;
using System.Drawing;
using OnlineStore.Models.Admin;
using System.Data.SqlClient;
using System.Data.Linq.SqlClient;

namespace OnlineStore.DataLayer
{
    public class Product : EntityBase
    {
        [Display(Name = "تولید کننده")]
        [ForeignKey("Producer")]
        public int ProducerID { get; set; }
        public Producer Producer { get; set; }

        [Display(Name = "گروه")]
        [ForeignKey("Group")]
        public int? GroupID { get; set; }
        public Group Group { get; set; }

        [Display(Name = "کاربر")]
        [MaxLength(128)]
        public string UserID { get; set; }

        [Display(Name = "کاربر ویرایش کننده")]
        [MaxLength(128)]
        public string EditingUserID { get; set; }

        [Display(Name = "عنوان")]
        [MaxLength(100)]
        public string Title { get; set; }

        [Display(Name = "عنوان انگلیسی")]
        [MaxLength(100)]
        public string Title_En { get; set; }

        [NotMapped]
        public string DisplayTitle
        {
            get
            {
                if (DisplayTitleType == DisplayTitleType.Title_Fa && !String.IsNullOrWhiteSpace(this.Title))
                    return Title;
                else if (DisplayTitleType == DisplayTitleType.Title_En && !String.IsNullOrWhiteSpace(this.Title_En))
                    return Title_En;
                else
                {
                    if (!String.IsNullOrWhiteSpace(Title))
                        return Title;
                    else if (!String.IsNullOrWhiteSpace(Title_En))
                        return Title_En;
                    else
                        return "نا مشخص";
                }
            }
        }

        [NotMapped]
        public string UrlPerfix
        {
            get
            {
                return Title_En + "-" + Title;
            }
        }

        [Display(Name = "عنوان نمایشی")]
        public DisplayTitleType DisplayTitleType { get; set; }

        [Display(Name = "تاریخ انتشار")]
        public DateTime PublishDate { get; set; }

        [Display(Name = "تاریخ انتشار")]
        [NotMapped]
        public string PersianPublishDate
        {
            get
            {
                if (PublishDate == new DateTime())
                    return "";

                return Utilities.ToPersianDate(PublishDate);
            }
            set
            {
                if (String.IsNullOrWhiteSpace(value))
                    PublishDate = DateTime.Now;

                PublishDate = Utilities.ToEnglishDate(value);
            }
        }

        [Display(Name = "عدم موجودی")]
        public bool IsUnavailable { get; set; }

        [Display(Name = "عدم نمایش")]
        public bool IsInVisible { get; set; }

        [Display(Name = "ارسال رایگان")]
        public bool IsFreeDelivery { get; set; }

        [Display(Name = "دارای انواع کالا")]
        public bool HasVarients { get; set; }

        [Display(Name = "وضعیت")]
        public ProductStatus ProductStatus { get; set; }

        [Display(Name = "پیام رد")]
        public string RejectMessage { get; set; }

        [Display(Name = "خلاصه")]
        public string Summary { get; set; }

        [Display(Name = "نقد و بررسی تخصصی")]
        public string Text { get; set; }

        [Display(Name = "تعداد بازدید")]
        public int VisitCount { get; set; }

        [Display(Name = "وضعیت قیمت")]
        public PriceStatus PriceStatus { get; set; }

        [Display(Name = "کد قیمت")]
        [MaxLength(50)]
        public string PriceCode { get; set; }

        [Display(Name = "تاریخ ایجاد")]
        public DateTime? CreatedDate { get; set; }

        [Display(Name = "ترتیب نمایش")]
        public int OrderID { get; set; }

        [Display(Name = "امتیاز محصول")]
        public float ProductScore { get; set; }

        public ICollection<ProductGroup> ProductGroups { get; set; }
        public ICollection<ProductPrice> ProductPrices { get; set; }

        public Product Clone(string userID = null)
        {
            var product = new Product();

            product.ProducerID = this.ProducerID;
            product.GroupID = this.GroupID;
            product.UserID = (String.IsNullOrWhiteSpace(userID) ? this.UserID : userID);
            product.Title = "کپی - " + this.Title;
            product.Title_En = "Copy - " + this.Title_En;
            product.DisplayTitleType = this.DisplayTitleType;
            product.PublishDate = this.PublishDate;
            product.IsUnavailable = this.IsUnavailable;
            product.IsInVisible = this.IsInVisible;
            product.IsFreeDelivery = this.IsFreeDelivery;
            product.HasVarients = this.HasVarients;
            product.ProductStatus = ProductStatus.NotChecked;
            product.RejectMessage = this.RejectMessage;
            product.Summary = this.Summary;
            product.Text = this.Text;
            product.PriceStatus = this.PriceStatus;

            return product;
        }

    }

    public static class Products
    {
        public static IList Get(int pageIndex,
                                int pageSize,
                                string pageOrder,
                                List<int> groups,
                                string title,
                                int keyword,
                                int producer,
                                int? price,
                                DateTime? fromDate,
                                DateTime? toDate,
                                bool? isUnavailable,
                                bool? isInVisible,
                                bool? noPrice,
                                ProductStatus? productStatus,
                                string userID
                                )
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var query = from item in db.Products
                            select new
                            {
                                item.ID,
                                item.Title,
                                item.Title_En,
                                ProducerID = item.ProducerID,
                                ProducerTitle = item.Producer.Title,
                                Price = (from p in item.ProductPrices
                                         where p.ProductID == item.ID && p.PriceType == PriceType.Sell
                                         orderby p.LastUpdate descending
                                         select p.Price).FirstOrDefault(),
                                item.PublishDate,
                                item.IsUnavailable,
                                item.IsInVisible,
                                item.VisitCount,
                                item.LastUpdate,
                                item.ProductStatus,
                                item.UserID,
                                item.EditingUserID,
                                item.HasVarients,
                                item.ProductScore
                            };

                if (groups != null && groups.Count > 0)
                    query = query.Where(item => db.ProductGroups.Any(group => group.ProductID == item.ID && groups.Contains(group.GroupID)));

                if (!string.IsNullOrWhiteSpace(title))
                    query = query.Where(item => item.Title.Contains(title) || item.Title_En.Contains(title));

                if (keyword != -1)
                {
                    query = query.Where(item => db.ProductKeywords.Any(key => key.ProductID == item.ID && key.KeywordID == keyword));
                }

                if (producer != -1)
                    query = query.Where(item => item.ProducerID == producer);

                if (price.HasValue)
                    query = query.Where(item => item.Price == price);

                if (fromDate.HasValue)
                    query = query.Where(item => item.PublishDate >= fromDate);

                if (toDate.HasValue)
                    query = query.Where(item => item.PublishDate <= toDate);

                if (isUnavailable.HasValue)
                    query = query.Where(item => item.IsUnavailable == isUnavailable);

                if (isInVisible.HasValue)
                    query = query.Where(item => item.IsInVisible == isInVisible);

                if (productStatus.HasValue)
                    query = query.Where(item => item.ProductStatus == productStatus);

                if (!String.IsNullOrWhiteSpace(userID))
                    query = query.Where(item => item.UserID == userID || item.EditingUserID == userID);

                if (noPrice.HasValue && noPrice.Value)
                {
                    query = from item in query
                            where
                            (!item.HasVarients && (!db.ProductPrices.Any(p => p.ProductID == item.ID && p.PriceType == PriceType.Sell && p.Price != 0)))
                            ||
                            (item.HasVarients && (!db.ProductVarients.Any(p => p.ProductID == item.ID)))
                            select item;
                }

                if (!string.IsNullOrWhiteSpace(pageOrder))
                    query = query.OrderBy(pageOrder);

                query = query.Skip(pageIndex * pageSize).Take(pageSize);

                return query.ToList();
            }
        }

        public static List<ProductItem> GetAll()
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var now = DateTime.Now.Date;

                var query = from item in db.Products
                            where item.ProductStatus == ProductStatus.Approved
                            && now >= item.PublishDate
                            && item.IsInVisible == false
                            orderby item.LastUpdate descending
                            select new ProductItem
                            {
                                ID = item.ID,
                                GroupID = item.GroupID,

                                GroupTitle = item.Group.Title,
                                GroupTitle_En = item.Group.TitleEn,

                                Title_Fa = item.Title,
                                Title_En = item.Title_En,

                                ImageFile = (from img in db.ProductImages
                                             where img.ProductID == item.ID && img.ProductImagePlace == ProductImagePlace.Home
                                             select img.Filename).FirstOrDefault(),

                                DisplayTitleType = item.DisplayTitleType,
                                HasVarients = item.HasVarients,
                                IsUnavailable = item.IsUnavailable,
                                LastUpdate = item.LastUpdate,
                                PriceStatus = item.PriceStatus
                            };

                var result = query.ToList();

                foreach (var item in result)
                {
                    item.ImageFile = UrlProvider.GetProductImage(item.ImageFile, null);

                    item.Prices = Products.GetProductPrices(item.ID, item.HasVarients, PriceType.Sell);

                    SetDiscounts(null, item.ID, item.HasVarients, item.Prices);
                }

                return result;
            }
        }

        public static List<ProductItem> GetList(int pageIndex, int pageSize, string pageOrder, out int count, List<int> groupIDs = null, List<string> attributes = null, List<int> producerIDs = null, int? keywordID = null)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                SqlParameter pageIndexParam = new SqlParameter("@pageIndex", pageIndex + 1);
                SqlParameter pageSizeParam = new SqlParameter("@pageSize", pageSize);
                SqlParameter pageOrderParam = new SqlParameter("@pageOrder", pageOrder);
                SqlParameter groupIDsParam = new SqlParameter("@groupIDs", (groupIDs != null && groupIDs.Count > 0 ? "(" + groupIDs.Select(a => a.ToString()).Aggregate((a, b) => a + "," + b) + ")" : ""));
                SqlParameter producerIDsParam = new SqlParameter("@producerIDs", (producerIDs != null && producerIDs.Count > 0 ? "(" + producerIDs.Select(a => a.ToString()).Aggregate((a, b) => a + "," + b) + ")" : ""));
                SqlParameter attributesParam = new SqlParameter("@attributes", (attributes != null && attributes.Count > 0 ? attributes.Aggregate((a, b) => a + "," + b) : ""));
                SqlParameter keywordIDParam = new SqlParameter("@keywordID", !keywordID.HasValue ? "" : keywordID.Value.ToString());
                SqlParameter countParam = new SqlParameter("@count", -1);
                countParam.Direction = System.Data.ParameterDirection.Output;

                var query = db.Database.SqlQuery<ProductItem>("GetProducts @pageIndex, @pageSize, @pageOrder, @groupIDs, @producerIDs, @attributes, @keywordID, @count = @count output", pageIndexParam, pageSizeParam, pageOrderParam, groupIDsParam, producerIDsParam, attributesParam, keywordIDParam, countParam);

                var result = query.ToList();

                count = (int)countParam.Value;

                return result;
            }
        }

        public static int Count(List<int> groupIDs, List<string> attributes = null, List<int> producers = null)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var now = DateTime.Now.Date;

                var query = from item in db.Products
                            where item.ProductStatus == ProductStatus.Approved
                            && item.ProductGroups.Any(@group => groupIDs.Contains(@group.GroupID))
                            && now >= item.PublishDate
                            select item;

                if (producers != null && producers.Count > 0)
                    query = query.Where(item => producers.Contains(item.ProducerID));

                if (attributes != null && attributes.Count > 0)
                {
                    foreach (var attr in attributes.Select(item => new { AttrID = int.Parse(item.Split('-')[0]), OpID = int.Parse(item.Split('-')[1]) }))
                    {
                        query = query.Where(item => db.AttributeValues.Any(av => av.ProductID == item.ID && av.AttributeID == attr.AttrID && av.AttributeOptionID == attr.OpID));
                    }
                }

                return query.Count();
            }
        }

        public static List<ListItem> GetList()
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var query = from item in db.Products
                            select new ListItem()
                            {
                                ID = item.ID.ToString(),
                                Title = item.Title
                            };

                return query.ToList();
            }
        }

        public static int Count(List<int> groups,
                                string title,
                                int keyword,
                                int producer,
                                int? price,
                                DateTime? fromDate,
                                DateTime? toDate,
                                bool? IsUnavailable,
                                bool? IsInVisible,
                                bool? noPrice,
                                ProductStatus? productStatus,
                                string userID
                               )
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var query = from item in db.Products
                            where item.ProductStatus == productStatus
                            select new
                            {
                                item.ID,
                                item.Title,
                                ProducerID = item.ProducerID,
                                ProducerTitle = item.Producer.Title,
                                Price = (from p in item.ProductPrices
                                         where p.ProductID == item.ID && p.PriceType == PriceType.Sell
                                         orderby p.LastUpdate descending
                                         select p.Price).FirstOrDefault(),
                                item.PublishDate,
                                item.IsUnavailable,
                                item.IsInVisible,
                                item.VisitCount,
                                item.LastUpdate,
                                item.ProductStatus,
                                item.UserID,
                                item.EditingUserID,
                                item.HasVarients
                            };

                if (groups != null && groups.Count > 0)
                    query = query.Where(item => db.ProductGroups.Any(group => group.ProductID == item.ID && groups.Contains(group.GroupID)));

                if (!string.IsNullOrWhiteSpace(title))
                    query = query.Where(item => item.Title.Contains(title));

                if (keyword != -1)
                {
                    query = query.Where(item => db.ProductKeywords.Any(key => key.ProductID == item.ID && key.KeywordID == keyword));
                }

                if (producer != -1)
                    query = query.Where(item => item.ProducerID == producer);

                if (price.HasValue)
                    query = query.Where(item => item.Price == price);

                if (fromDate.HasValue)
                    query = query.Where(item => item.PublishDate >= fromDate);

                if (toDate.HasValue)
                    query = query.Where(item => item.PublishDate <= toDate);

                if (IsUnavailable.HasValue)
                    query = query.Where(item => item.IsUnavailable == IsUnavailable);

                if (IsInVisible.HasValue)
                    query = query.Where(item => item.IsInVisible == IsInVisible);

                if (productStatus.HasValue)
                    query = query.Where(item => item.ProductStatus == productStatus);

                if (noPrice.HasValue && noPrice.Value)
                {
                    query = from item in query
                            where
                            (!item.HasVarients && (!db.ProductPrices.Any(p => p.ProductID == item.ID && p.PriceType == PriceType.Sell && p.Price != 0)))
                            ||
                            (item.HasVarients && (!db.ProductVarients.Any(p => p.ProductID == item.ID)))
                            select item;
                }

                if (!String.IsNullOrWhiteSpace(userID))
                    query = query.Where(item => item.UserID == userID || item.EditingUserID == userID);

                return query.Count();
            }
        }

        public static Product GetByID(int id)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var product = db.Products.Where(item => item.ID == id).Single();

                return product;
            }
        }

        public static string GetTitleByID(int id)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var product = from item in db.Products
                              where item.ID == id
                              select item.Title;

                return product.SingleOrDefault();
            }
        }

        public static bool GetIsUnavailableByID(int id)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var product = from item in db.Products
                              where item.ID == id
                              select item.IsUnavailable;

                return product.SingleOrDefault();
            }
        }

        public static ProductDetail GetDetails(int id, bool isAdmin = false)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var product = from item in db.Products
                              where item.ID == id
                              && ((!item.IsInVisible && item.ProductStatus == ProductStatus.Approved) || (isAdmin))
                              select new ProductDetail
                              {
                                  ID = item.ID,
                                  DisplayTitleType = item.DisplayTitleType,

                                  Title = item.Title,
                                  Title_En = item.Title_En,

                                  GroupID = item.GroupID,
                                  ProducerID = item.ProducerID,

                                  Summary = item.Summary,
                                  Text = item.Text,

                                  IsUnavailable = item.IsUnavailable,
                                  IsFreeDelivery = item.IsFreeDelivery,

                                  PriceStatus = item.PriceStatus,

                                  HasVarients = item.HasVarients,
                                  ProductScore = item.ProductScore,
                                  OrderID = item.OrderID,
                                  SumScore = (from sum in db.ProductRates
                                              where sum.ProductID == item.ID
                                              select sum.Rate).Sum(),
                                  ScoreCount = (from sum in db.ProductRates
                                                where sum.ProductID == item.ID
                                                select sum.Rate).Count(),
                                  LastUpdate = item.LastUpdate,
                                  ProducerTitle = item.Producer.TitleEn
                              };

                return product.FirstOrDefault();
            }
        }

        public static ProductDetail GetRates(int id)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var product = from item in db.Products
                              where item.ID == id
                              select new ProductDetail
                              {
                                  ProductScore = item.ProductScore,
                                  SumScore = (from sum in db.ProductRates
                                              where sum.ProductID == item.ID
                                              select sum.Rate).Sum(),
                                  ScoreCount = (from sum in db.ProductRates
                                                where sum.ProductID == item.ID
                                                select sum.Rate).Count(),
                              };

                return product.FirstOrDefault();
            }
        }

        public static List<JsonProductSearch> SimpleSearch(string key)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                DateTime now = DateTime.Now;

                var query = from item in db.Products
                            where now >= item.PublishDate
                            && item.IsInVisible == false
                            && (item.Title.Contains(key) || item.Title_En.Contains(key))
                            orderby item.LastUpdate descending
                            select new JsonProductSearch
                            {
                                ID = item.ID,
                                Title_Fa = item.Title,
                                Title_En = item.Title_En,
                                DisplayTitleType = item.DisplayTitleType,
                                ProductStatus = item.ProductStatus,
                                Image = db.ProductImages.FirstOrDefault(img =>
                                                                        img.ProductID == item.ID &&
                                                                         img.ProductImagePlace == ProductImagePlace.Home).Filename,
                                GroupID = item.GroupID.Value
                            };

                var result = query.Take(20).ToList();

                foreach (var item in result)
                {
                    var group = Groups.GetByID(item.GroupID.Value);

                    item.Title_Fa = group.Perfix + " " + item.Title_Fa;
                    item.Title_En = item.Title_En + " " + group.Perfix_En;

                    item.Url = UrlProvider.GetProductUrl(item.ID, group.UrlPerfix, item.UrlPerfix);
                    item.Image = UrlProvider.GetProductImage(item.Image, StaticValues.ThumbnailProductImageSize);
                }

                return result;

            }
        }

        public static List<JsonProductSearch> Search(string key)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                DateTime now = DateTime.Now;

                var query = from item in db.Products
                            where (item.ProductStatus == ProductStatus.Approved)
                            && now >= item.PublishDate
                            && item.IsInVisible == false
                            && (item.Title.Contains(key) || item.Title_En.Contains(key))
                            orderby item.LastUpdate descending
                            select new JsonProductSearch
                            {
                                ID = item.ID,
                                Title_Fa = item.Title,
                                Title_En = item.Title_En,
                                DisplayTitleType = item.DisplayTitleType,
                                ProductStatus = item.ProductStatus,
                                Image = db.ProductImages.FirstOrDefault(img =>
                                                                        img.ProductID == item.ID &&
                                                                         img.ProductImagePlace == ProductImagePlace.Home).Filename,
                                GroupID = item.GroupID.Value
                            };

                var result = query.Take(20).ToList();

                foreach (var item in result)
                {
                    var group = Groups.GetByID(item.GroupID.Value);

                    item.Title_Fa = group.Perfix + " " + item.Title_Fa;
                    item.Title_En = item.Title_En + " " + group.Perfix_En;

                    item.Url = UrlProvider.GetProductUrl(item.ID, group.UrlPerfix, item.UrlPerfix);
                    item.Image = UrlProvider.GetProductImage(item.Image, StaticValues.SearchImageSize);
                }

                return result;

            }
        }

        public static List<ProductItem> AdvancedSearch(string key)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                DateTime now = DateTime.Now;

                var query = from item in db.Products
                            where (item.ProductStatus == ProductStatus.Approved)
                            && now >= item.PublishDate
                            && item.IsInVisible == false
                            && (item.Title.Contains(key) || item.Title_En.Contains(key))
                            orderby item.LastUpdate descending
                            select new ProductItem
                            {
                                ID = item.ID,
                                GroupID = item.GroupID,
                                DisplayTitleType = item.DisplayTitleType,
                                Title_Fa = item.Title,
                                Title_En = item.Title_En,
                                HasVarients = item.HasVarients,
                                IsUnavailable = item.IsUnavailable,
                                CommentCount = (from comment in db.ScoreComments
                                                where comment.ProductID == item.ID && comment.ScoreCommentStatus == ScoreCommentStatus.Approved
                                                select comment).Count(),

                                ImageFile = (from img in db.ProductImages
                                             where img.ProductID == item.ID && img.ProductImagePlace == ProductImagePlace.Home
                                             select img.Filename).FirstOrDefault(),
                                ProductScore = item.ProductScore,
                                SumScore = (from sum in db.ProductRates
                                            where sum.ProductID == item.ID
                                            select sum.Rate).Sum(),
                                ScoreCount = (from sum in db.ProductRates
                                              where sum.ProductID == item.ID
                                              select sum.Rate).Count(),
                                PriceStatus = item.PriceStatus
                            };


                var result = query.Take(20).ToList();

                return result;
            }
        }

        public static List<JsonProductSearch> SearchByGroup_Key(int groupID, string key)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                DateTime now = DateTime.Now;

                var query = from item in db.Products
                            where item.ProductStatus == ProductStatus.Approved
                            && now >= item.PublishDate
                            && (item.Title.Contains(key) || item.Title_En.Contains(key))
                            && item.GroupID == groupID
                            orderby item.LastUpdate descending
                            select new JsonProductSearch
                            {
                                ID = item.ID,
                                GroupID = item.GroupID,
                                Title_Fa = item.Title,
                                Title_En = item.Title_En,
                                DisplayTitleType = item.DisplayTitleType,
                                Image = db.ProductImages.FirstOrDefault(img =>
                                                                        img.ProductID == item.ID &&
                                                                        img.ProductImagePlace == ProductImagePlace.Home).Filename,
                            };

                var result = query.Take(20).ToList();

                foreach (var item in result)
                {
                    item.Image = UrlProvider.GetProductImage(item.Image, StaticValues.ThumbnailProductImageSize);
                }

                return result;
            }
        }

        public static void Delete(int id)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var product = (from item in db.Products
                               where item.ID == id
                               select item).Single();

                db.Products.Remove(product);

                db.SaveChanges();
            }
        }

        public static void Insert(Product product)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                db.Products.Add(product);

                db.SaveChanges();
            }
        }

        public static void Update(Product product)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var orgProduct = db.Products.Where(item => item.ID == product.ID).Single();

                orgProduct.Title = product.Title;
                orgProduct.Title_En = product.Title_En;
                orgProduct.DisplayTitleType = product.DisplayTitleType;
                orgProduct.ProducerID = product.ProducerID;
                orgProduct.GroupID = product.GroupID;
                orgProduct.PublishDate = product.PublishDate;
                orgProduct.IsUnavailable = product.IsUnavailable;
                orgProduct.IsInVisible = product.IsInVisible;
                orgProduct.IsFreeDelivery = product.IsFreeDelivery;
                orgProduct.HasVarients = product.HasVarients;
                orgProduct.Summary = product.Summary;
                orgProduct.Text = product.Text;
                orgProduct.ProductStatus = product.ProductStatus;
                orgProduct.PriceStatus = product.PriceStatus;
                orgProduct.OrderID = product.OrderID;
                orgProduct.PriceCode = product.PriceCode;
                orgProduct.ProductScore = product.ProductScore;
                orgProduct.LastUpdate = product.LastUpdate;

                db.SaveChanges();
            }
        }

        public static void UpdateIsUnavailable(int? productID = null)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                List<SimpleProduct> list = null;

                if (productID.HasValue)
                    list = db.Products.Where(item => item.ID == productID.Value).Select(item => new SimpleProduct { ID = item.ID, IsUnavailable = item.IsUnavailable, HasVarients = item.HasVarients }).ToList();
                else
                    list = db.Products.Select(item => new SimpleProduct { ID = item.ID, IsUnavailable = item.IsUnavailable, HasVarients = item.HasVarients }).ToList();

                foreach (var item in list)
                {
                    var prices = Products.GetProductPrices(item.ID, item.HasVarients, PriceType.Sell);

                    if (!item.IsUnavailable)
                    {
                        var isUnavailable = false;
                        if (prices != null && prices.Count > 0)
                        {
                            var minPrice = prices.OrderBy(p => p.Price).First();
                            if (minPrice.Price == 0)
                            {
                                isUnavailable = true;
                            }

                            if (minPrice is JsonProductVarient)
                            {
                                var priceID = (minPrice as JsonProductVarient).PriceID;

                                if (ProductVarientPrices.GetByID(priceID).Count == 0)
                                    isUnavailable = true;
                            }
                        }
                        else
                        {
                            isUnavailable = true;
                        }

                        if (isUnavailable)
                        {
                            var product = db.Products.Single(pro => pro.ID == item.ID);
                            product.IsUnavailable = true;
                            db.SaveChanges();
                        }
                    }
                }
            }
        }

        public static void UpdateOrderID(int productID, int orderID)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var orgProduct = db.Products.Where(item => item.ID == productID).Single();

                orgProduct.OrderID = orderID;
                orgProduct.LastUpdate = DateTime.Now;

                db.SaveChanges();
            }
        }

        public static void FillProductItems(string userID, List<ProductItem> list, Size imageSize)
        {
            foreach (var item in list)
            {
                item.Marks = ProductMarks.GetMarksByProductID(item.ID);

                item.ImageFile = UrlProvider.GetProductImage(item.ImageFile, imageSize);

                #region Prices

                item.Prices = Products.GetProductPrices(item.ID, item.HasVarients, PriceType.Sell);

                SetDiscounts(userID, item.ID, item.HasVarients, item.Prices);

                #endregion Prices
            }
        }

        public static void SetDiscounts(string userID, int productID, bool hasVarients, List<PriceItem> prices)
        {
            var discount = ProductDiscounts.GetProductDiscount(productID, userID);

            foreach (var price in prices)
            {
                if (discount.Value > 0)
                {
                    switch (discount.DiscountType)
                    {
                        case DiscountType.Percent:
                            price.DiscountPrice = Utilities.DiscountPrice(discount.Value, price.Price);
                            price.DiscountPercent = discount.Value;
                            break;
                        case DiscountType.PriceAfter:
                            price.DiscountPrice = (int)discount.Value;
                            price.DiscountPercent = (int)Math.Round((1.0 - ((float)price.DiscountPrice / price.Price)) * 100);
                            break;
                        case DiscountType.PriceBefore:
                            price.DiscountPrice = (int)discount.Value;

                            var tmpPrice = price.Price;
                            var tmpDiscountPrice = price.DiscountPrice;

                            price.Price = tmpDiscountPrice;
                            price.DiscountPrice = tmpPrice;

                            price.DiscountPercent = (int)Math.Round((1.0 - ((float)price.DiscountPrice / price.Price)) * 100);
                            break;
                        case DiscountType.PriceBeforeAfter:
                            price.Price = (int)discount.Value;
                            price.DiscountPrice = (int)discount.Price_01;
                            price.DiscountPercent = (int)Math.Round((1.0 - ((float)price.DiscountPrice / price.Price)) * 100);
                            break;
                        default:
                            break;
                    }
                }
            }

            var hasVarientDiscount = ProductDiscounts.HasVarientDiscount(productID);
            if (hasVarients && hasVarientDiscount)
            {
                foreach (var price in prices)
                {
                    discount = ProductDiscounts.GetProductDiscount(productID, userID, price.ID);

                    if (discount.Value > 0)
                    {
                        switch (discount.DiscountType)
                        {
                            case DiscountType.Percent:
                                price.DiscountPrice = Utilities.DiscountPrice(discount.Value, price.Price);
                                price.DiscountPercent = discount.Value;
                                break;
                            case DiscountType.PriceAfter:
                                price.DiscountPrice = (int)discount.Value;
                                price.DiscountPercent = (int)Math.Round((1.0 - ((float)price.DiscountPrice / price.Price)) * 100);
                                break;
                            case DiscountType.PriceBefore:
                                price.DiscountPrice = (int)discount.Value;

                                var tmpPrice = price.Price;
                                var tmpDiscountPrice = price.DiscountPrice;

                                price.Price = tmpDiscountPrice;
                                price.DiscountPrice = tmpPrice;

                                price.DiscountPercent = (int)Math.Round((1.0 - ((float)price.DiscountPrice / price.Price)) * 100);
                                break;
                            default:
                                break;
                        }
                    }
                }
            }
        }

        public static List<PriceItem> GetProductPrices(int productID, bool hasVarients, PriceType priceType)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var result = new List<PriceItem>();

                if (hasVarients)
                {
                    var varients = (from varient in db.ProductVarients
                                    join price in db.ProductVarientPrices on varient.ID equals price.ProductVarientID
                                    where
                                    varient.ProductID == productID
                                    && varient.IsEnabled == true
                                    && price.PriceType == priceType
                                    orderby price.LastUpdate descending
                                    select new JsonProductVarient()
                                    {
                                        ID = varient.ID,
                                        PriceID = price.ID,
                                        Price = price.Price,
                                        PriceType = price.PriceType
                                    }).ToList();

                    foreach (var item in varients)
                        item.Attributes = ProductVarientAttributes.GetJsonByProductVarientID(item.ID);

                    var varientGroups = varients.GroupBy((item) =>
                    {
                        return item.PriceType + item.Attributes
                                                    .Select(attr => attr.AttributeID.ToString() + attr.AttributeOptionID.ToString())
                                                    .Aggregate((a, b) => a + b);
                    }).ToList();

                    foreach (var item in varientGroups)
                    {
                        var last = item.FirstOrDefault();

                        if (last != null)
                            result.Add(last);
                    }
                }
                else
                {
                    var productPrice = ProductPrices.GetLatestPrice(productID, priceType);

                    if (productPrice != null)
                        result.Add(new PriceItem()
                        {
                            ID = productPrice.ID,
                            Price = productPrice.Price
                        });
                }

                return result;
            }
        }

        public static void AddVisits(int id)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var orgProduct = db.Products.Where(item => item.ID == id).Single();

                orgProduct.VisitCount++;

                db.SaveChanges();
            }
        }

        public static int GetGroupID(int productID)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var groupID = (from item in db.Products
                               where item.ID == productID
                               select item.GroupID).Single();

                return groupID.Value;
            }
        }

        public static List<Product> GetByGroupID(int groupID)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var now = DateTime.Now.Date;

                var query = from item in db.Products
                            where
                            item.GroupID == groupID
                            && item.ProductStatus == ProductStatus.Approved
                            && now >= item.PublishDate
                            && item.IsInVisible == false
                            orderby item.LastUpdate descending
                            select item;

                return query.ToList();
            }
        }

        public static bool ExistsByGroupID(int groupID)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var list = from item in db.Products
                           where item.GroupID == groupID
                           select item;

                return list.Any();
            }
        }

        public static DateTime LatestDateByGroupID(int groupID)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var pro = from item in db.Products
                          where item.GroupID == groupID
                          orderby item.LastUpdate descending
                          select item.LastUpdate;

                return pro.First();
            }
        }

        public static DateTime LatestDate()
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var now = DateTime.Now;

                var pro = from item in db.Products
                          where item.ProductStatus == ProductStatus.Approved
                           && now >= item.PublishDate
                           && item.IsInVisible == false
                          orderby item.LastUpdate descending
                          select item.LastUpdate;

                return pro.First();
            }
        }

        public static DateTime LatestCreatedDate()
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var now = DateTime.Now;

                var pro = from item in db.Products
                          where item.ProductStatus == ProductStatus.Approved
                           && now >= item.PublishDate
                           && item.IsInVisible == false
                          orderby item.CreatedDate descending
                          select item.CreatedDate.Value;

                return pro.First();
            }
        }

        public static DateTime LatestCreatedDate(List<int> groupIDs)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var now = DateTime.Now;

                var pro = from item in db.Products
                          where item.ProductStatus == ProductStatus.Approved
                           && now >= item.PublishDate
                           && item.IsInVisible == false
                           && item.ProductGroups.Any(@group => groupIDs.Contains(@group.GroupID))
                          orderby item.CreatedDate descending
                          select item.CreatedDate.Value;

                return pro.First();
            }
        }

        public static bool CanAddToCart(
            int? productVarientID,
            int? productID,
            int? packageID)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var now = DateTime.Now;


                if (productID.HasValue || productVarientID.HasValue)
                {
                    var proQuery = from item in db.Products
                                   where
                                   (
                                    (productID.HasValue && item.ID == productID) ||
                                    (productVarientID.HasValue && db.ProductVarients.Any(a => a.ID == productVarientID.Value))
                                   )
                                   && item.ProductStatus == ProductStatus.Approved
                                   && now >= item.PublishDate
                                   && item.IsInVisible == false
                                   && item.IsUnavailable == false
                                   select item;

                    return proQuery.Any();
                }
                else if (packageID.HasValue)
                {
                    var packQuery = from item in db.Packages
                                    where item.PackageStatus == PackageStatus.Approved &&
                                    (now >= item.StartDate && now <= item.EndDate)
                                    select item;

                    return packQuery.Any();
                }

                return false;
            }
        }

        public static void ReferenceProduct(List<int> productIDs, string userID)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var products = db.Products.Where(item => productIDs.Contains(item.ID)).ToList();

                foreach (var item in products)
                {
                    item.EditingUserID = userID;
                }

                db.SaveChanges();
            }
        }

        public static List<ProductItem> GetLatestProducts()
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var now = DateTime.Now.Date;

                var query = from item in db.Products
                            where
                            now >= item.PublishDate &&
                            item.IsInVisible == false &&
                            item.IsUnavailable == false &&
                            item.ProductStatus == ProductStatus.Approved
                            orderby item.CreatedDate descending
                            select new ProductItem
                            {
                                ID = item.ID,
                                GroupID = item.GroupID,
                                Title_Fa = item.Title,
                                Title_En = item.Title_En,
                                IsUnavailable = item.IsUnavailable,
                                DisplayTitleType = item.DisplayTitleType,
                                HasVarients = item.HasVarients,
                                ImageFile = (from img in db.ProductImages
                                             where img.ProductID == item.ID && img.ProductImagePlace == ProductImagePlace.Home
                                             select img.Filename).FirstOrDefault(),
                                CreatedDate = item.CreatedDate.Value,
                                Summary = item.Summary
                            };

                return query.Take(10).ToList();
            }
        }

        public static List<ProductItem> SimilarProducts(int groupID, int orderID, int id)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                DateTime now = DateTime.Now;

                var up = from item in db.Products
                         where (item.ProductStatus == ProductStatus.Approved)
                         && now >= item.PublishDate
                         && item.IsInVisible == false
                         && item.GroupID == groupID
                         && item.OrderID >= orderID
                         && item.ID != id
                         orderby item.OrderID
                         select new ProductItem
                         {
                             ID = item.ID,
                             GroupID = item.GroupID,
                             DisplayTitleType = item.DisplayTitleType,
                             Title_Fa = item.Title,
                             Title_En = item.Title_En,
                             HasVarients = item.HasVarients,
                             IsUnavailable = item.IsUnavailable,
                             CommentCount = (from comment in db.ScoreComments
                                             where comment.ProductID == item.ID && comment.ScoreCommentStatus == ScoreCommentStatus.Approved
                                             select comment).Count(),

                             ImageFile = (from img in db.ProductImages
                                          where img.ProductID == item.ID && img.ProductImagePlace == ProductImagePlace.Home
                                          select img.Filename).FirstOrDefault(),
                             ProductScore = item.ProductScore,
                             SumScore = (from sum in db.ProductRates
                                         where sum.ProductID == item.ID
                                         select sum.Rate).Sum(),
                             ScoreCount = (from sum in db.ProductRates
                                           where sum.ProductID == item.ID
                                           select sum.Rate).Count(),
                             PriceStatus = item.PriceStatus
                         };

                var low = from item in db.Products
                          where (item.ProductStatus == ProductStatus.Approved)
                          && now >= item.PublishDate
                          && item.IsInVisible == false
                          && item.GroupID == groupID
                          && item.OrderID <= orderID
                          && item.ID != id
                          orderby item.OrderID descending
                          select new ProductItem
                          {
                              ID = item.ID,
                              GroupID = item.GroupID,
                              DisplayTitleType = item.DisplayTitleType,
                              Title_Fa = item.Title,
                              Title_En = item.Title_En,
                              HasVarients = item.HasVarients,
                              IsUnavailable = item.IsUnavailable,
                              CommentCount = (from comment in db.ScoreComments
                                              where comment.ProductID == item.ID && comment.ScoreCommentStatus == ScoreCommentStatus.Approved
                                              select comment).Count(),

                              ImageFile = (from img in db.ProductImages
                                           where img.ProductID == item.ID && img.ProductImagePlace == ProductImagePlace.Home
                                           select img.Filename).FirstOrDefault(),
                              ProductScore = item.ProductScore,
                              SumScore = (from sum in db.ProductRates
                                          where sum.ProductID == item.ID
                                          select sum.Rate).Sum(),
                              ScoreCount = (from sum in db.ProductRates
                                            where sum.ProductID == item.ID
                                            select sum.Rate).Count(),
                          };

                var result = up.Take(5).ToList();
                result.AddRange(low.Take(5).ToList());

                return result;
            }
        }

        public static List<ProductItem> GetLatestProducts(List<int> groupIDs)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var now = DateTime.Now.Date;

                var query = from item in db.Products
                            where item.ProductStatus == ProductStatus.Approved
                            && item.ProductGroups.Any(@group => groupIDs.Contains(@group.GroupID))
                            && now >= item.PublishDate
                            && item.IsInVisible == false
                            orderby item.CreatedDate descending
                            select new ProductItem
                            {
                                ID = item.ID,
                                GroupID = item.GroupID,

                                GroupTitle = item.Group.Title,
                                GroupTitle_En = item.Group.TitleEn,

                                Title_Fa = item.Title,
                                Title_En = item.Title_En,

                                DisplayTitleType = item.DisplayTitleType,
                                IsUnavailable = item.IsUnavailable,
                                ImageFile = (from img in db.ProductImages
                                             where img.ProductID == item.ID && img.ProductImagePlace == ProductImagePlace.Home
                                             select img.Filename).FirstOrDefault(),
                                CreatedDate = item.CreatedDate.Value,
                                PriceStatus = item.PriceStatus
                            };

                var result = query.Take(20).ToList();
                return result.ToList();
            }
        }

        public static List<RSSProductsBlog> GetLatest()
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var now = DateTime.Now.Date;

                var query = from item in db.Products
                            where item.ProductStatus == ProductStatus.Approved
                            && now >= item.PublishDate
                            && item.IsInVisible == false
                            orderby item.CreatedDate descending
                            select new RSSProductsBlog
                            {
                                ID = item.ID,
                                GroupID = item.GroupID,
                                GroupTitle = item.Group.Title,
                                Title_Fa = item.Title,
                                Title_En = item.Title_En,
                                Image = (from img in db.ProductImages
                                         where img.ProductID == item.ID && img.ProductImagePlace == ProductImagePlace.Home
                                         select img.Filename).FirstOrDefault(),
                                Date = item.CreatedDate.Value,
                                Summary = item.Summary,
                                Type = RSSRowType.Product
                            };

                return query.Take(10).ToList();
            }
        }

        public static List<ProductItem> GetRandom(int? groupID = null, int count = 10)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {

                DateTime now = DateTime.Now;

                var query = (from item in db.Products
                             where item.ProductStatus == ProductStatus.Approved
                             && now >= item.PublishDate
                             && !item.IsInVisible
                             && !item.IsUnavailable
                             orderby item.OrderID
                             select new ProductItem
                             {
                                 ID = item.ID,
                                 Title_Fa = item.Title,
                                 Title_En = item.Title_En,
                                 DisplayTitleType = item.DisplayTitleType,
                                 GroupID = item.GroupID,
                                 HasVarients = item.HasVarients,
                                 IsUnavailable = item.IsUnavailable,
                                 CommentCount = (from comment in db.ScoreComments
                                                 where comment.ProductID == item.ID && comment.ScoreCommentStatus == ScoreCommentStatus.Approved
                                                 select comment).Count(),
                                 ImageFile = (from img in db.ProductImages
                                              where img.ProductID == item.ID && img.ProductImagePlace == ProductImagePlace.Home
                                              select img.Filename).FirstOrDefault(),

                                 ProductScore = item.ProductScore,
                                 SumScore = (from sum in db.ProductRates
                                             where sum.ProductID == item.ID
                                             select sum.Rate).Sum(),
                                 ScoreCount = (from sum in db.ProductRates
                                               where sum.ProductID == item.ID
                                               select sum.Rate).Count(),
                                 PriceStatus = item.PriceStatus
                             });

                if (groupID.HasValue)
                    query = query.Where(item => item.GroupID == groupID.Value);

                Random rand = new Random();
                int toSkip = rand.Next(0, query.Count());

                return query.Skip(toSkip).Take(count).ToList();
            }
        }

        public static List<JsonSimpleProduct> GetByGroupIDs(string title = "", List<int> groupIDs = null)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var query = from item in db.Products
                            select item;

                if (!String.IsNullOrWhiteSpace(title))
                    query = query.Where(item => item.Title.Contains(title) ||
                                                item.Title_En.Contains(title));

                if (groupIDs.Count > 0)
                    query = query.Where(item => groupIDs.Contains(item.GroupID.Value));

                var result = from item in query
                             select new JsonSimpleProduct
                             {
                                 ID = item.ID,
                                 Title = item.Title,
                                 IsUnavailable = item.IsUnavailable,
                                 ImageFile = (from img in db.ProductImages
                                              where img.ProductID == item.ID && img.ProductImagePlace == ProductImagePlace.Home
                                              select img.Filename).FirstOrDefault(),
                                 OrderID = item.OrderID,
                                 PriceStatus = item.PriceStatus,
                                 Weight = item.Producer.Weight,
                                 HasVarients = item.HasVarients,
                                 CreatedDate = item.CreatedDate,
                                 LastUpdate = item.LastUpdate,
                             };

                return result.ToList();
            }
        }

        public static List<ProductItem> GetSpecialOffers(int pageIndex, int pageSize, string pageOrder)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var now = DateTime.Now.Date;

                var query = from item in db.Products
                            where
                            now >= item.PublishDate &&
                            item.IsInVisible == false &&
                            item.IsUnavailable == false &&
                            item.ProductStatus == ProductStatus.Approved
                            && db.ProductDiscounts.Any(dnt =>
                                                            dnt.StartDate <= now &&
                                                            dnt.EndDate >= now &&
                                                            dnt.ProductDiscountStatus == ProductDiscountStatus.Approved &&
                                                            dnt.ProductID == item.ID)
                            orderby item.CreatedDate descending
                            select new ProductItem
                            {
                                ID = item.ID,
                                GroupID = item.GroupID,
                                DisplayTitleType = item.DisplayTitleType,
                                Title_Fa = item.Title,
                                Title_En = item.Title_En,
                                HasVarients = item.HasVarients,
                                IsUnavailable = item.IsUnavailable,
                                CommentCount = (from comment in db.ScoreComments
                                                where comment.ProductID == item.ID && comment.ScoreCommentStatus == ScoreCommentStatus.Approved
                                                select comment).Count(),

                                ImageFile = (from img in db.ProductImages
                                             where img.ProductID == item.ID && img.ProductImagePlace == ProductImagePlace.Home
                                             select img.Filename).FirstOrDefault(),
                                ProductScore = item.ProductScore,
                                SumScore = (from sum in db.ProductRates
                                            where sum.ProductID == item.ID
                                            select sum.Rate).Sum(),
                                ScoreCount = (from sum in db.ProductRates
                                              where sum.ProductID == item.ID
                                              select sum.Rate).Count(),
                            };

                if (!String.IsNullOrWhiteSpace(pageOrder))
                    query = query.OrderBy(pageOrder);

                query = query.Skip(pageIndex * pageSize).Take(pageSize);

                return query.ToList();
            }
        }

        public static int CountSpecialOffers()
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var now = DateTime.Now.Date;

                var query = from item in db.Products
                            where
                            now >= item.PublishDate &&
                            item.IsInVisible == false &&
                            item.IsUnavailable == false &&
                            item.ProductStatus == ProductStatus.Approved
                            && db.ProductDiscounts.Any(dnt =>
                                                            dnt.StartDate <= now &&
                                                            dnt.EndDate >= now &&
                                                            dnt.ProductDiscountStatus == ProductDiscountStatus.Approved &&
                                                            dnt.ProductID == item.ID)
                            orderby item.CreatedDate descending
                            select item;

                return query.Count();
            }
        }

        public static List<JsonPackageProduct> GetAllForPackage()
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var now = DateTime.Now.Date;

                var query = from item in db.Products
                            where item.ProductStatus == ProductStatus.Approved
                            && now >= item.PublishDate
                            && item.IsInVisible == false
                            orderby item.LastUpdate descending
                            select new JsonPackageProduct
                            {
                                ID = item.ID,
                                Title = item.Title,
                                HasVarients = item.HasVarients,
                            };

                var result = query.ToList();

                return result;
            }
        }
    }
}
