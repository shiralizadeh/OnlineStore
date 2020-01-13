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
    public class ColleaguesController : PublicController
    {
        public ActionResult Index()
        {
            ContactUsInfo model = new ContactUsInfo();

            var desc = StaticContents.GetByName("Colleague").Content;

            model.Content = HttpUtility.HtmlDecode(desc);

            return View(model: model);
        }

        [HttpPost]
        public JsonResult SendRequest(Colleague colleague)
        {
            var jsonSuccessResult = new JsonSuccessResult();

            try
            {
                Colleagues.Insert(colleague);

                string fullName = colleague.FirstName + " " + colleague.LastName;

                // اطلاع رسانی به مدیر
                #region Apprise Admin 


                string body = "مدیریت محترم، در بخش همکاری با ما، پیام جدیدی ثبت شد:";
                body += "<br/>";
                body += String.Format("نام و نام خانوادگی: {0} <br/> زمینه همکاری: {1} <br/> شماره همراه: {2} <br/> پست الکترونیک: {3}",
                                      fullName,
                                      colleague.CooperationDescription,
                                      colleague.Mobile,
                                      colleague.Email);

                EmailServices.NotifyAdminsByEmail(AdminEmailType.NewColleague, body, null);

                #endregion Apprise Admin

                EmailServices.DeliveryColleagueRequest(colleague.Email, fullName, UserID);

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