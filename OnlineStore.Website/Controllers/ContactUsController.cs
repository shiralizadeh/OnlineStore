using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OnlineStore.DataLayer;
using OnlineStore.Models.Enums;
using OnlineStore.Models;
using OnlineStore.Models.Public;
using OnlineStore.Providers;
using OnlineStore.EntityFramework;
using OnlineStore.Services;
using OnlineStore.Providers.Controllers;

namespace OnlineStore.Website.Controllers
{
    public class ContactUsController : PublicController
    {
        [Route("ContactUs")]
        public ActionResult Index()
        {
            ContactUsInfo model = new ContactUsInfo();

            var desc = StaticContents.GetByName("ContactUs").Content;

            model.Content = HttpUtility.HtmlDecode(desc);

            return View(model: model);
        }

        [HttpPost]
        public JsonResult SendMessage(string fullName, string email, string subject, string message)
        {
            var jsonSuccessResult = new JsonSuccessResult();

            try
            {
                ContactUsMessage msg = new ContactUsMessage
                {
                    Subject = subject,
                    FullName = fullName,
                    Email = email,
                    Message = message,
                    LastUpdate = DateTime.Now,
                    ContactUsMessageStatus = ContactUsMessageStatus.NotChecked,
                    Key = Guid.NewGuid().ToString()
                };

                ContactUsMessages.Insert(msg);

                // اطلاع رسانی به مدیر
                #region Apprise Admin 

                string body = "مدیریت محترم، در بخش تماس با ما، پیام جدیدی ثبت شد:";
                body += "<br/>";
                body += String.Format("ایمیل: {0} <br/> موضوع: {1} <br/> پیام: {2}", email, subject, message);

                EmailServices.NotifyAdminsByEmail(AdminEmailType.NewContactMessage, body, null);

                #endregion Apprise Admin

                EmailServices.DeliveryContactUsMessage(email, fullName, UserID);

                jsonSuccessResult.Success = true;
            }
            catch (DbException ex)
            {
                jsonSuccessResult.Errors = ex.Errors.ToArray();
                jsonSuccessResult.Success = false;
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