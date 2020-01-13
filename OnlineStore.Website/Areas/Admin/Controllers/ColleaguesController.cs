using OnlineStore.DataLayer;
using OnlineStore.Models;
using System;
using System.Web.Mvc;
using OnlineStore.Providers.Controllers;
using System.Threading.Tasks;
using OnlineStore.Models.Enums;
using OnlineStore.Services;
using OnlineStore.Providers;

namespace OnlineStore.Website.Areas.Admin.Controllers
{
    public class ColleaguesController : AdminController
    {
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public JsonResult Get(int pageIndex, int pageSize, string pageOrder, string name)
        {

            var list = Colleagues.Get(pageIndex,
                                           pageSize,
                                           pageOrder,
                                           name);

            int total = Colleagues.Count(name);
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
                Colleagues.Delete(id);
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

        [HttpGet]
        public ActionResult Edit(int id)
        {
            Colleague colleague;

            colleague = Colleagues.GetByID(id);

            if (colleague.CooperationDescription != null)
            {
                colleague.CooperationDescription = colleague.CooperationDescription.Replace("\r\n", "<br/>");
            }
            if (colleague.Text != null)
            {
                colleague.Text = colleague.Text.Replace("\r\n", "<br/>");
            }

            return View(model: colleague);
        }

        [HttpPost]
        public ActionResult Edit(Colleague colleague)
        {
            try
            {
                colleague.LastUpdate = DateTime.Now;

                ViewBag.Success = true;

                Colleagues.Update(colleague);

            }
            catch (Exception ex)
            {
                SetErrors(ex);
            }

            return RedirectToAction("Index");
        }
    }
}