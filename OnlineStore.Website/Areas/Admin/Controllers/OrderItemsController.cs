using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OnlineStore.Models.User;
using OnlineStore.Providers.Controllers;
using OnlineStore.DataLayer;
using System.Threading.Tasks;
using System.Text;

namespace OnlineStore.Website.Areas.Admin.Controllers
{
    public class OrderItemsController : AdminController
    {
        public ActionResult Index(int id)
        {
            ViewBag.Title = "سفارش " + id;
            ViewBag.OrderItemID = id;

            var userID = Carts.GetByID(id).UserID;
            var user = Identity.OSUsers.GetByID(userID);
            var fullName = user.Firstname + " " + user.Lastname;
            var userName = user.UserName;
            var phone = user.Phone;
            var mobile = user.Mobile;

            StringBuilder model = new StringBuilder();

            model.Append("<div class='alert alert-info'>");
            model.Append("<h4>جزئیات سفارش:</h4><hr>");
            model.AppendFormat("کد سفارش: {0} <br/> نام کاربری: {1} <br/> نام و نام خانوادگی: {2} <br/> شماره تماس: {3} <br/> شماره همراه: {4}",
                                          id,
                                          userName,
                                          fullName,
                                          phone,
                                          mobile);
            model.Append("</div>");

            return View(model: model);
        }

        [HttpPost]
        public JsonResult Get(int pageIndex, int pageSize, string pageOrder, int id)
        {
            var list = CartItems.GetOrderItems(id);
            foreach (var item in list)
            {
                string[] name = new string[item.Gifts.Count];
                if (item.Gifts.Count > 0)
                {
                    int i = 0;
                    foreach (var gift in item.Gifts)
                    {
                        name[i] = gift.GiftTitle;
                        i++;
                    }

                    item.ProductTitle += "<br/>" + "<i class='icon-gift'></i>" + String.Join(",", name);
                }
            }

            int total = CartItems.CountOrderItems(id);
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