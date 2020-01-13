using System;
using System.Web.Mvc;
using OnlineStore.Providers;
using OnlineStore.DataLayer;
using System.Threading.Tasks;
using OnlineStore.Models.Enums;
using Microsoft.AspNet.Identity;
using OnlineStore.Providers.Controllers;
using OnlineStore.Models.Admin;

namespace OnlineStore.Website.Areas.Admin.Controllers
{
    public class UnsuccessfulOrdersController : AdminController
    {
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<JsonResult> Get(int pageIndex,
                                          int pageSize,
                                          string pageOrder,
                                          string userName,
                                          int? orderID,
                                          int? fromPrice,
                                          int? toPrice,
                                          string fromDate,
                                          string toDate,
                                          sbyte cartStatus,
                                          SendMethodType? sendMethodType,
                                          PaymentMethodType? paymentMethodType)
        {

            DateTime? sDate = null,
                      eDate = null;

            CartStatus? crtStatus = null;

            if (!String.IsNullOrWhiteSpace(fromDate))
                sDate = Utilities.ToEnglishDate(fromDate).Date;

            if (!String.IsNullOrWhiteSpace(toDate))
                eDate = Utilities.ToEnglishDate(toDate).Date;

            if (cartStatus != -1)
                crtStatus = (CartStatus)cartStatus;

            var list = Carts.Get(pageIndex,
                                 pageSize,
                                 pageOrder,
                                 orderID,
                                 fromPrice,
                                 toPrice,
                                 sDate,
                                 eDate,
                                 crtStatus,
                                 null,
                                 sendMethodType,
                                 paymentMethodType,
                                 "",
                                 OrderStatus.Unsuccessful);

            foreach (var item in list)
            {
                var user = (await UserManager.FindByIdAsync(item.UserID));
                if (user != null)
                {
                    item.UserName = user.UserName;
                    item.Firstname = user.Firstname;
                    item.Lastname = user.Lastname;
                }
            }

            int total = Carts.Count(orderID,
                                    fromPrice,
                                    toPrice,
                                    sDate,
                                    eDate,
                                    crtStatus,
                                    null,
                                    sendMethodType,
                                    paymentMethodType,
                                    "",
                                    OrderStatus.Unsuccessful);

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
    }
}