using OnlineStore.DataLayer;
using OnlineStore.Models.Public;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using OnlineStore.Providers;
using OnlineStore.Models;
using OnlineStore.Providers.Controllers;
using OnlineStore.EntityFramework;
using OnlineStore.Identity;
using AutoMapper;
using Microsoft.AspNet.Identity.EntityFramework;
using OnlineStore.Models.Enums;
using OnlineStore.Models.User;
using System.Configuration;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;
using OnlineStore.Services;
using System.Text;

namespace OnlineStore.Website.Controllers
{
    public class CartController : PublicController
    {
        [Route("Cart")]
        public ActionResult Index()
        {
            var activeSendMethods = SendMethods.GetActiveMethods();
            var activePaymentMethods = PaymentMethods.GetActiveMethods();
            ViewBuyerInfo buyer = new ViewBuyerInfo();

            if (User.Identity.IsAuthenticated)
            {
                var user = OSUsers.GetByID(UserID);

                #region Mapp To Buyer

                buyer.Firstname = user.Firstname;
                buyer.Lastname = user.Lastname;
                buyer.Email = user.Email;

                // TODO: UserAddresses
                buyer.Phone = user.Phone;
                buyer.Mobile = user.Mobile;
                buyer.HomeAddress = user.HomeAddress;
                buyer.PostalCode = user.PostalCode;
                if (user.StateID.HasValue)
                {
                    buyer.StateName = Cities.GetCityName(user.StateID.Value);
                }
                if (user.CityID.HasValue)
                {
                    buyer.CityName = Cities.GetCityName(user.CityID.Value);
                }

                #endregion Mapp To Buyer
            }

            var send = Mapper.Map<List<ViewSendMethod>>(activeSendMethods);
            var payment = Mapper.Map<List<ViewPaymentMethod>>(activePaymentMethods);

            CartSettings cart = new CartSettings
            {
                PaymentMethods = payment,
                SendMethods = send,
                IsAuthentication = User.Identity.IsAuthenticated,
                BuyerInfo = buyer
            };

            return View(cart);
        }

        public ActionResult Add(
            int? productVarientID = null,
            int? productID = null,
            int? packageID = null,
            byte quantity = 1)
        {
            var jsonSuccessResult = new JsonSuccessResult();

            try
            {
                Cart cart = CartController.GetCart(HttpContext);

                bool exists = true;
                bool canAdd = true;

                if (!CartItems.Exists(cart.ID, productVarientID, productID, packageID))
                {
                    canAdd = Products.CanAddToCart(productVarientID, productID, packageID);

                    if (canAdd)
                    {
                        var cartItem = new CartItem();

                        cartItem.CartID = cart.ID;

                        cartItem.ProductVarientID = productVarientID;
                        cartItem.ProductID = productID;
                        cartItem.PackageID = packageID;

                        cartItem.DateTime = DateTime.Now;
                        cartItem.Quantity = quantity;
                        cartItem.Price = -1;

                        CartItems.Insert(cartItem);
                    }

                    exists = false;
                }

                var cartItems = CartItems.GetByCartID(cart.ID, UserID);

                int total = 0,
                    totalDiscount = 0;

                foreach (var item in cartItems)
                {
                    totalDiscount += item.Quantity * (item.DiscountPercent > 0 ? item.DiscountPrice : item.Price);
                    total += item.Quantity * item.Price;
                }

                jsonSuccessResult.Data = new
                {
                    Exists = exists,
                    CanAdd = canAdd,
                    CartItems = cartItems,
                    Total = total,
                    TotalDiscount = totalDiscount,
                };
                jsonSuccessResult.Success = true;
            }
            catch (DbException ex)
            {
                jsonSuccessResult.Errors = ex.Errors.ToArray();
                jsonSuccessResult.Success = false;
            }
            catch (Exception ex)
            {
                jsonSuccessResult.Errors = new string[] { ex.Message };
                jsonSuccessResult.Success = false;
            }

            return new JsonResult()
            {
                Data = jsonSuccessResult
            };
        }

        public ActionResult Remove(
            int? productVarientID = null,
            int? productID = null,
            int? packageID = null)
        {
            var jsonSuccessResult = new JsonSuccessResult();

            Cart cart;
            try
            {
                cart = CartController.GetCart(HttpContext);

                bool exists = false;
                if (CartItems.Exists(cart.ID, productVarientID, productID, packageID))
                {
                    CartItems.Delete(cart.ID, productVarientID, productID, packageID);

                    exists = true;
                }

                var cartItems = CartItems.GetByCartID(cart.ID, UserID);

                int total = 0,
                    totalDiscount = 0;

                foreach (var item in cartItems)
                {
                    totalDiscount += item.Quantity * (item.DiscountPercent > 0 ? item.DiscountPrice : item.Price);
                    total += item.Quantity * item.Price;
                }

                jsonSuccessResult.Data = new
                {
                    Exists = exists,
                    CartItems = cartItems,
                    Total = total,
                    TotalDiscount = totalDiscount,
                };
                jsonSuccessResult.Success = true;
            }
            catch (DbException ex)
            {
                jsonSuccessResult.Errors = ex.Errors.ToArray();
                jsonSuccessResult.Success = false;
            }
            catch (Exception ex)
            {
                jsonSuccessResult.Errors = new string[] { ex.Message };
                jsonSuccessResult.Success = false;
            }

            return new JsonResult()
            {
                Data = jsonSuccessResult
            };
        }

        public ActionResult Changed(int quantity, int? productVarientID = null, int? productID = null)
        {
            var jsonSuccessResult = new JsonSuccessResult();

            Cart cart;
            try
            {
                cart = CartController.GetCart(HttpContext);

                CartItem cartItem = CartItems.GetByProductVarientID(cart.ID, productVarientID, productID);

                if (cartItem != null)
                {
                    cartItem.Quantity = quantity;

                    CartItems.Update(cartItem);
                }

                var cartItems = CartItems.GetByCartID(cart.ID, UserID);

                int total = 0,
                    totalDiscount = 0;

                foreach (var item in cartItems)
                {
                    totalDiscount += item.Quantity * (item.DiscountPercent > 0 ? item.DiscountPrice : item.Price);
                    total += item.Quantity * item.Price;
                }

                jsonSuccessResult.Data = new
                {
                    CartItems = cartItems,
                    Total = total,
                    TotalDiscount = totalDiscount,
                };
                jsonSuccessResult.Success = true;
            }
            catch (DbException ex)
            {
                jsonSuccessResult.Errors = ex.Errors.ToArray();
                jsonSuccessResult.Success = false;
            }
            catch (Exception ex)
            {
                jsonSuccessResult.Errors = new string[] { ex.Message };
                jsonSuccessResult.Success = false;
            }

            return new JsonResult()
            {
                Data = jsonSuccessResult
            };
        }

        public static Cart GetCart(HttpContextBase context)
        {
            var User = context.User;
            var UserID = User.Identity.GetUserId();
            var Request = context.Request;
            var Response = context.Response;

            Cart cart;

            if (User.Identity.IsAuthenticated)
            {
                cart = Carts.GetOrInsert(UserID, true);
            }
            else
            {
                string cartGuid = Guid.NewGuid().ToString();
                if (Request.Cookies["CartGuid"] == null)
                {
                    var cookie = new HttpCookie("CartGuid");
                    cookie.Expires = DateTime.Now.AddDays(30);
                    cookie.Value = cartGuid;

                    Response.Cookies.Add(cookie);
                }
                else
                {
                    cartGuid = Request.Cookies["CartGuid"].Value;
                }

                cart = Carts.GetOrInsert(cartGuid, false);
            }

            return cart;
        }

        [HttpPost]
        public JsonResult Payment(OSUser osUser, SendMethodType sendMethodType, PaymentMethodType paymentMethodType, string userDescription)
        {
            var jsonSuccessResult = new JsonSuccessResult();

            try
            {
                var payDate = DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString().PadLeft(2, '0') + DateTime.Now.Day.ToString().PadLeft(2, '0');
                var payTime = DateTime.Now.Hour.ToString().PadLeft(2, '0') + DateTime.Now.Minute.ToString().PadLeft(2, '0') + DateTime.Now.Second.ToString().PadLeft(2, '0');
                var orderID = payDate + payTime;

                string userID;
                int? cityID;

                if (User.Identity.IsAuthenticated)
                {
                    userID = UserID;
                    cityID = OSUsers.GetByID(UserID).CityID;
                }
                else
                {
                    cityID = osUser.CityID;

                    IdentityResult result = registerUser(ref osUser);

                    if (result.Succeeded)
                    {
                        UserNotifications.Send(StaticValues.AdminID, String.Format("ثبت نام کاربر - در فرم سبد خرید '{0}'", osUser.UserName), "/Admin/OSUsers/Edit/" + osUser.Id, NotificationType.Success);
                        userID = osUser.Id;
                    }
                    else
                        throw new Exception(result.Errors.Aggregate((a, b) => a + ", " + b));
                }

                int total = 0,
                    totalDiscount = 0,
                    delivaryPrice = -1;

                var cart = GetCart(HttpContext);
                var cartItems = CartItems.GetByCartID(cart.ID, (User.Identity.IsAuthenticated ? userID : null));
                var listGifts = new List<CartItemGift>();

                foreach (var item in cartItems)
                {
                    var price = (item.DiscountPercent > 0 ? item.DiscountPrice : item.Price);

                    #region Update CartItem

                    var cartItem = CartItems.GetByID(item.ID);
                    cartItem.Price = price;
                    cartItem.IsFreeDelivery = item.IsFreeDelivery;
                    CartItems.Update(cartItem);

                    #endregion Update CartItem

                    totalDiscount += item.Quantity * price;
                    total += item.Quantity * item.Price;

                    foreach (var gift in item.Gifts)
                    {
                        listGifts.Add(new CartItemGift
                        {
                            CartItemID = item.ID,
                            GiftID = gift.GiftID,
                            Price = gift.Price,
                            LastUpdate = DateTime.Now
                        });
                    }
                }

                if (
                    (sendMethodType == SendMethodType.Free && cityID == 468) || // مشهد
                    (StaticValues.MaxPriceFreeDelivery && totalDiscount >= 10000) ||// طرح های بالای ارسال
                    (cartItems.Any(a => a.IsFreeDelivery)) // محصولات دارای ارسال رایگان
                   )
                    delivaryPrice = 50000;
                else
                    delivaryPrice = StaticValues.DeliveryPrice;

                // ثبت هدایا
                if (listGifts.Count > 0)
                {
                    CartItemGifts.Insert(listGifts);
                }

                var toPay = (totalDiscount + delivaryPrice);

                string refID = String.Empty;

                if (paymentMethodType == PaymentMethodType.Online)
                {
                    Logs.Alert(Utilities.GetIP(), "PaymentMethodType.Online", String.Format("payDate: {0}, payTime: {1}, orderID: {2}, toPay: {3}", payDate, payTime, orderID, toPay));
                    refID = connectToMellat(payDate, payTime, orderID, toPay);
                }

                #region Update Cart

                cart.UserID = userID;
                cart.SendMethodType = sendMethodType;
                cart.PaymentMethodType = paymentMethodType;
                cart.Tax = Int32.Parse(StaticValues.Tax);
                cart.IP = Utilities.GetIP();
                cart.UserDescription = userDescription;

                cart.Total = total;
                cart.DelivaryPrice = delivaryPrice;
                cart.TotalDiscount = totalDiscount;
                cart.ToPay = toPay;

                cart.DateTime = cart.LastUpdate = DateTime.Now;
                cart.CartGuid = null;

                if (paymentMethodType == PaymentMethodType.Online)
                    cart.CartStatus = CartStatus.DuringPay;
                else if (paymentMethodType == PaymentMethodType.Card || paymentMethodType == PaymentMethodType.Home)
                    cart.CartStatus = CartStatus.FuturePay;

                cart.SendStatus = SendStatus.NotChecked;
                cart.OrderID = orderID;

                Carts.Update(cart);

                #endregion Update Cart

                logPaymentData(orderID, toPay, cart.ID);

                jsonSuccessResult.Success = true;

                if (paymentMethodType == PaymentMethodType.Online)
                    jsonSuccessResult.Data = new
                    {
                        PgwSite = StaticValues.PgwSite,
                        RefID = refID
                    };
                else
                {
                    jsonSuccessResult.Data = new
                    {
                        ToPayPrice = toPay
                    };

                    OSUser user;
                    // اطلاع رسانی به مدیر سایت
                    if (User.Identity.IsAuthenticated)
                        user = OSUsers.GetByID(UserID);
                    else
                        user = osUser;

                    NotifyNewOrder(user, cart, "-1");
                }
            }
            catch (DbException ex)
            {
                jsonSuccessResult.Errors = ex.Errors.ToArray();
                jsonSuccessResult.Success = false;

                Logs.Alert(Utilities.GetIP(), "Payment Error", ex.Errors.Aggregate((a, b) => a + ", " + b) + "_" + ex.StackTrace, LogType.Error);
            }
            catch (Exception ex)
            {
                jsonSuccessResult.Errors = new string[] { ex.Message };
                jsonSuccessResult.Success = false;

                Logs.Alert(Utilities.GetIP(), "Payment Error", ex.Message + "_" + ex.StackTrace, LogType.Error);
            }

            return new JsonResult()
            {
                Data = jsonSuccessResult,
            };
        }

        #region Methods

        private IdentityResult registerUser(ref OSUser osUser)
        {
            if (UserManager.FindByEmail(osUser.Email) == null)
            {
                osUser.UserName = osUser.Email;
                osUser.LastUpdate = DateTime.Now;
                osUser.IsActive = true;

                // اختصاص کد کاربر
                osUser.Id = Guid.NewGuid().ToString();

                // اختصاص نقش Public به کاربر
                osUser.Roles.Add(new IdentityUserRole() { RoleId = StaticValues.PublicRoleID, UserId = osUser.Id });

                // ایجاد کاربر
                var password = new Random().Next(100000, 999999).ToString();
                var result = UserManager.Create(osUser, password);

                if (!String.IsNullOrWhiteSpace(osUser.Mobile))
                {
                    SMSServices.Register(osUser.Firstname,
                                         osUser.Lastname,
                                         osUser.UserName,
                                         password,
                                         osUser.Mobile,
                                         osUser.Id);
                }

                EmailServices.Register(osUser.Firstname,
                                       osUser.Lastname,
                                       osUser.UserName,
                                       password,
                                       osUser.Email,
                                       osUser.Id);

                return result;
            }
            else
            {
                osUser = UserManager.FindByEmail(osUser.Email);

                return IdentityResult.Success;
            }
        }

        private string connectToMellat(string payDate, string payTime, string orderID, int toPay)
        {
            BypassCertificateError();

            var bpService = new BankMellat.PaymentGatewayClient();

            string result = bpService.bpPayRequest(Int64.Parse(StaticValues.TerminalId),
                                                   StaticValues.UserName,
                                                   StaticValues.UserPassword,
                                                   Int64.Parse(orderID),
                                                   toPay,
                                                   payDate,
                                                   payTime,
                                                   String.Empty,
                                                   StaticValues.CallBackUrl,
                                            0);

            String[] resultArray = result.Split(',');

            List<string> model = new List<string>();

            if (resultArray[0] == "0")
            {
                return resultArray[1];
            }
            else
            {
                throw new Exception("result: " + result);
            }
        }

        private static void logPaymentData(string orderID, int toPay, int cartID)
        {
            var now = DateTime.Now;

            PaymentLog log = new PaymentLog
            {
                CartID = cartID,
                IP = Utilities.GetIP(),
                BankType = BankType.Mellat,
                ConnectDate = now,
                PaymentStatus = CartStatus.InProgress,
                KeyID = orderID,
                Price = toPay,
                LastUpdate = now
            };

            PaymentLogs.Insert(log);
        }

        void BypassCertificateError()
        {
            ServicePointManager.ServerCertificateValidationCallback +=
                delegate (
                    Object sender1,
                    X509Certificate certificate,
                    X509Chain chain,
                    SslPolicyErrors sslPolicyErrors)
                {
                    return true;
                };
        }

        public static void NotifyNewOrder(OSUser user, Cart cart, string saleReferenceID)
        {
            var cartItems = CartItems.GetOrderDetails(cart.ID);

            var stateTitle = Cities.GetCityName(user.StateID.Value);
            var cityTitle = Cities.GetCityName(user.CityID.Value);

            StringBuilder smsBody = new StringBuilder();
            StringBuilder emailBody = new StringBuilder();

            #region SMS Body

            smsBody.Append("سفارش جدید: \n" + cart.ToPay.Value.ToPrice() + "\n");

            foreach (var item in cartItems)
            {
                smsBody.Append(item.ProductTitle);
                if (item.ProductVarentID.HasValue)
                {
                    smsBody.Append(" " + item.VarientTitle);
                }
                smsBody.Append("\n");
            }

            smsBody.Append(cart.PaymentMethodType.GetEnumDisplay() + "\n" + cart.SendMethodType.GetEnumDisplay());

            #endregion SMS Body

            #region Email Body

            emailBody.Append("سفارش جدید: <br/>" + cart.ToPay.Value.ToPrice() + "<br/>");
            emailBody.Append("محصولات: <br/>");

            foreach (var item in cartItems)
            {
                emailBody.Append(item.ProductTitle);
                if (item.ProductVarentID.HasValue)
                {
                    emailBody.Append(" " + item.VarientTitle);
                }
                emailBody.Append("<br/>");
            }

            emailBody.AppendFormat("نام و نام خانوادگی: {0} <br/>" +
                              "{1}" +
                              "شماره همراه: {2} <br/>" +
                              "روش پرداخت: {3} <br/>" +
                              "روش ارسال: {4} <br/>" +
                              "استان: {5} <br/>" +
                              "شهر: {6} <br/>" +
                              "آدرس: {7}",
                              user.Firstname + " " + user.Lastname,
                              saleReferenceID != "-1" ? "کد رهگیری: " + saleReferenceID + "<br/>" : String.Empty,
                              user.Mobile,
                              cart.PaymentMethodType.GetEnumDisplay(),
                              cart.SendMethodType.GetEnumDisplay(),
                              stateTitle,
                              cityTitle,
                              user.HomeAddress);

            #endregion Email Body

            EmailServices.NotifyAdminsByEmail(AdminEmailType.NewOrder, emailBody.ToString(), user.Id);
            SMSServices.SendSMS("09120062417", smsBody.ToString(), user.Id);
        }

        #endregion Methods
    }
}