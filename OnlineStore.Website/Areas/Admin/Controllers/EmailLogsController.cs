using OnlineStore.DataLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OnlineStore.Website.Areas.Admin.Controllers
{
    public class EmailLogsController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public JsonResult Get(int pageIndex, int pageSize, string pageOrder, string to)
        {
            if (pageOrder.Trim() == "ID")
            {
                pageOrder = "LastUpdate desc";
            }

            var list = EmailLogs.Get(pageIndex,
                                           pageSize,
                                           pageOrder,
                                           to);

            int total = EmailLogs.Count(to);
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