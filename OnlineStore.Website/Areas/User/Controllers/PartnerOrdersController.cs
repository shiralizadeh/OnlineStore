using OnlineStore.DataLayer;
using OnlineStore.Models.Enums;
using OnlineStore.Providers.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OnlineStore.Identity;

namespace OnlineStore.Website.Areas.User.Controllers
{
    [RoutePrefix("PartnerOrders")]
    public class PartnerOrdersController : PublicController
    {
        string url = "/Areas/User/Views/PartnerOrders/Index.cshtml";
        [Route]
        public ActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                var user = OSUsers.GetByID(UserID);
                if (String.IsNullOrWhiteSpace(user.Mobile) ||
                    String.IsNullOrWhiteSpace(user.Phone) ||
                    user.StateID == null ||
                    user.CityID == null ||
                    String.IsNullOrWhiteSpace(user.HomeAddress)
                    )
                {
                    ViewBag.UserInfo = false;
                }
            }
            return View(url);
        }

        public ActionResult AddOrder(SpecialOrder specialOrder)
        {
            specialOrder.UserID = UserID;
            specialOrder.SpecialOrderStatus = SpecialOrderStatus.NotChecked;

            SpecialOrders.Insert(specialOrder);

            ViewBag.Success = true;

            return View(url);
        }
    }
}