using OnlineStore.DataLayer;
using OnlineStore.Models;
using OnlineStore.Providers;
using System;
using System.Web.Mvc;
using OnlineStore.Providers.Controllers;
using OnlineStore.Models.Enums;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace OnlineStore.Website.Areas.Admin.Controllers
{
    public class ProductCommentsController : AdminController
    {
        public ActionResult Index(int? productID)
        {
            ViewBag.ProductID = productID;

            return View();
        }

        [HttpPost]
        public async Task<JsonResult> Get(int pageIndex, int pageSize, string pageOrder, int? productID, string email, byte? commentStatus)
        {
            CommentStatus? status = null;

            if (commentStatus.HasValue && commentStatus != -1)
            {
                status = (CommentStatus)commentStatus;
            }

            var list = ProductComments.Get(pageIndex,
                                           pageSize,
                                           pageOrder,
                                           productID,
                                           email,
                                           status);
            foreach (var item in list)
            {
                item.UserFullName = item.UserID != null
                                 ? (await UserManager.FindByIdAsync(item.UserID)).Firstname + " " + (await UserManager.FindByIdAsync(item.UserID)).Lastname
                                 : item.UserName;
            }

            int total = ProductComments.Count(productID, email, status);
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
                ProductComments.Delete(id);
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
            ProductComment comment;

            if (id.HasValue)
            {
                comment = ProductComments.GetByID(id.Value);
            }
            else
            {
                comment = new ProductComment();
            }

            return View(model: comment);
        }

        [HttpPost]
        public ActionResult Edit(ProductComment comment)
        {
            try
            {
                comment.LastUpdate = DateTime.Now;

                ViewBag.Success = true;

                if (comment.ID == -1)
                {
                    ProductComments.Insert(comment);

                    UserNotifications.Send(UserID, String.Format("ویرایش نظر محصول '{0}'", comment.Subject), "/Admin/ProductComments/Edit/" + comment.ID, NotificationType.Success);

                    comment = new ProductComment();

                }
                else
                {
                    ProductComments.Update(comment);
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
                ProductComments.Confirm(ids);

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