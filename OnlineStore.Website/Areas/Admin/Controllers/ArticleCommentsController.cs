using OnlineStore.DataLayer;
using OnlineStore.Models;
using System;
using System.Web.Mvc;
using OnlineStore.Providers.Controllers;
using OnlineStore.Providers;
using OnlineStore.Models.Enums;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace OnlineStore.Website.Areas.Admin.Controllers
{
    public class ArticleCommentsController : AdminController
    {
        public ActionResult Index(int? articleID)
        {
            ViewBag.ArticleID = articleID;
            return View();
        }

        [HttpPost]
        public async Task<JsonResult> Get(int pageIndex, int pageSize, string pageOrder, int? articleID, string email, sbyte? articleCommentStatus)
        {
            if (pageOrder.Trim() == "ID")
            {
                pageOrder = "LastUpdate desc";
            }
            ArticleCommentStatus? status = null;

            if (articleCommentStatus.HasValue && articleCommentStatus != -1)
            {
                status = (ArticleCommentStatus)articleCommentStatus;
            }
            var list = ArticleComments.Get(pageIndex,
                                           pageSize,
                                           pageOrder,
                                           articleID,
                                           email,
                                           status);
            foreach (var item in list)
            {
                item.UserFullName = item.UserID != null
                                 ? (await UserManager.FindByIdAsync(item.UserID)).Firstname + " " + (await UserManager.FindByIdAsync(item.UserID)).Lastname
                                 : item.UserName;
            }

            int total = ArticleComments.Count(articleID, email, status);
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
                ArticleComments.Delete(id);
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
            ArticleComment comment;

            if (id.HasValue)
            {
                comment = ArticleComments.GetByID(id.Value);
            }
            else
            {
                comment = new ArticleComment();
            }

            return View(model: comment);
        }

        [HttpPost]
        public ActionResult Edit(ArticleComment comment)
        {
            try
            {
                comment.LastUpdate = DateTime.Now;

                ViewBag.Success = true;

                if (comment.ID == -1)
                {
                    ArticleComments.Insert(comment);

                    UserNotifications.Send(UserID, String.Format("ویرایش نظر '{0}'", comment.Subject), "/Admin/ArticleComments/Edit/" + comment.ID, NotificationType.Success);

                    comment = new ArticleComment();

                }
                else
                {
                    ArticleComments.Update(comment);
                }

            }
            catch (Exception ex)
            {
                SetErrors(ex);
            }

            return ClearView(model: comment);
        }

        public JsonResult Confirm(List<int> ids)
        {
            var jsonSuccessResult = new JsonSuccessResult();

            try
            {
                ArticleComments.Confirm(ids);

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