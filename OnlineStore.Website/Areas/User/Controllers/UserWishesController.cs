using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OnlineStore.DataLayer;
using OnlineStore.Providers.Controllers;
using OnlineStore.Providers;
using OnlineStore.Models.User;

namespace OnlineStore.Website.Areas.User.Controllers
{
    [RoutePrefix("My-Account")]
    public class UserWishesController : UserController
    {
        [Route("My-Wishes")]
        public ActionResult Index()
        {
            var model = UserWishes.GetByUserID(UserID);

            Products.FillProductItems(UserID, model, StaticValues.DefaultProductImageSize);

            return View("/Areas/User/Views/UserWishes/index.cshtml", model);
        }

    }
}