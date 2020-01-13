using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OnlineStore.DataLayer;
using OnlineStore.Providers;
using OnlineStore.Providers.Controllers;
using OnlineStore.Models.Public;
using OnlineStore.Models;

namespace OnlineStore.Website.Controllers
{
    public class TagsController : PublicController
    {
        [HttpGet]
        [Route("tags/{KeywordTitle}")]
        [Route("tags/{KeywordTitle}/{PageIndex:int}")]
        public ActionResult List(string keywordTitle,
            int pageIndex = 1,
            int pageSize = 24,
            string pageOrder = "Weight")
        {

            if (pageIndex > 0)
            {
                pageIndex = pageIndex - 1;
            }
            else
                pageIndex = 0;

            if (String.IsNullOrWhiteSpace(keywordTitle))
                return new HttpNotFoundResult();

            keywordTitle = keywordTitle.DeNormalizeForUrl();

            var keyword = Keywords.GetByTitle(keywordTitle);

            if (keyword == null)
                return new HttpNotFoundResult();

            int count;
            var productList = Products.GetList(pageIndex, pageSize, pageOrder, out count, keywordID: keyword.ID);

            var totalPages = (int)Math.Ceiling((decimal)count / pageSize);
            var paging = Utilities.MakePaging(totalPages, pageIndex + 1);

            ViewBag.OriginalUrl = "/tags/" + keywordTitle;
            ViewBag.CanonicalUrl = ViewBag.OriginalUrl + "/" + (pageIndex + 1);

            if (totalPages > 1)
            {
                ViewBag.HasPaging = true;
                if (pageIndex == 0)
                    ViewBag.PrevPage = (int?)null;
                else
                    ViewBag.PrevPage = (pageIndex);

                if (pageIndex == totalPages - 1)
                    ViewBag.NextPage = (int?)null;
                else
                    ViewBag.NextPage = (pageIndex + 2);
            }

            Products.FillProductItems(UserID, productList, StaticValues.DefaultProductImageSize);

            var title = "کلیدواژه (" + keywordTitle + ")";
            if (pageIndex > 0)
            {
                title += " صفحه - " + (pageIndex + 1);
            }

            ViewBag.Title = title;
            ViewBag.Keyword = keywordTitle;

            var model = new TagsSettings
            {
                Products = productList,
                Paging = paging,
                TotalPages = totalPages,
                CurrentPageIndex = pageIndex,
                OriginalUrl = ViewBag.OriginalUrl,
                PageTitle = title,
                CanonicalUrl = ViewBag.CanonicalUrl
            };

            return View(model);
        }

        [HttpPost]
        [Route("tags/{KeywordTitle}")]
        public JsonResult AjaxList(
           string keywordTitle,
           int pageIndex,
           int pageSize,
           string pageOrder)
        {
            if (pageIndex > 0)
            {
                pageIndex = pageIndex - 1;
            }
            else
                pageIndex = 0;

            var jsonSuccessResult = new JsonSuccessResult();

            try
            {
                keywordTitle = keywordTitle.DeNormalizeForUrl();

                var keyword = Keywords.GetByTitle(keywordTitle);

                ViewBag.Title = keyword.Title;

                int count;
                var productList = Products.GetList(pageIndex, pageSize, pageOrder, out count, keywordID: keyword.ID);

                var totalPages = (int)Math.Ceiling((decimal)count / pageSize);
                var paging = Utilities.MakePaging(totalPages, pageIndex + 1);

                foreach (var item in productList)
                {
                    var group = Groups.GetByID(item.GroupID.Value);
                    item.Url = UrlProvider.GetProductUrl(item.ID, group.UrlPerfix, item.UrlPerfix);
                }

                var title = "کلیدواژه (" + keywordTitle + ")";
                if (pageIndex > 0)
                {
                    title += " صفحه - " + (pageIndex + 1);
                }

                ViewBag.Title = title + " - " + StaticValues.WebsiteTitle;


                ViewBag.OriginalUrl = "/tags/" + keywordTitle;
                ViewBag.CanonicalUrl = ViewBag.OriginalUrl + "/" + (pageIndex + 1);

                var model = new TagsSettings
                {
                    Products = productList,
                    Paging = paging,
                    TotalPages = totalPages,
                    CurrentPageIndex = pageIndex,
                    OriginalUrl = ViewBag.OriginalUrl,
                    PageTitle = ViewBag.Title,
                    CanonicalUrl = ViewBag.CanonicalUrl
                };

                Products.FillProductItems(UserID, model.Products, StaticValues.DefaultProductImageSize);

                jsonSuccessResult.Data = model;
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