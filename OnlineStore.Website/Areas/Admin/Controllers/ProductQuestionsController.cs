using OnlineStore.DataLayer;
using OnlineStore.Models;
using OnlineStore.Providers;
using System;
using System.Web.Mvc;
using OnlineStore.Providers.Controllers;
using OnlineStore.Models.Enums;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Collections;
using OnlineStore.Services;
using OnlineStore.Identity;

namespace OnlineStore.Website.Areas.Admin.Controllers
{
    public class ProductQuestionsController : AdminController
    {
        public ActionResult Index(int? productID)
        {
            ViewBag.ProductID = productID;

            return View();
        }

        [HttpPost]
        public async Task<JsonResult> Get(int pageIndex, int pageSize, string pageOrder, int? productID, string question, sbyte questionStatus)
        {
            QuestionStatus? status = null;

            if (questionStatus != -1)
                status = (QuestionStatus)questionStatus;

            if (pageOrder.Trim()=="ID")
            {
                pageOrder = "LastUpdate Desc";
            }

            var list = ProductQuestions.Get(pageIndex,
                                           pageSize,
                                           pageOrder,
                                           productID,
                                           question,
                                           status);
            foreach (var item in list)
            {
                item.UserFullName = item.UserID != null
                                 ? (await UserManager.FindByIdAsync(item.UserID)).Firstname + " " + (await UserManager.FindByIdAsync(item.UserID)).Lastname
                                 : item.UserName;
            }

            int total = ProductQuestions.Count(productID, question, status);
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
                ProductQuestions.Delete(id);
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

        [HttpGet]
        public ActionResult Edit(int? id)
        {
            ProductQuestion productQuestion;

            if (id.HasValue)
            {
                productQuestion = ProductQuestions.GetByID(id.Value);
            }
            else
            {
                productQuestion = new ProductQuestion();
            }

            return View(model: productQuestion);
        }

        [HttpPost]
        public ActionResult Edit(ProductQuestion productQuestion, string sendEmail)
        {
            try
            {
                productQuestion.LastUpdate = DateTime.Now;

                ViewBag.Success = true;

                if (productQuestion.ID == -1)
                {
                    productQuestion.UserID = UserID;
                    productQuestion.DateTime = DateTime.Now;

                    ProductQuestions.Insert(productQuestion);

                    UserNotifications.Send(UserID, String.Format("جدید - سوالات متداول '{0}'", productQuestion.Question), "/Admin/ProductQuestions/Edit/" + productQuestion.ID, NotificationType.Success);

                    productQuestion = new ProductQuestion();

                }
                else
                {
                    ProductQuestions.Update(productQuestion);

                    if (!String.IsNullOrWhiteSpace(productQuestion.UserID) && sendEmail == "on" && productQuestion.QuestionStatus == QuestionStatus.Answered)
                    {
                        var user = OSUsers.GetByID(productQuestion.UserID);

                        EmailServices.SendEmail(user.Email, "پاسخ به پرسش شما", productQuestion.Reply, user.Id);
                    }
                }

            }
            catch (Exception ex)
            {
                SetErrors(ex);
            }

            return ClearView(model: productQuestion);
        }

        public JsonResult Confirm(List<int> ids)
        {
            var jsonSuccessResult = new JsonSuccessResult();

            try
            {
                ProductQuestions.UpdateQuestions(ids);

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