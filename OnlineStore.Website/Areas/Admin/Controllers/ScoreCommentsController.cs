using OnlineStore.DataLayer;
using OnlineStore.Models;
using OnlineStore.Providers;
using System;
using System.Web.Mvc;
using OnlineStore.Providers.Controllers;
using OnlineStore.Models.Enums;
using OnlineStore.Models.Admin;
using System.Threading.Tasks;
using System.Collections.Generic;
using AutoMapper;

namespace OnlineStore.Website.Areas.Admin.Controllers
{
    public class ScoreCommentsController : AdminController
    {
        public ActionResult Index(int? productID)
        {
            ViewBag.ProductID = productID;

            return View();
        }

        [HttpPost]
        public async Task<JsonResult> Get(int pageIndex, int pageSize, string pageOrder, int? productID, byte? scoreCommentStatus)
        {
            ScoreCommentStatus? status = null;

            if (scoreCommentStatus.HasValue)
            {
                status = (ScoreCommentStatus)scoreCommentStatus.Value;
            }

            if (pageOrder.Trim() == "ID")
            {
                pageOrder = "LastUpdate desc";
            }
            var list = ScoreComments.Get(pageIndex,
                                         pageSize,
                                         pageOrder,
                                         productID,
                                         status);
            foreach (var item in list)
            {
                item.UserFullName = (await UserManager.FindByIdAsync(item.UserID)).Firstname + " " + (await UserManager.FindByIdAsync(item.UserID)).Lastname;
            }

            int total = ScoreComments.Count(productID, status);
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
                ScoreComments.Delete(id);
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
        public ActionResult Edit(int id)
        {
            EditScoreComment editScoreComment;

            editScoreComment = ScoreComments.GetByID(id);

            return View(model: editScoreComment);
        }

        [HttpPost]
        public ActionResult Edit(EditScoreComment editScoreComment)
        {
            try
            {
                var scoreComment = Mapper.Map<ScoreComment>(editScoreComment);

                scoreComment.LastUpdate = DateTime.Now;

                ViewBag.Success = true;

                ScoreComments.Update(scoreComment);

            }
            catch (Exception ex)
            {
                SetErrors(ex);
            }

            return RedirectToAction("Index", "ScoreComments", new { ProductID = Request.QueryString["ProductID"] });
        }

        public JsonResult Confirm(List<int> ids)
        {
            var jsonSuccessResult = new JsonSuccessResult();

            try
            {
                ScoreComments.Confirm(ids);

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