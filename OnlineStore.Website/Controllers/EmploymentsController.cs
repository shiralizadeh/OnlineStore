using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OnlineStore.DataLayer;
using OnlineStore.Models.Enum;
using OnlineStore.Providers.Controllers;
using OnlineStore.Providers;
using OnlineStore.Services;
using OnlineStore.Models.Enums;

namespace OnlineStore.Website.Controllers
{
    public class EmploymentsController : PublicController
    {
        public ActionResult Index()
        {
            var employment = new Employment();

            return View(employment);
        }

        [HttpPost]
        public ActionResult Index(Employment employment)
        {
            try
            {
                employment.DateTime = employment.LastUpdate = DateTime.Now;
                employment.EmploymentStatus = EmploymentStatus.NotChecked;

                var files = Utilities.SaveFiles(Request.Files, Utilities.GetNormalFileName(employment.NationalCode), StaticPaths.ResumeFiles);
                if (files.Count > 0)
                {
                    employment.ResumeFile = files[0].Title;
                }

                Employments.Insert(employment);

                ViewBag.IsSuccess = true;

                // اطلاع رسانی به مدیر
                #region Apprise Admin 

                string body = "مدیریت محترم، در بخش استخدام در آنلاین استور، فردی رزومه خود را ارسال کرده است:";
                body += "<br/>";
                body += String.Format("ایمیل: {0} <br/> نام و نام خانوادگی: {1}", employment.Email, employment.FirstName + " " + employment.LastName);

                EmailServices.NotifyAdminsByEmail(AdminEmailType.NewEmployment, body, null);

                #endregion Apprise Admin

                EmailServices.DeliveryEmploymentInfo(employment.Email, employment.FirstName + " " + employment.LastName, UserID);
            }
            catch
            {
                ViewBag.IsSuccess = false;
            }

            return View(employment);
        }
    }
}