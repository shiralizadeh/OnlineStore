using OnlineStore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OnlineStore.DataLayer;
using OnlineStore.Providers.Controllers;
using OnlineStore.Models.Admin;

namespace OnlineStore.Website.Areas.Admin.Controllers
{
    public class ProductAccessoriesController : AdminController
    {
        public ActionResult Index(int productID)
        {
            string title = Products.GetTitleByID(productID);
            ViewBag.Title = "لوازم جانبی کالای « " + title + " » ";
            ViewBag.ProductID = productID;

            AjaxSettings settings = new AjaxSettings
            {
                Url = "/ProductAccessories/Search"
            };

            return View(model: settings);
        }

        [HttpPost]
        public JsonResult Get(int productID)
        {
            var jsonSuccessResult = new JsonSuccessResult();

            try
            {
                var list = ProductAccessories.Get(productID);

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

        public JsonResult Search(string key)
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

        public JsonResult Update(int productID, string products)
        {
            var jsonSuccessResult = new JsonSuccessResult();

            try
            {
                string[] arrProducts = products.Split(',');

                // حذف
                #region Delete All

                ProductAccessories.DeleteProductAccessories(productID);

                #endregion Delete All

                // ثبت مجدد
                #region Add

                List<ProductAccessory> listItems = new List<ProductAccessory>();

                foreach (var item in arrProducts)
                {
                    if (!String.IsNullOrWhiteSpace(item))
                    {
                        ProductAccessory product = new ProductAccessory
                        {
                            ProductID = productID,
                            AccessoryID = Int32.Parse(item),
                            LastUpdate = DateTime.Now,
                        };

                        listItems.Add(product);
                    }
                }

                ProductAccessories.Insert(listItems);

                #endregion Add

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