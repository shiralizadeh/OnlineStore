using OnlineStore.DataLayer;
using OnlineStore.Models;
using OnlineStore.Models.Admin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OnlineStore.Website.Areas.Admin.Controllers
{
    public class ProductKeywordsController : Controller
    {
        public ActionResult Index(int keywordID)
        {
            ViewBag.Title = "مدیریت " + Keywords.GetByID(keywordID).Title;

            AjaxSettings settings = new AjaxSettings
            {
                Url = "/ProductKeywords/Search"
            };

            return View(model: settings);
        }

        [HttpPost]
        public JsonResult Get(int keywordID)
        {
            var jsonSuccessResult = new JsonSuccessResult();

            try
            {
                var list = ProductKeywords.GetByKeywordID(keywordID);

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

        public JsonResult Update(int keywordID, string products)
        {
            var jsonSuccessResult = new JsonSuccessResult();

            try
            {
                string[] arrProducts = products.Split(',');

                var curList = ProductKeywords.GetByKeywordID(keywordID);

                foreach (var product in arrProducts)
                {
                    int productID = Int32.Parse(product);

                    if (!curList.Any(item => item.ProductID == productID
                                          && item.KeywordID == keywordID))
                    {
                        var kp = new ProductKeyword
                        {
                            ProductID = productID,
                            KeywordID = keywordID,
                            LastUpdate = DateTime.Now
                        };

                        ProductKeywords.Insert(kp);
                    }
                    else
                    {
                        curList.Remove(curList.Single(cls => cls.KeywordID == keywordID
                                                          && cls.ProductID == productID));
                    }
                }

                foreach (var item in curList)
                    ProductKeywords.Delete(item.ID);

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