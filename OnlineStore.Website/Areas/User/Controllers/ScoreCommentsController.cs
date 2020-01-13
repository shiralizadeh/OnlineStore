using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OnlineStore.DataLayer;
using OnlineStore.Providers.Controllers;
using OnlineStore.Models;
using OnlineStore.Models.User;
using AutoMapper;

namespace OnlineStore.Website.Areas.User.Controllers
{
    [RoutePrefix("My-Account/My-Comments")]
    public class ScoreCommentsController : UserController
    {
        string url = "/Areas/User/Views/ScoreComments/";

        [Route]
        public ActionResult Index()
        {
            var comments = ScoreComments.GetUserComments(UserID);

            return View(url + "Index.cshtml", model: comments);
        }

        [HttpPost]
        [Route("Delete")]
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

        [Route("Edit/{id}")]
        public ActionResult Edit(int id)
        {
            ScoreComment comment;
            var cmt = ScoreComments.GetByID(id);

            comment = Mapper.Map<ScoreComment>(cmt);

            return View(url + "Edit.cshtml", comment);
        }

        [HttpPost]
        [Route("Edit")]
        public ActionResult Edit(ScoreComment comment)
        {
            try
            {
                comment.LastUpdate = DateTime.Now;

                ViewBag.Success = true;
                ScoreComments.UpdateByUser(comment);

            }
            catch (Exception ex)
            {
                SetErrors(ex);
            }

            return View(url + "Edit.cshtml", comment);
        }

    }
}