using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OnlineStore.DataLayer;
using OnlineStore.Providers.Controllers;

namespace OnlineStore.Website.Areas.User.Controllers
{
    [RoutePrefix("My-Account/My-Likes")]
    public class LikesController : UserController
    {
        [Route]
        public ActionResult Index()
        {
            var model = ProductCommentRates.GetByUserID(UserID);

            return View("/Areas/User/Views/Likes/Index.cshtml", model);
        }
    }
}