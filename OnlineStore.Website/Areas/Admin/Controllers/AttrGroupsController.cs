using OnlineStore.DataLayer;
using OnlineStore.Models;
using OnlineStore.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OnlineStore.Providers.Controllers;
using OnlineStore.Models.Enums;
using OnlineStore.Models.Admin;
using AutoMapper;

namespace OnlineStore.Website.Areas.Admin.Controllers
{
    public class AttrGroupsController : AdminController
    {
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public JsonResult Get(int pageIndex, int pageSize, string pageOrder, string title, List<int> groups)
        {
            if (pageOrder.Trim() == "ID")
                pageOrder = "OrderID";

            var list = AttrGroups.Get(pageIndex, pageSize, pageOrder, title, groups);

            int total = AttrGroups.Count(title, groups);
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
                AttrGroups.Delete(id);
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
            EditAttrGroup editAttrGroup;

            if (id.HasValue)
            {
                editAttrGroup = Mapper.Map<EditAttrGroup>(AttrGroups.GetByID(id.Value));
                editAttrGroup.Groups = AttrGroupGroups.GetByAttrGroupID(editAttrGroup.ID).Select(item => item.GroupID).ToList();
            }
            else
                editAttrGroup = new EditAttrGroup();

            return View(editAttrGroup);
        }

        [HttpPost]
        public ActionResult Edit(EditAttrGroup editAttrGroup)
        {
            try
            {
                var attrGroup = Mapper.Map<AttrGroup>(editAttrGroup);

                attrGroup.LastUpdate = DateTime.Now;

                ViewBag.Success = true;

                if (attrGroup.ID == -1)
                {
                    AttrGroups.Insert(attrGroup);

                    SaveGroups(editAttrGroup, attrGroup.ID);

                    UserNotifications.Send(UserID, String.Format("جدید - گروه ویژگی '{0}'", attrGroup.Title), "/Admin/AttrGroups/Edit/" + attrGroup.ID, NotificationType.Success);
                    editAttrGroup = new EditAttrGroup();
                }
                else
                {
                    AttrGroups.Update(attrGroup);

                    SaveGroups(editAttrGroup, attrGroup.ID);

                    editAttrGroup.Groups = AttrGroupGroups.GetByAttrGroupID(editAttrGroup.ID).Select(item => item.GroupID).ToList();
                }
            }
            catch (Exception ex)
            {
                SetErrors(ex);
            }

            return ClearView(editAttrGroup);
        }

        public JsonResult FilterByGroups(List<int> groups)
        {
            var jsonSuccessResult = new JsonSuccessResult();

            try
            {
                if (groups != null && groups.Count > 0)
                {
                    jsonSuccessResult.Data = AttrGroups.GetByGroupIDs(groups);
                }
                else
                {
                    jsonSuccessResult.Data = AttrGroups.GetAll();
                }

                jsonSuccessResult.Success = true;
            }
            catch (Exception ex)
            {
                jsonSuccessResult.Errors = new string[] { ex.Message };
                jsonSuccessResult.Success = false;
            }


            JsonResult result = new JsonResult()
            {
                Data = jsonSuccessResult
            };

            return result;
        }

        private static void SaveGroups(EditAttrGroup editAttrGroup, int attrGroupID)
        {
            var curList = AttrGroupGroups.GetByAttrGroupID(attrGroupID);

            foreach (var groupID in editAttrGroup.Groups)
            {
                if (!curList.Any(item => item.GroupID == groupID))
                {
                    var attrGroupGroup = new AttrGroupGroup();

                    attrGroupGroup.AttrGroupID = attrGroupID;
                    attrGroupGroup.GroupID = groupID;

                    AttrGroupGroups.Insert(attrGroupGroup);
                }
                else
                {
                    curList.Remove(curList.Single(cls => cls.GroupID == groupID));
                }
            }

            foreach (var item in curList)
                AttrGroupGroups.Delete(item.ID);
        }
    }
}