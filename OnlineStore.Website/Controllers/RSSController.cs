using OnlineStore.DataLayer;
using OnlineStore.Models.Enums;
using OnlineStore.Models.Public;
using OnlineStore.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace OnlineStore.Website.Controllers
{
    class CacheItem
    {
        public DateTime? LastUpdate { get; set; }
        public StringBuilder Data { get; set; }
    }

    public class RSSController : Controller
    {
        private static Dictionary<string, CacheItem> cache = new Dictionary<string, CacheItem>();

        public ActionResult Prices()
        {
            var cache = GetCache("Prices");

            var latestDate = Products.LatestDate();

            if (!cache.LastUpdate.HasValue || cache.LastUpdate.Value < latestDate)
            {
                cache.LastUpdate = latestDate;
                cache.Data = refreshPrices();
            }

            return Content(cache.Data.ToString(), "text/xml");
        }

        public ActionResult LatestProducts()
        {
            var cache = GetCache("Products");

            var latestDate = Products.LatestCreatedDate();

            if (!cache.LastUpdate.HasValue || cache.LastUpdate.Value < latestDate)
            {
                cache.LastUpdate = latestDate;
                cache.Data = refreshProducts();
            }

            return Content(cache.Data.ToString(), "text/xml");
        }

        [Route("RSS/Group/{GroupTitle}")]
        public ActionResult LatestProducts(string groupTitle)
        {
            if (String.IsNullOrWhiteSpace(groupTitle))
                return new HttpNotFoundResult();

            groupTitle = groupTitle.DeNormalizeForUrl();

            var group = Groups.GetByTitle(groupTitle, GroupType.Products);

            if (group == null)
                return new HttpNotFoundResult();

            var groupChilds = new List<Group>();

            if (Groups.HasChild(group.ID))
                groupChilds.AddRange(Groups.GetChildsRecursive(group.ID));
            else
                groupChilds.Add(group);

            List<int> groupChildIDs = groupChilds.Select(item => item.ID).ToList();

            var cache = GetCache("Group_" + group.ID);

            var latestDate = Products.LatestCreatedDate(groupChildIDs);

            if (!cache.LastUpdate.HasValue || cache.LastUpdate.Value < latestDate)
            {
                cache.LastUpdate = latestDate;
                cache.Data = refreshGroups(groupTitle, groupChildIDs, group.Title);
            }

            return Content(cache.Data.ToString(), "text/xml");
        }

        public ActionResult LatestPosts()
        {
            var cache = GetCache("Posts");

            var latestDate = Articles.LatestLastUpdate();

            if (!cache.LastUpdate.HasValue || cache.LastUpdate.Value < latestDate)
            {
                cache.LastUpdate = latestDate;
                cache.Data = refreshPosts();
            }

            return Content(cache.Data.ToString(), "text/xml");
        }

        [Route("RSS/Blog/{GroupID:int}")]
        public ActionResult LatestPosts(int groupID)
        {
            var cache = GetCache("BlogGroup_" + groupID);

            var latestDate = Articles.LatestLastUpdate(groupID);

            if (!cache.LastUpdate.HasValue || cache.LastUpdate.Value < latestDate)
            {
                cache.LastUpdate = latestDate;
                cache.Data = refreshPosts(groupID);
            }

            return Content(cache.Data.ToString(), "text/xml");
        }

        public ActionResult LatestComments(int id)
        {
            var cache = GetCache("Comments_" + id);

            var latestDate = ScoreComments.LatestLastUpdate(id);

            if (!cache.LastUpdate.HasValue || cache.LastUpdate.Value < latestDate)
            {
                cache.LastUpdate = latestDate;
                cache.Data = refreshComments(id);
            }

            return Content(cache.Data.ToString(), "text/xml");
        }

        public ActionResult Index()
        {
            var cache = GetCache("ProductsBlog");

            var proLatestDate = Products.LatestCreatedDate();
            var posLatestDate = Articles.LatestLastUpdate();

            DateTime date;
            if (proLatestDate > posLatestDate)
            {
                date = proLatestDate;
            }
            else
            {
                date = posLatestDate;
            }

            if (!cache.LastUpdate.HasValue || cache.LastUpdate.Value < date)
            {
                cache.LastUpdate = date;
                cache.Data = refreshAll();
            }

            return Content(cache.Data.ToString(), "text/xml");
        }

        #region Methods

        private static CacheItem GetCache(string key)
        {
            CacheItem cacheItem = null;

            if (cache.Any(item => item.Key == key))
                cacheItem = cache.First(item => item.Key == key).Value;
            else
            {
                cacheItem = new CacheItem();
                cache.Add(key, cacheItem);
            }

            return cacheItem;
        }

        private static StringBuilder refreshPrices()
        {
            var prices = new StringBuilder();

            var products = Products.GetAll();

            prices.Append("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");

            prices.Append("<rss version=\"2.0\">");
            prices.Append("<channel xmlns:atom=\"http://www.w3.org/2005/Atom\">");
            prices.Append("<title>" + StaticValues.WebsiteTitle + "</title>");
            prices.Append("<link>" + StaticValues.WebsiteUrl + "</link>");
            prices.Append("<description>" + StaticValues.WebsiteDescription + "</description>");
            prices.Append("<language>fa-IR</language>");
            prices.Append("<copyright>Copyright " + DateTime.Now.Year + " " + StaticValues.WebsiteTitle + ". All rights reserved.</copyright>");
            prices.Append("<lastBuildDate>" + RSSDateFormat(products.Last().LastUpdate) + "</lastBuildDate>");

            foreach (var item in products)
            {
                var group = Groups.GetByID(item.GroupID.Value);

                var title_Fa = item.Title_Fa.Clone();
                var title_En = item.Title_En.Clone();

                item.Title_Fa = group.Perfix + " " + item.Title_Fa;
                item.Title_En = item.Title_En + " " + group.Perfix_En;

                string url = (StaticValues.WebsiteUrl + UrlProvider.GetProductUrl(item.ID, group.UrlPerfix, item.UrlPerfix)).HtmlEncode();

                prices.Append("<item>");
                prices.Append("<guid isPermaLink=\"true\">" + url + "</guid>");
                prices.Append("<link>" + url + "</link>");
                prices.Append("<category>" + group.Title.HtmlEncode() + "</category>");
                prices.Append("<title>" + (title_En + " - " + title_Fa).HtmlEncode() + "</title>");

                prices.Append("<description>");

                var price = 0;
                if (item.Prices != null && item.Prices.Count > 0)
                {
                    var minPrice = item.Prices.OrderBy(a => a.Price).First();
                    price = minPrice.DiscountPercent > 0 ? minPrice.DiscountPrice : minPrice.Price;
                }

                if (price == 0 || item.IsUnavailable)
                    prices.Append("موجود نیست");
                else
                    prices.Append(price.ToPrice());

                prices.Append("</description>");

                prices.Append("<pubDate>" + RSSDateFormat(item.LastUpdate) + "</pubDate>");
                prices.Append("</item>");
            }

            prices.Append("</channel>");
            prices.Append("</rss>");

            return prices;
        }

        private static StringBuilder refreshProducts()
        {
            var latestProducts = new StringBuilder();

            var products = Products.GetLatestProducts();

            latestProducts.Append("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");

            latestProducts.Append("<rss version=\"2.0\">");
            latestProducts.Append("<channel xmlns:atom=\"http://www.w3.org/2005/Atom\">");
            latestProducts.Append("<title>آخرین محصولات آنلاین استور</title>");
            latestProducts.Append("<link>" + StaticValues.WebsiteUrl + "</link>");
            latestProducts.Append("<description>" + StaticValues.WebsiteDescription + "</description>");
            latestProducts.Append("<language>fa-IR</language>");
            latestProducts.Append("<copyright>Copyright " + DateTime.Now.Year + " " + StaticValues.WebsiteTitle + ". All rights reserved.</copyright>");
            latestProducts.Append("<lastBuildDate>" + RSSDateFormat(products.First().CreatedDate) + "</lastBuildDate>");

            foreach (var item in products)
            {
                var group = Groups.GetByID(item.GroupID.Value);
                string url = (StaticValues.WebsiteUrl + UrlProvider.GetProductUrl(item.ID, group.UrlPerfix, item.UrlPerfix)).HtmlEncode();

                latestProducts.Append("<item>");
                latestProducts.Append("<guid isPermaLink=\"true\">" + url + "</guid>");
                latestProducts.Append("<link>" + url + "</link>");
                latestProducts.Append("<category>" + group.Title.HtmlEncode() + "</category>");
                latestProducts.Append("<title>" + (item.Title_En + " - " + item.Title_Fa).HtmlEncode() + "</title>");

                latestProducts.Append("<description>");
                latestProducts.Append(("<img src='" + UrlProvider.GetProductImage(item.ImageFile, StaticValues.DefaultProductImageSize) + "' />").HtmlEncode());
                latestProducts.Append(("<br/>").HtmlEncode());
                latestProducts.Append(item.Summary.HtmlEncode());
                latestProducts.Append("</description>");

                latestProducts.Append("<pubDate>" + RSSDateFormat(item.CreatedDate) + "</pubDate>");
                latestProducts.Append("</item>");
            }

            latestProducts.Append("</channel>");
            latestProducts.Append("</rss>");

            return latestProducts;
        }

        private static StringBuilder refreshPosts(int? groupID = null)
        {
            string groupTitle = String.Empty;
            var latestPosts = new StringBuilder();

            var posts = Articles.GetLatestPosts(groupID);
            if (groupID.HasValue)
            {
                groupTitle = Groups.GetByID(groupID.Value).Title;
            }

            latestPosts.Append("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");

            latestPosts.Append("<rss version=\"2.0\">");
            latestPosts.Append("<channel xmlns:atom=\"http://www.w3.org/2005/Atom\">");
            if (!groupID.HasValue)
            {
                latestPosts.Append("<title>آخرین اخبار آنلاین استور</title>");
            }
            else
            {
                latestPosts.Append("<title>آخرین اخبار " + groupTitle + " آنلاین استور</title>");
            }
            latestPosts.Append("<link>" + StaticValues.WebsiteUrl + "</link>");
            latestPosts.Append("<description>" + StaticValues.WebsiteDescription + "</description>");
            latestPosts.Append("<language>fa-IR</language>");
            latestPosts.Append("<copyright>Copyright " + DateTime.Now.Year + " " + StaticValues.WebsiteTitle + ". All rights reserved.</copyright>");
            latestPosts.Append("<lastBuildDate>" + RSSDateFormat(posts.First().LastUpdate) + "</lastBuildDate>");

            foreach (var item in posts)
            {
                var group = Groups.GetByID(item.GroupID);
                var url = (StaticValues.WebsiteUrl + UrlProvider.GetPostUrl(item.ID, item.Title, group.TitleEn)).HtmlEncode();

                latestPosts.Append("<item>");
                latestPosts.Append("<guid isPermaLink=\"true\">" + url + "</guid>");
                latestPosts.Append("<link>" + url + "</link>");
                latestPosts.Append("<category>" + item.GroupTitle.HtmlEncode() + "</category>");
                latestPosts.Append("<title>" + item.Title.HtmlEncode() + "</title>");

                latestPosts.Append("<description>");
                latestPosts.Append(("<img src='" + UrlProvider.GetPostImage(item.Image, StaticValues.DefaultPostImageSize) + "'/>").HtmlEncode());
                latestPosts.Append(("<br/>").HtmlEncode());
                latestPosts.Append(item.Summary.HtmlEncode());
                latestPosts.Append("</description>");

                latestPosts.Append("<pubDate>" + RSSDateFormat(item.LastUpdate) + "</pubDate>");
                latestPosts.Append("</item>");
            }

            latestPosts.Append("</channel>");
            latestPosts.Append("</rss>");

            return latestPosts;
        }

        private static StringBuilder refreshGroups(string groupTitle, List<int> groupChildIDs, string faGroupTitle)
        {
            var latestGroupProducts = new StringBuilder();

            var products = Products.GetLatestProducts(groupChildIDs);

            latestGroupProducts.Append("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");

            latestGroupProducts.Append("<rss version=\"2.0\">");
            latestGroupProducts.Append("<channel xmlns:atom=\"http://www.w3.org/2005/Atom\">");
            latestGroupProducts.Append("<title>آخرین محصولات " + faGroupTitle + " آنلاین استور</title>");
            latestGroupProducts.Append("<link>" + StaticValues.WebsiteUrl + "</link>");
            latestGroupProducts.Append("<description>" + StaticValues.WebsiteDescription + "</description>");
            latestGroupProducts.Append("<language>fa-IR</language>");
            latestGroupProducts.Append("<copyright>Copyright " + DateTime.Now.Year + " " + StaticValues.WebsiteTitle + ". All rights reserved.</copyright>");
            latestGroupProducts.Append("<lastBuildDate>" + RSSDateFormat(products.First().CreatedDate) + "</lastBuildDate>");

            foreach (var item in products)
            {
                string url = (StaticValues.WebsiteUrl + UrlProvider.GetProductUrl(item.ID, item.GroupTitle_En, item.UrlPerfix).HtmlEncode());

                latestGroupProducts.Append("<item>");
                latestGroupProducts.Append("<guid isPermaLink=\"true\">" + url + "</guid>");
                latestGroupProducts.Append("<link>" + url + "</link>");
                latestGroupProducts.Append("<category>" + groupTitle.HtmlEncode() + "</category>");
                latestGroupProducts.Append("<title>" + (item.Title_En + " - " + item.Title_Fa).HtmlEncode() + "</title>");

                latestGroupProducts.Append("<description>");
                latestGroupProducts.Append(("<img src='" + UrlProvider.GetProductImage(item.ImageFile, StaticValues.DefaultProductImageSize) + "' />").HtmlEncode());
                latestGroupProducts.Append(("<br/>").HtmlEncode());
                latestGroupProducts.Append(item.Summary.HtmlEncode());
                latestGroupProducts.Append("</description>");

                latestGroupProducts.Append("<pubDate>" + RSSDateFormat(item.CreatedDate) + "</pubDate>");
                latestGroupProducts.Append("</item>");
            }

            latestGroupProducts.Append("</channel>");
            latestGroupProducts.Append("</rss>");

            return latestGroupProducts;
        }

        private static StringBuilder refreshComments(int productID)
        {
            var latestComments = new StringBuilder();

            var productTitle = Products.GetTitleByID(productID);
            var comments = ScoreComments.GetLatestComments(productID);

            latestComments.Append("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");

            latestComments.Append("<rss version=\"2.0\">");
            latestComments.Append("<channel xmlns:atom=\"http://www.w3.org/2005/Atom\">");
            latestComments.Append("<title>آخرین نظرات محصول " + productTitle + "</title>");
            latestComments.Append("<link>" + StaticValues.WebsiteUrl + "</link>");
            latestComments.Append("<description>" + StaticValues.WebsiteDescription + "</description>");
            latestComments.Append("<language>fa-IR</language>");
            latestComments.Append("<copyright>Copyright " + DateTime.Now.Year + " " + StaticValues.WebsiteTitle + ". All rights reserved.</copyright>");
            latestComments.Append("<lastBuildDate>");
            latestComments.Append(comments.Count > 0 ? RSSDateFormat(comments.First().LastUpdate) : "");
            latestComments.Append("</lastBuildDate>");

            foreach (var item in comments)
            {
                var product = Products.GetByID(item.ProductID);
                var group = Groups.GetByID(product.GroupID.Value);
                string url = (StaticValues.WebsiteUrl + UrlProvider.GetProductUrl(item.ProductID, group.UrlPerfix, product.UrlPerfix)).HtmlEncode();

                latestComments.Append("<item>");
                latestComments.Append("<guid isPermaLink=\"true\">" + url + "</guid>");
                latestComments.Append("<link>" + url + "</link>");
                latestComments.Append("<category>" + group.Title.HtmlEncode() + "</category>");
                latestComments.AppendFormat("<title> نظر شماره {0} در {1}</title>", item.ID, (product.Title_En + " - " + product.Title).HtmlEncode());

                latestComments.Append("<description>");
                latestComments.Append(item.Text.HtmlEncode());
                latestComments.Append("</description>");

                latestComments.Append("<pubDate>" + RSSDateFormat(item.LastUpdate) + "</pubDate>");
                latestComments.Append("</item>");
            }

            latestComments.Append("</channel>");
            latestComments.Append("</rss>");

            return latestComments;
        }

        private static StringBuilder refreshAll()
        {
            var latest = new StringBuilder();

            var products = Products.GetLatest();
            var posts = Articles.GetLatest();
            var result = products.Union(posts);

            result = result.OrderByDescending(item => item.Date);

            latest.Append("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
            latest.Append("<rss version=\"2.0\">");
            latest.Append("<channel xmlns:atom=\"http://www.w3.org/2005/Atom\">");
            latest.Append("<title>آخرین های آنلاین استور</title>");
            latest.Append("<link>" + StaticValues.WebsiteUrl + "</link>");
            latest.Append("<description>" + StaticValues.WebsiteDescription + "</description>");
            latest.Append("<language>fa-IR</language>");
            latest.Append("<copyright>Copyright " + DateTime.Now.Year + " " + StaticValues.WebsiteTitle + ". All rights reserved.</copyright>");
            latest.Append("<lastBuildDate>" + RSSDateFormat(result.First().Date) + "</lastBuildDate>");

            foreach (var item in result)
            {
                string url = String.Empty;

                if (item.Type == RSSRowType.Product)
                {
                    var group = Groups.GetByID(item.GroupID.Value);

                    url = (StaticValues.WebsiteUrl + UrlProvider.GetProductUrl(item.ID, group.UrlPerfix, item.UrlPerfix)).HtmlEncode();
                }
                else
                {
                    var group = Groups.GetByID(item.GroupID.Value);

                    url = (StaticValues.WebsiteUrl + UrlProvider.GetPostUrl(item.ID, item.Title_Fa, group.TitleEn)).HtmlEncode();
                }

                latest.Append("<item>");
                latest.Append("<guid isPermaLink=\"true\">" + url + "</guid>");
                latest.Append("<link>" + url + "</link>");
                latest.Append("<category>" + item.GroupTitle.HtmlEncode() + "</category>");
                latest.Append("<title>" + item.UrlPerfix.HtmlEncode() + "</title>");

                latest.Append("<description>");
                latest.Append(("<img src='" + (item.Type == RSSRowType.Product
                                                                                 ? UrlProvider.GetProductImage(item.Image, StaticValues.DefaultProductImageSize)
                                                                                 : UrlProvider.GetPostImage(item.Image, StaticValues.DefaultProductImageSize)) + "' />").HtmlEncode());
                latest.Append(("<br/>").HtmlEncode());
                latest.Append(item.Summary.HtmlEncode());
                latest.Append("</description>");

                latest.Append("<pubDate>" + RSSDateFormat(item.Date) + "</pubDate>");
                latest.Append("</item>");
            }

            latest.Append("</channel>");
            latest.Append("</rss>");

            return latest;
        }

        static string RSSDateFormat(DateTime pubDate)
        {
            var value = pubDate.ToString("ddd',' d MMM yyyy HH':'mm':'ss") + " " + pubDate.ToString("zzzz").Replace(":", "");
            return value;
        }

        #endregion Methods
    }
}