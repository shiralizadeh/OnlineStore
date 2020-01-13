using OnlineStore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OnlineStore.DataLayer;
using OnlineStore.Models.Enums;
using OnlineStore.Models.Public;
using AutoMapper;
using System.Text.RegularExpressions;
using OnlineStore.Providers;
using OnlineStore.Providers.Controllers;

namespace OnlineStore.Website.Controllers
{
    public class SearchController : PublicController
    {
        public ActionResult Index(string text)
        {
            var key = (string)text.Clone();

            List<int> groupIDs = new List<int>();

            var products = Products.AdvancedSearch(key);
            Products.FillProductItems(UserID, products, StaticValues.DefaultProductImageSize);

            var blogs = Articles.SimpleSearch(key, ArticleStatus.Approved, StaticValues.DefaultPostImageSize);
            var producers = Producers.SimpleSearch(key, groupIDs, StaticValues.ProducerImageSize);
            var groups = Groups.SimpleSearch(key);

            var isFa = Utilities.ContainsUnicodeCharacter(key);
            key = key.GetReversed(isFa);

            if (products.Count == 0)
            {
                products = Products.AdvancedSearch(key);
                Products.FillProductItems(UserID, products, StaticValues.DefaultProductImageSize);
            }

            if (blogs.Count == 0)
            {
                blogs = Articles.SimpleSearch(key, ArticleStatus.Approved, StaticValues.DefaultPostImageSize);
            }

            if (groups.Count == 0)
            {
                groups = Groups.SimpleSearch(key);
            }

            var mappedGroups = Mapper.Map<List<JsonProductGroup>>(groups);

            ViewBag.Title = "جستجو - " + text;
            ViewBag.Description = "جستجوی کلمه '" + text + "'";
            ViewBag.Keywords = "جستجو, " + text;
            ViewBag.OGImage = StaticValues.WebsiteUrl + "/images/small-logo.jpg";

            var model = new AdvancedSearch
            {
                Products = products,
                Blogs = blogs,
                Producers = producers,
                Groups = mappedGroups,
            };

            return View(model: model);
        }

        public JsonResult SimpleSearch(string key)
        {
            var jsonSuccessResult = new JsonSuccessResult();

            try
            {
                List<int> groupIDs = new List<int>();


                var products = Products.Search(key);

                var blogs = Articles.SimpleSearch(key, ArticleStatus.Approved);
                var groups = Groups.SimpleSearch(key);

                var isFa = Utilities.ContainsUnicodeCharacter(key);
                key = key.GetReversed(isFa);

                if (products.Count == 0)
                {
                    products = Products.Search(key);
                }

                if (blogs.Count == 0)
                {
                    blogs = Articles.SimpleSearch(key, ArticleStatus.Approved);
                }
                if (groups.Count == 0)
                {
                    groups = Groups.SimpleSearch(key);
                }

                var mappedGroups = Mapper.Map<List<JsonProductGroup>>(groups);

                var model = new JsonSimpleSearch
                {
                    Products = products,
                    Blogs = blogs,
                    Groups = mappedGroups,
                };

                jsonSuccessResult.Data = model;
                jsonSuccessResult.Success = true;
            }
            catch (Exception ex)
            {
                jsonSuccessResult.Errors = new string[] { ex.Message };
                jsonSuccessResult.Success = false;
            }

            return new JsonResult()
            {
                Data = jsonSuccessResult
            };
        }
    }
}