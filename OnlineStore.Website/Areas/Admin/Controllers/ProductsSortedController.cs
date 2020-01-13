using OnlineStore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OnlineStore.DataLayer;
using OnlineStore.Providers.Controllers;
using OnlineStore.Providers;

namespace OnlineStore.Website.Areas.Admin.Controllers
{
    public class ProductsSortedController : AdminController
    {
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public JsonResult GetProducts(List<int> groupIDs)
        {
            var jsonSuccessResult = new JsonSuccessResult();

            try
            {
                var list = Products.GetByGroupIDs(groupIDs: groupIDs);

                foreach (var item in list)
                {
                    item.ImageFile = UrlProvider.GetProductImage(item.ImageFile, StaticValues.DefaultProductImageSize);
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

        [HttpPost]
        public JsonResult SetOrder(int productID, string orderID)
        {
            var jsonSuccessResult = new JsonSuccessResult();

            try
            {
                int intOrder;
                bool order = Int32.TryParse(orderID, out intOrder);

                if (order)
                {
                    Products.UpdateOrderID(productID, intOrder);
                    jsonSuccessResult.Success = true;
                }
                else
                {
                    jsonSuccessResult.Success = false;
                }
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