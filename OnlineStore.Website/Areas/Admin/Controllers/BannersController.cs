using OnlineStore.DataLayer;
using OnlineStore.Models;
using OnlineStore.Providers;
using System;
using System.Web.Mvc;
using OnlineStore.Providers.Controllers;
using OnlineStore.Models.Enums;

namespace OnlineStore.Website.Areas.Admin.Controllers
{
    public class BannersController : AdminController
    {
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public JsonResult Get(int pageIndex, int pageSize, string pageOrder)
        {
            var list = Banners.Get(pageIndex, pageSize, pageOrder);

            int total = Banners.Count();
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
                Banners.Delete(id);
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
            Banner banner;

            if (id.HasValue)
                banner = Banners.GetByID(id.Value);
            else
                banner = new Banner();

            return View(banner);
        }

        [HttpPost]
        public ActionResult Edit(Banner banner)
        {
            try
            {
                var files = Utilities.SaveFiles(Request.Files, Utilities.GetNormalFileName(banner.Title), StaticPaths.BannerImages);

                if (files.Count > 0)
                    banner.Filename = files[0].Title;

                banner.LastUpdate = DateTime.Now;

                ViewBag.Success = true;

                if (banner.ID == -1)
                {
                    banner.Key = Guid.NewGuid();

                    Banners.Insert(banner);

                    UserNotifications.Send(UserID, String.Format("جدید - بنر '{0}'", banner.Title), "/Admin/Banners/Edit/" + banner.ID, NotificationType.Success);
                    banner = new Banner();
                }
                else
                {
                    Banners.Update(banner);
                }
            }
            catch (Exception ex)
            {
                SetErrors(ex);
            }

            return ClearView(banner);
        }
    }
}