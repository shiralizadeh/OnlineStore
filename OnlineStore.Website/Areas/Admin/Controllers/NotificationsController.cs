using OnlineStore.Providers.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OnlineStore.Website.Areas.Admin.Controllers
{
    public class NotificationsController : AdminController
    {
        public ActionResult Index()
        {
            return View();
        }
    }
}