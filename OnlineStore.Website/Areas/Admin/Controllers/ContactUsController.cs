using OnlineStore.DataLayer;
using OnlineStore.Models;
using System;
using System.Web.Mvc;
using OnlineStore.Providers.Controllers;
using System.Threading.Tasks;
using OnlineStore.Models.Enums;
using OnlineStore.Services;
using OnlineStore.Providers;

namespace OnlineStore.Website.Areas.Admin.Controllers
{
    public class ContactUsController : AdminController
    {
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public JsonResult Get(int pageIndex, int pageSize, string pageOrder, string fullName, string email, sbyte MessageStatus)
        {

            if (pageOrder.Trim() == "ID")
            {
                pageOrder = "LastUpdate desc";
            }
            ContactUsMessageStatus? status = null;

            if (MessageStatus != -1)
                status = (ContactUsMessageStatus)MessageStatus;

            var list = ContactUsMessages.Get(pageIndex,
                                           pageSize,
                                           pageOrder,
                                           fullName,
                                           email,
                                           status);

            int total = ContactUsMessages.Count(fullName, email, status);
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
                ContactUsMessages.Delete(id);
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
            ContactUsMessage message;

            message = ContactUsMessages.GetByID(id);

            return View(model: message);
        }

        [HttpPost]
        public ActionResult Edit(ContactUsMessage msg, string sendEmail)
        {
            try
            {
                var old = ContactUsMessages.GetByID(msg.ID);

                if (!String.IsNullOrWhiteSpace(msg.Answer) && sendEmail == "on")
                {
                    EmailServices.ContactUsMessage(old.Email, old.Subject, old.Message, msg.Answer, UserID);
                }

                msg.LastUpdate = DateTime.Now;

                ViewBag.Success = true;

                ContactUsMessages.Update(msg);

            }
            catch (Exception ex)
            {
                SetErrors(ex);
            }

            return RedirectToAction("Index");
        }
    }
}