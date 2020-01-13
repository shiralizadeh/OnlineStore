using OnlineStore.DataLayer;
using OnlineStore.Models.Public;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using OnlineStore.Identity;
using OnlineStore.Providers;
using OnlineStore.Models.Enums;
using OnlineStore.Models;
using Public = OnlineStore.Models.Public;
using OnlineStore.Models.Admin;
using OnlineStore.Providers.Controllers;
using OnlineStore.EntityFramework;
using System.Collections;
using OnlineStore.Services;
using System.IO;
using AutoMapper;
using System.Threading;

namespace OnlineStore.Website.Controllers
{
    public class ProductsController : PublicController
    {
        private GroupType _groupType;
        private static System.Text.RegularExpressions.Regex isProducer = new System.Text.RegularExpressions.Regex(@"^[\w-,]+$");

        public static List<GroupOption> GroupOptions
        {
            get
            {
                return (List<GroupOption>)StaticValues.GroupOptions;
            }
        }

        public ProductsController()
        {
            _groupType = GroupType.Products;
        }

        [HttpGet]
        [Route("Products/{GroupTitle}")]
        [Route("Products/{GroupTitle}/{PageIndex:int}")]
        [Route("Products/{GroupTitle}/{ProducersOrAttributes}/{PageIndex:int}")]
        [Route("Products/{GroupTitle}/{Producers}/{Attributes}/{PageIndex:int}")]
        public ActionResult List(
            string groupTitle,
            int pageIndex = 1,
            int pageSize = 24,
            string pageOrder = "Weight",
            string producers = "",
            string attributes = "")
        {
            var producersOrAttributes = (string)RouteData.Values["ProducersOrAttributes"];
            if (producersOrAttributes != null)
            {
                if (isProducer.IsMatch(producersOrAttributes))
                    producers = producersOrAttributes;
                else
                    attributes = producersOrAttributes;
            }

            List<int> producersList = Producers.GetByProducerName(producers.Split(',').Select(a => a.DeNormalizeForUrl()).ToList()).Select(a => a.ID).ToList();
            List<string> attributesList = (!String.IsNullOrWhiteSpace(attributes) ? attributes.Split(',').ToList() : new List<string>());

            var isSearch = (producersList.Count > 0 || attributesList.Count > 0);

            if (pageIndex > 0)
            {
                pageIndex = pageIndex - 1;
            }
            else
                pageIndex = 0;

            if (String.IsNullOrWhiteSpace(groupTitle))
                return new HttpNotFoundResult();

            groupTitle = groupTitle.DeNormalizeForUrl();

            var group = Groups.GetByTitle(groupTitle, _groupType);

            if (group == null)
                return new HttpNotFoundResult();

            var groupChilds = new List<Group>();
            var groupParents = new List<Group>();

            var groupHasChild = Groups.HasChild(group.ID);

            if (groupHasChild)
                groupChilds.AddRange(Groups.GetChildsRecursive(group.ID));
            else
                groupChilds.Add(group);

            if (group.ParentID != null)
                groupParents.AddRange(Groups.GetParentsRecursive(group));
            else
                groupParents.Add(group);

            var groupIDs = groupParents.Select(item => item.ID).ToList();
            List<int> groupChildIDs = groupChilds.Select(item => item.ID).ToList();

            groupIDs.AddRange(groupChildIDs);

            List<BreadCrumbLink> breadCrumbLinks = groupParents.Select(item => new BreadCrumbLink { Title = item.Title, Link = UrlProvider.GetGroupUrl(item.UrlPerfix) }).ToList();
            List<ViewAttribute> groupsAttributes = new List<ViewAttribute>();

            if (!groupHasChild)
                groupsAttributes = Attributes.GetByGroupIDs(groupIDs, true);

            int count;
            var productList = Products.GetList(pageIndex, pageSize, pageOrder, out count, groupChildIDs, attributesList, producersList);

            var totalPages = (int)Math.Ceiling((decimal)count / pageSize);
            var paging = Utilities.MakePaging(totalPages, pageIndex + 1);

            var groupsProducers = Producers.GetProducerItemByGroupIDs(groupIDs);

            List<Producer> producersItems = new List<Producer>();

            if (producers != null)
            {
                producersItems = Producers.GetByIDs(producersList);
            }

            List<ViewAttributeOption> attributesItems = new List<ViewAttributeOption>();

            if (attributes != null)
            {
                var tmp = attributesList.Select(item => int.Parse(item.Split('-')[1])).ToList();
                attributesItems = AttributeOptions.GetByIDs(tmp);
            }

            var groupUrl = UrlProvider.GetGroupUrl(group.UrlPerfix);

            var urlParts = new string[] {
                    groupUrl,
                    (producersItems != null && producersItems.Count > 0 ? String.Join(",", producersItems.Select(a => a.TitleEn.NormalizeForUrl())) : String.Empty),
                    (attributesList != null && attributesList.Count > 0 ? String.Join(",", attributes) : String.Empty)
                };

            ViewBag.OriginalUrl = String.Join("/", urlParts.Where(item => !String.IsNullOrWhiteSpace(item)).ToArray());

            if (totalPages > 1)
            {
                ViewBag.HasPaging = true;
                if (pageIndex == 0)
                    ViewBag.PrevPage = (int?)null;
                else
                    ViewBag.PrevPage = (pageIndex);

                if (pageIndex == totalPages - 1)
                    ViewBag.NextPage = (int?)null;
                else
                    ViewBag.NextPage = (pageIndex + 2);
            }

            Products.FillProductItems(UserID, productList, StaticValues.DefaultProductImageSize);

            var titleParts = new string[] {
                    group.Title,
                    (producersItems != null && producersItems.Count > 0 ? String.Join(" و ", producersItems.Select(a => a.Title)) : String.Empty),
                    (attributesItems != null && attributesItems.Count > 0 ? "(" + String.Join(" + ", attributesItems.Select(a => a.Title)) + ")" : String.Empty),
                    (pageIndex > 0 ? "- صفحه " + (pageIndex + 1) : String.Empty)
                };

            var title = String.Join(" ", titleParts.Where(a => !String.IsNullOrWhiteSpace(a)).ToArray());
            ViewBag.Title = title;

            if (groupsProducers.Count > 0)
            {
                ViewBag.Description = "خرید آنلاین " + groupsProducers.Take(10).Select(item => item.Title + " (" + item.TitleEn + ")").Aggregate((a, b) => b + ", " + a) + " در فروشگاه اینترنتی آنلاین استور" + (pageIndex > 0 ? " - صفحه " + (pageIndex + 1) : String.Empty);
                ViewBag.Keywords = group.Title + ", " + group.TitleEn + ", " + groupChilds.Select(item => item.Title + ", " + item.TitleEn).Aggregate((a, b) => b + ", " + a) + ", " + groupsProducers.Take(10).Select(item => item.Title + ", " + item.TitleEn).Aggregate((a, b) => b + ", " + a);
            }

            ViewBag.CanonicalUrl = String.Join("/", urlParts.Where(item => !String.IsNullOrWhiteSpace(item)).ToArray()) + "/" + (pageIndex + 1);

            foreach (var item in groupChilds)
            {
                ViewBag.Keywords += item.Title + ", " + item.TitleEn + ", ";
            }

            if (isSearch)
            {
                breadCrumbLinks.Add(new BreadCrumbLink()
                {
                    Title = title,
                    Link = ViewBag.CanonicalUrl
                });
            }

            var model = new ProductListSettings
            {
                Products = productList,
                Producers = groupsProducers,
                BreadCrumbUrls = breadCrumbLinks,
                Attributes = groupsAttributes,
                Paging = paging,
                TotalPages = totalPages,
                CurrentPageIndex = pageIndex,
                OriginalUrl = ViewBag.OriginalUrl,
                GroupUrl = groupUrl,
                GroupID = group.ID,
                ProducersFilter = producersList,
                AttributesFilter = attributesItems
            };

            return View(model);
        }

        [HttpPost]
        [Route("Products/{GroupTitle}")]
        public JsonResult AjaxList(
            string groupTitle,
            int pageIndex,
            int pageSize,
            string pageOrder,
            List<string> attributes = null,
            List<int> producers = null)
        {
            if (pageIndex > 0)
            {
                pageIndex = pageIndex - 1;
            }
            else
                pageIndex = 0;

            var jsonSuccessResult = new JsonSuccessResult();

            try
            {
                groupTitle = groupTitle.DeNormalizeForUrl();

                var group = Groups.GetByTitle(groupTitle, _groupType);

                ViewBag.Title = group.Title;

                var groupChilds = new List<Group>();
                var groupParents = new List<Group>();

                if (Groups.HasChild(group.ID))
                    groupChilds.AddRange(Groups.GetChildsRecursive(group.ID));
                else
                    groupChilds.Add(group);

                if (group.ParentID != null)
                    groupParents.AddRange(Groups.GetParentsRecursive(group));
                else
                    groupParents.Add(group);

                var groupIDs = groupParents.Select(item => item.ID).ToList();

                List<int> groupChildIDs = groupChilds.Select(item => item.ID).ToList();

                List<Producer> producersItems = new List<Producer>();

                if (producers != null)
                {
                    producersItems = Producers.GetByIDs(producers);
                }

                List<ViewAttributeOption> attributesItems = new List<ViewAttributeOption>();

                if (attributes != null)
                {
                    var tmp = attributes.Select(item => int.Parse(item.Split('-')[1])).ToList();
                    attributesItems = AttributeOptions.GetByIDs(tmp);
                }

                List<BreadCrumbLink> breadCrumbLinks = groupParents.Select(item => new BreadCrumbLink { Title = item.Title, Link = UrlProvider.GetGroupUrl(item.UrlPerfix) }).ToList();

                int count;
                var productList = Products.GetList(pageIndex, pageSize, pageOrder, out count, groupChildIDs, attributes, producers);
                //var count = Products.Count(groupChildIDs, attributes, producers);

                var totalPages = (int)Math.Ceiling((decimal)count / pageSize);
                var paging = Utilities.MakePaging(totalPages, pageIndex + 1);

                foreach (var item in productList)
                    item.Url = UrlProvider.GetProductUrl(item.ID, group.UrlPerfix, item.UrlPerfix);

                var titleParts = new string[] {
                    group.Title,
                    (producersItems != null && producersItems.Count > 0 ? String.Join(" و ", producersItems.Select(a => a.Title)) : String.Empty),
                    (attributes != null && attributes.Count > 0 ? "(" + String.Join(" + ", attributesItems.Select(a => a.Title)) + ")" : String.Empty),
                    (pageIndex > 0 ? "- صفحه " + (pageIndex + 1) : String.Empty)
                };

                var title = String.Join(" ", titleParts.Where(a => !String.IsNullOrWhiteSpace(a)).ToArray());
                ViewBag.Title = title + " - " + StaticValues.WebsiteTitle;

                var urlParts = new string[] {
                    UrlProvider.GetGroupUrl(group.UrlPerfix),
                    (producersItems != null && producersItems.Count > 0 ? String.Join(",", producersItems.Select(a => a.TitleEn.NormalizeForUrl())) : String.Empty),
                    (attributes != null && attributes.Count > 0 ? String.Join(",", attributes) : String.Empty)
                };

                ViewBag.OriginalUrl = String.Join("/", urlParts.Where(item => !String.IsNullOrWhiteSpace(item)).ToArray());

                ViewBag.CanonicalUrl = ViewBag.OriginalUrl + "/" + (pageIndex + 1);

                var model = new ProductListSettings
                {
                    Products = productList,
                    Paging = paging,
                    TotalPages = totalPages,
                    CurrentPageIndex = pageIndex,
                    OriginalUrl = ViewBag.OriginalUrl,
                    PageTitle = ViewBag.Title,
                    CanonicalUrl = ViewBag.CanonicalUrl,
                };

                Products.FillProductItems(UserID, model.Products, StaticValues.DefaultProductImageSize);

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

        public static string RenderControl(ViewAttribute item)
        {
            string control = String.Empty;

            switch (item.AttributeType)
            {
                case AttributeType.Text:
                    control += "<input id='attr_" + item.ID + "' name='attr_" + item.ID + "' type='text' value='" + (item.Value != null ? item.Value : "") + "' class='form-control' />";
                    break;
                case AttributeType.Number:
                    control += "<input id='attr_" + item.ID + "' name='attr_" + item.ID + "' type='text' value='" + (item.Value != null ? item.Value : "") + "' class='form -control number' />";
                    break;
                case AttributeType.SingleItem:
                    control += "<select id='attr_" + item.ID + "' name='attr_" + item.ID + "' class='form-control' />";

                    foreach (var op in item.Options)
                    {
                        control += "<option value='" + op.ID + "'>" + op.Title + " </option> ";
                    }

                    control += "</select>";
                    break;
                case AttributeType.MultipleItem:
                    control += "<select id='attr_" + item.ID + "' name='attr_" + item.ID + "' multiple class='form-control' >";

                    foreach (var op in item.Options)
                    {
                        control += "<option value='" + op.ID + "'>" + op.Title + " </option> ";
                    }
                    control += "</select>";
                    break;
                case AttributeType.Check:
                    control = "<div class='checkbox'>";
                    control += "<input id='attr_" + item.ID + "' name='attr_" + item.ID + "' type='checkbox' " + (item.Value != null ? "checked" : "") + " />";
                    control += "<label for='attr_" + item.ID + "'>" + item.Title + "</label>";
                    control += "</div>";
                    break;
                case AttributeType.MultilineText:
                    control += "<textarea id='attr_" + item.ID + "' name='attr_" + item.ID + "' class='form-control' >" + (item.Value != null ? item.Value : "") + "</textarea>";
                    break;
                default:
                    control += "نا معلوم";
                    break;
            }

            return control;
        }

        public static string RenderSearchControl(ViewAttribute item, List<ViewAttributeOption> attributesFilter, int groupID)
        {
            string control = String.Empty;

            switch (item.AttributeType)
            {
                case AttributeType.Text:
                    control += "<input id='attr_" + item.ID + "' name='attr_" + item.ID + "' type='text' value='" + (item.Value != null ? item.Value : "") + "' class='form-control' />";
                    break;
                case AttributeType.Number:
                    control += "<input id='attr_" + item.ID + "' name='attr_" + item.ID + "' type='text' value='" + (item.Value != null ? item.Value : "") + "' class='form -control number' />";
                    break;
                case AttributeType.SingleItem:
                    control += "<ul id='attr_" + item.ID + "' name='attr_" + item.ID + "' class='" + (item.Options.Count > 6 ? "limitheight" : "") + "' />";
                    List<ViewOption> sOptionList = item.Options.ToList();

                    foreach (var op in item.Options)
                    {
                        var isExist = GroupOptions.Any(go => go.GroupID == groupID && go.Options.Contains(op.ID));

                        if (isExist)
                        {
                            var tmp = "<li data-id='" + op.ID + "'>" +
                                "    <input id='AttrOp_" + op.ID + "' type='checkbox' value='' " + (attributesFilter.Any(a => a.AttributeID == item.ID && a.ID == op.ID) ? "checked" : String.Empty) + "/>" +
                                "    <label for='AttrOp_" + op.ID + "'>" + op.Title + "</label>" +
                                "</li>";

                            control += tmp;
                        }
                        else
                        {
                            sOptionList.Remove(op);
                        }
                    }

                    control += "</ul>";

                    if (sOptionList.Count > 6)
                        control += "<a href='#' class='btn-more'>+ بیشتر</a>";

                    break;
                case AttributeType.MultipleItem:
                    control += "<ul id='attr_" + item.ID + "' name='attr_" + item.ID + "' class='" + (item.Options.Count > 6 ? "limitheight" : "") + "' />";
                    List<ViewOption> mOptionList = item.Options.ToList();

                    foreach (var op in item.Options)
                    {
                        if (op.ID == 1337)
                            continue;

                        var isExist = GroupOptions.Any(go => go.GroupID == groupID && go.Options.Contains(op.ID));

                        if (isExist)
                        {
                            var tmp = "<li data-id='" + op.ID + "'>" +
                            "    <input id='AttrOp_" + op.ID + "' type='checkbox' value='' " + (attributesFilter.Any(a => a.AttributeID == item.ID && a.ID == op.ID) ? "checked" : String.Empty) + "/>" +
                            "    <label for='AttrOp_" + op.ID + "'>" + op.Title + "</label>" +
                            "</li>";

                            control += tmp;
                        }
                        else
                        {
                            mOptionList.Remove(op);
                        }
                    }

                    control += "</ul>";

                    if (mOptionList.Count > 6)
                        control += "<a href='#' class='btn-more'>+ بیشتر</a>";

                    break;
                case AttributeType.Check:
                    control = "<div class='checkbox'>";
                    control += "<input id='attr_" + item.ID + "' name='attr_" + item.ID + "' type='checkbox' " + (item.Value != null ? "checked" : "") + " />";
                    control += "<label for='attr_" + item.ID + "'>" + item.Title + "</label>";
                    control += "</div>";
                    break;
                case AttributeType.MultilineText:
                    control += "<textarea id='attr_" + item.ID + "' name='attr_" + item.ID + "' class='form-control' >" + (item.Value != null ? item.Value : "") + "</textarea>";
                    break;
                default:
                    control += "نا معلوم";
                    break;
            }

            return control;
        }

        [Route("Products/{group}/{title}-{id:int}")]
        public ActionResult Details(int id)
        {
            bool isAdmin = false;
            if (base.IsAdmin || base.IsWriter)
            {
                isAdmin = true;
            }

            var groups = ProductGroups.GetParents(id);
            var groupIDs = groups.Select(item => item.GroupID).ToList();

            var productDetail = Products.GetDetails(id, isAdmin);
            if (productDetail == null)
            {
                return HttpNotFound();
            }

            var group = Groups.GetByID(productDetail.GroupID.Value);

            productDetail.Title = group.Perfix + " " + productDetail.Title;
            productDetail.Title_En = productDetail.Title_En + " " + group.Perfix_En;

            var allImages = ProductImages.GetGalleryImages(id);
            var defaultImage = allImages.Where(item => item.ProductImagePlace == ProductImagePlace.Home).FirstOrDefault();
            var commentList = ProductComments.ShowCommentsList(id);
            var questionList = ProductQuestions.ShowQuestionsList(id);

            #region Prices

            productDetail.Prices = Products.GetProductPrices(productDetail.ID, productDetail.HasVarients, PriceType.Sell);

            #region Discount Price

            string userID = null;
            if (User.Identity.IsAuthenticated)
            {
                userID = UserID;
            }

            Products.SetDiscounts(userID, productDetail.ID, productDetail.HasVarients, productDetail.Prices);

            #endregion Discount Price

            #endregion Prices

            #region DefaultImage

            if (defaultImage == null)
            {
                defaultImage = new EditProductImage()
                {
                    Filename = StaticFiles.DefaultProductImage
                };
            }

            #endregion DefaultImage

            #region Scores

            var scoreParameters = GroupScoreParameters.GetByGroupID(groupIDs);
            var scoreCommentList = ScoreComments.GetScoreComments(id);

            //میانگین
            List<int> parametrsList = scoreParameters.Select(item => item.ID).ToList();
            List<ScoresAverage> scoresAverages = ScoreParameterValues.CalculateAverage(id);

            #endregion Scores

            #region Related Products

            var relatedProducts = RelatedProducts.GetByProductID(id);
            Products.FillProductItems(UserID, relatedProducts, StaticValues.RelatedProductImageSize);

            var relatedProductSettings = new RelatedProductSettings
            {
                Products = relatedProducts,
                Title = "محصولات مرتبط"
            };

            #endregion Related Products

            #region Similar Products

            var similarProducts = Products.SimilarProducts(productDetail.GroupID.Value, productDetail.OrderID, id);
            Products.FillProductItems(UserID, similarProducts, StaticValues.DefaultProductImageSize);

            #endregion Similar Products

            #region Product Accessories

            var productAccessories = ProductAccessories.GetByProductID(id);
            Products.FillProductItems(UserID, productAccessories, StaticValues.DefaultProductImageSize);

            #endregion Product Accessories

            #region Attributes

            var attributes = Attributes.GetByGroupIDs(groupIDs);
            var removeList = new List<ViewAttribute>();

            foreach (var item in attributes)
            {

                item.Value = AttributeValues.GetValue(id, item.ID);

                // بررسی شرایط ویژگی وابسته
                #region Dependent

                if (item.DependentID.HasValue)
                {
                    var dependent = attributes.FirstOrDefault(d => d.ID == item.DependentID.Value);
                    var value = AttributeValues.GetValue(id, item.DependentID.Value);

                    switch (dependent.AttributeType)
                    {
                        case AttributeType.SingleItem:
                            if (value == null || (int)value != item.DependentOptionID)
                                removeList.Add(item);
                            break;

                        case AttributeType.MultipleItem:
                            var listVal = (IList)value;
                            if (value == null || !listVal.Contains(item.DependentOptionID))
                                removeList.Add(item);
                            break;

                        case AttributeType.Check:
                            if (!(bool)value)
                                removeList.Add(item);
                            break;

                        default:
                            break;
                    }
                }

                #endregion Dependent

                if (item.Value != null)
                {
                    switch (item.AttributeType)
                    {
                        case AttributeType.Text:
                        case AttributeType.Number:
                        case AttributeType.MultilineText:
                            if (String.IsNullOrWhiteSpace(item.Value.ToString()))
                                removeList.Add(item);
                            break;
                        case AttributeType.SingleItem:
                            var option = item.Options.FirstOrDefault(op => op.ID == (int)item.Value);
                            if (option == null)
                            {
                                removeList.Add(item);
                            }
                            break;
                        case AttributeType.MultipleItem:
                            var options = item.Options.Where(op => ((IList)item.Value).Contains(op.ID));

                            if (options.Count() == 0)
                            {
                                removeList.Add(item);
                            }
                            break;
                        default:
                            break;
                    }

                    item.Value = AttributeValues.RenderValue(item);
                }
                else
                {
                    removeList.Add(item);
                }
            }

            foreach (var item in removeList)
                attributes.Remove(item);

            #endregion Attrbutes

            #region Gifts

            var gifts = ProductGifts.GetByProductID(id);

            #endregion Gifts

            var producer = Mapper.Map<Public.ViewProducer>(Producers.GetByID(productDetail.ProducerID));

            // increase Visits
            increaseVisits(id);

            ViewBag.Title = productDetail.DisplayTitle;
            ViewBag.Description =
                (
                !String.IsNullOrWhiteSpace(productDetail.Summary) ?
                productDetail.Summary :
                "خرید آنلاین و بررسی تخصصی " + productDetail.Title + " در فروشگاه اینترنتی آنلاین استور"
                );

            ViewBag.Keywords = groups.Select(item => item.Title + ", " + item.TitleEn).Aggregate((a, b) => b + ", " + a) +
                ", " + productDetail.Title.Split(' ').Aggregate((a, b) => b + ", " + a) +
                ", " + productDetail.Title_En.Split(' ').Aggregate((a, b) => b + ", " + a);
            ViewBag.OGType = "product";
            ViewBag.OGImage = StaticValues.WebsiteUrl + StaticPaths.ProductImages + defaultImage.Filename;
            ViewBag.CanonicalUrl = UrlProvider.GetProductUrl(productDetail.ID, group.UrlPerfix, productDetail.UrlPerfix);

            var groupParents = Groups.GetParentsRecursive(group);
            List<BreadCrumbLink> breadCrumbLinks = groupParents.Select(item => new BreadCrumbLink { Title = item.Title, Tooltip = item.TitleEn, Link = UrlProvider.GetGroupUrl(item.UrlPerfix) }).ToList();
            breadCrumbLinks.Insert(0, new BreadCrumbLink() { Title = StaticValues.HomeTitle, Tooltip = StaticValues.HomeTitle_En, Link = "/" });

            if (producer.ID != 8)
            {
                breadCrumbLinks.Add(new BreadCrumbLink() { Title = group.Title + " " + producer.Title, Tooltip = producer.TitleEn + " " + group.TitleEn, Link = UrlProvider.GetGroupProducerUrl(group.UrlPerfix, producer.TitleEn) });
            }

            breadCrumbLinks.Add(new BreadCrumbLink() { Title = productDetail.DisplayTitle, Tooltip = productDetail.OtherDisplayTitle, Link = "/" });

            Public.ProductDetailSettings model = new ProductDetailSettings
            {
                ProductDetail = productDetail,
                Producer = producer,
                DefaultImage = defaultImage,
                ProductImages = allImages,
                CommentsCount = scoreCommentList.Count,
                ProductQuestions = questionList,
                ScoreParameters = scoreParameters,
                ScoreComments = scoreCommentList,
                ScoresAverages = scoresAverages,
                RelatedProducts = relatedProductSettings,
                SimilarProducts = similarProducts,
                ProductAttributes = attributes,
                ProductAccessories = productAccessories,
                BreadCrumbLinks = breadCrumbLinks,
                Gifts = gifts
            };

            return View(model);
        }

        [HttpPost]
        [Route("Products/AddComment")]
        public JsonResult AddComment(int id, string userName, string email, string subject, string text)
        {
            var jsonSuccessResult = new JsonSuccessResult();

            try
            {
                ProductComment comment = new ProductComment
                {
                    ProductID = id,
                    CommentStatus = CommentStatus.NotChecked,
                    UserName = userName,
                    Email = email,
                    Subject = subject,
                    Text = text,
                    LastUpdate = DateTime.Now,
                    UserID = User.Identity.IsAuthenticated ? UserID : null
                };

                ProductComments.Insert(comment);

                jsonSuccessResult.Success = true;
            }
            catch (DbException ex)
            {
                jsonSuccessResult.Errors = ex.Errors.ToArray();
                jsonSuccessResult.Success = false;
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

        [HttpPost]
        [Route("Products/SendQuestion")]
        public JsonResult SendQuestion(int id, string question, string UserName)
        {
            var jsonSuccessResult = new JsonSuccessResult();

            try
            {
                ProductQuestion productQuestion = new ProductQuestion
                {
                    ProductID = id,
                    QuestionStatus = QuestionStatus.NotChecked,
                    UserName = UserName,
                    Question = question,
                    LastUpdate = DateTime.Now,
                    DateTime = DateTime.Now,
                    IsVisible = false,
                    UserID = User.Identity.IsAuthenticated ? UserID : null
                };

                ProductQuestions.Insert(productQuestion);

                // اطلاع رسانی به مدیر
                #region Apprise Admin 

                string body = "مدیریت محترم، در بخش پرسش های متداول، پرسش جدیدی ثبت شد:";
                body += "<br/>";
                body += String.Format("نام: {0} <br/> پرسش: {1}", UserName, question);

                EmailServices.NotifyAdminsByEmail(AdminEmailType.NewProductQuestion, body, UserID);

                #endregion Apprise Admin

                jsonSuccessResult.Success = true;
            }
            catch (DbException ex)
            {
                jsonSuccessResult.Errors = ex.Errors.ToArray();
                jsonSuccessResult.Success = false;
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

        [HttpPost]
        [Route("Products/SendScore")]
        public JsonResult SendScore(int id, string scoreParameters, string values, string text)
        {
            var jsonSuccessResult = new JsonSuccessResult();

            try
            {
                #region Insert Into ScoreComment

                var comment = new ScoreComment
                {
                    ProductID = id,
                    UserID = UserID,
                    Text = text,
                    ScoreCommentStatus = ScoreCommentStatus.NotChecked,
                    LastUpdate = DateTime.Now
                };

                ScoreComments.Insert(comment);

                int scoreCommentID = comment.ID;

                #endregion Insert Into ScoreComment

                #region Insert Rates

                var arrScoreParameters = scoreParameters.Split(',');
                var arrValues = values.Split(',');

                List<ScoreParameterValue> list = new List<ScoreParameterValue>();

                for (int i = 0; i < arrScoreParameters.Length; i++)
                {
                    ScoreParameterValue scoreParameterValue = new ScoreParameterValue
                    {
                        ScoreCommentID = scoreCommentID,
                        ScoreParameterID = Int32.Parse(arrScoreParameters[i]),
                        Rate = Int32.Parse(arrValues[i]),
                        LastUpdate = DateTime.Now
                    };

                    list.Add(scoreParameterValue);
                }

                ScoreParameterValues.Insert(list);

                #endregion Insert Rates

                // اطلاع رسانی به مدیر
                #region Apprise Admin 

                string body = "مدیریت محترم، در بخش امتیازات محصولات، نظر جدیدی ثبت شد:";
                body += "<br/>";
                body += String.Format("متن نظر: {0}", text);

                EmailServices.NotifyAdminsByEmail(AdminEmailType.NewProductComment, body, UserID);

                #endregion Apprise Admin


                jsonSuccessResult.Success = true;
            }
            catch (DbException ex)
            {
                jsonSuccessResult.Errors = ex.Errors.ToArray();
                jsonSuccessResult.Success = false;
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

        [HttpPost]
        [Route("Products/GetScoreValues")]
        public JsonResult GetScoreValues(int id)
        {
            var jsonSuccessResult = new JsonSuccessResult();

            try
            {
                jsonSuccessResult.Data = ScoreComments.GetUserScores(id, UserID);
                jsonSuccessResult.Success = true;
            }
            catch (DbException ex)
            {
                jsonSuccessResult.Errors = ex.Errors.ToArray();
                jsonSuccessResult.Success = false;
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

        [HttpPost]
        [Route("Products/EvaluationProductComment")]
        public JsonResult EvaluationProductComment(int commentID, bool isLikeClick)
        {
            var jsonSuccessResult = new JsonSuccessResult();

            try
            {
                // بررسی نظر
                evaluation(commentID, isLikeClick);

                // دریافت مقدار Like , DisLike
                JsonProductCommentRate rates = new JsonProductCommentRate
                {
                    LikesCount = ProductCommentRates.CountRates(true, commentID),
                    DisLikesCount = ProductCommentRates.CountRates(false, commentID)
                };

                jsonSuccessResult.Data = rates;
                jsonSuccessResult.Success = true;
            }
            catch (DbException ex)
            {
                jsonSuccessResult.Errors = ex.Errors.ToArray();
                jsonSuccessResult.Success = false;
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

        [HttpPost]
        [Route("Products/SendToFriend")]
        public JsonResult SendToFriend(int id, string friendEmail, string message)
        {
            var jsonSuccessResult = new JsonSuccessResult();

            try
            {

                string fullName = String.Empty,
                       userID = String.Empty,
                       productLink = String.Empty;

                if (User.Identity.IsAuthenticated)
                {
                    userID = UserID;
                    var user = OSUsers.GetByID(UserID);

                    if (!String.IsNullOrWhiteSpace(user.Firstname) && !String.IsNullOrWhiteSpace(user.Lastname))
                    {
                        fullName = "خانم/آقای " + user.Firstname + " " + user.Lastname;
                    }
                }

                var productDetail = Products.GetDetails(id);



                var image = ProductImages.GetDefaultImage(id).Filename;
                var otherTitle = productDetail.DisplayTitleType == DisplayTitleType.Title_Fa ? productDetail.Title_En : productDetail.Title;

                var group = Groups.GetByID(productDetail.GroupID.Value);

                productDetail.Title = group.Perfix + " " + productDetail.Title;
                productDetail.Title_En = productDetail.Title_En + " " + group.Perfix_En;

                var url = UrlProvider.GetProductUrl(id, group.UrlPerfix, productDetail.UrlPerfix);


                #region Insert

                var suggest = new ProductSuggestion();

                if (userID != string.Empty)
                {
                    suggest.UserID = userID;
                }
                suggest.IP = Utilities.GetIP();
                suggest.FriendEmail = friendEmail;
                suggest.Message = message;
                suggest.ProductID = id;
                suggest.LastUpdate = DateTime.Now;

                ProductSuggestions.Insert(suggest);

                #endregion Insert


                #region Prices

                productDetail.Prices = Products.GetProductPrices(productDetail.ID, productDetail.HasVarients, PriceType.Sell);

                #region Discount Price

                Products.SetDiscounts(userID, productDetail.ID, productDetail.HasVarients, productDetail.Prices);

                #endregion Discount Price

                #endregion Prices

                var priceBox = renderPartialViewToString("_Price", new PriceSettings { IsUnavailable = productDetail.IsUnavailable, SimplePrice = true, Prices = productDetail.Prices });
                priceBox = renderPriceBox(priceBox);
                EmailServices.SendToFriend(fullName,
                                           url,
                                           productDetail.DisplayTitle,
                                           otherTitle,
                                           UrlProvider.GetProductImage(image, StaticValues.DetailProductImageSize),
                                           friendEmail,
                                           message,
                                           userID,
                                           priceBox);

                jsonSuccessResult.Success = true;
            }
            catch (DbException ex)
            {
                jsonSuccessResult.Errors = ex.Errors.ToArray();
                jsonSuccessResult.Success = false;
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

        [HttpPost]
        [Route("Products/GetUserInfo")]
        public JsonResult GetUserInfo()
        {
            var jsonSuccessResult = new JsonSuccessResult();

            try
            {
                if (User.Identity.IsAuthenticated)
                {
                    var user = OSUsers.GetByID(UserID);

                    var data = new
                    {
                        Email = user.Email,
                        Mobile = user.Mobile
                    };

                    jsonSuccessResult.Data = data;
                }

                jsonSuccessResult.Success = true;
            }
            catch (DbException ex)
            {
                jsonSuccessResult.Errors = ex.Errors.ToArray();
                jsonSuccessResult.Success = false;
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

        [HttpPost]
        [Route("Products/AddRequest")]
        public JsonResult AddRequest(int productID, string email, string mobile, string description)
        {
            var product = Products.GetByID(productID);
            var group = Groups.GetByID(product.GroupID.Value);
            string productName = product.Title + "(" + product.Title_En + ")";
            string productLink = "http://online-store.com" + UrlProvider.GetProductUrl(productID, group.UrlPerfix, product.UrlPerfix);

            var jsonSuccessResult = new JsonSuccessResult();

            try
            {
                ProductRequest request = new ProductRequest
                {
                    Email = email,
                    Mobile = mobile,
                    Description = description,
                    LastUpdate = DateTime.Now,
                    DateTime = DateTime.Now,
                    ProductID = productID,
                    UserID = UserID,
                    ProductRequestStatus = ProductRequestStatus.NotChecked
                };

                ProductRequests.Insert(request);

                EmailServices.ProductRequest(email, UserID, productName, productLink);

                jsonSuccessResult.Success = true;
            }
            catch (DbException ex)
            {
                jsonSuccessResult.Errors = ex.Errors.ToArray();
                jsonSuccessResult.Success = false;
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


        [HttpPost]
        [Route("Products/InsertRate")]
        public JsonResult InsertRate(int productID, float rate)
        {
            var jsonSuccessResult = new JsonSuccessResult();

            try
            {
                string ip = Utilities.GetIP();
                var rpt = ProductRates.RepeatPreferentials(productID, ip, UserID);

                if (rpt < 5)
                {
                    var productRate = new ProductRate
                    {
                        ProductID = productID,
                        UserID = UserID,
                        Rate = rate,
                        IP = ip,
                        LastUpdate = DateTime.Now
                    };

                    ProductRates.Insert(productRate);

                    var rates = Products.GetRates(productID);
                    var total = Utilities.CalcRaty(rates.SumScore.Value, rates.ScoreCount, rates.ProductScore).ToString("0.0");

                    var data = new
                    {
                        CountScore = rates.ScoreCount + 1,
                        TotalScore = total
                    };

                    jsonSuccessResult.Success = true;
                    jsonSuccessResult.Data = data;
                }
                else
                {
                    jsonSuccessResult.Success = false;
                    jsonSuccessResult.Errors = new string[] { "Repeat" };
                }
            }
            catch (DbException ex)
            {
                jsonSuccessResult.Errors = ex.Errors.ToArray();
                jsonSuccessResult.Success = false;
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

        [Route("Random/{type}")]
        public RedirectResult Random(string type)
        {
            var jsonSuccessResult = new JsonSuccessResult();

            try
            {
                string url = String.Empty;

                if (type.ToLower() == "product")
                {
                    var product = Products.GetRandom(1)[0];
                    var group = Groups.GetByID(product.GroupID.Value);

                    product.Title_Fa = group.Perfix + " " + product.Title_Fa;
                    product.Title_En = product.Title_En + " " + group.Perfix_En;

                    url = UrlProvider.GetProductUrl(product.ID, group.UrlPerfix, product.UrlPerfix);
                }
                else if (type.ToLower() == "group")
                {
                    var group = Groups.GetRandom();

                    if (group.GroupType == GroupType.Products)
                        url = UrlProvider.GetGroupUrl(group.UrlPerfix);
                    else if (group.GroupType == GroupType.Blogs)
                        url = UrlProvider.GetBlogGroupUrl(group.UrlPerfix);
                }
                else
                {
                    url = "/";
                }

                return Redirect(url);
            }
            catch (Exception ex)
            {
                jsonSuccessResult.Errors = new string[] { ex.Message };
                jsonSuccessResult.Success = false;
            }

            return Redirect("/");
        }

        #region Methods

        private void evaluation(int commentID, bool isLikeClick)
        {
            var comment = ProductCommentRates.GetByUserID_CommentID(UserID, commentID);

            if (comment == null)
            {
                var rate = new ProductCommentRate
                {
                    IsLike = isLikeClick,
                    ScoreCommentID = commentID,
                    UserID = UserID,
                    LastUpdate = DateTime.Now
                };

                ProductCommentRates.Insert(rate);
            }
            else
            {

                comment.IsLike = !comment.IsLike;

                ProductCommentRates.Update(comment);

            }
        }

        protected string renderPartialViewToString(string viewName, object model)
        {
            if (string.IsNullOrEmpty(viewName))
                viewName = ControllerContext.RouteData.GetRequiredString("action");

            ViewData.Model = model;

            using (StringWriter sw = new StringWriter())
            {
                ViewEngineResult viewResult = ViewEngines.Engines.FindPartialView(ControllerContext, viewName);
                ViewContext viewContext = new ViewContext(ControllerContext, viewResult.View, ViewData, TempData, sw);
                viewResult.View.Render(viewContext, sw);

                return sw.GetStringBuilder().ToString();
            }
        }

        private string renderPriceBox(string priceBox)
        {
            var result = priceBox.Replace("class=\"price-isunavailable\"", "style='color:#ed1c24'")
                                 .Replace("class=\"regular-price\"", "style='color:#ff3d00'")
                                 .Replace("class=\"original-price\"", "style='text-decoration:line-through;color:#78909c;margin-top:5px;'")
                                 .Replace("class=\"topay-price\"", "style='color:#ff3d00'")
                                 .Replace("class=\"price-unit\"", "style='color:#455a64'")
                                 .Replace("\r\n", String.Empty);

            return result;
        }

        private void increaseVisits(int id)
        {
            var ip = Utilities.GetIP();
            var now = DateTime.Now;

            var threadStart = new ThreadStart(() =>
            {
                Products.AddVisits(id);
                ProductVisit visit = new ProductVisit
                {
                    IP = ip,
                    LastUpdate = now,
                    UserID = UserID,
                    ProductID = id
                };

                ProductVisits.Insert(visit);
            });
            Thread thread = new Thread(threadStart);
            thread.Start();
        }

        #endregion Methods
    }
}