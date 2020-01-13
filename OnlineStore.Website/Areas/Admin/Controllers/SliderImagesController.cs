using OnlineStore.DataLayer;
using OnlineStore.Models;
using OnlineStore.Providers;
using System;
using System.Web.Mvc;
using OnlineStore.Providers.Controllers;
using OnlineStore.Models.Enums;
using System.Collections.Generic;

namespace OnlineStore.Website.Areas.Admin.Controllers
{
    public class SliderImagesController : AdminController
    {
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public JsonResult Get(int pageIndex, int pageSize, string pageOrder, string title, sbyte sliderType)
        {
            SliderType? sType = null;

            if (sliderType != -1)
                sType = (SliderType)sliderType;

            var list = SliderImages.Get(pageIndex, pageSize, pageOrder, title, sType);

            int total = SliderImages.Count(title, sType);
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
                SliderImages.Delete(id);
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
            SliderImage sliderImage;

            if (id.HasValue)
                sliderImage = SliderImages.GetByID(id.Value);
            else
                sliderImage = new SliderImage();

            return View(sliderImage);
        }

        [HttpPost]
        public ActionResult Edit(SliderImage sliderImage)
        {
            try
            {
                List<Utilities.FileUploadSettings> files;
                string fileName = Utilities.GetNormalFileName(sliderImage.Title + "_" + sliderImage.SubTitle);

                if (sliderImage.SliderType == SliderType.Home)
                {
                    files = Utilities.SaveFiles(Request.Files, fileName, StaticPaths.SliderImages);
                }
                else
                {
                    files = Utilities.SaveFiles(Request.Files, fileName, StaticPaths.OfferImages);
                }

                if (files.Count > 0)
                    sliderImage.Filename = files[0].Title;

                sliderImage.LastUpdate = DateTime.Now;

                ViewBag.Success = true;

                var id = sliderImage.ID;

                if (sliderImage.ID == -1)
                {
                    SliderImages.Insert(sliderImage);
                    UserNotifications.Send(UserID, String.Format("جدید - عکس اسلایدر '{0}'", sliderImage.Title), "/Admin/SliderImages/Edit/" + sliderImage.ID, NotificationType.Success);
                }
                else
                {
                    SliderImages.Update(sliderImage);
                }

                #region Set Task

                var taskText = String.Format("زمان اسلایدر \"{0}\" در حال اتمام است.", sliderImage.Title);
                var taskDate = sliderImage.EndDate.AddDays(-1);

                UserTasks.SetTask("اتمام زمان اسلایدر",
                                  taskText,
                                  StaticValues.AdminID,
                                  "SliderImages_" + sliderImage.ID,
                                  "/Admin/SliderImages/Edit/" + sliderImage.ID,
                                  taskDate);

                #endregion Set Task

                if (id == -1)
                {
                    sliderImage = new SliderImage();
                }

            }
            catch (Exception ex)
            {
                SetErrors(ex);
            }

            return ClearView(sliderImage);
        }
    }
}