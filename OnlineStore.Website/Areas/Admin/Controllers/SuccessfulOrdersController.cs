using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OnlineStore.Models.User;
using OnlineStore.Models.Admin;
using OnlineStore.Providers.Controllers;
using OnlineStore.Providers;
using OnlineStore.DataLayer;
using System.Threading.Tasks;
using AutoMapper;
using OnlineStore.Models.Public;
using OnlineStore.Models.Enums;
using OnlineStore.Models;
using OnlineStore.Services;
using OnlineStore.Identity;
using System.Web.UI.WebControls;
using System.IO;
using System.Web.UI;

namespace OnlineStore.Website.Areas.Admin.Controllers
{
    public class SuccessfulOrdersController : OrdersController
    {
        public SuccessfulOrdersController()
        {
            _orderStatus = OrderStatus.Suseeccful;
            ViewBag.Controller = "SuccessfulOrders";
        }
    }
}