using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OnlineStore.Providers.Controllers;
using OnlineStore.Models;
using OnlineStore.DataLayer;
using OnlineStore.Providers;
using OnlineStore.Models.Admin;
using OnlineStore.Models.Enums;

namespace OnlineStore.Website.Areas.Admin.Controllers
{
    public class PriceListLogsController : AdminController
    {
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult GetLogs(string fromDate, string toDate)
        {
            var jsonSuccessResult = new JsonSuccessResult();

            try
            {
                DateTime? fDate = null,
                          tDate = null;

                if (!String.IsNullOrWhiteSpace(fromDate))
                {
                    fDate = Utilities.ToEnglishDate(fromDate);
                }
                if (!String.IsNullOrWhiteSpace(toDate))
                {
                    tDate = Utilities.ToEnglishDate(toDate);
                }

                var list = PriceListLogs.Get(fDate, tDate);

                renderValue(ref list);

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

        public static void renderValue(ref List<JsonPriceListLogGroup> priceList)
        {
            foreach (var item in priceList)
            {
                var logs = item.PriceListLogs;
                foreach (var lgs in logs)
                {
                    lgs.PriceListFieldName = lgs.PriceListField.GetEnumDisplay();

                    switch (lgs.PriceListField)
                    {
                        case PriceListFieldName.Price:
                            int poVal = Int32.Parse(lgs.OldValue);
                            int pnVal = Int32.Parse(lgs.NewValue);

                            if (poVal < pnVal)
                            {
                                lgs.ColorClass = "green-record";
                            }
                            else
                            {
                                lgs.ColorClass = "red-record";
                            }

                            break;

                        case PriceListFieldName.IsAvailable:
                            bool ioVal = Boolean.Parse(lgs.OldValue);
                            bool inVal = Boolean.Parse(lgs.NewValue);

                            if (!ioVal && inVal)
                            {
                                lgs.NewValue = "موجود";
                                lgs.OldValue = "ناموجود";
                                lgs.ColorClass = "green-record";
                            }
                            else
                            {
                                lgs.OldValue = "موجود";
                                lgs.NewValue = "ناموجود";
                                lgs.ColorClass = "red-record";
                            }
                            break;
                        default:
                            break;
                    }
                }
            }
        }

    }
}