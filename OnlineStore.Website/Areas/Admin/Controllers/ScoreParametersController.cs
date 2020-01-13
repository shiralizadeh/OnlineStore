using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OnlineStore.DataLayer;
using OnlineStore.Models;
using OnlineStore.Models.Admin;
using AutoMapper;
using OnlineStore.Providers.Controllers;
using OnlineStore.Models.Enums;

namespace OnlineStore.Website.Areas.Admin.Controllers
{
    public class ScoreParametersController : AdminController
    {
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public JsonResult Get(int pageIndex, int pageSize, string pageOrder, string title, string isActive)
        {
            bool? active = null;

            if (isActive != "-1")
                active = Boolean.Parse(isActive);

            var list = ScoreParameters.Get(pageIndex, pageSize, pageOrder, title, active);

            int total = ScoreParameters.Count(title, active);
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
                ScoreParameters.Delete(id);
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
            EditScoreParameter parameter;

            if (id.HasValue)
            {
                var editScore = ScoreParameters.GetByID(id.Value);

                parameter = new EditScoreParameter
                {
                    ID = editScore.ID,
                    IsActive = editScore.IsActive,
                    Title = editScore.Title,
                    Groups = GroupScoreParameters.GetByScoreParameterID(id.Value).Select(item => item.GroupID).ToList()
                };
            }
            else
                parameter = new EditScoreParameter();

            return View(parameter);
        }

        [HttpPost]
        public ActionResult Edit(EditScoreParameter editScoreParameter)
        {
            try
            {
                var parameter = Mapper.Map<ScoreParameter>(editScoreParameter);

                parameter.LastUpdate = DateTime.Now;

                ViewBag.Success = true;

                int scoreParameterID = parameter.ID;
                if (scoreParameterID == -1)
                {
                    ScoreParameters.Insert(parameter);
                    scoreParameterID = parameter.ID;

                    // ثبت و ویرایش گروه های انتخاب شده
                    SaveGroups(editScoreParameter, scoreParameterID);

                    UserNotifications.Send(UserID, String.Format("جدید - پارامتر امتیازدهی '{0}'", editScoreParameter.Title), "/Admin/ScoreParameters/Edit/" + editScoreParameter.ID, NotificationType.Success);
                    editScoreParameter = new EditScoreParameter();
                }
                else
                {
                    ScoreParameters.Update(parameter);

                    // ثبت و ویرایش گروه های انتخاب شده
                    SaveGroups(editScoreParameter, scoreParameterID);

                    editScoreParameter.Groups = GroupScoreParameters.GetByScoreParameterID(scoreParameterID).Select(item => item.GroupID).ToList();

                }


            }
            catch (Exception ex)
            {
                SetErrors(ex);
            }

            return ClearView(editScoreParameter);
        }

        private static void SaveGroups(EditScoreParameter editScoreParameter, int scoreParameterID)
        {
            var curList = GroupScoreParameters.GetByScoreParameterID(scoreParameterID);

            foreach (var groupID in editScoreParameter.Groups)
            {
                if (!curList.Any(item => item.GroupID == groupID))
                {
                    var groupScoreParameter = new GroupScoreParameter();

                    groupScoreParameter.ScoreParameterID = scoreParameterID;
                    groupScoreParameter.GroupID = groupID;

                    GroupScoreParameters.Insert(groupScoreParameter);
                }
                else
                {
                    curList.Remove(curList.Single(cls => cls.GroupID == groupID));
                }
            }

            foreach (var item in curList)
                GroupScoreParameters.Delete(item.ID);
        }

    }
}