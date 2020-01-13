using OnlineStore.DataLayer;
using OnlineStore.Models;
using OnlineStore.Models.Admin;
using OnlineStore.Providers.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OnlineStore.Website.Areas.Admin.Controllers
{
    public class CopyController : AdminController
    {
        public ActionResult Product()
        {
            ViewBag.Title = "کپی محصول";

            AjaxSettings settings = new AjaxSettings
            {
                Url = "/Copy/SearchProducts"
            };

            return View(model: settings);
        }

        [HttpPost]
        public ActionResult Product(int productID)
        {
            var product = Products.GetByID(productID).Clone(UserID);
            product.CreatedDate = DateTime.Now;

            Products.Insert(product);

            foreach (var item in ProductGroups.GetByProductID(productID))
            {
                ProductGroups.Insert(new ProductGroup()
                {
                    ProductID = product.ID,
                    GroupID = item.GroupID
                });
            }

            foreach (var item in AttributeValues.GetByProductID(productID))
            {
                AttributeValues.Insert(new AttributeValue()
                {
                    ProductID = product.ID,
                    AttributeID = item.AttributeID,
                    AttributeOptionID = item.AttributeOptionID,
                    Value = item.Value
                });
            }

            return Redirect("/Admin/Products/Edit/" + product.ID);
        }

        public JsonResult SearchProducts(string key)
        {
            var jsonSuccessResult = new JsonSuccessResult();

            try
            {
                var list = Products.SimpleSearch(key);

                jsonSuccessResult.Data = list;
                jsonSuccessResult.Success = true;
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