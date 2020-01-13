using OnlineStore.DataLayer;
using OnlineStore.Models;
using OnlineStore.Providers;
using System;
using System.Web.Mvc;
using OnlineStore.Providers.Controllers;
using OnlineStore.Models.Enums;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace OnlineStore.Website.Areas.User.Controllers
{
    [RoutePrefix("My-Account/My-Posts")]
    public class ArticlesController : UserController
    {
        protected ArticleType _articleType;
        protected GroupType _groupType;

        public ArticlesController()
        {
            _articleType = ArticleType.Blog;
            _groupType = GroupType.Blogs;
            ViewBag.Controller = "Articles";

        }

        [Route]
        public ActionResult Index()
        {
            var list = Articles.GetByUserID(_articleType, UserID);

            return View("/Areas/User/Views/Articles/Index.cshtml", model: list);
        }

        [HttpPost]
        [Route("Delete")]
        public JsonResult Delete(int id)
        {
            var jsonSuccessResult = new JsonSuccessResult();

            try
            {
                Articles.Delete(id);
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

        [Route("Edit")]
        [Route("Edit/{id}")]
        public ActionResult Edit(int? id)
        {
            Article article;

            if (id.HasValue)
            {
                article = Articles.GetByID(id.Value);
            }
            else
            {
                article = new Article();
            }

            return View("/Areas/User/Views/Articles/Edit.cshtml", article);
        }

        [HttpPost]
        [Route("Edit")]
        public ActionResult Edit(Article article)
        {
            try
            {
                var files = Utilities.SaveFiles(Request.Files, Utilities.GetNormalFileName(article.Title), StaticPaths.ArticleImages);

                if (files.Count > 0)
                    article.Image = files[0].Title;

                article.ArticleType = _articleType;
                article.UserID = UserID;
                article.LastUpdate = DateTime.Now;
                article.ArticleStatus = ArticleStatus.NotChecked;

                ViewBag.Success = true;

                if (article.ID == -1)
                {
                    article.IsVisible = false;
                    article.PublishDate = DateTime.Now;

                    Articles.Insert(article);

                    UserNotifications.Send(UserID, String.Format("جدید - مطلب وبلاگ '{0}'", article.Title), "/Admin/Articles/Edit/" + article.ID, NotificationType.Success);

                    article = new Article();

                }
                else
                {
                    Articles.UpdateByUser(article);
                }

            }
            catch (Exception ex)
            {
                SetErrors(ex);
            }

            return ClearView(article, "/Areas/User/Views/Articles/Edit.cshtml");
        }

        #region TreeView

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

        #endregion TreeView

    }
}