using OnlineStore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OnlineStore.DataLayer;
using OnlineStore.Providers;
using OnlineStore.Models.Admin;
using AutoMapper;
using OnlineStore.Providers.Controllers;
using OnlineStore.Models.Enums;

namespace OnlineStore.Website.Areas.Admin.Controllers
{
    public class PackagesController : AdminController
    {
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult Get(int pageIndex, int pageSize, string pageOrder, string title, string startDate, string endDate)
        {
            if (pageOrder.Trim() == "ID")
                pageOrder = "OrderID";

            DateTime? sDate = null,
                      eDate = null;

            if (!String.IsNullOrWhiteSpace(startDate))
                sDate = Utilities.ToEnglishDate(startDate).Date;

            if (!String.IsNullOrWhiteSpace(endDate))
                eDate = Utilities.ToEnglishDate(endDate).Date;

            var list = Packages.Get(pageIndex, pageSize, pageOrder, title, sDate, eDate);

            int total = Packages.Count(title, sDate, eDate);
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
                Packages.Delete(id);
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
            EditPackage editPackage;

            if (id.HasValue)
            {
                editPackage = Mapper.Map<EditPackage>(Packages.GetByID(id.Value));
                var products = PackageProducts.GetByPackageID(editPackage.ID);

                editPackage.Text = HttpUtility.HtmlDecode(editPackage.Text);
                editPackage.Images = PackageImages.GetByPackageID(editPackage.ID);
                editPackage.Products = Mapper.Map<List<EditPackageProduct>>(products);

            }
            else
            {
                editPackage = new EditPackage();
            }

            return View(editPackage);
        }

        public JsonResult GetProducts()
        {
            var jsonSuccessResult = new JsonSuccessResult();

            try
            {
                jsonSuccessResult.Data = Products.GetAllForPackage();
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

        public JsonResult GetVarients(int productID)
        {
            var jsonSuccessResult = new JsonSuccessResult();

            try
            {
                var list = ProductVarients.GetShortVarientByProductID(productID);

                jsonSuccessResult.Success = true;
                jsonSuccessResult.Data = list;
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
        [HttpPost]
        public ActionResult Edit(EditPackage editPackage, string score)
        {
            try
            {
                float scoreValue = score != "" ? float.Parse(score) : 0;

                var package = Mapper.Map<Package>(editPackage);

                package.LastUpdate = DateTime.Now;
                package.PackageScore = scoreValue;
                ViewBag.Success = true;

                int packageID = package.ID;
                if (packageID == -1)
                {
                    package.CreatedDate = DateTime.Now;

                    Packages.Insert(package);
                    packageID = package.ID;

                    SaveImages(editPackage, packageID);
                    SaveProducts(editPackage, packageID);

                    UserNotifications.Send(UserID, String.Format("جدید - بسته '{0}'", editPackage.Title), "/Admin/Packages/Edit/" + editPackage.ID, NotificationType.Success);
                    editPackage = new EditPackage();
                }
                else
                {
                    Packages.Update(package);

                    SaveImages(editPackage, packageID);
                    SaveProducts(editPackage, packageID);

                    editPackage.Text = HttpUtility.HtmlDecode(editPackage.Text);
                    editPackage.PackageScore = package.PackageScore;
                }
            }
            catch (Exception ex)
            {
                SetErrors(ex);
            }

            return ClearView(editPackage);
        }

        #region Methods

        private static void SaveImages(EditPackage editPackage, int packageID)
        {
            var curList = PackageImages.GetByPackageID(packageID);

            foreach (var image in editPackage.Images)
            {
                if (!curList.Any(item => item.ID == image.ID))
                {
                    var packageImage = Mapper.Map<PackageImage>(image);

                    packageImage.PackageID = packageID;

                    PackageImages.Insert(packageImage);
                }
                else
                {
                    PackageImages.UpdatePackageImagePlace(image.ID, image.ProductImagePlace);
                    curList.Remove(curList.Single(cls => cls.ID == image.ID));
                }
            }

            foreach (var item in curList)
                PackageImages.Delete(item.ID);
        }

        private static void SaveProducts(EditPackage editPackage, int packageID)
        {
            var curList = PackageProducts.GetByPackageID(packageID);

            foreach (var product in editPackage.Products)
            {
                var packageProduct = Mapper.Map<PackageProduct>(product);

                if (packageProduct.ProductVarientID == -1)
                {
                    packageProduct.ProductVarientID = null;
                }

                if (!curList.Any(item => item.ID == product.ID))
                {
                    packageProduct.PackageID = packageID;
                    packageProduct.NewPrice = packageProduct.NewPrice * (ExtensionMethods.IsRial ? 1 : 10);
                    packageProduct.OldPrice = packageProduct.OldPrice * (ExtensionMethods.IsRial ? 1 : 10);

                    PackageProducts.Insert(packageProduct);
                }
                else
                {
                    PackageProducts.Update(packageProduct);
                    curList.Remove(curList.Single(cls => cls.ID == packageProduct.ID));
                }
            }

            foreach (var item in curList)
                PackageProducts.Delete(item.ID);
        }


        #endregion Methods
    }
}