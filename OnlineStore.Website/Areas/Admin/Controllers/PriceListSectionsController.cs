using OnlineStore.DataLayer;
using OnlineStore.Models;
using OnlineStore.Providers;
using System;
using System.Web.Mvc;
using OnlineStore.Providers.Controllers;
using OnlineStore.Models.Enums;
using AutoMapper;
using OnlineStore.Models.Admin;
using System.Linq;
using System.Web;

namespace OnlineStore.Website.Areas.Admin.Controllers
{
    public class PriceListSectionsController : AdminController
    {
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public JsonResult Get(int pageIndex, int pageSize, string pageOrder, string title)
        {
            if (pageOrder.Trim() == "ID")
                pageOrder = "OrderID";

            var list = PriceListSections.Get(pageIndex, pageSize, pageOrder, title);

            int total = PriceListSections.Count(title);
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
                PriceListSections.Delete(id);
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
            PriceListSection priceListSection;

            if (id.HasValue)
            {
                priceListSection = PriceListSections.GetByID(id.Value);
            }
            else
                priceListSection = new PriceListSection();

            return View(priceListSection);
        }

        [HttpPost]
        public ActionResult Edit(PriceListSection priceListSection)
        {
            try
            {
                var files = Utilities.SaveFiles(Request.Files, Utilities.GetNormalFileName(priceListSection.Title), StaticPaths.PriceListSectionImages);

                if (files.Count > 0)
                    priceListSection.Image = files[0].Title;

                priceListSection.LastUpdate = DateTime.Now;

                ViewBag.Success = true;

                if (priceListSection.ID == -1)
                {
                    PriceListSections.Insert(priceListSection);

                    UserNotifications.Send(UserID, String.Format("جدید - برندهای لیست بنک داری '{0}'", priceListSection.Title), "/Admin/PriceListSections/Edit/" + priceListSection.ID, NotificationType.Success);
                    priceListSection = new PriceListSection();
                }
                else
                {
                    PriceListSections.Update(priceListSection);
                }
            }
            catch (Exception ex)
            {
                SetErrors(ex);
            }

            return ClearView(priceListSection);
        }

    }
}