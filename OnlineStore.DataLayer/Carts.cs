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
using OnlineStore.Models.User;
using OnlineStore.Models.Admin;
using OnlineStore.Models.Public;
using OnlineStore.Providers;
using System.Web;

namespace OnlineStore.DataLayer
{
    public class Cart : EntityBase
    {
        [MaxLength(128)]
        public string CartGuid { get; set; }

        [MaxLength(128)]
        public string UserID { get; set; }

        public SendMethodType SendMethodType { get; set; }

        public PaymentMethodType PaymentMethodType { get; set; }

        [ForeignKey("GiftCard")]
        public int? GiftCardID { get; set; }
        public GiftCard GiftCard { get; set; }

        public int? Total { get; set; }
        public int? DelivaryPrice { get; set; }
        public int? TotalDiscount { get; set; }
        public int? ToPay { get; set; }

        public string Description { get; set; }

        [MaxLength(1000)]
        public string UserDescription { get; set; }

        [MaxLength(15)]
        public string IP { get; set; }

        public CartStatus CartStatus { get; set; }

        public SendStatus SendStatus { get; set; }

        public ConfirmationStatus ConfirmationStatus { get; set; }

        public DateTime? ConfirmationDate { get; set; }

        public DateTime? SendDate { get; set; }

        public DateTime? DeliveryDate { get; set; }

        public int Tax { get; set; }

        public DateTime? DateTime { get; set; }

        [MaxLength(100)]
        public string OrderID { get; set; }

        public int ResCode { get; set; }

        [MaxLength(50)]
        public string SaleReferenceID { get; set; }

        [MaxLength(50)]
        public string RefID { get; set; }

        public string BillNumber { get; set; }
        public bool IsArchive { get; set; }
    }

    public static class Carts
    {
        public static List<ViewCart> Get(int pageIndex,
                                         int pageSize,
                                         string pageOrder,
                                         int? orderID,
                                         int? fromPrice,
                                         int? toPrice,
                                         DateTime? fromDate,
                                         DateTime? toDate,
                                         CartStatus? cartStatus,
                                         SendStatus? sendStatus,
                                         SendMethodType? sendMethodType,
                                         PaymentMethodType? paymentMethodType,
                                         string saleReferenceID,
                                         OrderStatus orderStatus)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var query = from item in db.Carts
                            where item.UserID != null
                            && !item.IsArchive
                            && item.CartStatus != CartStatus.InProgress
                            && (sendMethodType == null || item.SendMethodType == sendMethodType)
                            && (paymentMethodType == null || item.PaymentMethodType == paymentMethodType)
                            select new ViewCart
                            {
                                ID = item.ID,
                                UserID = item.UserID,
                                LastUpdate = item.LastUpdate,
                                DateTime = item.DateTime,
                                Total = item.Total,
                                ToPay = item.ToPay,
                                SendMethodType = item.SendMethodType,
                                PaymentMethodType = item.PaymentMethodType,
                                SendStatus = item.SendStatus,
                                CartStatus = item.CartStatus,
                                OrderID = item.OrderID,
                                SaleReferenceID = item.SaleReferenceID
                            };

                if (orderID.HasValue)
                    query = query.Where(item => item.ID == orderID);

                if (fromPrice.HasValue)
                    query = query.Where(item => item.ToPay >= fromPrice);

                if (toPrice.HasValue)
                    query = query.Where(item => item.ToPay <= toPrice);

                if (fromDate.HasValue)
                    query = query.Where(item => item.DateTime >= fromDate);

                if (toDate.HasValue)
                    query = query.Where(item => item.DateTime <= toDate);

                if (cartStatus.HasValue)
                    query = query.Where(item => item.CartStatus == cartStatus);
                else
                    switch (orderStatus)
                    {
                        case OrderStatus.Suseeccful:
                            query = query.Where(item => item.CartStatus == CartStatus.Success || item.CartStatus == CartStatus.FuturePay);
                            break;
                        case OrderStatus.Unsuccessful:
                            query = query.Where(item => item.CartStatus == CartStatus.Fail || item.CartStatus == CartStatus.DuringPay);
                            break;
                        default:
                            break;
                    }

                if (sendStatus.HasValue)
                    query = query.Where(item => item.SendStatus == sendStatus);

                if (!String.IsNullOrWhiteSpace(saleReferenceID))
                    query = query.Where(item => item.SaleReferenceID.Contains(saleReferenceID));

                if (pageOrder.Trim() == "ID")
                    query = query.OrderBy(item => item.CartStatus).OrderByDescending(item => item.DateTime);
                else
                    if (!string.IsNullOrWhiteSpace(pageOrder))
                    query = query.OrderBy(pageOrder);

                if (pageSize != -1)
                {
                    query = query.Skip(pageIndex * pageSize).Take(pageSize);
                }

                return query.ToList();
            }
        }

        public static int Count(int? orderID,
                                int? fromPrice,
                                int? toPrice,
                                DateTime? fromDate,
                                DateTime? toDate,
                                CartStatus? cartStatus,
                                SendStatus? sendStatus,
                                SendMethodType? sendMethodType,
                                PaymentMethodType? paymentMethodType,
                                string saleReferenceID,
                                OrderStatus orderStatus)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var query = from item in db.Carts
                            where item.UserID != null
                            && item.CartStatus != CartStatus.InProgress
                            && (sendMethodType == null || item.SendMethodType == sendMethodType)
                            && (paymentMethodType == null || item.PaymentMethodType == paymentMethodType)
                            select item;

                if (orderID.HasValue)
                    query = query.Where(item => item.ID == orderID);

                if (fromPrice.HasValue)
                    query = query.Where(item => item.ToPay >= fromPrice);

                if (toPrice.HasValue)
                    query = query.Where(item => item.ToPay <= toPrice);

                if (fromDate.HasValue)
                    query = query.Where(item => item.DateTime >= fromDate);

                if (toDate.HasValue)
                    query = query.Where(item => item.DateTime <= toDate);

                if (cartStatus.HasValue)
                    query = query.Where(item => item.CartStatus == cartStatus);
                else
                    switch (orderStatus)
                    {
                        case OrderStatus.Suseeccful:
                            query = query.Where(item => item.CartStatus == CartStatus.Success || item.CartStatus == CartStatus.FuturePay);
                            break;
                        case OrderStatus.Unsuccessful:
                            query = query.Where(item => item.CartStatus == CartStatus.Fail || item.CartStatus == CartStatus.DuringPay);
                            break;
                        default:
                            break;
                    }

                if (sendStatus.HasValue)
                    query = query.Where(item => item.SendStatus == sendStatus);

                if (!String.IsNullOrWhiteSpace(saleReferenceID))
                    query = query.Where(item => item.SaleReferenceID.Contains(saleReferenceID));

                return query.Count();
            }
        }

        public static Cart GetByID(int id)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var cart = db.Carts.Where(item => item.ID == id).Single();

                return cart;
            }
        }

        public static Factor GetFactor(int cartID)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var cart = (from item in db.Carts
                            where item.ID == cartID
                            select new Factor
                            {
                                ID = item.ID,
                                DelivaryPrice = item.DelivaryPrice,
                                Tax = item.Tax,
                                ToPay = item.ToPay,
                                Total = item.Total,
                                DateTime = item.DateTime.Value,
                                UserID = item.UserID,
                                SaleReferenceID = item.SaleReferenceID,
                                ResCode = item.ResCode,
                                CartStatus = item.CartStatus,
                                PaymentMethodType = item.PaymentMethodType,
                                SendMethodType = item.SendMethodType,
                                UserDescription = item.UserDescription

                            }).FirstOrDefault();

                return cart;
            }
        }

        public static Cart GetByOrderID(string orderID)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var cart = (from item in db.Carts
                            where item.OrderID == orderID
                            select item).FirstOrDefault();

                return cart;
            }
        }

        public static void Delete(int id)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var cart = (from item in db.Carts
                            where item.ID == id
                            select item).Single();

                db.Carts.Remove(cart);

                db.SaveChanges();
            }
        }

        public static int DeleteFailed(int id)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var cart = (from item in db.Carts
                            where item.ID == id
                            && item.CartStatus == CartStatus.Fail
                            select item).SingleOrDefault();

                if (cart != null)
                {
                    db.Carts.Remove(cart);

                    db.SaveChanges();
                    return 1;
                }

                return 0;
            }
        }

        public static int Archive(int id)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var cart = (from item in db.Carts
                            where item.ID == id
                            select item).SingleOrDefault();

                if (cart != null)
                {
                    cart.IsArchive = true;

                    db.SaveChanges();
                    return 1;
                }

                return 0;
            }
        }

        public static void Insert(Cart cart)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                db.Carts.Add(cart);

                db.SaveChanges();
            }
        }

        public static void Update(Cart cart)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var orgCart = db.Carts.Where(item => item.ID == cart.ID).Single();

                orgCart.CartGuid = cart.CartGuid;
                orgCart.UserID = cart.UserID;
                orgCart.SendMethodType = cart.SendMethodType;
                orgCart.PaymentMethodType = cart.PaymentMethodType;
                orgCart.GiftCardID = cart.GiftCardID;
                orgCart.Total = cart.Total;
                orgCart.ToPay = cart.ToPay;
                orgCart.Tax = cart.Tax;
                orgCart.Description = cart.Description;
                orgCart.UserDescription = cart.UserDescription;
                orgCart.IP = cart.IP;
                orgCart.CartStatus = cart.CartStatus;
                orgCart.SendStatus = cart.SendStatus;
                orgCart.DateTime = cart.DateTime;
                orgCart.LastUpdate = cart.LastUpdate;
                orgCart.DelivaryPrice = cart.DelivaryPrice;
                orgCart.OrderID = cart.OrderID;

                db.SaveChanges();
            }
        }

        public static void UpdateByAdmin(Cart cart)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var orgCart = db.Carts.Where(item => item.ID == cart.ID).Single();

                orgCart.Description = cart.Description;
                //orgCart.CartStatus = cart.CartStatus;
                orgCart.SendStatus = cart.SendStatus;
                orgCart.SendDate = cart.SendDate;
                orgCart.DeliveryDate = cart.DeliveryDate;
                orgCart.ConfirmationStatus = cart.ConfirmationStatus;
                orgCart.ConfirmationDate = cart.ConfirmationDate;
                orgCart.BillNumber = cart.BillNumber;
                orgCart.LastUpdate = cart.LastUpdate;

                db.SaveChanges();
            }
        }

        public static void UpdatePaymentInfo(Cart cart)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var orgCart = db.Carts.Where(item => item.ID == cart.ID).Single();

                orgCart.CartGuid = cart.CartGuid;
                orgCart.CartStatus = cart.CartStatus;
                orgCart.RefID = cart.RefID;
                orgCart.ResCode = cart.ResCode;
                orgCart.LastUpdate = cart.LastUpdate;
                if (cart.SaleReferenceID != null)
                {
                    orgCart.SaleReferenceID = cart.SaleReferenceID;
                }

                db.SaveChanges();
            }
        }

        public static Cart GetOrInsert(string cartID, bool isUserID, bool insert = true)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                Cart cart;

                if (isUserID)
                    cart = db.Carts.FirstOrDefault(item => item.UserID == cartID && item.CartStatus == CartStatus.InProgress);
                else
                    cart = db.Carts.FirstOrDefault(item => item.CartGuid == cartID && item.CartStatus == CartStatus.InProgress);

                if (cart == null && insert)
                {
                    cart = new Cart();

                    if (isUserID)
                        cart.UserID = cartID;
                    else
                        cart.CartGuid = cartID;

                    cart.IP = Utilities.GetIP();
                    cart.CartStatus = CartStatus.InProgress;

                    db.Carts.Add(cart);

                    db.SaveChanges();
                }

                return cart;
            }
        }

        public static List<ViewOrder> GetByUserID(string id)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var cart = from item in db.Carts
                           where item.UserID == id && item.CartStatus == CartStatus.Success
                           select new ViewOrder
                           {
                               CartID = item.ID,
                               SendMethodType = item.SendMethodType,
                               PaymentMethodType = item.PaymentMethodType,
                               ToPay = item.ToPay.HasValue ? item.ToPay.Value : 0,
                               Total = item.Total.HasValue ? item.Total.Value : 0,
                               LastUpdate = item.LastUpdate,
                               DateTime = item.DateTime,
                               SendStatus = item.SendStatus,
                               Tax = item.Tax,
                               OrderID = item.OrderID,
                               SaleReferenceID = item.SaleReferenceID
                           };

                return cart.ToList();
            }
        }

        public static int OrdersCount()
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var query = from item in db.Carts
                            where item.UserID != null
                            && item.CartStatus == CartStatus.Success
                            select item;

                return query.Count();
            }
        }

        public static int CountOrdersByUserID(string userID, SendStatus? sendStatus = null)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var query = from item in db.Carts
                            where item.UserID == userID
                            && item.CartStatus == CartStatus.Success
                            select item;

                if (sendStatus.HasValue)
                {
                    query = query.Where(item => item.SendStatus == sendStatus);
                }

                return query.Count();
            }
        }

        public static int NewOrdersCount()
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var query = from item in db.Carts
                            where item.UserID != null
                            && item.CartStatus == CartStatus.Success
                            && item.SendStatus == SendStatus.NotChecked
                            select item;

                return query.Count();
            }
        }

        public static string GetUserByID(int id)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var cart = (from item in db.Carts
                            where item.ID == id
                            select item.UserID).FirstOrDefault();

                return cart;
            }
        }

        public static JsonTrackOrder GetTrackOrderInfo(string saleReferenceID)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var cart = from item in db.Carts
                           where item.SaleReferenceID == saleReferenceID
                           select new JsonTrackOrder
                           {
                               SendStatus = item.SendStatus,
                               BillNumber = item.BillNumber,
                               SendMethodType = item.SendMethodType
                           };

                return cart.FirstOrDefault();
            }
        }

    }
}
