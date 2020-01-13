using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OnlineStore.DataLayer;
using OnlineStore.Models;
using OnlineStore.Providers.Controllers;
using OnlineStore.Models.Admin;

namespace OnlineStore.Website.Areas.Admin.Controllers
{
    public class HomeBoxProductsController : AdminController
    {
        public ActionResult Index(int homeBoxID)
        {
            ViewBag.Title = "مدیریت " + HomeBoxes.GetByID(homeBoxID).Title;

            AjaxSettings settings = new AjaxSettings
            {
                Url = "/HomeBoxProducts/Search"
            };

            return View(model: settings);
        }

        [HttpPost]
        public JsonResult Get(int homeBoxID)
        {
            var jsonSuccessResult = new JsonSuccessResult();

            try
            {
                var list = HomeBoxProducts.Get(homeBoxID);

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

        public JsonResult Update(int homeBoxID, string products)
        {
            var jsonSuccessResult = new JsonSuccessResult();

            try
            {
                string[] arrProducts = products.Split(',');

                // حذف
                #region Delete All

                HomeBoxProducts.DeleteBoxItems(homeBoxID);

                #endregion Delete All

                // ثبت مجدد
                #region Add

                List<HomeBoxProduct> listItems = new List<HomeBoxProduct>();

                foreach (var item in arrProducts)
                {
                    if (!String.IsNullOrWhiteSpace(item))
                    {
                        HomeBoxProduct product = new HomeBoxProduct
                        {
                            HomeBoxID = homeBoxID,
                            ProductID = Int32.Parse(item),
                            LastUpdate = DateTime.Now,
                        };

                        listItems.Add(product);
                    }
                }

                HomeBoxProducts.Insert(listItems);

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