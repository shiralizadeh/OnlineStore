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
    public class ArticlesGroupsController : GroupsController
    {
        public ArticlesGroupsController()
        {
            _groupType = GroupType.Blogs;
            ViewBag.Controller = "ArticlesGroups";
        }
    }
}