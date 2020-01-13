using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OnlineStore.Providers.Controllers;
using OnlineStore.DataLayer;
using OnlineStore.Models.Admin;
using OnlineStore.Models;
using OnlineStore.Models.Enums;

namespace OnlineStore.Website.Areas.Admin.Controllers
{
    public class PriceListController : AdminController
    {
        public ActionResult Index(byte id)
        {
            var list = PriceListSections.GetWithProducts((PriceListSectionType)id);

            return View(list);
        }

        public JsonResult SortProducts(int[] productItems)
        {
            var jsonSuccessResult = new JsonSuccessResult();

            try
            {
                int order = 0;

                foreach (var item in productItems)
                {
                    PriceListProducts.UpdateOrderID(item, order);
                    order++;
                }

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

        public JsonResult SortSections(string[] Col1, string[] Col2, string[] Col3, string[] Col4, string[] Col5)
        {
            var jsonSuccessResult = new JsonSuccessResult();

            try
            {
                #region Column1

                int order = 0;

                if (Col1 != null)
                {
                    foreach (var item in Col1)
                    {
                        PriceListSections.SortSections(Int32.Parse(item), order, 1);
                        order++;
                    }
                }

                #endregion Column1

                #region Column2

                order = 0;

                if (Col2 != null)
                {
                    foreach (var item in Col2)
                    {
                        PriceListSections.SortSections(Int32.Parse(item), order, 2);
                        order++;
                    }
                }

                #endregion Column2

                #region Column3

                order = 0;

                if (Col3 != null)
                {
                    foreach (var item in Col3)
                    {
                        PriceListSections.SortSections(Int32.Parse(item), order, 3);
                        order++;
                    }
                }

                #endregion Column3

                #region Column4

                order = 0;

                if (Col4 != null)
                {
                    foreach (var item in Col4)
                    {
                        PriceListSections.SortSections(Int32.Parse(item), order, 4);
                        order++;
                    }
                }

                #endregion Column4

                #region Column5

                order = 0;

                if (Col5 != null)
                {
                    foreach (var item in Col5)
                    {
                        PriceListSections.SortSections(Int32.Parse(item), order, 5);
                        order++;
                    }
                }

                #endregion Column5

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

        public JsonResult SaveChanges(int id, string newValue, PriceListFieldName priceListFieldName)
        {
            var jsonSuccessResult = new JsonSuccessResult();

            try
            {
                string oldValue = String.Empty;

                // Update Row
                PriceListProducts.SaveChanges(id, newValue, priceListFieldName, out oldValue);

                // Insert Log
                #region Log

                PriceListLog log = new PriceListLog
                {
                    OldValue = oldValue,
                    NewValue = newValue,
                    PriceListFieldName = priceListFieldName,
                    PriceListProductID = id,
                    LastUpdate = DateTime.Now
                };

                PriceListLogs.Inset(log);

                #endregion Log

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

        public ActionResult Print(byte id)
        {
            var list = PriceListSections.GetWithProducts((PriceListSectionType)id);

            return View(list);
        }
    }
}