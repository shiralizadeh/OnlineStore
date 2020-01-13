using OnlineStore.DataLayer;
using OnlineStore.Providers;
using OnlineStore.Providers.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OnlineStore.Website.Areas.Admin.Controllers
{
    public class ToolboxController : AdminController
    {
        // GET: Admin/Toolbox
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult UpdateIsUnavailable(string type)
        {
            try
            {
                Products.UpdateIsUnavailable();

                ViewBag.Success = true;
            }
            catch (Exception ex)
            {
                SetErrors(ex);
            }

            return View("Index");
        }

        [HttpPost]
        public ActionResult UpdateAttrOptions(string type)
        {
            try
            {
                StaticValues.GroupOptions = AttributeValues.GetOptionIDsByGroup();
                ViewBag.Success = true;
            }
            catch (Exception ex)
            {
                SetErrors(ex);
            }

            return View("Index");
        }

    }
}