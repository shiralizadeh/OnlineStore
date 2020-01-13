using System;
using System.Linq;
using System.Web.Mvc;
using OnlineStore.DataLayer;
using OnlineStore.Identity;
using OnlineStore.Providers.Controllers;
using OnlineStore.Providers;
using OnlineStore.Models.Enums;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity;
using OnlineStore.Services;

namespace OnlineStore.Website.Areas.User.Controllers
{
    public class RegisterController : PublicController
    {
        const string url = "/Areas/User/Views/Register/";

        [HttpGet]
        [Route("Register")]
        public ActionResult Index()
        {
            return View(url + "Index.cshtml");
        }

        [HttpPost]
        [Route("Register")]
        [ValidateAntiForgeryToken]
        public ActionResult Index(OSUser osUser, string password)
        {
            try
            {
                osUser.LastUpdate = DateTime.Now;
                osUser.IsActive = true;
                osUser.ImageFile = "70x70.jpg";

                ViewBag.Success = true;

                // اختصاص کد کاربر
                osUser.Id = Guid.NewGuid().ToString();

                // اختصاص نقش Public به کاربر
                osUser.Roles.Add(new IdentityUserRole() { RoleId = StaticValues.PublicRoleID, UserId = osUser.Id });

                // ایجاد کاربر
                var result = UserManager.Create(osUser, password);

                if (result.Succeeded)
                {
                    UserNotifications.Send(StaticValues.AdminID, String.Format("ثبت نام کاربر '{0}'", osUser.UserName), "/Admin/OSUsers/Edit/" + osUser.Id, NotificationType.Success);

                    if (!String.IsNullOrWhiteSpace(osUser.Mobile))
                    {
                        SMSServices.Register(osUser.Firstname,
                                             osUser.Lastname,
                                             osUser.UserName,
                                             password,
                                             osUser.Mobile,
                                             osUser.Id);
                    }

                    EmailServices.Register(osUser.Firstname,
                                           osUser.Lastname,
                                           osUser.UserName,
                                           password,
                                           osUser.Email,
                                           osUser.Id);
                }
                else
                    throw new Exception(result.Errors.Aggregate((a, b) => a + ", " + b));
            }
            catch (Exception ex)
            {
                SetErrors(ex);
            }

            return View(url + "Index.cshtml");
        }
    }
}