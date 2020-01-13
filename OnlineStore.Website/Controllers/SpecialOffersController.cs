using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OnlineStore.Providers.Controllers;
using OnlineStore.DataLayer;
using OnlineStore.Providers;
using OnlineStore.Models.Public;
using OnlineStore.Models;

namespace OnlineStore.Website.Controllers
{
    public class SpecialOffersController : PublicController
    {
        [HttpGet]
        [Route("SpecialOffers/")]
        [Route("SpecialOffers/{PageIndex:int}")]
        public ActionResult Index(
            int pageIndex = 1,
            int pageSize = 24,
            string pageOrder = "")
        {
            if (pageIndex > 0)
            {
                pageIndex = pageIndex - 1;
            }
            else
                pageIndex = 0;

            int count;
            var productList = Products.GetSpecialOffers(pageIndex, pageSize, pageOrder);
            count = Products.CountSpecialOffers();

            var totalPages = (int)Math.Ceiling((decimal)count / pageSize);
            var paging = Utilities.MakePaging(totalPages, pageIndex + 1);

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

            var model = new SpecialOfferSettings
            {
                Products = productList,
                Paging = paging,
                TotalPages = totalPages,
                CurrentPageIndex = pageIndex
            };

            return View(model);
        }

        [HttpPost]
        [Route("SpecialOffers/")]
        public JsonResult AjaxList(
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
                var productList = Products.GetSpecialOffers(pageIndex, pageSize, pageOrder);
                var count = Products.CountSpecialOffers();

                var totalPages = (int)Math.Ceiling((decimal)count / pageSize);
                var paging = Utilities.MakePaging(totalPages, pageIndex + 1);

                foreach (var item in productList)
                {
                    var group = Groups.GetByID(item.GroupID.Value);
                    item.Url = UrlProvider.GetProductUrl(item.ID, group.UrlPerfix, item.UrlPerfix);
                }

                var model = new SpecialOfferSettings
                {
                    Products = productList,
                    Paging = paging,
                    TotalPages = totalPages,
                    CurrentPageIndex = pageIndex
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