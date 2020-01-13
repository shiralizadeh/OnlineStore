using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineStore.Providers
{
    public static class UrlProvider
    {
        public static string Get404Url()
        {
            return StaticValues.NotFoundUrl;
        }

        public static string GetProductUrl(int productID, string group, string perfix)
        {
            var url = "/Products/" + group.NormalizeForUrl() + "/" + perfix.NormalizeForUrl() + "-" + productID;

            return url;
        }

        public static string GetQuickViewUrl(int productID, string group, string perfix)
        {
            var url = "/Products/" + group.NormalizeForUrl() + "/QuickView/" + perfix.NormalizeForUrl() + "-" + productID;

            return url;
        }

        public static string GetGroupUrl(string urlPerfix)
        {
            var url = "/Products/" + urlPerfix.NormalizeForUrl();

            return url;
        }

        public static string GetBannerUrl(Guid guid)
        {
            var url = "/Banner/" + guid.ToString();

            return url;
        }

        public static string GetGroupUrl(string urlPerfix, string producer)
        {
            var url = String.Format("/Products/{0}/{1}/1", urlPerfix.NormalizeForUrl(), producer);

            return url;
        }

        public static string GetCompareUrl(int productID)
        {
            var url = "/CompareProducts/" + productID;

            return url;
        }

        public static string GetFactorUrl(int cartID)
        {
            var url = "/My-Account/My-Orders/Factor/" + cartID;

            return url;
        }

        public static string GetBlogGroupUrl(string groupTitle)
        {
            string url = String.Empty;

            groupTitle = groupTitle.NormalizeForUrl();

            url = String.Format("/Blog/{0}", groupTitle);

            return url;
        }

        public static string GetPostUrl(int id, string title, string groupTitle)
        {
            string url = String.Empty;

            title = title.NormalizeForUrl();
            groupTitle = groupTitle.NormalizeForUrl();

            url = String.Format("/Blog/{0}/{1}-{2}", groupTitle, title, id);

            return url;
        }

        public static string GetMenuItemUrl(string title, string link, byte menuItemType)
        {
            string url = String.Empty;

            switch (menuItemType)
            {
                case 0:
                    url = "#";
                    break;
                case 1:
                    url = link;
                    break;
                case 2:
                    if (link != "#" && link != String.Empty)
                    {
                        url = link;
                    }
                    else
                    {
                        title = title.NormalizeForUrl();
                        url = "/Page/" + title;
                    }
                    break;
                default:
                    break;
            }

            return url;
        }

        public static string GetProducerUrl(string enTitle, string title, int id)
        {
            string url = String.Empty;
            title = title.NormalizeForUrl();
            enTitle = enTitle.NormalizeForUrl();

            url = String.Format("/Producers/{0}-{1}-{2}", enTitle, title, id);

            return url;
        }

        public static string GetGroupProducerUrl(
            string urlPerfix,
            string producerTitle_En)
        {
            var url = "/Products/" + urlPerfix.NormalizeForUrl() + "/" + producerTitle_En.NormalizeForUrl() + "/1";

            return url;
        }

        public static string GetPackageUrl(string title, int id)
        {
            string url = String.Empty;
            title = title.NormalizeForUrl();

            url = String.Format("/Packages/{0}-{1}", title, id);

            return url;
        }

        public static string GetProductImage(string imageFile, Size? size)
        {
            if (!size.HasValue)
                return imageFile;

            if (String.IsNullOrWhiteSpace(imageFile))
                imageFile = "Default.jpg";

            string url = String.Format("{0}?width={1}&height={2}", StaticPaths.ProductImages + imageFile, size.Value.Width, size.Value.Height);

            return url;
        }

        public static string GetBannerImage(string imageFile, Size? size = null)
        {
            string url;

            if (size.HasValue)
                url = String.Format("{0}?width={1}&height={2}", StaticPaths.BannerImages + imageFile, size.Value.Width, size.Value.Height);
            else
                url = StaticPaths.BannerImages + imageFile;

            return url;
        }

        public static string GetProducerImage(string imageFile, Size size)
        {
            if (String.IsNullOrWhiteSpace(imageFile))
            {
                imageFile = "200x90.png";
            }
            var url = String.Format("{0}?width={1}&height={2}", StaticPaths.ProducerImages + imageFile, size.Width, size.Height);

            return url;
        }

        public static string GetCustomerImage(string imageFile, Size size)
        {
            if (String.IsNullOrWhiteSpace(imageFile))
            {
                imageFile = "70x70.jpg";
            }

            var url = String.Format("{0}?width={1}&height={2}", StaticPaths.CustomerImages + imageFile, size.Width, size.Height);

            return url;
        }

        public static string GetSliderImage(string imageFile, Size size)
        {
            var url = String.Format("{0}", StaticPaths.SliderImages + imageFile, size.Width, size.Height);

            return url;
        }

        public static string GetOfferImage(string imageFile, Size size)
        {
            var url = String.Format("{0}", StaticPaths.OfferImages + imageFile, size.Width, size.Height);

            return url;
        }

        public static string GetPostImage(string imageFile, Size? size = null)
        {
            if (size.HasValue)
            {
                var width = size.Value.Width;
                var height = size.Value.Height;

                if (String.IsNullOrWhiteSpace(imageFile))
                {
                    imageFile = String.Format("{0}x{1}.jpg", width, height);
                }

                var url = String.Format("{0}?width={1}&height={2}", StaticPaths.ArticleImages + imageFile, width, height);

                return url;
            }
            else
            {
                var url = String.Format("{0}", StaticPaths.ArticleImages + imageFile);

                return url;
            }
        }

        public static string GetUserImage(string imageFile, Size size)
        {
            if (String.IsNullOrWhiteSpace(imageFile))
            {
                imageFile = String.Format("{0}x{1}.jpg", size.Width, size.Height);
            }

            var url = String.Format("{0}?width={1}&height={2}", StaticPaths.OSUsers + imageFile, size.Width, size.Height);

            return url;
        }

        public static string GetPriceListSectionImage(string imageFile, Size? size)
        {
            if (!size.HasValue)
                return imageFile;

            if (String.IsNullOrWhiteSpace(imageFile))
                imageFile = "Default.jpg";

            string url = String.Format("{0}?width={1}&height={2}", StaticPaths.PriceListSectionImages + imageFile, size.Value.Width, size.Value.Height);

            return url;
        }

        public static string GetMenuItemBannerImage(string imageFile, Size? size = null)
        {
            string url;

            if (size.HasValue)
                url = String.Format("{0}?width={1}&height={2}", StaticPaths.MenuItemBannerImages + imageFile, size.Value.Width, size.Value.Height);
            else
                url = StaticPaths.MenuItemBannerImages + imageFile;

            return url;
        }

        public static string GetPackageImage(string imageFile, Size? size)
        {
            if (!size.HasValue)
                return imageFile;

            if (String.IsNullOrWhiteSpace(imageFile))
                imageFile = "Default.jpg";

            string url = String.Format("{0}?width={1}&height={2}", StaticPaths.PackageImages + imageFile, size.Value.Width, size.Value.Height);

            return url;
        }



    }
}
