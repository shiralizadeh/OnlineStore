using OnlineStore.DataLayer;
using OnlineStore.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OnlineStore.Website.Controllers
{
    public class RulesController : Controller
    {
        [Route("قوانین-سایت")]
        public ActionResult Index()
        {
            var content = StaticContents.GetByName("Rules");

            content.Content = HttpUtility.HtmlDecode(content.Content);

            return View(model: content);
        }
    }
}