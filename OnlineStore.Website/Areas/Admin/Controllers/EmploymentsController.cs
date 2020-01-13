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
    public class EmploymentsController : AdminController
    {
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public JsonResult Get(int pageIndex, int pageSize, string pageOrder, string name)
        {

            var list = Employments.Get(pageIndex,
                                           pageSize,
                                           pageOrder,
                                           name);

            int total = Employments.Count(name);
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
                Employments.Delete(id);
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
            Employment employment;

            employment = Employments.GetByID(id);
            if (employment.EducationCoursesText != null)
            {
                employment.EducationCoursesText = employment.EducationCoursesText.Replace("\r\n", "<br/>");
            }
            if (employment.ResumeText != null)
            {
                employment.ResumeText = employment.ResumeText.Replace("\r\n", "<br/>");
            }
            if (employment.StudyText != null)
            {
                employment.StudyText = employment.StudyText.Replace("\r\n", "<br/>");
            }

            return View(model: employment);
        }

        [HttpPost]
        public ActionResult Edit(Employment employment)
        {
            try
            {
                employment.LastUpdate = DateTime.Now;

                ViewBag.Success = true;

                Employments.Update(employment);

            }
            catch (Exception ex)
            {
                SetErrors(ex);
            }

            return RedirectToAction("Index");
        }
    }
}