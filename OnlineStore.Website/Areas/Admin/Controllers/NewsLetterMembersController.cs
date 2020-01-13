using OnlineStore.DataLayer;
using OnlineStore.Models;
using System;
using System.Web.Mvc;
using OnlineStore.Providers.Controllers;
using System.Threading.Tasks;

namespace OnlineStore.Website.Areas.Admin.Controllers
{
    public class NewsLetterMembersController : AdminController
    {
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public JsonResult Get(int pageIndex, int pageSize, string pageOrder)
        {
            if (pageOrder.Trim() == "ID")
            {
                pageOrder = "LastUpdate desc";
            }

            var list = NewsLetterMembers.Get(pageIndex,
                                           pageSize,
                                           pageOrder);

            int total = NewsLetterMembers.Count();
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
                NewsLetterMembers.Delete(id);
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