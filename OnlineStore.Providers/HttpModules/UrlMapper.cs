using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace OnlineStore.Providers.HttpModules
{
    public class UrlMapper : IHttpModule
    {
        public void Dispose()
        {
        }

        public void Init(HttpApplication app)
        {
            app.AuthorizeRequest += App_AuthorizeRequest;
        }

        private void App_AuthorizeRequest(object sender, EventArgs e)
        {
            HttpApplication app = (HttpApplication)sender;

            var url = app.Request.RawUrl;

            Regex regexSitemap = new Regex(@"^\/sitemap\/(.+)/(.+)\.xml$");

            if (url.ToLower() == "/sitemap.xml")
            {
                app.Context.RewritePath("/Sitemap/Index");
            }
            else if (url.ToLower() == "/sitemap/staticpages.xml")
            {
                app.Context.RewritePath("/Sitemap/StaticPages");
            }
            else if (url.ToLower() == "/sitemap/productgroups.xml")
            {
                app.Context.RewritePath("/Sitemap/ProductGroups");
            }
            else if (url.ToLower() == "/sitemap/bloggroups.xml")
            {
                app.Context.RewritePath("/Sitemap/BlogGroups");
            }
            else if (regexSitemap.IsMatch(url))
            {
                var matchSitemap = regexSitemap.Match(url);

                app.Context.RewritePath(String.Format("/Sitemap/{0}/{1}", matchSitemap.Groups[1], matchSitemap.Groups[2]));
            }
        }
    }
}