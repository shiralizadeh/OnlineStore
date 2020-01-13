using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OnlineStore.DataLayer;
using OnlineStore.Models;
using OnlineStore.EntityFramework;

namespace OnlineStore.Website.Controllers
{
    public class CitiesController : Controller
    {
        public JsonResult Get(int id)
        {
            var jsonSuccessResult = new JsonSuccessResult();

            try
            {
                jsonSuccessResult.Data = Cities.GetChilds(id);

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