using OnlineStore.DataLayer;
using OnlineStore.Models;
using OnlineStore.Providers;
using System;
using System.Web.Mvc;
using OnlineStore.Providers.Controllers;
using OnlineStore.Models.Enums;
using AutoMapper;
using OnlineStore.Models.Admin;
using System.Linq;
using System.Web;

namespace OnlineStore.Website.Areas.Admin.Controllers
{
    public class PriceListProductsController : AdminController
    {
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public JsonResult Get(int pageIndex, int pageSize, string pageOrder, string title)
        {
            if (pageOrder.Trim() == "ID")
                pageOrder = "OrderID";

            var list = PriceListProducts.Get(pageIndex, pageSize, pageOrder, title);

            int total = PriceListProducts.Count(title);
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
                PriceListProducts.Delete(id);
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

        public ActionResult Edit(int? id)
        {
            PriceListProduct priceListProduct;

            if (id.HasValue)
            {
                priceListProduct = PriceListProducts.GetByID(id.Value);
            }
            else
                priceListProduct = new PriceListProduct();

            return View(priceListProduct);
        }

        [HttpPost]
        public ActionResult Edit(PriceListProduct priceListProduct)
        {
            try
            {
                priceListProduct.LastUpdate = DateTime.Now;

                ViewBag.Success = true;

                if (priceListProduct.ID == -1)
                {
                    PriceListProducts.Insert(priceListProduct);

                    UserNotifications.Send(UserID, String.Format("جدید - محصولات لیست بنک داری '{0}'", priceListProduct.Title), "/Admin/PriceListProducts/Edit/" + priceListProduct.ID, NotificationType.Success);
                    priceListProduct = new PriceListProduct();
                }
                else
                {
                    PriceListProducts.Update(priceListProduct);
                }
            }
            catch (Exception ex)
            {
                SetErrors(ex);
            }

            return ClearView(priceListProduct);
        }

    }
}