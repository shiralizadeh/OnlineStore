using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OnlineStore.DataLayer;

namespace OnlineStore.Website.Controllers
{
    public class BannerController : Controller
    {
        [Route("Banner/{key}")]
        public RedirectResult Index(string key)
        {
            string link = String.Empty;
            var guid = Guid.Parse(key);

            bool found = Banners.AddClick(guid);

            if (found)
            {
                link = Banners.GetByGuid(guid).Link;
            }
            else
            {
                MenuItemBanners.AddClick(guid);
                link = MenuItemBanners.GetByGuid(guid).Link;
            }

            return Redirect(link);
        }
    }
}