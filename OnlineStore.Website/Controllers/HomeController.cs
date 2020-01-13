using AutoMapper;
using OnlineStore.DataLayer;
using OnlineStore.Models.Enums;
using OnlineStore.Models.Public;
using OnlineStore.Providers;
using OnlineStore.Providers.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace OnlineStore.Website.Controllers
{
    public class HomeController : PublicController
    {
        public ActionResult Index(int pageIndex = 0, int pageSize = 25, string pageOrder = "Newest")
        {
            ViewBag.OpenMenu = true;

            var homeSlides = SliderImages.GetSlides(SliderType.Home);
            var offerSlides = SliderImages.GetSlides(SliderType.Offer);

            var banner = Banners.ShowPageBanners("Home");
            var producer = Producers.Get();
            var latestNews = Articles.GetLatestPosts();

            var latestProducts = Products.GetLatestProducts();
            Products.FillProductItems(UserID, latestProducts, StaticValues.LatestProductImageSize);

            var random = Products.GetRandom(4);
            Products.FillProductItems(UserID, random, StaticValues.DefaultProductImageSize);

            var homeSlider = Mapper.Map<List<ViewSliderImage>>(homeSlides);
            var offerSlider = Mapper.Map<List<ViewSliderImage>>(offerSlides);

            var banners = Mapper.Map<List<ViewBanner>>(banner);
            var producerImages = Mapper.Map<List<ViewProducer>>(producer);

            #region Home Boxes

            var homeBoxes = HomeBoxes.Get();
            foreach (var item in homeBoxes)
            {
                if (item.HomeBoxType == HomeBoxType.Horizontal)
                {
                    item.Products = HomeBoxProducts.GetProductItemByBoxID(item.ID);

                    Products.FillProductItems(UserID, item.Products, StaticValues.DefaultProductImageSize);
                }
                else if (item.HomeBoxType == HomeBoxType.Group)
                {
                    item.Items = HomeBoxProducts.GetHomeBoxItemsByBoxID(item.ID);
                }
            }

            #endregion Home Boxes

            var model = new HomeSettings();

            model.CustomerComments = CustomerComments.ShowCustomerCommentsList();

            model.HomeSlider = homeSlider;
            model.OfferSlider = offerSlider;

            model.Banners = banners;
            model.Producers = producerImages;
            model.HomeBoxes = homeBoxes;

            model.LatestProducts = latestProducts;
            model.LatestNews = latestNews;
            model.RandomProducts = random;

            ViewBag.Title = String.Empty;
            ViewBag.Description = StaticValues.WebsiteDescription;
            ViewBag.Keywords = StaticValues.WebsiteKeywords;
            ViewBag.OGType = "article";
            ViewBag.OGImage = StaticValues.WebsiteUrl + "/images/small-logo.jpg";

            return View(model);
        }

    }
}