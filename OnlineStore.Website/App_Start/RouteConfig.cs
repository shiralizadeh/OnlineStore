using OnlineStore.DataLayer;
using OnlineStore.Models.Enums;
using OnlineStore.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace OnlineStore.Website
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.MapMvcAttributeRoutes();

            var menuItems = MenuItems.GetByMenuItemType(MenuItemType.Page);

            foreach (var item in menuItems)
            {
                routes.MapRoute(
                    name: "Page" + item.ID,
                    url: item.Link.NormalizeUrl(),
                    defaults: new { controller = "Pages", action = "Index", id = item.ID }
                );
            }

            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional },
                namespaces: new string[] { "OnlineStore.Website.Controllers" }
            );

            
        }
    }
}
