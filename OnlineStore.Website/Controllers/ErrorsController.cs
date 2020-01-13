using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OnlineStore.Website.Controllers
{
    public class ErrorsController : Controller
    {
        [Route("404")]
        public ActionResult Error_404()
        {
            Response.StatusCode = 404;

            return View();
        }

        [Route("500")]
        public ActionResult Error_500()
        {
            var ex = HttpContext.Items["Exception"] as Exception;

            if (ex != null)
            {
                Response.Write(ex.Message);
            }

            Response.StatusCode = 500;

            return View();
        }
    }
}