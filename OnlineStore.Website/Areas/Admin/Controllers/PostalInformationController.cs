using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OnlineStore.Providers.Controllers;
using OnlineStore.DataLayer;
using OnlineStore.Identity;
using AutoMapper;
using OnlineStore.Models.Public;
using OnlineStore.Models.Admin;

namespace OnlineStore.Website.Areas.Admin.Controllers
{
    public class PostalInformationController : AdminController
    {
        public ActionResult Index(string id)
        {
            List<PostalInformation> PostalInfoList = new List<PostalInformation>();

            var IDs = id.Split(',');

            foreach (var item in IDs)
            {
                var cart = Carts.GetByID(Int32.Parse(item));

                if (cart.UserID != null)
                {
                    var user = OSUsers.GetByID(cart.UserID);
                    var buyer = Mapper.Map<ViewBuyerInfo>(user);

                    buyer.StateName = user.StateID.HasValue ? Cities.GetCityName(user.StateID.Value) : String.Empty;
                    buyer.CityName = user.CityID.HasValue ? Cities.GetCityName(user.CityID.Value) : String.Empty;

                    PostalInformation postalInfo = new PostalInformation
                    {
                        ViewBuyerInfo = buyer,
                        Description = cart.UserDescription
                    };

                    PostalInfoList.Add(postalInfo);
                }

            }


            return View(PostalInfoList);
        }
    }
}