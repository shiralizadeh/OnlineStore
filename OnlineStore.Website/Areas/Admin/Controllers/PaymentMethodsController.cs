using OnlineStore.DataLayer;
using OnlineStore.Models;
using OnlineStore.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OnlineStore.Providers.Controllers;
using OnlineStore.Models.Enums;

namespace OnlineStore.Website.Areas.Admin.Controllers
{
    public class PaymentMethodsController : AdminController
    {
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public JsonResult Get(int pageIndex, int pageSize, string pageOrder)
        {
            var list = PaymentMethods.Get(pageIndex, pageSize, pageOrder);

            int total = PaymentMethods.Count();
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
                PaymentMethods.Delete(id);
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
            PaymentMethod paymentMethod;

            if (id.HasValue)
                paymentMethod = PaymentMethods.GetByID(id.Value);
            else
                paymentMethod = new PaymentMethod();

            return View(paymentMethod);
        }

        [HttpPost]
        public ActionResult Edit(PaymentMethod paymentMethod)
        {
            try
            {
                var files = Utilities.SaveFiles(Request.Files, Utilities.GetNormalFileName(paymentMethod.Title), StaticPaths.PaymentMethods);

                if (files.Count > 0)
                    paymentMethod.Filename = files[0].Title;

                paymentMethod.LastUpdate = DateTime.Now;

                ViewBag.Success = true;

                if (paymentMethod.ID == -1)
                {
                    PaymentMethods.Insert(paymentMethod);

                    UserNotifications.Send(UserID, String.Format("جدید - روش پرداخت '{0}'", paymentMethod.Title), "/Admin/PaymentMethod/Edit/" + paymentMethod.ID, NotificationType.Success);
                    paymentMethod = new PaymentMethod();
                }
                else
                {
                    PaymentMethods.Update(paymentMethod);
                }
            }
            catch (Exception ex)
            {
                SetErrors(ex);
            }

            return ClearView(paymentMethod);
        }
    }
}