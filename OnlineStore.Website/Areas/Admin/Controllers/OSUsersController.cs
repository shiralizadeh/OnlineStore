using AutoMapper;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using OnlineStore.DataLayer;
using OnlineStore.Identity;
using OnlineStore.Models;
using OnlineStore.Models.Admin;
using OnlineStore.Models.Enums;
using OnlineStore.Providers;
using OnlineStore.Providers.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace OnlineStore.Website.Areas.Admin.Controllers
{
    public class OSUsersController : AdminController
    {
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public JsonResult Get(int pageIndex, int pageSize, string pageOrder, string userName, string fullName, string email, string isActive)
        {
            bool? active = null;

            if (isActive != "-1")
                active = Boolean.Parse(isActive);

            if (pageOrder.Trim() == "ID")
            {
                pageOrder = "LastUpdate desc";
            }
            var list = OSUsers.Get(pageIndex,
                                   pageSize,
                                   pageOrder,
                                   userName,
                                   fullName,
                                   email,
                                   active);

            int total = OSUsers.Count(userName, fullName, email, active);
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

        public JsonResult Delete(string id)
        {
            var jsonSuccessResult = new JsonSuccessResult();

            try
            {
                OSUsers.Delete(id);
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

        public ActionResult Edit(string id)
        {
            EditOSUser editOSUser;

            if (!string.IsNullOrWhiteSpace(id))
            {
                var osUser = OSUsers.GetByID(id);

                editOSUser = Mapper.Map<EditOSUser>(osUser);
                editOSUser.RoleIDs = UserRoles.GetByUserID(id).Select(item => item.RoleId).ToArray();
            }
            else
            {
                editOSUser = new EditOSUser();
            }

            return View(editOSUser);
        }

        [HttpPost]
        public ActionResult Edit(EditOSUser editOSUser, bool? gender, int? stateID, int? cityID)
        {
            try
            {
                var osUser = Mapper.Map<OSUser>(editOSUser);

                var files = Utilities.SaveFiles(Request.Files, Utilities.GetNormalFileName(osUser.UserName), StaticPaths.OSUsers);

                if (files.Count > 0)
                    osUser.ImageFile = files[0].Title;

                osUser.LastUpdate = DateTime.Now;

                ViewBag.Success = true;

                if (string.IsNullOrWhiteSpace(osUser.Id))
                {
                    osUser.Id = Guid.NewGuid().ToString();

                    foreach (var item in editOSUser.RoleIDs)
                    {
                        osUser.Roles.Add(new IdentityUserRole() { RoleId = item, UserId = osUser.Id });
                    }

                    var result = UserManager.Create(osUser, editOSUser.Password);

                    if (result.Succeeded)
                    {
                        UserNotifications.Send(UserID, String.Format("جدید - کاربر '{0}'", osUser.UserName), "/Admin/OSUsers/Edit/" + osUser.Id, NotificationType.Success);
                        editOSUser = new EditOSUser();
                    }
                    else
                        throw new Exception(result.Errors.Aggregate((a, b) => a + ", " + b));
                }
                else
                {
                    var orgOSUser = UserManager.FindById(osUser.Id);

                    orgOSUser.Firstname = osUser.Firstname;
                    orgOSUser.Lastname = osUser.Lastname;
                    orgOSUser.UserName = osUser.UserName;
                    orgOSUser.Email = osUser.Email;
                    orgOSUser.LastUpdate = DateTime.Now;
                    orgOSUser.ImageFile = osUser.ImageFile;
                    orgOSUser.IsActive = osUser.IsActive;
                    orgOSUser.NationalCode = osUser.NationalCode;
                    orgOSUser.Phone = osUser.Phone;
                    orgOSUser.Mobile = osUser.Mobile;
                    orgOSUser.BirthDate = osUser.BirthDate;
                    if (gender.HasValue)
                    {
                        orgOSUser.Gender = gender.Value;
                    }

                    //TODO: UserAddresses
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

                    orgOSUser.CardNumber = osUser.CardNumber;

                    UserManager.Update(orgOSUser);

                    if (!String.IsNullOrWhiteSpace(editOSUser.Password))
                    {
                        UserManager.RemovePassword(editOSUser.Id);
                        UserManager.AddPassword(editOSUser.Id, editOSUser.Password);
                    }

                    var tmpRoles = UserRoles.GetByUserID(editOSUser.Id);

                    foreach (var item in editOSUser.RoleIDs)
                    {
                        var role = Roles.GetByID(item);
                        var tmpRole = tmpRoles.SingleOrDefault(r => r.RoleId == item);

                        if (tmpRole == null)
                        {
                            UserManager.AddToRole(editOSUser.Id, role.Name);
                        }
                        else
                        {
                            tmpRoles.Remove(tmpRole);
                        }
                    }

                    foreach (var item in tmpRoles)
                    {
                        var role = Roles.GetByID(item.RoleId);
                        UserManager.RemoveFromRole(editOSUser.Id, role.Name);
                    }
                }
            }
            catch (Exception ex)
            {
                SetErrors(ex);
            }

            return ClearView(editOSUser);
        }
    }
}