using OnlineStore.Providers.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OnlineStore.Models.Admin;
using OnlineStore.Identity;
using OnlineStore.DataLayer;
using OnlineStore.Providers;
using OnlineStore.Models;
using AutoMapper;
using OnlineStore.Models.Enums;

namespace OnlineStore.Website.Areas.Admin.Controllers
{
    public class DashboardController : AdminController
    {
        public ActionResult Index()
        {
            var tasks = UserTasks.GetUserTask(UserID);

            var dashboard = new DashboardSettings
            {
                SiteUsers = IdentityDbContext.Entity.Users.Count(),
                Orders = Carts.OrdersCount(),
                NewOrders = Carts.NewOrdersCount(),
                //TODO میزان سود محاسبه شود
                Profit = 34500.ToPrice("{0}"),
                Tasks = tasks
            };

            return View(dashboard);
        }

        [AllowAnonymous]
        public bool IsLogin()
        {
            return User.Identity.IsAuthenticated;
        }

        public JsonResult HideTask(int id)
        {
            var jsonSuccessResult = new JsonSuccessResult();

            try
            {
                UserTasks.DoneTask(id);
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

        public JsonResult GetTask(int id)
        {
            var jsonSuccessResult = new JsonSuccessResult();

            try
            {
                var data = UserTasks.GetByID(id);
                jsonSuccessResult.Data = Mapper.Map<JsonUserTask>(data);
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

        [HttpPost]
        public JsonResult SetTask(int id, string UserTaskDate)
        {
            var jsonSuccessResult = new JsonSuccessResult();

            try
            {
                var task = new UserTask();

                task.ID = id;
                task.UserTaskDate = Utilities.ToEnglishDate(UserTaskDate);
                task.UserTaskStatus = UserTaskStatus.NotDone;

                UserTasks.UpdateDateTask(task);

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

    }
}