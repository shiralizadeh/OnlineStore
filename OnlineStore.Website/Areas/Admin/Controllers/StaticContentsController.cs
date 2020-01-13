using OnlineStore.DataLayer;
using System;
using System.Web.Mvc;
using OnlineStore.Providers.Controllers;
using OnlineStore.Models.Admin;
using AutoMapper;
using System.Web;

namespace OnlineStore.Website.Areas.Admin.Controllers
{
    public class StaticContentsController : AdminController
    {
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public JsonResult Get(int pageIndex, int pageSize, string pageOrder)
        {
            var list = StaticContents.Get(pageIndex,
                                           pageSize,
                                           pageOrder);

            int total = StaticContents.Count();
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

        [HttpGet]
        public ActionResult Edit(int id)
        {
            StaticContent content;

            content = StaticContents.GetByID(id);

            var staticContent = Mapper.Map<EditStaticContent>(content);

            switch (content.StaticContentType)
            {
                case OnlineStore.Models.Enums.StaticContentType.Text:
                    staticContent.SimpleContent = content.Content;
                    break;
                case OnlineStore.Models.Enums.StaticContentType.Editor:
                    staticContent.EditorContent = HttpUtility.HtmlDecode(content.Content);
                    break;
            }

            return View(model: staticContent);
        }

        [HttpPost]
        public ActionResult Edit(EditStaticContent staticContent)
        {
            try
            {
                var content = Mapper.Map<StaticContent>(staticContent);

                switch (content.StaticContentType)
                {
                    case OnlineStore.Models.Enums.StaticContentType.Text:
                        content.Content = staticContent.SimpleContent;
                        break;
                    case OnlineStore.Models.Enums.StaticContentType.Editor:
                        content.Content = staticContent.EditorContent;
                        break;
                }

                content.LastUpdate = DateTime.Now;

                ViewBag.Success = true;

                StaticContents.Update(content);
            }
            catch (Exception ex)
            {
                SetErrors(ex);
            }

            return RedirectToAction("Index");
        }
    }
}