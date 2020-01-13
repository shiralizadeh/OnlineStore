using OnlineStore.DataLayer;
using OnlineStore.Models;
using System;
using System.Linq;
using System.Web.Mvc;
using OnlineStore.Providers.Controllers;
using OnlineStore.Providers;
using OnlineStore.Models.Enums;
using OnlineStore.Identity;
using System.Data.Entity;

namespace OnlineStore.Website.Areas.Admin.Controllers
{
    public class ProductDiscountsController : AdminController
    {
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public JsonResult Get(int pageIndex, int pageSize, string pageOrder, string title, string fromDate, string toDate)
        {
            DateTime? sDate = null,
                      eDate = null;

            if (fromDate != String.Empty)
                sDate = Utilities.ToEnglishDate(fromDate).Date;

            if (toDate != String.Empty)
                eDate = Utilities.ToEnglishDate(toDate).Date;

            var list = ProductDiscounts.Get(pageIndex,
                                            pageSize,
                                            pageOrder,
                                            title,
                                            sDate,
                                            eDate);

            foreach (var item in list)
            {
                if (item.ProductID.HasValue)
                {
                    var product = Products.GetByID(item.ProductID.Value);
                    item.ProductTitle = product.Title;
                }
                else
                    item.ProductTitle = "-";

                if (item.GroupID.HasValue)
                {
                    var group = Groups.GetByID(item.GroupID.Value);
                    item.GroupTitle = group.Title;
                }
                else
                    item.GroupTitle = "-";

                if (item.RoleID != null)
                {
                    var role = IdentityDbContext.Entity.Roles.Where(r => r.Id == item.RoleID).Single();
                    item.RoleTitle = role.Name;
                }
                else
                    item.RoleTitle = "-";
            }

            int total = ProductDiscounts.Count(title, sDate, eDate);
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
                ProductDiscounts.Delete(id);
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
            ProductDiscount productDiscount;

            if (id.HasValue)
            {
                productDiscount = ProductDiscounts.GetByID(id.Value);
                productDiscount.Price = (productDiscount.Price / (ExtensionMethods.IsRial ? 1 : 10));
            }
            else
                productDiscount = new ProductDiscount();

            if (productDiscount.ProductID.HasValue)
                ViewBag.ProductDiscountProductID = true;
            else
                ViewBag.ProductDiscountProductID = false;

            if (productDiscount.GroupID.HasValue)
                ViewBag.ProductDiscountGroupID = true;
            else
                ViewBag.ProductDiscountGroupID = false;

            if (productDiscount.RoleID != null)
                ViewBag.ProductDiscountRoleID = true;
            else
                ViewBag.ProductDiscountRoleID = false;

            return View(productDiscount);
        }

        [HttpPost]
        public ActionResult Edit(ProductDiscount productDiscount, string discountStatus)
        {
            try
            {
                ViewBag.ProductDiscountProductID =
                    ViewBag.ProductDiscountGroupID =
                    ViewBag.ProductDiscountRoleID = false;

                switch (discountStatus)
                {
                    case "0":
                        ViewBag.ProductDiscountProductID = true;

                        //productDiscount.ProductID = null;
                        productDiscount.GroupID = null;
                        productDiscount.RoleID = null;
                        break;
                    case "1":
                        ViewBag.ProductDiscountGroupID = true;

                        productDiscount.ProductID = null;
                        //productDiscount.GroupID = null;
                        productDiscount.RoleID = null;
                        break;
                    case "2":
                        ViewBag.ProductDiscountRoleID = true;

                        productDiscount.ProductID = null;
                        productDiscount.GroupID = null;
                        //productDiscount.RoleID = null;
                        break;
                    default:
                        break;
                }

                switch (productDiscount.DiscountType)
                {
                    case DiscountType.Percent:
                        productDiscount.Price = 0;
                        break;
                    case DiscountType.PriceAfter:
                        productDiscount.Price = (productDiscount.Price * (ExtensionMethods.IsRial ? 1 : 10));
                        productDiscount.Percent = 0;
                        break;
                    case DiscountType.PriceBefore:
                        productDiscount.Price = (productDiscount.Price * (ExtensionMethods.IsRial ? 1 : 10));
                        productDiscount.Percent = 0;
                        break;
                    default:
                        break;
                }

                productDiscount.LastUpdate = DateTime.Now;

                ViewBag.Success = true;
                var id = productDiscount.ID;

                if (productDiscount.ID == -1)
                {
                    ProductDiscounts.Insert(productDiscount);

                    UserNotifications.Send(UserID, String.Format("جدید - تخفیف محصول '{0}'", productDiscount.Percent), "/Admin/ProductDiscounts/Edit/" + productDiscount.ID, NotificationType.Success);
                }
                else
                {
                    ProductDiscounts.Update(productDiscount);
                }

                productDiscount.Price = (productDiscount.Price / (ExtensionMethods.IsRial ? 1 : 10));

                #region Set Task

                var taskText = String.Format("تخفیف \"{0}\" در حال اتمام است.", productDiscount.Title, productDiscount.PersianEndDate);
                var taskDate = productDiscount.EndDate.AddDays(-1);

                UserTasks.SetTask("اتمام مهلت تخفیف",
                                  taskText,
                                  StaticValues.AdminID,
                                  "ProductDiscounts_" + productDiscount.ID,
                                  "/Admin/ProductDiscounts/Edit/" + productDiscount.ID,
                                  taskDate);

                #endregion Set Task

                if (id == -1)
                {
                    productDiscount = new ProductDiscount();
                }
            }
            catch (Exception ex)
            {
                SetErrors(ex);
            }

            return ClearView(productDiscount);
        }

    }
}