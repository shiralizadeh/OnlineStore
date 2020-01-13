using OnlineStore.DataLayer;
using OnlineStore.Models;
using OnlineStore.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OnlineStore.Providers.Controllers;
using OnlineStore.Models.Enums;

namespace OnlineStore.Website.Areas.Admin.Controllers
{
    public class GiftCardsController : AdminController
    {
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public JsonResult Get(int pageIndex, int pageSize, string pageOrder, string serial, string fromDate, string toDate, string isUsed)
        {
            DateTime? sDate = null,
                      eDate = null;
            bool? used = null;

            if (fromDate != String.Empty)
            {
                sDate = Utilities.ToEnglishDate(fromDate).Date;
            }

            if (toDate != String.Empty)
            {
                eDate = Utilities.ToEnglishDate(toDate).Date;
            }

            if (isUsed != "-1")
            {
                used = Boolean.Parse(isUsed);
            }

            var list = GiftCards.Get(pageIndex,
                                     pageSize,
                                     pageOrder,
                                     serial,
                                     sDate,
                                     eDate,
                                     used);

            int total = GiftCards.Count(serial, sDate, eDate, used);
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
                GiftCards.Delete(id);
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
            GiftCard giftCard;

            if (id.HasValue)
                giftCard = GiftCards.GetByID(id.Value);
            else
                giftCard = new GiftCard();

            return View(giftCard);
        }

        [HttpPost]
        public ActionResult Edit(GiftCard giftCard)
        {
            try
            {
                giftCard.LastUpdate = DateTime.Now;

                ViewBag.Success = true;

                if (giftCard.ID == -1)
                {
                    GiftCards.Insert(giftCard);

                    UserNotifications.Send(UserID, String.Format("جدید - سریال تخفیف '{0}' با '{1}' درصد اضافه شد", giftCard.Serial, giftCard.Percent), "/Admin/GiftCards/Edit/" + giftCard.ID, NotificationType.Success);
                    giftCard = new GiftCard();
                }
                else
                {
                    GiftCards.Update(giftCard);
                }
            }
            catch (Exception ex)
            {
                SetErrors(ex);
            }

            return ClearView(giftCard);
        }
    }
}