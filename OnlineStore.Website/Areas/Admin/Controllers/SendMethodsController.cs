using OnlineStore.DataLayer;
using OnlineStore.Models;
using OnlineStore.Providers;
using System;
using System.Web.Mvc;
using OnlineStore.Providers.Controllers;
using OnlineStore.Models.Enums;

namespace OnlineStore.Website.Areas.Admin.Controllers
{
    public class SendMethodsController : AdminController
    {
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public JsonResult Get(int pageIndex, int pageSize, string pageOrder)
        {
            var list = SendMethods.Get(pageIndex, pageSize, pageOrder);

            int total = SendMethods.Count();
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
                SendMethods.Delete(id);
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
            SendMethod sendMethod;

            if (id.HasValue)
                sendMethod = SendMethods.GetByID(id.Value);
            else
                sendMethod = new SendMethod();

            return View(sendMethod);
        }

        [HttpPost]
        public ActionResult Edit(SendMethod sendMethod)
        {
            try
            {
                var files = Utilities.SaveFiles(Request.Files, Utilities.GetNormalFileName(sendMethod.Title), StaticPaths.SendMethods);

                if (files.Count > 0)
                    sendMethod.Filename = files[0].Title;

                sendMethod.LastUpdate = DateTime.Now;

                ViewBag.Success = true;

                if (sendMethod.ID == -1)
                {
                    SendMethods.Insert(sendMethod);

                    UserNotifications.Send(UserID, String.Format("جدید - روش ارسال '{0}'", sendMethod.Title), "/Admin/SendMethods/Edit/" + sendMethod.ID, NotificationType.Success);
                    sendMethod = new SendMethod();
                }
                else
                {
                    SendMethods.Update(sendMethod);
                }
            }
            catch (Exception ex)
            {
                SetErrors(ex);
            }

            return ClearView(sendMethod);
        }
    }
}