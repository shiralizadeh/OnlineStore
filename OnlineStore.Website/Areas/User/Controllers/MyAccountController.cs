using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OnlineStore.Providers.Controllers;
using OnlineStore.DataLayer;
using OnlineStore.Models.Enums;
using OnlineStore.Models.User;
using OnlineStore.Models.Public;
using OnlineStore.Identity;
using AutoMapper;

namespace OnlineStore.Website.Areas.User.Controllers
{
    [RoutePrefix("My-Account")]
    public class MyAccountController : UserController
    {
        [Route]
        public ActionResult Index()
        {
            var user = OSUsers.GetByID(UserID);

            var buyer = Mapper.Map<ViewBuyerInfo>(user);

            // TODO: UserAddresses
            buyer.StateName = user.StateID.HasValue ? Cities.GetCityName(user.StateID.Value) : String.Empty;
            buyer.CityName = user.CityID.HasValue ? Cities.GetCityName(user.CityID.Value) : String.Empty;

            MyAccountSettings model = new MyAccountSettings();

            model.Orders = Carts.CountOrdersByUserID(UserID);
            model.OrdersSubmitted = Carts.CountOrdersByUserID(UserID, SendStatus.Sent);
            model.OrdersDelivered = Carts.CountOrdersByUserID(UserID, SendStatus.Delivered);
            model.Comments = ScoreComments.CountByUserID(UserID);
            model.CommentRates = ProductCommentRates.CountByUserID(UserID);
            model.Wishes = UserWishes.CountByUserID(UserID);
            model.Posts = Articles.CountByUserID(ArticleType.Blog, UserID);
            model.UserInfo = buyer;

            return View("/Areas/User/Views/MyAccount/Index.cshtml", model: model);
        }
    }
}