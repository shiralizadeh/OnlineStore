using OnlineStore.DataLayer;
using OnlineStore.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OnlineStore.Website.Controllers
{
    public class AboutUsController : Controller
    {
        [Route("درباره-ما")]
        public ActionResult Index()
        {
            var content = StaticContents.GetByName("AboutUs");

            content.Content = HttpUtility.HtmlDecode(content.Content);

            return View(model: content);
        }
    }
}