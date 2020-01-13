using OnlineStore.DataLayer;
using OnlineStore.Models;
using OnlineStore.Providers;
using System;
using System.Web.Mvc;
using OnlineStore.Providers.Controllers;
using OnlineStore.Models.Enums;

namespace OnlineStore.Website.Areas.Admin.Controllers
{
    public class CustomerCommentsController : AdminController
    {
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public JsonResult Get(int pageIndex, int pageSize, string pageOrder, string userName, string isVisible)
        {
            bool? visible = null;

            if (isVisible != "-1")
                visible = Boolean.Parse(isVisible);

            var list = CustomerComments.Get(pageIndex, pageSize, pageOrder, userName, visible);

            int total = CustomerComments.Count(userName, visible);
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
                CustomerComments.Delete(id);
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
            CustomerComment comment;

            if (id.HasValue)
                comment = CustomerComments.GetByID(id.Value);
            else
                comment = new CustomerComment();

            return View(comment);
        }

        [HttpPost]
        public ActionResult Edit(CustomerComment customerComment)
        {
            try
            {
                var files = Utilities.SaveFiles(Request.Files, Utilities.GetNormalFileName(customerComment.UserName), StaticPaths.CustomerImages);

                if (files.Count > 0)
                    customerComment.Image = files[0].Title;

                customerComment.LastUpdate = DateTime.Now;

                ViewBag.Success = true;

                if (customerComment.ID == -1)
                {
                    CustomerComments.Insert(customerComment);

                    UserNotifications.Send(UserID, String.Format("جدید - سخن مشتریان '{0}'", customerComment.Text), "/Admin/CustomerComments/Edit/" + customerComment.ID, NotificationType.Success);
                    customerComment = new CustomerComment();
                }
                else
                {
                    CustomerComments.Update(customerComment);
                }
            }
            catch (Exception ex)
            {
                SetErrors(ex);
            }

            return ClearView(customerComment);
        }
    }
}