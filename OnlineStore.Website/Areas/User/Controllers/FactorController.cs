using OnlineStore.DataLayer;
using OnlineStore.Identity;
using OnlineStore.Models.Enums;
using OnlineStore.Models.Public;
using OnlineStore.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OnlineStore.Providers.Controllers;
using AutoMapper;

namespace OnlineStore.Website.Areas.User.Controllers
{
    [RoutePrefix("My-Account/My-Orders")]
    public class FactorController : UserController
    {
        [Route("Factor/{id}")]
        public ActionResult Index(int id)
        {
            FactorSettings factor;

            var cart = Carts.GetFactor(id);

            ViewBag.Message = Utilities.MellatBankResult(cart.ResCode.ToString());

            // پرداخت موفقیت آمیز
            if (cart.CartStatus == CartStatus.Success || cart.CartStatus == CartStatus.FuturePay)
            {
                #region Success Payment

                ViewBag.Message += "<br/> کد رهگیری: " + cart.SaleReferenceID;

                var cartItems = CartItems.GetOrderItems(cart.ID);
                var user = OSUsers.GetByID(cart.UserID);

                #region Mapp To Buyer

                var buyer = Mapper.Map<ViewBuyerInfo>(user);

                buyer.StateName = user.StateID.HasValue ? Cities.GetCityName(user.StateID.Value) : String.Empty;
                buyer.CityName = user.CityID.HasValue ? Cities.GetCityName(user.CityID.Value) : String.Empty;

                #endregion Mapp To Buyer

                factor = new FactorSettings
                {
                    IsSuccess = true,
                    CartItems = cartItems,
                    FactorInfo = cart,
                    BuyerInfo = buyer,
                    CompanyEmail = StaticValues.Email,
                    CompanyLogo = StaticValues.Logo,
                    CompanyName = StaticValues.WebsiteTitle,
                    CompanyWebsite = StaticValues.WebsiteUrl,
                    CompanyPhone = StaticValues.Phone
                };

                #endregion Success Payment
            }
            else
            {
                #region Fail Payment

                factor = new FactorSettings
                {
                    IsSuccess = false
                };

                #endregion Fail Payment
            }

            return View("/Areas/User/Views/Factor/Index.cshtml", model: factor);

        }
    }
}