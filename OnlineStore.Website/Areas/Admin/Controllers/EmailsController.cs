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
    public class EmailsController : AdminController
    {
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public JsonResult Get(int pageIndex, int pageSize, string pageOrder)
        {
            var list = Emails.Get(pageIndex, pageSize, pageOrder);

            int total = Emails.Count();
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
                Emails.Delete(id);
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
            Email email;

            if (id.HasValue)
                email = Emails.GetByID(id.Value);
            else
                email = new Email();

            return View(email);
        }

        [HttpPost]
        public ActionResult Edit(Email email)
        {
            try
            {
                email.LastUpdate = DateTime.Now;

                ViewBag.Success = true;

                if (email.ID == -1)
                {
                    Emails.Insert(email);

                    UserNotifications.Send(UserID, String.Format("جدید - ایمیل '{0}'", email.EmailAddress), "/Admin/Emails/Edit/" + email.ID, NotificationType.Success);
                    email = new Email();
                }
                else
                {
                    Emails.Update(email);
                }
            }
            catch (Exception ex)
            {
                SetErrors(ex);
            }

            return ClearView(email);
        }

        public JsonResult GetList()
        {
            var jsonSuccessResult = new JsonSuccessResult();

            try
            {
                var list = Emails.GetList();

                jsonSuccessResult.Success = true;
                jsonSuccessResult.Data = list;
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