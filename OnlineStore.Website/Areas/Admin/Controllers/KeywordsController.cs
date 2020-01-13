using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OnlineStore.Providers.Controllers;
using OnlineStore.DataLayer;
using OnlineStore.Models;
using OnlineStore.Models.Enums;
using AutoMapper;

namespace OnlineStore.Website.Areas.Admin.Controllers
{
    public class KeywordsController : AdminController
    {
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public JsonResult Get(int pageIndex, int pageSize, string pageOrder, string title)
        {
            var list = Keywords.Get(pageIndex, pageSize, pageOrder, title);

            int total = Keywords.Count(title);
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
                Keywords.Delete(id);
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
            Keyword keyword;

            if (id.HasValue)
                keyword = Keywords.GetByID(id.Value);
            else
                keyword = new Keyword();

            return View(keyword);
        }

        [HttpPost]
        public ActionResult Edit(Keyword keyword, string delKey)
        {
            try
            {
                keyword.LastUpdate = DateTime.Now;

                ViewBag.Success = true;

                if (keyword.ID == -1)
                {
                    Keywords.Insert(keyword);

                    UserNotifications.Send(UserID, String.Format("جدید - کلیدواژه '{0}'", keyword.Title), "/Admin/HomeBoxes/Edit/" + keyword.ID, NotificationType.Success);
                    keyword = new Keyword();
                }
                else
                {
                    if (delKey == "on" && !keyword.IsActive)
                    {
                        ProductKeywords.DeleteByKeywordID(keyword.ID);
                    }
                    Keywords.Update(keyword);
                }
            }
            catch (Exception ex)
            {
                SetErrors(ex);
            }

            return ClearView(keyword);
        }

    }
}