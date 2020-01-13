using OnlineStore.DataLayer;
using OnlineStore.Models;
using OnlineStore.Providers;
using System;
using System.Web.Mvc;
using OnlineStore.Providers.Controllers;
using OnlineStore.Models.Enums;

namespace OnlineStore.Website.Areas.Admin.Controllers
{
    public class HomeBoxItemsController : AdminController
    {
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public JsonResult Get(int pageIndex, int pageSize, string pageOrder, string title, int homeBoxID)
        {
            var list = HomeBoxItems.Get(pageIndex, pageSize, pageOrder, title, homeBoxID);

            int total = HomeBoxItems.Count(title, homeBoxID);
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
                HomeBoxItems.Delete(id);
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
            HomeBoxItem homeBoxItem;

            if (id.HasValue)
                homeBoxItem = HomeBoxItems.GetByID(id.Value);
            else
                homeBoxItem = new HomeBoxItem();

            return View(homeBoxItem);
        }

        [HttpPost]
        public ActionResult Edit(HomeBoxItem homeBoxItem, int homeBoxID)
        {
            try
            {
                var files = Utilities.SaveFiles(Request.Files, Utilities.GetNormalFileName(homeBoxItem.Title), StaticPaths.HomeBoxItems);

                if (files.Count > 0)
                    homeBoxItem.Filename = files[0].Title;

                homeBoxItem.LastUpdate = DateTime.Now;

                ViewBag.Success = true;

                if (homeBoxItem.ID == -1)
                {
                    homeBoxItem.HomeBoxID = homeBoxID;
                    HomeBoxItems.Insert(homeBoxItem);

                    UserNotifications.Send(UserID, String.Format("جدید - عکس باکس های صفحه نخست '{0}'", homeBoxItem.Title), "/Admin/HomeBoxItems/Edit/" + homeBoxItem.ID, NotificationType.Success);
                    homeBoxItem = new HomeBoxItem();
                }
                else
                {
                    HomeBoxItems.Update(homeBoxItem);
                }
            }
            catch (Exception ex)
            {
                SetErrors(ex);
            }

            return ClearView(homeBoxItem);
        }
    }
}