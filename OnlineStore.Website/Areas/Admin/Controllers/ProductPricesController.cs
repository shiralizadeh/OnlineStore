using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OnlineStore.Models.Admin;
using OnlineStore.DataLayer;
using OnlineStore.Models;
using OnlineStore.Providers;
using OnlineStore.Providers.Controllers;

namespace OnlineStore.Website.Areas.Admin.Controllers
{
    public class ProductPricesController : AdminController
    {
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public JsonResult Search(string title, List<int> groupIDs)
        {
            var jsonSuccessResult = new JsonSuccessResult();

            try
            {
                var list = Products.GetByGroupIDs(title, groupIDs);

                foreach (var item in list)
                {
                    item.ImageFile = UrlProvider.GetProductImage(item.ImageFile, StaticValues.DefaultProductImageSize);
                    item.Varients = ProductVarients.GetShortVarientByProductID(item.ID);
                }

                jsonSuccessResult.Success = true;
                jsonSuccessResult.Data = list;
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

    }
}