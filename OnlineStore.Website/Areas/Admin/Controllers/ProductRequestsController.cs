using OnlineStore.DataLayer;
using OnlineStore.Models;
using System;
using System.Web.Mvc;
using OnlineStore.Providers.Controllers;
using System.Threading.Tasks;
using OnlineStore.Models.Enums;

namespace OnlineStore.Website.Areas.Admin.Controllers
{
    public class ProductRequestsController : AdminController
    {
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public JsonResult Get(int pageIndex, int pageSize, string pageOrder, int productID, sbyte productRequestStatus)
        {
            ProductRequestStatus? status = null;

            if (productRequestStatus != -1)
            {
                status = (ProductRequestStatus)productRequestStatus;
            }

            if (pageOrder.Trim() == "ID")
            {
                pageOrder = "LastUpdate desc";
            }

            var list = ProductRequests.Get(pageIndex,
                                           pageSize,
                                           pageOrder,
                                           productID,
                                           status
                                           );

            int total = ProductRequests.Count(productID, status);
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
                ProductRequests.Delete(id);
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

        [HttpGet]
        public ActionResult Edit(int id)
        {
            ProductRequest request;

            request = ProductRequests.GetByID(id);

            return View(model: request);
        }

        [HttpPost]
        public ActionResult Edit(ProductRequest request)
        {
            try
            {
                request.LastUpdate = DateTime.Now;
                request.ProductRequestStatus = request.ProductRequestStatus;

                ViewBag.Success = true;

                ProductRequests.Update(request);

            }
            catch (Exception ex)
            {
                SetErrors(ex);
            }

            return RedirectToAction("Index");
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
    }
}