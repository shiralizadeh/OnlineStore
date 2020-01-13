using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineStore.Providers
{
    public static class StaticValues
    {
        static StaticValues()
        {
            Features.Add(new FeatureItem()
            {
                ID = 1,
                Title = "24/7",
                Description = "",
            });
        }

        public const string AdminID = "a7f329d8-fe23-499b-a557-11fde969ff1b";
        public const string PublicRoleID = "2a23eff7-4fea-45c6-b41f-3c98f3a1dabc";


        public static string WebsiteTitle = "";
        public static string Email = "";
        public static string Address = "";
        public static string PostalCode = "";
        public static string Phone = "";
        public static string Tax = "";
        public static string WebsiteUrl = "";
        public static string WebsiteDescription = "Online Store Description";
        public static string WebsiteKeywords = "Online Store Keywords";
        public static string Logo = "";
        public static int DeliveryPrice = 0;
        public static IList GroupOptions = null;

        public static string Writer = "";
        public static string Accountant = "";
        public static string Administrator = "";

        public static string HomeTitle = "Online Store";
        public static string HomeTitle_En = "Online Store";

        public static List<FeatureItem> Features = new List<FeatureItem>();

        public static readonly string PgwSite = ConfigurationManager.AppSettings["PgwSite"];
        public static readonly string CallBackUrl = ConfigurationManager.AppSettings["CallBackUrl"];
        public static readonly string TerminalId = ConfigurationManager.AppSettings["TerminalId"];
        public static readonly string UserName = ConfigurationManager.AppSettings["UserName"];
        public static readonly string UserPassword = ConfigurationManager.AppSettings["UserPassword"];

        public static readonly string InfoEmail = ConfigurationManager.AppSettings["InfoEmail"];
        public static readonly string InfoPassword = ConfigurationManager.AppSettings["InfoPassword"];

        public static readonly string EverySendUser = ConfigurationManager.AppSettings["EverySendUser"];
        public static readonly string EverySendPassword = ConfigurationManager.AppSettings["EverySendPassword"];

        public static readonly string AsnafUser = ConfigurationManager.AppSettings["AsnafUser"];
        public static readonly string AsnafPassword = ConfigurationManager.AppSettings["AsnafPassword"];
        public static readonly string AsnafSMSID = ConfigurationManager.AppSettings["AsnafSMSID"];

        #region Products

        public static Size MiniCartProductImageSize = new Size(65, 77);
        public static Size CartProductImageSize = new Size(130, 160);
        public static Size DefaultProductImageSize = new Size(180, 200);
        public static Size DetailProductImageSize = new Size(344, 344);
        public static Size ThumbnailProductImageSize = new Size(60, 60);
        public static Size GalleryProductImageSize = new Size(81, 107);
        public static Size ProducerImageSize = new Size(200, 90);
        public static Size RelatedProductImageSize = new Size(280, 170);
        public static Size LatestProductImageSize = new Size(72, 72);

        #endregion Products

        #region Home Banners

        public static Size HTRBannerImageSize = new Size(450, 620);
        public static Size HCBannerImageSize = new Size(470, 171);
        public static Size HTLBannerImageSize = new Size(370, 372);
        public static Size HBBannerImageSize = new Size(870, 197);

        #endregion Home Banners

        #region Blog

        public static Size ThumbnailPostImageSize = new Size(215, 135);
        public static Size DefaultPostImageSize = new Size(280, 180);
        public static Size DetailPostImageSize = new Size(880, 495);
        public static Size LatestNewsImageSize = new Size(175, 160);

        #endregion Blog

        #region Menu Banners

        public static Size BigBannerSize = new Size(530, 320);
        public static Size SmallBannerSize = new Size(195, 320);
        public static Size OtherBannersSize = new Size(245, 240);

        #endregion Menu Banners

        public static Size CustomerImageSize = new Size(70, 70);
        public static Size ThumbnailAdminImageSize = new Size(29, 29);
        public static Size SliderImageSize = new Size(730, 402);
        public static Size SearchImageSize = new Size(50, 50);
        public static Size SectionImageSize = new Size(200, 60);

        public static string NotFoundUrl = "/404";

        public static bool MaxPriceFreeDelivery = false;
    }

    public class FeatureItem
    {
        public int ID { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
    }
}
