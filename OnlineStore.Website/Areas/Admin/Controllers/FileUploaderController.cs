using OnlineStore.DataLayer;
using OnlineStore.Models;
using OnlineStore.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OnlineStore.Providers.Controllers;
using OnlineStore.Models.Enums;
using System.IO;

namespace OnlineStore.Website.Areas.Admin.Controllers
{
    public class FileUploaderController : AdminController
    {
        [HttpPost]
        public JsonResult Upload(string title, string path, string file)
        {
            var jsonSuccessResult = new JsonSuccessResult();

            try
            {
                System.Threading.Thread.Sleep(2000);

                var rnd = new Random(DateTime.Now.Millisecond);
                var fileext = Path.GetExtension(file);
                var filename = Utilities.GetNormalFileName(title).NormalizeForUrl() + "_" + rnd.Next(1000, 9999) + fileext;

                path = Server.MapPath(path);

                Request.SaveAs(path + filename, false);

                jsonSuccessResult.Data = filename;
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