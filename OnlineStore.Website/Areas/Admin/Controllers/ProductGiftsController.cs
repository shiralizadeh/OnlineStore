using OnlineStore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OnlineStore.DataLayer;
using OnlineStore.Providers.Controllers;
using OnlineStore.Models.Admin;
using OnlineStore.Providers;

namespace OnlineStore.Website.Areas.Admin.Controllers
{
    public class ProductGiftsController : AdminController
    {
        public ActionResult Index(int productID)
        {
            string title = Products.GetTitleByID(productID);
            ViewBag.Title = "هدایای « " + title + " » ";
            ViewBag.ProductID = productID;

            AjaxSettings settings = new AjaxSettings
            {
                Url = "/ProductGifts/Search"
            };

            return View(model: settings);
        }

        [HttpPost]
        public JsonResult Get(int productID)
        {
            var jsonSuccessResult = new JsonSuccessResult();

            try
            {
                var list = ProductGifts.Get(productID);

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

        public JsonResult Update(int productID, List<JsonProductGift> products)
        {
            var jsonSuccessResult = new JsonSuccessResult();

            try
            {
                // حذف
                #region Delete All

                ProductGifts.DeleteGifts(productID);

                #endregion Delete All

                // ثبت مجدد
                #region Add

                List<ProductGift> listItems = new List<ProductGift>();

                foreach (var item in products)
                {
                    ProductGift product = new ProductGift
                    {
                        ProductID = productID,
                        GiftID = item.GiftID,
                        StartDate = item.StartDate,
                        EndDate = item.EndDate,
                        LastUpdate = DateTime.Now
                    };

                    listItems.Add(product);
                }

                ProductGifts.Insert(listItems);

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