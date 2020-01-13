using OnlineStore.DataLayer;
using OnlineStore.Models;
using OnlineStore.Providers;
using System;
using System.Web.Mvc;
using OnlineStore.Providers.Controllers;
using OnlineStore.Models.Enums;
using OnlineStore.Identity;
using AutoMapper;
using OnlineStore.Models.Admin;
using System.Collections.Generic;

namespace OnlineStore.Website.Areas.Admin.Controllers
{
    public class UserTasksController : AdminController
    {
        public ActionResult Index()
        {
            List<string> roleIDs = new List<string>{
                StaticValues.Writer,
                StaticValues.Accountant,
                StaticValues.Administrator
            };

            var users = UserRoles.GetByRoles(roleIDs);

            var model = Mapper.Map<List<UserShortInfo>>(users);

            return View(model: model);
        }

        [HttpPost]
        public JsonResult Get(int pageIndex, int pageSize, string pageOrder, string title, string userID)
        {
            if (UserID == "-1")
            {
                userID = String.Empty;
            }

            var list = UserTasks.Get(pageIndex, pageSize, pageOrder, title, UserID);

            int total = UserTasks.Count(title, UserID);
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
                UserTasks.Delete(id);
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
            UserTask userTask;

            if (id.HasValue)
                userTask = UserTasks.GetByID(id.Value);
            else
                userTask = new UserTask();

            return View(userTask);
        }

        [HttpPost]
        public ActionResult Edit(UserTask userTask)
        {
            try
            {
                userTask.LastUpdate = DateTime.Now;

                ViewBag.Success = true;

                if (userTask.ID == -1)
                {
                    UserTasks.Insert(userTask);

                    UserNotifications.Send(UserID, String.Format("جدید - وظیفه '{0}'", userTask.Title), "/Admin/UserTasks/Edit/" + userTask.ID, NotificationType.Success);
                    userTask = new UserTask();
                }
                else
                {
                    UserTasks.Update(userTask);
                }
            }
            catch (Exception ex)
            {
                SetErrors(ex);
            }

            return ClearView(userTask);
        }

    }
}