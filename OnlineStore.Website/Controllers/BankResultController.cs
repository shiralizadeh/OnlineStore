using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OnlineStore.Models.Public;
using OnlineStore.DataLayer;
using AutoMapper;
using OnlineStore.Identity;
using OnlineStore.Providers;
using OnlineStore.Models.Enums;
using OnlineStore.Services;
using System.Configuration;

using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using OnlineStore.Website.Models;
using OnlineStore.Providers.Controllers;
using System.Text;

namespace OnlineStore.Website.Controllers
{
    public class BankResultController : PublicController
    {
        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        public async Task<ActionResult> Index(string refID, int resCode, string saleOrderID, string saleReferenceID)
        {
            var cart = Carts.GetByOrderID(saleOrderID);

            string resSettle = String.Empty,
                   resVerify = String.Empty;

            var cartStatus = CartStatus.Fail;

            if (resCode == 0)
            {
                cartStatus = CartStatus.Success;
            }

            updateCart(ref resSettle,
                       ref resVerify,
                       refID,
                       saleOrderID,
                       saleReferenceID,
                       resCode,
                       cartStatus,
                       cart.ID);

            logPaymentData(resSettle,
                           resVerify,
                           saleReferenceID,
                           saleOrderID,
                           resCode,
                           cartStatus);

            var user = OSUsers.GetByID(cart.UserID);

            if (!User.Identity.IsAuthenticated)
            {
                await SignInAsync(user, true);
            }

            if (resCode == 0)
            {
                sendMessage(user, saleReferenceID, cart);
            }

            return Redirect("My-Account/My-Orders/Factor/" + cart.ID);
        }

        private static void sendMessage(OSUser user, string saleReferenceID, Cart cart)
        {

            if (!String.IsNullOrWhiteSpace(user.Mobile))
            {
                SMSServices.SuccessfullPayment(user.Firstname,
                                               user.Lastname,
                                               saleReferenceID,
                                               user.Mobile,
                                               user.Id);
                // اطلاع رسانی به مدیر
                CartController.NotifyNewOrder(user, cart, saleReferenceID);

            }

            EmailServices.SuccessfullPayment(user.Firstname,
                                             user.Lastname,
                                             saleReferenceID,
                                             user.Email,
                                             user.Id);
        }

        private static void updateCart(ref string resSettle,
                                       ref string resVerify,
                                       string refID,
                                       string saleOrderID,
                                       string saleReferenceID,
                                       int resCode,
                                       CartStatus cartStatus,
                                       int cartID)
        {

            Cart updateStatus = new Cart();

            // پرداخت موفقیت آمیز
            if (resCode == 0)
            {
                resVerify = verifyRequest(saleOrderID, saleReferenceID);
                resSettle = settleRequest(saleOrderID, saleReferenceID);

                updateStatus.SaleReferenceID = saleReferenceID;
            }

            // ویرایش اطلاعات پرداخت
            updateStatus.CartStatus = cartStatus;
            updateStatus.ID = cartID;
            updateStatus.LastUpdate = DateTime.Now;
            updateStatus.ResCode = resCode;
            updateStatus.RefID = refID;

            Carts.UpdatePaymentInfo(updateStatus);
        }

        private static void logPaymentData(string resSettle,
                                           string resVerify,
                                           string saleReferenceID,
                                           string orderID,
                                           int resultCode,
                                           CartStatus paymentStatus)
        {
            var bankData = new BankData
            {
                ResCode = resultCode,
                SaleReferenceID = resultCode == 0 ? saleReferenceID : "-1",
                SettleCode = resultCode == 0 ? Int32.Parse(resSettle) : -1,
                VerifyCode = resultCode == 0 ? Int32.Parse(resVerify) : -1
            };

            string data = Newtonsoft.Json.JsonConvert.SerializeObject(bankData);

            var paymentLog = new PaymentLog
            {
                KeyID = orderID,
                SettleDate = DateTime.Now,
                PaymentStatus = paymentStatus,
                Data = data,
                LastUpdate = DateTime.Now
            };

            PaymentLogs.UpdateByOrderID(paymentLog);
        }

        /// <summary>
        /// درخواست واریز وجه از بانک
        /// </summary>
        /// <returns>کد نتیجه</returns>
        private static string settleRequest(string saleOrderID, string saleReferenceId)
        {
            Int64 saleID = Int64.Parse(saleOrderID);

            var bpService = new BankMellat.PaymentGatewayClient();

            return bpService.bpSettleRequest(Int64.Parse(StaticValues.TerminalId),
                                                         StaticValues.UserName,
                                                         StaticValues.UserPassword,
                                                         saleID,
                                                         saleID,
                                                         Int64.Parse(saleReferenceId));

        }

        /// <summary>
        /// درخواست تایید پرداخت وجه
        /// </summary>
        /// <returns>کد نتیجه</returns>
        private static string verifyRequest(string saleOrderID, string saleReferenceId)
        {
            Int64 saleID = Int64.Parse(saleOrderID);

            var bpService = new BankMellat.PaymentGatewayClient();
            return bpService.bpVerifyRequest(Int64.Parse(StaticValues.TerminalId),
                                                         StaticValues.UserName,
                                                         StaticValues.UserPassword,
                                                         saleID,
                                                         saleID,
                                                         Int64.Parse(saleReferenceId));

        }

        private async Task SignInAsync(OSUser user, bool isPersistent)
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ExternalCookie);
            AuthenticationManager.SignIn(new AuthenticationProperties() { IsPersistent = isPersistent, AllowRefresh = true, ExpiresUtc = DateTimeOffset.Now.AddDays(2) }, await user.GenerateUserIdentityAsync(UserManager));
        }

    }
}