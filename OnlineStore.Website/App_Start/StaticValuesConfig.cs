using OnlineStore.DataLayer;
using OnlineStore.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using OnlineStore.Models.Public;
using OnlineStore.Services;
using AutoMapper;
using OnlineStore.Identity;

namespace OnlineStore.Website
{
    public static class StaticValuesConfig
    {
        public static void Configure()
        {
            StaticValues.WebsiteTitle = StaticContents.GetContentByName("CompanyName");
            StaticValues.Email = StaticContents.GetContentByName("Email");
            StaticValues.Address = StaticContents.GetContentByName("Address");
            StaticValues.Phone = StaticContents.GetContentByName("Phone");
            StaticValues.PostalCode = StaticContents.GetContentByName("PostalCode");
            StaticValues.Tax = StaticContents.GetContentByName("Tax");
            StaticValues.WebsiteUrl = StaticContents.GetContentByName("Website");
            StaticValues.Logo = StaticContents.GetContentByName("Logo");
            StaticValues.Writer = UserRoles.GetByRoleName("Writer");
            StaticValues.Accountant = UserRoles.GetByRoleName("Accountant");
            StaticValues.Administrator = UserRoles.GetByRoleName("Administrator");

            #region Emails

            var infoEmail = Emails.GetByID(1);
            EmailServices.InfoMail = Mapper.Map<ViewEmail>(infoEmail);

            var saleEmail = Emails.GetByID(2);
            EmailServices.SaleMail = Mapper.Map<ViewEmail>(saleEmail);

            var registerEmail = Emails.GetByID(3);
            EmailServices.RegisterMail = Mapper.Map<ViewEmail>(registerEmail);

            var newsletterEmail = Emails.GetByID(4);
            EmailServices.NewsletterMail = Mapper.Map<ViewEmail>(newsletterEmail);

            #endregion Emails

            var deliveryPrice = 0;
            int.TryParse(StaticContents.GetContentByName("DeliveryPrice"), out deliveryPrice);
            StaticValues.DeliveryPrice = deliveryPrice;

            StaticValues.GroupOptions = AttributeValues.GetOptionIDsByGroup();
        }
    }
}