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
    public class GroupsController : AdminController
    {
        protected GroupType _groupType;

        public GroupsController()
        {
            _groupType = GroupType.Products;
            ViewBag.Controller = "Groups";
        }

        public ActionResult Index()
        {
            return View("/Areas/Admin/Views/Groups/Index.cshtml");
        }

        [HttpPost]
        public JsonResult Get(int pageIndex, int pageSize, string pageOrder, string title)
        {
            var list = Groups.Get(pageIndex, pageSize, pageOrder, _groupType, title);

            int total = Groups.Count(_groupType, title);
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

        public JsonResult GetGroups(bool multiple)
        {
            JsonResult result = new JsonResult()
            {
                Data = FillUsersGroups_Root(multiple ? TreeViewSelectMode.Multiple : TreeViewSelectMode.Single),
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };

            return result;
        }

        private List<TreeItem> FillUsersGroups_Root(TreeViewSelectMode mode)
        {
            List<TreeItem> list = new List<TreeItem>();

            foreach (var item in Groups.GetRoot(_groupType))
            {
                TreeItem node = new TreeItem();
                node.label = item.Title;
                node.id = item.ID;

                switch (mode)
                {
                    case TreeViewSelectMode.Single:
                        //node.radio = true;
                        break;
                    case TreeViewSelectMode.Multiple:
                        node.checkbox = true;
                        break;
                    default:
                        break;
                }

                node.branch = new List<TreeItem>();
                FillUsersGroups_Children(node, mode);

                list.Add(node);
            }

            return list;
        }

        private void FillUsersGroups_Children(TreeItem parentNode, TreeViewSelectMode mode)
        {
            foreach (var item in Groups.GetByParentID(parentNode.id))
            {
                TreeItem node = new TreeItem();

                node.label = item.Title;
                node.id = item.ID;

                switch (mode)
                {
                    case TreeViewSelectMode.Single:
                        //node.radio = true;
                        break;
                    case TreeViewSelectMode.Multiple:
                        node.checkbox = true;
                        break;
                    default:
                        break;
                }

                node.branch = new List<TreeItem>();
                FillUsersGroups_Children(node, mode);

                parentNode.branch.Add(node);
            }
        }

        public JsonResult Delete(int id)
        {
            var jsonSuccessResult = new JsonSuccessResult();

            try
            {
                Groups.Delete(id);
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
            EditGroup group;

            if (id.HasValue)
            {
                group = Mapper.Map<EditGroup>(Groups.GetByID(id.Value));
                group.Banners = GroupBanners.GetByGroupID(group.ID);

            }
            else
                group = new EditGroup();

            return View("/Areas/Admin/Views/Groups/Edit.cshtml", group);
        }

        [HttpPost]
        public ActionResult Edit(EditGroup editGroup)
        {
            try
            {
                var group = Mapper.Map<Group>(editGroup);

                group.LastUpdate = DateTime.Now;
                group.GroupType = _groupType;

                var files = Utilities.SaveFiles(Request.Files, Utilities.GetNormalFileName(group.Title), StaticPaths.GroupImages);

                if (files.Count > 0)
                {
                    if (files.Any(a => a.FileUpload == "Image"))
                    {
                        var fImage = files.Where(a => a.FileUpload == "Image").Single();
                        group.Image = fImage.Title;
                    }

                    if (files.Any(a => a.FileUpload == "ButtonImage"))
                    {
                        var fBtnImage = files.Where(a => a.FileUpload == "ButtonImage").Single();
                        group.ButtonImage = fBtnImage.Title;
                    }
                }

                ViewBag.Success = true;

                int grouID = editGroup.ID;

                if (group.ID == -1)
                {
                    Groups.Insert(group);
                    grouID = group.ID;

                    SaveBanners(editGroup, grouID);

                    UserNotifications.Send(UserID, String.Format("جدید - گروه '{0}'", group.Title), "/Admin/Groups/Edit/" + group.ID, NotificationType.Success);
                    editGroup = new EditGroup();
                }
                else
                {
                    Groups.Update(group);
                    SaveBanners(editGroup, grouID);
                }
            }
            catch (Exception ex)
            {
                SetErrors(ex);
            }

            return ClearView(editGroup, "/Areas/Admin/Views/Groups/Edit.cshtml");
        }

        #region Methods

        private static void SaveBanners(EditGroup editGroup, int groupID)
        {
            var curList = GroupBanners.GetByGroupID(groupID);

            foreach (var banner in editGroup.Banners)
            {
                if (!curList.Any(item => item.ID == banner.ID))
                {
                    var groupBanner = Mapper.Map<GroupBanner>(banner);

                    groupBanner.GroupID = groupID;

                    GroupBanners.Insert(groupBanner);
                }
                else
                {
                    GroupBanners.UpdateGroupBannerType(banner.ID, banner.GroupBannerType);
                    curList.Remove(curList.Single(cls => cls.ID == banner.ID));
                }
            }

            foreach (var item in curList)
                GroupBanners.Delete(item.ID);
        }

        #endregion Methods
    }
}