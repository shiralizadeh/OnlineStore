using OnlineStore.EntityFramework;
using OnlineStore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OnlineStore.DataLayer;

namespace OnlineStore.Website.Controllers
{
    public class TrackOrderController : Controller
    {
        public ActionResult Index()
        {
            var content = StaticContents.GetByName("TrackOrder").Content;
            return View(model: content);
        }

        [HttpPost]
        public JsonResult ShowStatus(string saleReferenceID)
        {
            var jsonSuccessResult = new JsonSuccessResult();

            try
            {
                var orderInfo = Carts.GetTrackOrderInfo(saleReferenceID);

                jsonSuccessResult.Data = orderInfo;
                jsonSuccessResult.Success = true;
            }
            catch (DbException ex)
            {
                jsonSuccessResult.Errors = ex.Errors.ToArray();
                jsonSuccessResult.Success = false;
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