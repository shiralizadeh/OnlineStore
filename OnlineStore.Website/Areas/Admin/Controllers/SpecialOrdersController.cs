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

namespace OnlineStore.Website.Areas.Admin.Controllers
{
    public class SpecialOrdersController : AdminController
    {
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public JsonResult Get(int pageIndex, int pageSize, string pageOrder)
        {
            var list = SpecialOrders.Get(pageIndex, pageSize, pageOrder);

            int total = SpecialOrders.Count();
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
                SpecialOrders.Delete(id);
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

        public ActionResult Edit(int? id)
        {
            SpecialOrder specialOrder;

            if (id.HasValue)
                specialOrder = SpecialOrders.GetByID(id.Value);
            else
                specialOrder = new SpecialOrder();

            return View(specialOrder);
        }

        [HttpPost]
        public ActionResult Edit(SpecialOrder specialOrder)
        {
            try
            {
                specialOrder.LastUpdate = DateTime.Now;

                ViewBag.Success = true;

                if (specialOrder.ID == -1)
                {
                    SpecialOrders.Insert(specialOrder);

                    specialOrder = new SpecialOrder();
                }
                else
                {
                    SpecialOrders.Update(specialOrder);
                }
            }
            catch (Exception ex)
            {
                SetErrors(ex);
            }

            return ClearView(specialOrder);
        }
    }
}