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
    public class MenuItemsController : AdminController
    {
        public ActionResult Index()
        {
            var groups = MenuItems.GetAll();
            return View(model: groups);
        }

        [HttpPost]
        public JsonResult Get(int pageIndex, int pageSize, string pageOrder, sbyte menuItemType, string title, int groupID)
        {
            List<int> childs = new List<int>();
            if (groupID != -1)
            {
                childs = MenuItems.GetChildsRecursive(groupID).Select(item => item.ID).ToList();
            }

            MenuItemType? itemType = null;

            if (menuItemType != -1)
                itemType = (MenuItemType)menuItemType;

            var list = MenuItems.Get(pageIndex, pageSize, pageOrder, itemType, title, childs, groupID);

            int total = MenuItems.Count(itemType, title, childs, groupID);
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

        public JsonResult GetItems(bool multiple)
        {
            JsonResult result = new JsonResult()
            {
                Data = FillUsersGroups_Root(multiple ? TreeViewSelectMode.Multiple : TreeViewSelectMode.Single),
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };

            return result;
        }

        public JsonResult Delete(int id)
        {
            var jsonSuccessResult = new JsonSuccessResult();

            try
            {
                MenuItems.Delete(id);
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
            EditMenuItem item;

            if (id.HasValue)
            {
                item = Mapper.Map<EditMenuItem>(MenuItems.GetByID(id.Value));

                item.Content = HttpUtility.HtmlDecode(item.Content);
                item.Banners = MenuItemBanners.GetByMenuItemID(item.ID);

            }
            else
                item = new EditMenuItem();

            return View(item);
        }

        [HttpPost]
        public ActionResult Edit(EditMenuItem editMenuItem)
        {
            try
            {
                var menuItem = Mapper.Map<MenuItem>(editMenuItem);

                menuItem.LastUpdate = DateTime.Now;

                ViewBag.Success = true;
                int menuItemID = editMenuItem.ID;

                if (menuItem.ID == -1)
                {
                    MenuItems.Insert(menuItem);
                    menuItemID = menuItem.ID;

                    SaveBanners(editMenuItem, menuItemID);

                    UserNotifications.Send(UserID, String.Format("جدید برای منو - آیتم '{0}'", menuItem.Title), "/Admin/MenuItems/Edit/" + menuItem.ID, NotificationType.Success);
                    editMenuItem = new EditMenuItem();
                }
                else
                {
                    MenuItems.Update(menuItem);

                    SaveBanners(editMenuItem, menuItemID);

                    menuItem.Content = HttpUtility.HtmlDecode(menuItem.Content);
                }
            }
            catch (Exception ex)
            {
                SetErrors(ex);
            }

            return ClearView(editMenuItem);
        }

        #region Methods

        private List<TreeItem> FillUsersGroups_Root(TreeViewSelectMode mode)
        {
            List<TreeItem> list = new List<TreeItem>();

            foreach (var item in MenuItems.GetRoot())
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
            foreach (var item in MenuItems.GetByParentID(parentNode.id))
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

        private static void SaveBanners(EditMenuItem editMenuItem, int menuItemID)
        {
            var curList = MenuItemBanners.GetByMenuItemID(menuItemID);

            foreach (var banner in editMenuItem.Banners)
            {
                if (!curList.Any(item => item.ID == banner.ID))
                {
                    var menuItemBanner = Mapper.Map<MenuItemBanner>(banner);

                    menuItemBanner.MenuItemID = menuItemID;
                    menuItemBanner.Key = Guid.NewGuid();

                    MenuItemBanners.Insert(menuItemBanner);
                }
                else
                {
                    MenuItemBanners.UpdateMenuItemBannerType(banner.ID, banner.MenuItemBannerType);
                    curList.Remove(curList.Single(cls => cls.ID == banner.ID));
                }
            }

            foreach (var item in curList)
                MenuItemBanners.Delete(item.ID);
        }

        #endregion Methods
    }
}