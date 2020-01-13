using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OnlineStore.DataLayer;
using OnlineStore.Models.Public;
using OnlineStore.Models.Enums;
using OnlineStore.Providers.Controllers;
using OnlineStore.Providers;
using OnlineStore.Models;

namespace OnlineStore.Website.Controllers
{
    public class PackagesController : PublicController
    {
        [Route("Packages")]
        [Route("Packages/{PageIndex:int}")]
        public ActionResult List(
            int pageIndex = 1,
            int pageSize = 24,
            string pageOrder = "OrderID")
        {
            if (pageIndex > 0)
            {
                pageIndex = pageIndex - 1;
            }
            else
                pageIndex = 0;

            int count;
            var packageList = Packages.GetList(pageIndex, pageSize, pageOrder);
            count = Packages.CountList();

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

            var model = new PackageListSettings
            {
                Packages = packageList,
                Paging = paging,
                TotalPages = totalPages,
                CurrentPageIndex = pageIndex
            };

            return View(model);
        }

        [HttpPost]
        [Route("Packages")]
        public JsonResult AjaxList(
           int pageIndex,
           int pageSize,
           string pageOrder = "OrderID")
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
                var packageList = Packages.GetList(pageIndex, pageSize, pageOrder);
                int count = Packages.CountList();

                var totalPages = (int)Math.Ceiling((decimal)count / pageSize);
                var paging = Utilities.MakePaging(totalPages, pageIndex + 1);

                ViewBag.Title = "بسته های محصولات - صفحه " + (pageIndex + 1);

                var model = new PackageListSettings
                {
                    Packages = packageList,
                    Paging = paging,
                    TotalPages = totalPages,
                    CurrentPageIndex = pageIndex,
                    PageTitle = ViewBag.Title
                };

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

        [Route("Packages/{title}-{id:int}")]
        public ActionResult Details(int id)
        {
            bool isAdmin = false;
            if (base.IsAdmin || base.IsWriter)
            {
                isAdmin = true;
            }

            var packageDetails = Packages.GetDetails(id, isAdmin);
            if (packageDetails == null)
            {
                return HttpNotFound();
            }

            var allImages = PackageImages.GetByPackageID(id);
            var defaultImage = allImages.Where(item => item.ProductImagePlace == ProductImagePlace.Home).FirstOrDefault();

            var products = PackageProducts.GetProducts(id);
            packageDetails.NewPrice = products.Sum(item => item.NewPrice);
            packageDetails.OldPrice = products.Sum(item => item.OldPrice);

            PackageDetailSettings packageDetailSettings = new PackageDetailSettings
            {
                PackageDetails = packageDetails,
                DefaultImage = defaultImage,
                PackageImages = allImages,
                PackageProducts = products
            };

            return View(packageDetailSettings);
        }
    }
}