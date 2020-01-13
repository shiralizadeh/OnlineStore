using OnlineStore.DataLayer;
using OnlineStore.Models;
using System;
using System.Web.Mvc;
using OnlineStore.Providers.Controllers;
using System.Threading.Tasks;
using OnlineStore.Models.Enums;
using OnlineStore.Providers;
using System.Collections.Generic;

namespace OnlineStore.Website.Areas.Admin.Controllers
{
    public class ProductVisitsController : AdminController
    {
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public JsonResult Get(int pageIndex,
                              int pageSize,
                              string pageOrder,
                              int productID,
                              string fromDate,
                              string toDate,
                              List<int> groups
                              )
        {
            DateTime? sDate = null,
                      eDate = null;

            if (!String.IsNullOrWhiteSpace(fromDate))
                sDate = Utilities.ToEnglishDate(fromDate).Date;

            if (!String.IsNullOrWhiteSpace(toDate))
                eDate = Utilities.ToEnglishDate(toDate).Date;

           
            var list = ProductVisits.Get(pageIndex,
                                           pageSize,
                                           pageOrder,
                                           productID,
                                           sDate,
                                           eDate,
                                           groups
                                           );

            int total = ProductVisits.Count(productID, sDate, eDate, groups);
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

        [HttpPost]
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
    }
}