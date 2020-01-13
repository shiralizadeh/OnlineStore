using OnlineStore.DataLayer;
using OnlineStore.Models;
using OnlineStore.Providers;
using System;
using System.Web.Mvc;
using OnlineStore.Providers.Controllers;
using OnlineStore.Models.Enums;
using AutoMapper;
using OnlineStore.Models.Admin;
using System.Linq;
using System.Web;

namespace OnlineStore.Website.Areas.Admin.Controllers
{
    public class ProducersController : AdminController
    {
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public JsonResult Get(int pageIndex, int pageSize, string pageOrder, string title)
        {
            if (pageOrder.Trim() == "ID")
                pageOrder = "OrderID";

            var list = Producers.Get(pageIndex, pageSize, pageOrder, title);

            int total = Producers.Count(title);
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
                Producers.Delete(id);
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
            EditProducer editProducer;

            if (id.HasValue)
            {
                editProducer = Mapper.Map<EditProducer>(Producers.GetByID(id.Value));
                editProducer.Groups = ProducerGroups.GetByProducerID(editProducer.ID).Select(item => item.GroupID).ToList();
                editProducer.Text = HttpUtility.HtmlDecode(editProducer.Text);
            }
            else
                editProducer = new EditProducer();

            return View(editProducer);
        }

        [HttpPost]
        public ActionResult Edit(EditProducer editProducer)
        {
            try
            {
                var producer = Mapper.Map<Producer>(editProducer);

                var files = Utilities.SaveFiles(Request.Files, Utilities.GetNormalFileName(producer.Title), StaticPaths.ProducerImages);

                if (files.Count > 0)
                    producer.Filename = files[0].Title;

                producer.LastUpdate = DateTime.Now;

                ViewBag.Success = true;

                if (producer.ID == -1)
                {
                    Producers.Insert(producer);

                    SaveGroups(editProducer, producer.ID);

                    UserNotifications.Send(UserID, String.Format("جدید - تولید کننده '{0}'", producer.Title), "/Admin/Producers/Edit/" + producer.ID, NotificationType.Success);
                    editProducer = new EditProducer();
                }
                else
                {
                    Producers.Update(producer);

                    SaveGroups(editProducer, producer.ID);

                    editProducer.Groups = ProducerGroups.GetByProducerID(editProducer.ID).Select(item => item.GroupID).ToList();
                    editProducer.Text = HttpUtility.HtmlDecode(editProducer.Text);

                }
            }
            catch (Exception ex)
            {
                SetErrors(ex);
            }

            return ClearView(editProducer);
        }

        private void SaveGroups(EditProducer editProducer, int producerID)
        {
            var curList = ProducerGroups.GetByProducerID(producerID);

            foreach (var groupID in editProducer.Groups)
            {
                if (!curList.Any(item => item.GroupID == groupID))
                {
                    var producerGroup = new ProducerGroup();

                    producerGroup.ProducerID = producerID;
                    producerGroup.GroupID = groupID;

                    ProducerGroups.Insert(producerGroup);
                }
                else
                {
                    var item = curList.SingleOrDefault(cls => cls.GroupID == groupID);

                    if (item != null)
                        curList.Remove(item);
                }
            }

            foreach (var item in curList)
                ProducerGroups.Delete(item.ID);
        }
    }
}