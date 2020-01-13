using System;
using System.Web.Mvc;
using OnlineStore.DataLayer;
using OnlineStore.Identity;
using OnlineStore.Providers.Controllers;
using OnlineStore.Providers;
using OnlineStore.Models.Enums;
using Microsoft.AspNet.Identity;
using OnlineStore.Models.User;
using AutoMapper;

namespace OnlineStore.Website.Areas.User.Controllers
{
    [RoutePrefix("My-Account")]
    public class ProfileController : UserController
    {
        const string url = "/Areas/User/Views/Profile/";

        [HttpGet]
        [Route("My-Profile")]
        public ActionResult Index()
        {
            var osUser = UserManager.FindById(UserID);

            var editOSUser = Mapper.Map<EditOSUser>(osUser);

            return View(url + "Index.cshtml", editOSUser);
        }

        [HttpPost]
        [Route("My-Profile")]
        [ValidateAntiForgeryToken]
        public ActionResult Index(EditOSUser editOSUser, bool? gender, int? stateID, int? cityID)
        {
            try
            {
                var osUser = Mapper.Map<OSUser>(editOSUser);

                var files = Utilities.SaveFiles(Request.Files, Utilities.GetNormalFileName(User.Identity.GetUserName()), StaticPaths.OSUsers);

                var orgOSUser = UserManager.FindById(UserID);

                orgOSUser.Firstname = osUser.Firstname;
                orgOSUser.Lastname = osUser.Lastname;
                orgOSUser.Email = osUser.Email;
                orgOSUser.LastUpdate = DateTime.Now;
                orgOSUser.NationalCode = osUser.NationalCode;
                orgOSUser.BirthDate = osUser.BirthDate;

                // TODO: UserAddresses
                orgOSUser.Phone = osUser.Phone;
                orgOSUser.Mobile = osUser.Mobile;
                if (gender.HasValue)
                {
                    orgOSUser.Gender = gender.Value;
                }
                if (stateID != -1)
                {
                    orgOSUser.StateID = stateID;
                }
                if (cityID != -1)
                {
                    orgOSUser.CityID = cityID;
                }
                orgOSUser.HomeAddress = osUser.HomeAddress;
                orgOSUser.PostalCode = osUser.PostalCode;

                if (files.Count > 0)
                {
                    orgOSUser.ImageFile = files[0].Title;
                }
                orgOSUser.CardNumber = osUser.CardNumber;

                UserManager.Update(orgOSUser);

                if (!String.IsNullOrWhiteSpace(editOSUser.Password))
                {
                    UserManager.RemovePassword(UserID);
                    UserManager.AddPassword(UserID, editOSUser.Password);
                }

                ViewBag.Success = true;

                UserNotifications.Send(UserID, String.Format("ویرایش اطلاعات کاربری '{0}'", osUser.UserName), "My-Account/My-Profile", NotificationType.Success);

                editOSUser = Mapper.Map<EditOSUser>(orgOSUser);

                return View(url + "Index.cshtml", editOSUser);

            }
            catch (Exception ex)
            {
                SetErrors(ex);
                return View(url + "Index.cshtml");

            }
        }

    }
}