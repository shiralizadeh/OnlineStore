using Microsoft.AspNet.Identity;
using OnlineStore.DataLayer;
using OnlineStore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using OnlineStore.Providers.Controllers;

namespace OnlineStore.Website.Areas.Admin.Controllers
{
    public class ProductSuggestionsController : AdminController
    {
        public ActionResult Index(int? productID)
        {
            ViewBag.ProductID = productID;

            return View();
        }

        [HttpPost]
        public async Task<JsonResult> Get(int pageIndex, int pageSize, string pageOrder, int? productID, string friendEmail, string message)
        {
            var list = ProductSuggestions.Get(pageIndex,
                                           pageSize,
                                           pageOrder,
                                           productID,
                                           friendEmail,
                                           message);
            foreach (var item in list)
            {
                item.UserName = item.UserID != null
                                 ? (await UserManager.FindByIdAsync(item.UserID)).UserName
                                 : "نامعلوم";
            }

            int total = ProductSuggestions.Count(productID, friendEmail, message);
            int totalPage = (int)Math.Ceiling((decimal)total / pageSize);

            if (pageSize > total)
                pageSize = total;

            if (list.Count < pageSize)
                pageSize = list.Count;

            JsonResult result = new JsonResult()
            {
                Data = new
                {
                    TotalPages = totalPage,
                    PageIndex = pageIndex,
                    PageSize = pageSize,
                    Rows = list
                },
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };

            return result;
        }

        public JsonResult Delete(int id)
        {
            var jsonSuccessResult = new JsonSuccessResult();

            try
            {
                ProductSuggestions.Delete(id);
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