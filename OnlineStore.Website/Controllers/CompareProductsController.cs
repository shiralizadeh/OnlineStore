using OnlineStore.DataLayer;
using OnlineStore.Models.Public;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using OnlineStore.Providers;
using OnlineStore.Models.Enums;
using OnlineStore.Models;
using OnlineStore.Models.Admin;
using System.Collections;
using OnlineStore.Providers.Controllers;

namespace OnlineStore.Website.Controllers
{
    public class CompareProductsController : PublicController
    {
        [Route("CompareProducts/{ids}")]
        public ActionResult Index(string ids)
        {
            ViewBag.NoIndex = true;

            int? groupID = null;

            List<int> productIDs = ids.Split(',').Where(item => !String.IsNullOrEmpty(item)).Select(item => int.Parse(item)).ToList();
            List<JsonProductCompare> list = new List<JsonProductCompare>();

            foreach (var productID in productIDs)
            {
                if (groupID == null)
                    groupID = Products.GetGroupID(productID);

                var productItem = GetJsonProductCompare(productID, groupID.Value);

                list.Add(productItem);
            }

            ViewBag.Title = "مقایسه کالا :" + Products.GetTitleByID(productIDs.First());

            return View(list);
        }

        [HttpPost]
        [Route("CompareProducts/AddToCompare")]
        public JsonResult AddToCompare(int groupID, int newProductID)
        {
            var jsonSuccessResult = new JsonSuccessResult();

            try
            {
                var productItem = GetJsonProductCompare(newProductID, groupID);

                jsonSuccessResult.Data = productItem;
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

        [Route("CompareProducts/Search")]
        public JsonResult Search(int groupID, string key)
        {
            var jsonSuccessResult = new JsonSuccessResult();

            try
            {
                var list = Products.SearchByGroup_Key(groupID, key);

                jsonSuccessResult.Data = list;
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

        #region Methods

        private JsonProductCompare GetJsonProductCompare(int productID, int groupID)
        {
            var product = Products.GetDetails(productID);

            var group = Groups.GetByID(groupID);

            product.Title = group.Perfix + " " + product.Title;
            product.Title_En = product.Title_En + " " + group.Perfix_En;

            var url = UrlProvider.GetProductUrl(productID, group.UrlPerfix, product.UrlPerfix);

            #region Image File

            string fileName = "";

            var imageInfo = ProductImages.GetDefaultImage(productID);

            if (imageInfo != null)
            {
                fileName = imageInfo.Filename;
            }
            var image = UrlProvider.GetProductImage(fileName,
                                                StaticValues.ThumbnailProductImageSize);

            #endregion Image File

            #region Scores

            float totalScore = Utilities.CalcRaty(product.SumScore.Value, product.ScoreCount, product.ProductScore);

            List<ScoresAverage> scoresAverages = ScoreParameterValues.CalculateAverage(productID);

            #endregion Scores

            #region Attributes

            var groups = ProductGroups.GetByProductID(productID).Select(item => item.GroupID).ToList();
            var attrbutes = Attributes.GetByGroupIDs(groups);
            foreach (var item in attrbutes)
            {
                item.Value = AttributeValues.GetValue(productID, item.ID);

                if (item.Value != null)
                {
                    item.Value = AttributeValues.RenderValue(item);
                }
            }

            #endregion Attributes

            #region Prices

            product.Prices = Products.GetProductPrices(product.ID, product.HasVarients, PriceType.Sell);

            string userID = null;
            if (User.Identity.IsAuthenticated)
            {
                userID = UserID;
            }

            Products.SetDiscounts(userID, product.ID, product.HasVarients, product.Prices);

            #endregion Prices

            var productItem = new JsonProductCompare
            {
                ProductID = productID,
                DisplayTitleType = product.DisplayTitleType,
                Title_Fa = product.Title,
                Title_En = product.Title_En,
                Url = url,
                Prices = product.Prices,
                Image = image,
                Score = totalScore,
                PreferentialsCount = product.ScoreCount + 1,
                Attributes = attrbutes,
                ScoresAverages = scoresAverages,
                GroupID = groupID,
                IsUnavailable = product.IsUnavailable
            };

            return productItem;
        }


        #endregion Methods
    }
}