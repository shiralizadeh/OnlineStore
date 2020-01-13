using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OnlineStore.DataLayer;
using OnlineStore.Models.Public;
using AutoMapper;
using OnlineStore.Providers;

namespace OnlineStore.Website.Controllers
{
    public class PagesController : Controller
    {
        public ActionResult Index(int id)
        {
            var page = MenuItems.GetByID(id);
            var parents = MenuItems.GetParentsRecursive(page);

            var mappedPage = Mapper.Map<SitePage>(page);
            var mappedParents = Mapper.Map<List<BreadCrumbLink>>(parents);

            SitePage model = mappedPage;

            model.BreadCrumb = mappedParents;

            return View(model);
        }
    }
}