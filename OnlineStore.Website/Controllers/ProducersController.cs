using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using OnlineStore.DataLayer;
using OnlineStore.Models.Public;
using AutoMapper;

namespace OnlineStore.Website.Controllers
{
    public class ProducersController : Controller
    {
        [Route("Producers/{enTitle}-{faTitle}-{id:int}")]
        public ActionResult Index(int id)
        {
            var producer = Producers.GetByID(id);
            var groups = Groups.GetRelatedGroupsByProducer(id);

            var producerDetail = Mapper.Map<ViewProducer>(producer);

            var model = new ProducerSettings
            {
                ProducerDetails = producerDetail,
                ProductGroups = groups
            };

            ViewBag.Title = producer.TitleEn + " (" + producer.Title + ") ";

            return View(model);
        }
    }
}