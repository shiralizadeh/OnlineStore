using OnlineStore.DataLayer;
using OnlineStore.Models;
using OnlineStore.Providers;
using System;
using System.Web.Mvc;
using OnlineStore.Providers.Controllers;
using OnlineStore.Models.Enums;
using System.Threading.Tasks;
using System.Collections.Generic;
using OnlineStore.Models.Public;
using System.Web;

namespace OnlineStore.Website.Areas.Admin.Controllers
{
    public class ArticlesController : AdminController
    {
        protected ArticleType _articleType;
        protected GroupType _groupType;

        public ArticlesController()
        {
            _articleType = ArticleType.Blog;
            _groupType = GroupType.Blogs;
            ViewBag.Controller = "Articles";
        }

        public ActionResult Index()
        {
            List<ViewArticleGroup> articleGroups = Groups.GetAll(GroupType.Blogs);

            return View("/Areas/Admin/Views/Articles/Index.cshtml", model: articleGroups);
        }

        [HttpPost]
        public async Task<JsonResult> Get(int pageIndex,
                                          int pageSize,
                                          string pageOrder,
                                          int group,
                                          string userName,
                                          string title,
                                          string fromDate,
                                          string toDate,
                                          sbyte articleStatus
                                          )
        {

            DateTime? sDate = null,
                      eDate = null;

            ArticleStatus? status = null;

            if (!String.IsNullOrWhiteSpace(fromDate))
                sDate = Utilities.ToEnglishDate(fromDate).Date;

            if (!String.IsNullOrWhiteSpace(toDate))
                eDate = Utilities.ToEnglishDate(toDate).Date;

            if (articleStatus != -1)
                status = (ArticleStatus)articleStatus;

            if (pageOrder.Trim() == "ID")
                pageOrder = "LastUpdate desc";

            var list = Articles.Get(pageIndex,
                                    pageSize,
                                    pageOrder,
                                    _articleType,
                                    group,
                                    userName,
                                    title,
                                    sDate,
                                    eDate,
                                    status
                                    );

            foreach (var item in list)
            {
                try
                {
                    item.UserTitle = (await UserManager.FindByIdAsync(item.UserID)).UserName;
                }
                catch (Exception)
                {
                    item.UserTitle = StaticValues.HomeTitle;
                }
            }

            int total = Articles.Count(_articleType, group, userName, title, sDate, eDate, status);
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

        public ActionResult Edit(int? id)
        {
            Article article;

            if (id.HasValue)
            {
                article = Articles.GetByID(id.Value);
                article.Text = HttpUtility.HtmlDecode(article.Text);
            }
            else
            {
                article = new Article();
            }

            return View("/Areas/Admin/Views/Articles/Edit.cshtml", article);
        }

        [HttpPost]
        public ActionResult Edit(Article article, string score)
        {
            try
            {
                float scoreValue = score != "" ? float.Parse(score) : 0;

                string fileName = article.Title.Length > 50 ? article.Title.Substring(0, 50) : article.Title;
                var files = Utilities.SaveFiles(Request.Files, Utilities.GetNormalFileName(fileName), StaticPaths.ArticleImages);

                if (files.Count > 0)
                    article.Image = files[0].Title;

                article.ArticleType = _articleType;
                article.UserID = UserID;
                article.VisitCount = new Random().Next(1, 10);
                article.ArticleScore = scoreValue;
                article.LastUpdate = DateTime.Now;

                ViewBag.Success = true;

                if (article.ID == -1)
                {
                    Articles.Insert(article);

                    UserNotifications.Send(UserID, String.Format("جدید - مطلب وبلاگ '{0}'", article.Title), "/Admin/Articles/Edit/" + article.ID, NotificationType.Success);

                    article = new Article();

                }
                else
                {
                    Articles.Update(article);

                    article.Text = HttpUtility.HtmlDecode(article.Text);
                }

            }
            catch (Exception ex)
            {
                SetErrors(ex);
            }

            return ClearView(article, "/Areas/Admin/Views/Articles/Edit.cshtml");
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