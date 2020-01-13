using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OnlineStore.Models.User;
using OnlineStore.Providers.Controllers;
using OnlineStore.DataLayer;
using OnlineStore.Models.Public;

namespace OnlineStore.Website.Areas.User.Controllers
{
    [RoutePrefix("My-Account/My-Orders")]
    public class OrdersController : UserController
    {
        string url = "/Areas/User/Views/Orders/";

        [Route]
        public ActionResult Index()
        {
            var model = Carts.GetByUserID(UserID);

            return View(url + "Index.cshtml", model: model);
        }

        [Route("Details/{id:int}")]
        public ActionResult OrderItems(int id)
        {
            var model = CartItems.GetOrderItems(id);

            return View(url + "OrderItems.cshtml", model: model);
        }

    }
}