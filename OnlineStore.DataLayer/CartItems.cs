using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OnlineStore.EntityFramework;
using System.Collections;
using OnlineStore.Models.Public;
using OnlineStore.Models.Enums;
using OnlineStore.Models.User;
using OnlineStore.Providers;

namespace OnlineStore.DataLayer
{
    public class CartItem : EntityBase
    {
        [ForeignKey("Cart")]
        public int CartID { get; set; }
        public Cart Cart { get; set; }

        [ForeignKey("ProductVarient")]
        public int? ProductVarientID { get; set; }
        public ProductVarient ProductVarient { get; set; }

        [ForeignKey("Product")]
        public int? ProductID { get; set; }
        public Product Product { get; set; }

        [ForeignKey("Package")]
        public int? PackageID { get; set; }
        public Package Package { get; set; }

        public int Price { get; set; }

        public int Quantity { get; set; }

        public bool IsFreeDelivery { get; set; }

        public DateTime DateTime { get; set; }
    }

    public static class CartItems
    {
        public static IList Get(int pageIndex, int pageSize, string pageOrder)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var query = from item in db.CartItems
                            select new
                            {
                                item.ID,
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
                var query = from item in db.CartItems
                            select item;

                return query.Count();
            }
        }

        public static CartItem GetByID(int id)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var cartItem = db.CartItems.Where(item => item.ID == id).Single();

                return cartItem;
            }
        }

        public static List<ViewCartItem> GetByCartID(int cartID, string userID)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var query = from item in db.CartItems
                            where item.CartID == cartID
                            select new ViewCartItem()
                            {
                                ID = item.ID,

                                ProductVarientID = item.ProductVarientID,
                                ProductID = item.ProductID,
                                PackageID = item.PackageID,

                                Quantity = item.Quantity
                            };

                var result = query.ToList();
                foreach (var item in result)
                {
                    if (item.ProductVarientID.HasValue || item.ProductID.HasValue)
                    {
                        var priceItem = new PriceItem();

                        if (item.ProductVarientID.HasValue)
                        {
                            var productVarient = ProductVarients.GetByID(item.ProductVarientID.Value);

                            item.CartProductID = productVarient.ProductID;
                            item.Price = ProductVarientPrices.GetPriceByProductVarientID(item.ProductVarientID.Value);

                            priceItem.ID = item.ProductVarientID.Value;
                        }
                        else if (item.ProductID.HasValue)
                        {
                            item.CartProductID = item.ProductID.Value;
                        }

                        var product = Products.GetByID(item.CartProductID);
                        var group = Groups.GetByID(product.GroupID.Value);

                        product.Title = group.Perfix + " " + product.Title;
                        product.Title_En = product.Title_En + " " + group.Perfix_En;

                        item.Title = product.DisplayTitle;

                        item.Image = (from img in db.ProductImages
                                      where img.ProductID == item.CartProductID &&
                                      img.ProductImagePlace == ProductImagePlace.Home
                                      select img.Filename).FirstOrDefault();

                        item.HasVarients = product.HasVarients;

                        item.Image = UrlProvider.GetProductImage(item.Image, StaticValues.MiniCartProductImageSize);

                        item.Url = UrlProvider.GetProductUrl(item.CartProductID, group.UrlPerfix, product.UrlPerfix);

                        #region Prices

                        if (item.ProductVarientID.HasValue)
                        {
                            item.VarientTitle = ProductVarientAttributes.GetVarients(item.ProductVarientID.Value);
                        }
                        else if (item.ProductID.HasValue)
                        {
                            item.Price = (from price in db.ProductPrices
                                          where price.ProductID == item.CartProductID &&
                                          price.PriceType == PriceType.Sell
                                          orderby price.LastUpdate descending
                                          select price.Price).FirstOrDefault();
                        }

                        priceItem.Price = item.Price;
                        var prices = new List<PriceItem>() { priceItem };

                        var productID = (item.ProductID.HasValue ? item.ProductID.Value : ProductVarients.GetProductID(item.ProductVarientID.Value));

                        Products.SetDiscounts(userID, product.ID, product.HasVarients, prices);

                        item.DiscountPercent = priceItem.DiscountPercent;
                        item.DiscountPrice = priceItem.DiscountPrice;

                        #endregion Prices

                        DateTime now = DateTime.Now;

                        var gifts = (from gift in db.ProductGifts
                                     where ((item.ProductID != null && gift.ProductID == item.ProductID) ||
                                           (item.ProductID == null && gift.ProductID == item.CartProductID)) &&
                                           gift.StartDate <= now && gift.EndDate >= now
                                     select new ViewCartItemGift
                                     {
                                         ID = gift.ID,
                                         ProductID = gift.ProductID,
                                         GiftTitle = gift.Gift.Title,
                                         GiftID = gift.GiftID
                                     }).ToList();

                        foreach (var gift in gifts)
                        {
                            var gProduct = Products.GetByID(gift.GiftID);
                            var gGroup = Groups.GetByID(gProduct.GroupID.Value);
                            gift.Url = UrlProvider.GetProductUrl(gProduct.ID, gGroup.UrlPerfix, gProduct.UrlPerfix);

                            var giftPrices = Products.GetProductPrices(gift.GiftID, false, PriceType.Sell);
                            var price = giftPrices.LastOrDefault();

                            if (price != null)
                            {
                                gift.Price = price.Price;
                            }
                        }

                        item.Gifts = gifts;

                        item.IsFreeDelivery = product.IsFreeDelivery;
                    }
                    else if (item.PackageID.HasValue)
                    {
                        var package = Packages.GetByID(item.PackageID.Value);

                        item.Title = package.Title;

                        item.Image = (from img in db.PackageImages
                                      where img.PackageID == package.ID &&
                                      img.PackageImagePlace == ProductImagePlace.Home
                                      select img.Filename).FirstOrDefault();

                        item.Image = UrlProvider.GetPackageImage(item.Image, StaticValues.MiniCartProductImageSize);

                        item.Url = UrlProvider.GetPackageUrl(item.Title, item.PackageID.Value);

                        #region Prices

                        var products = PackageProducts.GetProducts(item.PackageID.Value);

                        item.Price = products.Sum(a => a.OldPrice);
                        item.DiscountPrice = products.Sum(a => a.NewPrice);
                        item.DiscountPercent = (item.Price * 100) / item.DiscountPrice;

                        #endregion Prices
                    }
                }

                return result;
            }
        }

        public static CartItem GetByProductID(int cartID, int productID)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var query = from item in db.CartItems
                            where item.CartID == cartID && item.ProductVarient.ProductID == productID
                            select item;

                return query.SingleOrDefault();
            }
        }

        public static CartItem GetByProductVarientID(int cartID, int? productVarientID = null, int? productID = null)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                if (productVarientID.HasValue)
                    return db.CartItems.Where(item => item.CartID == cartID && item.ProductVarientID == productVarientID).SingleOrDefault();
                else if (productID.HasValue)
                    return db.CartItems.Where(item => item.CartID == cartID && item.ProductVarientID == productVarientID).SingleOrDefault();
                else
                    return null;
            }
        }

        public static bool Exists(
            int cartID,
            int? productVarientID,
            int? productID,
            int? packageID)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                if (productVarientID.HasValue)
                    return db.CartItems.Any(item => item.CartID == cartID && item.ProductVarientID == productVarientID && item.ProductID == null && item.PackageID == null);
                else if (productID.HasValue)
                    return db.CartItems.Any(item => item.CartID == cartID && item.ProductID == productID && item.ProductVarientID == null && item.PackageID == null);
                else if (packageID.HasValue)
                    return db.CartItems.Any(item => item.CartID == cartID && item.PackageID == packageID && item.ProductVarientID == null && item.ProductID == null);
                else
                    return false;
            }
        }

        public static List<ViewOrderItem> GetOrderItems(int id)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var cartItem = from item in db.CartItems
                               where item.CartID == id
                               select new ViewOrderItem
                               {
                                   ProductTitle = item.ProductVarient != null ?
                                                  item.ProductVarient.Product.Title :
                                                  item.Product.Title,
                                   GroupID = item.ProductVarient != null ?
                                             item.ProductVarient.Product.GroupID.Value :
                                             item.Product.GroupID.Value,
                                   Price = item.Price,
                                   Quantity = item.Quantity,
                                   DateTime = item.DateTime,
                                   ProductID = item.ProductVarient != null ?
                                               item.ProductVarient.ProductID :
                                               item.ProductID.Value,
                                   ProductVarentID = item.ProductVarientID,
                                   Gifts = (from gift in db.CartItemGifts
                                            where gift.CartItemID == item.ID
                                            select new ViewCartItemGift
                                            {
                                                ID = gift.ID,
                                                GiftTitle = gift.Gift.Title,
                                                GiftID = gift.GiftID,
                                                Price = gift.Price
                                            }).ToList()

                               };

                var result = cartItem.ToList();

                foreach (var item in result)
                {
                    if (item.ProductVarentID.HasValue)
                    {
                        item.VarientTitle = ProductVarientAttributes.GetVarients(item.ProductVarentID.Value);
                    }

                    foreach (var gift in item.Gifts)
                    {
                        var gProduct = Products.GetByID(gift.GiftID);
                        var gGroup = Groups.GetByID(gProduct.GroupID.Value);
                        gift.Url = UrlProvider.GetProductUrl(gProduct.ID, gGroup.UrlPerfix, gProduct.UrlPerfix);
                    }

                    var group = Groups.GetByID(item.GroupID);
                    item.ProductTitle = group.Perfix + " " + item.ProductTitle;
                }

                return result;
            }
        }

        public static List<ViewOrderItem> GetOrderDetails(int id)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var cartItem = from item in db.CartItems
                               where item.CartID == id
                               select new ViewOrderItem
                               {
                                   ProductTitle = item.ProductVarient != null ?
                                                  item.ProductVarient.Product.Title :
                                                  item.Product.Title,
                                   GroupID = item.ProductVarient != null ?
                                             item.ProductVarient.Product.GroupID.Value :
                                             item.Product.GroupID.Value,
                                   ProductID = item.ProductVarient != null ?
                                               item.ProductVarient.ProductID :
                                               item.ProductID.Value,
                                   ProductVarentID = item.ProductVarientID,

                               };

                var result = cartItem.ToList();

                foreach (var item in result)
                {
                    if (item.ProductVarentID.HasValue)
                    {
                        item.VarientTitle = ProductVarientAttributes.GetVarients(item.ProductVarentID.Value);
                    }

                    var group = Groups.GetByID(item.GroupID);
                    item.ProductTitle = group.Perfix + " " + item.ProductTitle;
                }

                return result;
            }
        }

        /// <summary>
        /// For Admin
        /// </summary>
        /// <param name="id">کد سفارش</param>
        /// <returns>لیست کالاهای هر سفارش</returns>
        public static IList GetOrderItems(int pageIndex, int pageSize, string pageOrder, int id)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var query = from item in db.CartItems
                            where item.CartID == id
                            select new
                            {
                                ProductTitle = item.ProductVarient.Product.Title,
                                Price = item.Price,
                                Quantity = item.Quantity,
                                DateTime = item.DateTime,
                                LastUpdate = item.LastUpdate
                            };

                if (!string.IsNullOrWhiteSpace(pageOrder))
                    query = query.OrderBy(pageOrder);

                query = query.Skip(pageIndex * pageSize).Take(pageSize);

                return query.ToList();
            }
        }

        public static int CountOrderItems(int id)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var query = from item in db.CartItems
                            where item.CartID == id
                            select item;

                return query.Count();
            }
        }

        public static void Delete(int id)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var cartItem = (from item in db.CartItems
                                where item.ID == id
                                select item).Single();

                db.CartItems.Remove(cartItem);

                db.SaveChanges();
            }
        }

        public static void Delete(
            int cartID,
            int? productVarientID = null,
            int? productID = null,
            int? packageID = null)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var query = (from item in db.CartItems
                             where item.CartID == cartID &&
                             ((productVarientID.HasValue && item.ProductVarientID == productVarientID) ||
                             (productID.HasValue && item.ProductID == productID) ||
                             (packageID.HasValue && item.PackageID == packageID))
                             select item).ToList();

                db.CartItems.RemoveRange(query);

                db.SaveChanges();
            }
        }

        public static void Insert(CartItem cartItem)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                db.CartItems.Add(cartItem);

                db.SaveChanges();
            }
        }

        public static void Update(CartItem cartItem)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var orgCartItem = db.CartItems.Where(item => item.ID == cartItem.ID).Single();

                orgCartItem.CartID = cartItem.CartID;
                orgCartItem.ProductVarientID = cartItem.ProductVarientID;
                orgCartItem.Price = cartItem.Price;
                orgCartItem.Quantity = cartItem.Quantity;
                orgCartItem.IsFreeDelivery = cartItem.IsFreeDelivery;
                orgCartItem.DateTime = cartItem.DateTime;
                orgCartItem.LastUpdate = cartItem.LastUpdate;

                db.SaveChanges();
            }
        }

        public static void UpdatePrice(CartItem cartItem)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var orgCartItem = db.CartItems.Where(item => item.ID == cartItem.ID).Single();

                orgCartItem.Price = cartItem.Price;
                orgCartItem.LastUpdate = cartItem.LastUpdate;

                db.SaveChanges();
            }
        }

        public static int CountByProductID(int productID)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var query = from item in db.CartItems
                            where item.ProductID == productID || item.ProductVarient.ProductID == productID
                            select item;

                return query.Count();
            }
        }

    }
}
