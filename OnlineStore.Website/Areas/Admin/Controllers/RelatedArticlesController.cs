using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OnlineStore.DataLayer;
using OnlineStore.Models;
using OnlineStore.Providers.Controllers;
using OnlineStore.Models.Admin;

namespace OnlineStore.Website.Areas.Admin.Controllers
{
    public class RelatedArticlesController : AdminController
    {
        public ActionResult Index(int articleID)
        {
            string title = Articles.GetTitleByID(articleID).Title;
            ViewBag.Title = "مطالب مرتبط با مقاله « " + title + " » ";
            ViewBag.ArticleID = articleID;

            AjaxSettings settings = new AjaxSettings
            {
                Url = "/RelatedArticles/Search"
            };

            return View(model: settings);
        }

        [HttpPost]
        public JsonResult Get(int articleID)
        {
            var jsonSuccessResult = new JsonSuccessResult();

            try
            {
                var list = RelatedArticles.Get(articleID);

                jsonSuccessResult.Data = list;
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

        public JsonResult Search(string key)
        {
            var jsonSuccessResult = new JsonSuccessResult();

            try
            {
                var list = Articles.SimpleSearch(key);

                jsonSuccessResult.Data = list;
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

        public JsonResult Update(int articleID, string articles)
        {
            var jsonSuccessResult = new JsonSuccessResult();

            try
            {
                string[] arrArticles = articles.Split(',');

                // حذف
                #region Delete All

                RelatedArticles.DeleteRelatedArticles(articleID);

                #endregion Delete All

                // ثبت مجدد
                #region Add

                List<RelatedArticle> listItems = new List<RelatedArticle>();

                foreach (var item in arrArticles)
                {
                    if (!String.IsNullOrWhiteSpace(item))
                    {
                        RelatedArticle article = new RelatedArticle
                        {
                            ArticleID = articleID,
                            RelationID = Int32.Parse(item),
                            LastUpdate = DateTime.Now,
                        };

                        listItems.Add(article);
                    }
                }

                RelatedArticles.Insert(listItems);

                #endregion Add

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