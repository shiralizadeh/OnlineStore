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
    public class HomeBoxesController : AdminController
    {
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public JsonResult Get(int pageIndex, int pageSize, string pageOrder)
        {
            var list = HomeBoxes.Get(pageIndex, pageSize, pageOrder);

            int total = HomeBoxes.Count();
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
                HomeBoxes.Delete(id);
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
            HomeBox homeBox;

            if (id.HasValue)
                homeBox = HomeBoxes.GetByID(id.Value);
            else
                homeBox = new HomeBox();

            return View(homeBox);
        }

        [HttpPost]
        public ActionResult Edit(HomeBox homeBox)
        {
            try
            {
                homeBox.LastUpdate = DateTime.Now;

                ViewBag.Success = true;

                if (homeBox.ID == -1)
                {
                    HomeBoxes.Insert(homeBox);

                    UserNotifications.Send(UserID, String.Format("جدید - باکس های محصولات '{0}'", homeBox.Title), "/Admin/HomeBoxes/Edit/" + homeBox.ID, NotificationType.Success);
                    homeBox = new HomeBox();
                }
                else
                {
                    HomeBoxes.Update(homeBox);
                }
            }
            catch (Exception ex)
            {
                SetErrors(ex);
            }

            return ClearView(homeBox);
        }
    }
}