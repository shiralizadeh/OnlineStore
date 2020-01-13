using OnlineStore.DataLayer;
using OnlineStore.Models.Enums;
using OnlineStore.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace OnlineStore.Website.Controllers
{
    public class SitemapController : Controller
    {
        public ContentResult Index()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");

            sb.Append("<sitemapindex xmlns=\"http://www.sitemaps.org/schemas/sitemap/0.9\">");

            sb.Append("  <sitemap>");
            sb.Append("    <loc>" + StaticValues.WebsiteUrl + "/sitemap/StaticPages.xml</loc>");
            sb.Append("    <lastmod>" + MenuItems.LastestDate().ToString("yyyy-MM-dd") + "</lastmod>");
            sb.Append("  </sitemap>");

            sb.Append("  <sitemap>");
            sb.Append("    <loc>" + StaticValues.WebsiteUrl + "/sitemap/ProductGroups.xml</loc>");
            sb.Append("    <lastmod>" + Groups.LastestDate(GroupType.Products).ToString("yyyy-MM-dd") + "</lastmod>");
            sb.Append("  </sitemap>");

            foreach (var item in Groups.GetByGroupType(GroupType.Products))
            {
                var exists = DataLayer.Products.ExistsByGroupID(item.ID);

                if (exists)
                {
                    var latestDate = DataLayer.Products.LatestDateByGroupID(item.ID);

                    sb.Append("  <sitemap>");
                    sb.Append("    <loc>" + StaticValues.WebsiteUrl + "/sitemap/products/" + item.UrlPerfix.NormalizeForUrl() + ".xml</loc>");
                    sb.Append("    <lastmod>" + latestDate.ToString("yyyy-MM-dd") + "</lastmod>");
                    sb.Append("  </sitemap>");
                }
            }

            sb.Append("  <sitemap>");
            sb.Append("    <loc>" + StaticValues.WebsiteUrl + "/sitemap/BlogGroups.xml</loc>");
            sb.Append("    <lastmod>" + Groups.LastestDate(GroupType.Blogs).ToString("yyyy-MM-dd") + "</lastmod>");
            sb.Append("  </sitemap>");

            foreach (var item in Groups.GetByGroupType(GroupType.Blogs))
            {
                var exists = DataLayer.Articles.ExistsByGroupID(item.ID);

                if (exists)
                {
                    var latestDate = DataLayer.Articles.LatestDateByGroupID(item.ID);

                    sb.Append("  <sitemap>");
                    sb.Append("    <loc>" + StaticValues.WebsiteUrl + "/sitemap/blog/" + item.UrlPerfix.NormalizeForUrl() + ".xml</loc>");
                    sb.Append("    <lastmod>" + latestDate.ToString("yyyy-MM-dd") + "</lastmod>");
                    sb.Append("  </sitemap>");
                }
            }

            sb.Append("</sitemapindex>");

            return Content(sb.ToString(), "text/xml");
        }

        public ContentResult StaticPages()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");

            sb.Append("<urlset xmlns=\"http://www.sitemaps.org/schemas/sitemap/0.9\">");

            foreach (var item in MenuItems.GetByMenuItemType(MenuItemType.Page))
            {
                sb.Append("  <url>");
                sb.Append("    <loc>" + StaticValues.WebsiteUrl + "/" + item.Link.NormalizeUrl() + "</loc>");
                sb.Append("    <lastmod>" + item.LastUpdate.ToString("yyyy-MM-dd") + "</lastmod>");
                sb.Append("  </url>  ");
            }

            sb.Append("</urlset>");

            return Content(sb.ToString(), "text/xml");
        }

        public ContentResult ProductGroups()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");

            sb.Append("<urlset xmlns=\"http://www.sitemaps.org/schemas/sitemap/0.9\">");

            foreach (var item in Groups.GetByGroupType(GroupType.Products))
            {
                var isExists = DataLayer.Products.ExistsByGroupID(item.ID);

                if (isExists)
                {
                    var lastestDate = DataLayer.Products.LatestDateByGroupID(item.ID);

                    sb.AppendLine("  <url>");
                    sb.AppendLine("    <loc>" + StaticValues.WebsiteUrl + UrlProvider.GetGroupUrl(item.UrlPerfix) + "</loc>");
                    sb.AppendLine("    <lastmod>" + lastestDate.ToString("yyyy-MM-dd") + "</lastmod>");
                    sb.AppendLine("  </url>  ");
                }
            }

            sb.Append("</urlset>");

            return Content(sb.ToString(), "text/xml");
        }

        public ContentResult BlogGroups()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");

            sb.Append("<urlset xmlns=\"http://www.sitemaps.org/schemas/sitemap/0.9\">");

            foreach (var item in Groups.GetByGroupType(GroupType.Blogs))
            {
                var lastestDate = Articles.LatestDateByGroupID(item.ID);

                sb.Append("  <url>");
                sb.Append("    <loc>" + StaticValues.WebsiteUrl + UrlProvider.GetBlogGroupUrl(item.TitleEn) + "</loc>");
                sb.Append("    <lastmod>" + lastestDate.ToString("yyyy-MM-dd") + "</lastmod>");
                sb.Append("  </url>  ");
            }

            sb.Append("</urlset>");

            return Content(sb.ToString(), "text/xml");
        }

        public ContentResult Products(string id)
        {
            var groupTitle = id.DeNormalizeForUrl();

            var group = Groups.GetByTitle(groupTitle, GroupType.Products);

            StringBuilder sb = new StringBuilder();

            sb.Append("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");

            sb.Append("<urlset xmlns=\"http://www.sitemaps.org/schemas/sitemap/0.9\">");

            foreach (var item in DataLayer.Products.GetByGroupID(group.ID))
            {
                item.Title = group.Perfix + " " + item.Title;
                item.Title_En = item.Title_En + " " + group.Perfix_En;

                var lastestDate = item.LastUpdate;

                sb.Append("  <url>");
                sb.Append("    <loc>" + StaticValues.WebsiteUrl + UrlProvider.GetProductUrl(item.ID, group.UrlPerfix, item.UrlPerfix) + "</loc>");
                sb.Append("    <lastmod>" + lastestDate.ToString("yyyy-MM-dd") + "</lastmod>");
                sb.Append("  </url>");
            }

            sb.Append("</urlset>");

            return Content(sb.ToString(), "text/xml");
        }

        public ContentResult Blog(string id)
        {
            var groupTitle = id.DeNormalizeForUrl();

            var group = Groups.GetByTitle(groupTitle, GroupType.Blogs);

            StringBuilder sb = new StringBuilder();

            sb.Append("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");

            sb.Append("<urlset xmlns=\"http://www.sitemaps.org/schemas/sitemap/0.9\">");

            foreach (var item in DataLayer.Articles.GetByGroupID(group.ID))
            {
                var lastestDate = item.LastUpdate;

                sb.Append("  <url>");
                sb.Append("    <loc>" + StaticValues.WebsiteUrl + UrlProvider.GetPostUrl(item.ID, item.Title, group.TitleEn) + "</loc>");
                sb.Append("    <lastmod>" + lastestDate.ToString("yyyy-MM-dd") + "</lastmod>");
                sb.Append("  </url>");
            }

            sb.Append("</urlset>");

            return Content(sb.ToString(), "text/xml");
        }
    }
}