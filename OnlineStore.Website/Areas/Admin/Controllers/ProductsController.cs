using OnlineStore.DataLayer;
using OnlineStore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using OnlineStore.Providers.Controllers;
using OnlineStore.Models.Enums;
using OnlineStore.Models.Admin;
using Public = OnlineStore.Models.Public;
using AutoMapper;
using System.Collections.Specialized;
using System.Web;
using OnlineStore.Providers;
using OnlineStore.Identity;
using OnlineStore.Services;

namespace OnlineStore.Website.Areas.Admin.Controllers
{
    public class ProductsController : AdminController
    {
        public ActionResult Index()
        {
            var model = new ProductSettings();

            var producers = Producers.GetAll();
            var users = UserRoles.GetByRoles(new List<string> { StaticValues.Writer });

            model.Producers = Mapper.Map<List<Public.ViewProducer>>(producers);
            model.Users = Mapper.Map<List<UserShortInfo>>(users);

            return View(model: model);
        }

        [HttpPost]
        public ActionResult DeleteProductVarient(int id)
        {
            var jsonSuccessResult = new JsonSuccessResult();

            try
            {
                ProductVarients.Delete(id);

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

        [HttpPost]
        public ActionResult UpdateProductVarient(int id, bool isEnabled)
        {
            var jsonSuccessResult = new JsonSuccessResult();

            try
            {
                var productVarient = ProductVarients.GetByID(id);

                productVarient.IsEnabled = isEnabled;

                ProductVarients.Update(productVarient);

                Products.UpdateIsUnavailable(id);

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

        [HttpPost]
        public JsonResult Get(int pageIndex,
                              int pageSize,
                              string pageOrder,
                              List<int> groups,
                              string title,
                              int keyword,
                              int producer,
                              int? price,
                              string fromDate,
                              string toDate,
                              string isUnavailable,
                              string isInVisible,
                              bool noPrice,
                              sbyte productStatus
                              )
        {
            DateTime? sDate = null,
                      eDate = null;

            bool? unavailable = null,
                  inVisible = null;

            ProductStatus? status = null;

            string user = null;

            if (!String.IsNullOrWhiteSpace(fromDate))
                sDate = Utilities.ToEnglishDate(fromDate).Date;

            if (!String.IsNullOrWhiteSpace(toDate))
                eDate = Utilities.ToEnglishDate(toDate).Date;

            if (isUnavailable != "-1")
                unavailable = Boolean.Parse(isUnavailable);

            if (isInVisible != "-1")
                inVisible = Boolean.Parse(isInVisible);

            if (productStatus != -1)
                status = (ProductStatus)productStatus;

            if (!base.IsAdmin)
            {
                user = UserID;
            }

            if (pageOrder.Trim() == "ID")
                pageOrder = "LastUpdate desc";

            var list = Products.Get(pageIndex,
                                    pageSize,
                                    pageOrder,
                                    groups,
                                    title,
                                    keyword,
                                    producer,
                                    price,
                                    sDate,
                                    eDate,
                                    unavailable,
                                    inVisible,
                                    noPrice,
                                    status,
                                    user
                                    );

            int total = Products.Count(groups,
                                    title,
                                    keyword,
                                    producer,
                                    price,
                                    sDate,
                                    eDate,
                                    unavailable,
                                    inVisible,
                                    noPrice,
                                    status,
                                    user);

            int totalPage = (int)Math.Ceiling((decimal)total / pageSize);

            if (pageSize > total)
                pageSize = total;

            if (list.Count < pageSize)
                pageSize = list.Count;

            JsonResult result = new JsonResult()
            {
                Data = new
                {
                    TotalPages = totalPage,
                    PageIndex = pageIndex,
                    PageSize = pageSize,
                    Rows = list
                },
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };

            return result;
        }

        public JsonResult GetProducers(List<int> groupIDs)
        {
            var jsonSuccessResult = new JsonSuccessResult();

            try
            {
                jsonSuccessResult.Data = Producers.GetByGroupIDs(groupIDs);
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

        public JsonResult GetAttributes(int productID, List<int> groupIDs)
        {
            var jsonSuccessResult = new JsonSuccessResult();

            try
            {
                var attrs = Attributes.GetByGroupIDs(groupIDs);

                if (productID != -1)
                {
                    foreach (var item in attrs)
                    {
                        item.Value = AttributeValues.GetValue(productID, item.ID);
                    }
                }

                jsonSuccessResult.Data = attrs;
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

        public JsonResult Delete(int id)
        {
            var jsonSuccessResult = new JsonSuccessResult();

            try
            {
                var cartItems = CartItems.CountByProductID(id);
                var gifts = ProductGifts.CountByProductID(id);

                if (cartItems == 0 && gifts == 0)
                {
                    Products.Delete(id);
                    jsonSuccessResult.Success = true;
                }

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

        public ActionResult Edit(int? id)
        {
            EditProduct editProduct;

            if (id.HasValue)
            {
                editProduct = Mapper.Map<EditProduct>(Products.GetByID(id.Value));

                editProduct.Text = HttpUtility.HtmlDecode(editProduct.Text);
                editProduct.GroupUrlPerfix = Groups.GetByID(editProduct.GroupID.Value).UrlPerfix;
                editProduct.Groups = ProductGroups.GetByProductID(editProduct.ID).Select(item => item.GroupID).ToList();
                editProduct.Images = ProductImages.GetByProductID(editProduct.ID);
                editProduct.Files = ProductFiles.GetByProductID(editProduct.ID);
                editProduct.Marks = ProductMarks.GetByProductID(editProduct.ID);
                editProduct.Points = ProductPoints.GetByProductID(editProduct.ID);
                editProduct.Keywords = ProductKeywords.GetByProductID(editProduct.ID);
                editProduct.Notes = ProductNotes.GetByProductID(editProduct.ID);
                editProduct.ProductPricesLinks = ProductPricesLinks.GetByProductID(editProduct.ID);

                editProduct.Supplies = ProductSupplies.GetByProductID(editProduct.ID);
                editProduct.Prices = ProductPrices.GetByProductID(editProduct.ID);
                editProduct.Varients = ProductVarients.GetByProductID(editProduct.ID);

                editProduct.Discounts = ProductDiscounts.GetAllByProductID(editProduct.ID);
            }
            else
            {
                editProduct = new EditProduct();
                editProduct.userID = UserID;
            }

            return View(editProduct);
        }

        [HttpPost]
        public ActionResult Edit(EditProduct editProduct, string score, string chkSendSms)
        {
            try
            {
                float scoreValue = score != "" ? float.Parse(score) : 0;

                var product = Mapper.Map<Product>(editProduct);

                product.LastUpdate = DateTime.Now;
                product.ProductScore = scoreValue;
                ViewBag.Success = true;

                int productID = product.ID;
                if (productID == -1)
                {
                    if (base.IsAdmin)
                        product.ProductStatus = editProduct.ProductStatus;
                    else
                        product.ProductStatus = ProductStatus.NotChecked;

                    product.UserID = base.UserID;
                    product.CreatedDate = DateTime.Now;

                    Products.Insert(product);
                    productID = product.ID;

                    string[] dKeys = GetDefaultKeys(editProduct);

                    SaveGroups(editProduct, productID);
                    SaveImages(editProduct, productID);
                    SaveFiles(editProduct, productID);
                    SaveMarks(editProduct, productID);
                    SavePoints(editProduct, productID);
                    SaveKeywords(editProduct, productID);
                    SaveDefaultKeywords(dKeys, productID);
                    SaveNotes(editProduct, productID, UserID);
                    SaveProductLinks(editProduct, productID);

                    SaveSupplies(editProduct, productID);
                    SavePrices(editProduct, productID);
                    SaveAttributes(Request, productID);
                    SaveVarients(editProduct, productID);

                    UserNotifications.Send(UserID, String.Format("جدید - محصول '{0}'", editProduct.Title), "/Admin/Products/Edit/" + editProduct.ID, NotificationType.Success);
                    editProduct = new EditProduct();
                    editProduct.userID = product.UserID;
                }
                else
                {
                    Products.Update(product);

                    SaveGroups(editProduct, productID);
                    SaveImages(editProduct, productID);
                    SaveFiles(editProduct, productID);
                    SaveMarks(editProduct, productID);
                    SavePoints(editProduct, productID);
                    SaveKeywords(editProduct, productID);
                    SaveNotes(editProduct, productID, UserID);
                    SaveProductLinks(editProduct, productID);

                    SaveSupplies(editProduct, productID);
                    SavePrices(editProduct, productID);
                    SaveAttributes(Request, productID);
                    SaveVarients(editProduct, productID);

                    editProduct.Text = HttpUtility.HtmlDecode(editProduct.Text);

                    editProduct.Groups = ProductGroups.GetByProductID(editProduct.ID).Select(item => item.GroupID).ToList();
                    editProduct.Supplies = ProductSupplies.GetByProductID(editProduct.ID);
                    editProduct.Prices = ProductPrices.GetByProductID(editProduct.ID);
                    editProduct.Varients = ProductVarients.GetByProductID(editProduct.ID);
                    editProduct.Keywords = ProductKeywords.GetByProductID(editProduct.ID);
                    editProduct.Notes = ProductNotes.GetByProductID(editProduct.ID);

                    editProduct.userID = product.UserID;
                    editProduct.GroupUrlPerfix = Groups.GetByID(editProduct.GroupID.Value).UrlPerfix;
                    editProduct.ProductScore = scoreValue;

                    if (chkSendSms == "on")
                    {
                        SendMessageToRequests(editProduct, productID);
                    }
                }

                Products.UpdateIsUnavailable(product.ID);

            }
            catch (Exception ex)
            {
                SetErrors(ex);
            }

            return ClearView(editProduct);
        }

        public JsonResult SearchKeywords(string key)
        {
            var jsonSuccessResult = new JsonSuccessResult();

            try
            {
                var list = Keywords.Search(key);

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

        public JsonResult ReferenceProduct(List<int> productIDs, String userID)
        {
            var jsonSuccessResult = new JsonSuccessResult();

            try
            {
                Products.ReferenceProduct(productIDs, userID);
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

        public ActionResult GetProductVarients(int productID)
        {
            var jsonSuccessResult = new JsonSuccessResult();

            try
            {
                jsonSuccessResult.Data = ProductVarients.GetByProductID(productID);
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

        public JsonResult GetProductRequests(int productID)
        {
            var jsonSuccessResult = new JsonSuccessResult();

            try
            {
                var emails = ProductRequests.GetNewEmails(productID);
                var mobiles = ProductRequests.GetNewMobiles(productID);

                var data = new JsonProductRequest
                {
                    Emails = emails,
                    Mobiles = mobiles
                };

                jsonSuccessResult.Success = true;
                jsonSuccessResult.Data = data;
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

        public JsonResult FilterProducers(List<int> groups)
        {
            var jsonSuccessResult = new JsonSuccessResult();

            try
            {
                if (groups != null && groups.Count > 0)
                {
                    jsonSuccessResult.Data = Producers.GetByGroupIDs(groups);
                }
                else
                {
                    jsonSuccessResult.Data = Producers.GetAll();
                }

                jsonSuccessResult.Success = true;
            }
            catch (Exception ex)
            {
                jsonSuccessResult.Errors = new string[] { ex.Message };
                jsonSuccessResult.Success = false;
            }


            JsonResult result = new JsonResult()
            {
                Data = jsonSuccessResult
            };

            return result;
        }

        #region Methods

        private static void SaveGroups(EditProduct editProduct, int productID)
        {
            var curList = ProductGroups.GetByProductID(productID);

            foreach (var groupID in editProduct.Groups)
            {
                if (!curList.Any(item => item.GroupID == groupID))
                {
                    var productGroup = new ProductGroup();

                    productGroup.ProductID = productID;
                    productGroup.GroupID = groupID;

                    ProductGroups.Insert(productGroup);
                }
                else
                {
                    curList.Remove(curList.Single(cls => cls.GroupID == groupID));
                }
            }

            foreach (var item in curList)
                ProductGroups.Delete(item.ID);
        }

        private static void SaveImages(EditProduct editProduct, int productID)
        {
            var curList = ProductImages.GetByProductID(productID);

            foreach (var image in editProduct.Images)
            {
                if (!curList.Any(item => item.ID == image.ID))
                {
                    var productImage = Mapper.Map<ProductImage>(image);

                    productImage.ProductID = productID;

                    ProductImages.Insert(productImage);
                }
                else
                {
                    ProductImages.UpdateProductImagePlace(image.ID, image.ProductImagePlace);
                    curList.Remove(curList.Single(cls => cls.ID == image.ID));
                }
            }

            foreach (var item in curList)
                ProductImages.Delete(item.ID);
        }

        private static void SaveVarients(EditProduct editProduct, int productID)
        {
            if (editProduct.Varients.Count == 0)
                return;

            var curList = ProductVarients.GetByProductID(productID);

            foreach (var varient in editProduct.Varients)
            {
                if (!curList.Any(item => item.ID == varient.ID))
                {
                    if (varient.Price <= 0)
                        continue;

                    var productVarient = Mapper.Map<ProductVarient>(varient);

                    productVarient.ProductID = productID;
                    productVarient.Title = productVarient.Title;
                    productVarient.PriceCode = productVarient.PriceCode;

                    ProductVarients.Insert(productVarient);

                    foreach (var item in varient.Attributes)
                    {
                        var productVarientAttribute = Mapper.Map<ProductVarientAttribute>(item);

                        productVarientAttribute.ProductVarientID = productVarient.ID;

                        ProductVarientAttributes.Insert(productVarientAttribute);
                    }

                    var varientPrice = new ProductVarientPrice();

                    varientPrice.ProductVarientID = productVarient.ID;
                    varientPrice.Count = varient.Count;
                    varientPrice.Price = varient.Price * (ExtensionMethods.IsRial ? 1 : 10);
                    varientPrice.PriceType = varient.PriceType;

                    ProductVarientPrices.Insert(varientPrice);
                }
                else
                {
                    curList.Remove(curList.Single(cls => cls.ID == varient.ID));

                    if (varient.Price <= 0)
                        continue;

                    var varientPrice = new ProductVarientPrice();

                    varientPrice.ProductVarientID = varient.ID;
                    varientPrice.Count = varient.Count;
                    varientPrice.Price = varient.Price * (ExtensionMethods.IsRial ? 1 : 10);
                    varientPrice.PriceType = varient.PriceType;

                    ProductVarientPrices.Insert(varientPrice);
                }
            }

            foreach (var item in curList)
                ProductVarients.DeleteWithAttributes(item.ID);
        }

        private static void SaveFiles(EditProduct editProduct, int productID)
        {
            var curList = ProductFiles.GetByProductID(productID);

            foreach (var file in editProduct.Files)
            {
                if (!curList.Any(item => item.ID == file.ID))
                {
                    var productFile = Mapper.Map<ProductFile>(file);

                    productFile.ProductID = productID;

                    ProductFiles.Insert(productFile);
                }
                else
                {
                    ProductFiles.UpdateTitle(file.ID, file.Title);
                    curList.Remove(curList.Single(cls => cls.ID == file.ID));
                }
            }

            foreach (var item in curList)
                ProductFiles.Delete(item.ID);
        }

        private static void SaveMarks(EditProduct editProduct, int productID)
        {
            var curList = ProductMarks.GetByProductID(productID);

            foreach (var mark in editProduct.Marks)
            {
                if (!curList.Any(item => item.ID == mark.ID))
                {
                    var productMark = Mapper.Map<ProductMark>(mark);

                    productMark.ProductID = productID;

                    ProductMarks.Insert(productMark);
                }
                else
                {
                    ProductMarks.Update(mark);
                    curList.Remove(curList.Single(cls => cls.ID == mark.ID));
                }
            }

            foreach (var item in curList)
                ProductMarks.Delete(item.ID);
        }

        private static void SavePoints(EditProduct editProduct, int productID)
        {
            var curList = ProductPoints.GetByProductID(productID);

            foreach (var point in editProduct.Points)
            {
                if (!curList.Any(item => item.ID == point.ID))
                {
                    var productPoint = Mapper.Map<ProductPoint>(point);

                    productPoint.ProductID = productID;

                    ProductPoints.Insert(productPoint);
                }
                else
                {
                    ProductPoints.Update(point);
                    curList.Remove(curList.Single(cls => cls.ID == point.ID));
                }
            }

            foreach (var item in curList)
                ProductPoints.Delete(item.ID);
        }

        private static void SaveKeywords(EditProduct editProduct, int productID)
        {
            var curList = ProductKeywords.GetByProductID(productID);

            foreach (var key in editProduct.Keywords)
            {
                if (key.KeywordID == -1)
                {
                    key.KeywordID = InsertKeyword(key.Title);
                }

                if (!curList.Any(item => item.ID == key.ID))
                {
                    var rpt = IsRepeatKey(key.KeywordID, productID);

                    if (!rpt)
                    {
                        var productKeyword = Mapper.Map<ProductKeyword>(key);

                        productKeyword.ProductID = productID;

                        ProductKeywords.Insert(productKeyword);
                    }
                }
                else
                {
                    ProductKeywords.Update(key);
                    curList.Remove(curList.Single(cls => cls.ID == key.ID));
                }
            }

            foreach (var item in curList)
                ProductKeywords.Delete(item.ID);
        }

        private static void SaveDefaultKeywords(string[] keywords, int productID)
        {
            List<ProductKeyword> keys = new List<ProductKeyword>();

            foreach (var key in keywords)
            {
                if (key != null && key != String.Empty)
                {
                    var orgKey = Keywords.GetByTitle(key);

                    if (orgKey != null)
                    {
                        if (orgKey.IsActive)
                        {
                            var pk = new ProductKeyword
                            {
                                KeywordID = orgKey.ID,
                                ProductID = productID,
                                LastUpdate = DateTime.Now
                            };

                            keys.Add(pk);
                        }
                    }
                    else
                    {
                        int keyID = InsertKeyword(key);

                        var pk = new ProductKeyword
                        {
                            KeywordID = keyID,
                            ProductID = productID,
                            LastUpdate = DateTime.Now
                        };

                        keys.Add(pk);
                    }
                }
            }

            if (keys.Count > 0)
            {
                ProductKeywords.Insert(keys);
            }
        }

        private static string[] GetDefaultKeys(EditProduct editProduct)
        {
            var producer = Producers.GetByID(editProduct.ProducerID);
            List<Group> grps = Groups.GetByIDs(editProduct.Groups);

            var titleFa = editProduct.Title.Split(' ');
            var titleEn = editProduct.Title_En.Split(' ');
            var producerFa = producer.Title.Split(' ');
            var producerEn = producer.TitleEn.Split(' ');
            //var groupFa = grps.Select(item => item.Title).ToArray();
            //var groupEn = grps.Select(item => item.TitleEn).ToArray();

            string[] defaultKeys = titleFa.Union(titleEn)
                                          .Union(producerFa)
                                          .Union(producerEn)
                                          .ToArray<string>();

            return defaultKeys;
        }

        private static bool IsRepeatKey(int keywordID, int productID)
        {
            var key = ProductKeywords.GetByKeywordID_ProductID(keywordID, productID);

            if (key != null)
            {
                return true;
            }

            return false;
        }

        private static void SaveNotes(EditProduct editProduct, int productID, string userID)
        {
            var curList = ProductNotes.GetByProductID(productID);

            foreach (var note in editProduct.Notes)
            {
                if (!curList.Any(item => item.ID == note.ID))
                {
                    var productNote = Mapper.Map<ProductNote>(note);

                    productNote.UserID = userID;
                    productNote.ProductID = productID;
                    productNote.LastUpdate = DateTime.Now;

                    ProductNotes.Insert(productNote);
                }
            }
        }

        private static int InsertKeyword(string keyTitle)
        {
            var keyword = new Keyword
            {
                Title = keyTitle,
                LastUpdate = DateTime.Now,
                IsActive = true
            };

            Keywords.Insert(keyword);

            return keyword.ID;
        }

        private static void SaveSupplies(EditProduct editProduct, int productID)
        {
            if (editProduct.Supply.Count != 0)
            {
                var productSupply = new ProductSupply();

                productSupply.ProductID = productID;
                productSupply.SupplyType = SupplyType.AdminEdit;
                productSupply.Count = editProduct.Supply.Count;
                productSupply.Description = editProduct.Supply.Description;

                ProductSupplies.Insert(productSupply);
            }
        }

        private static void SavePrices(EditProduct editProduct, int productID)
        {
            if (editProduct.Price.Price != 0)
            {
                var productPrice = new ProductPrice();

                productPrice.ProductID = productID;
                productPrice.PriceType = editProduct.Price.PriceType;
                productPrice.Price = editProduct.Price.Price * (ExtensionMethods.IsRial ? 1 : 10);
                productPrice.Description = editProduct.Price.Description;

                ProductPrices.Insert(productPrice);
            }
        }

        private void SaveAttributes(HttpRequestBase req, int productID)
        {
            var exists = false;
            foreach (string key in req.Form)
            {
                if (key.StartsWith("attr_"))
                {
                    exists = true;
                    break;
                }
            }

            if (!exists)
                return;

            AttributeValues.DeleteBy(productID);

            var attrValues = new List<AttributeValue>();
            foreach (string key in req.Form)
            {
                if (key.StartsWith("attr_"))
                {
                    var id = int.Parse(key.Replace("attr_", String.Empty));
                    var value = req.Form[key];

                    var attr = Attributes.GetByID(id);
                    var attrValue = new AttributeValue();

                    attrValue.ProductID = productID;
                    attrValue.AttributeID = attr.ID;

                    attrValues.Add(attrValue);

                    switch (attr.AttributeType)
                    {
                        case AttributeType.Text:
                            attrValue.Value = value;
                            break;
                        case AttributeType.Number:
                            attrValue.Value = value;
                            break;
                        case AttributeType.SingleItem:
                            if (!String.IsNullOrWhiteSpace(value))
                                attrValue.AttributeOptionID = int.Parse(value);
                            break;
                        case AttributeType.MultipleItem:
                            attrValues.Remove(attrValues.Last());

                            var selecteds = value.Split(',');
                            foreach (var item in selecteds)
                            {
                                var atv = (AttributeValue)attrValue.Clone();
                                atv.AttributeOptionID = int.Parse(item);

                                attrValues.Add(atv);
                            }

                            break;
                        case AttributeType.Check:
                            //attrValue.Value = bool.Parse(value).ToString();
                            break;
                        case AttributeType.MultilineText:
                            attrValue.Value = value;
                            break;
                        default:
                            break;
                    }
                }
            }

            foreach (var item in attrValues)
                AttributeValues.Insert(item);
        }

        private static void SaveProductLinks(EditProduct editProduct, int productID)
        {
            var curList = ProductPricesLinks.GetByProductID(productID);

            foreach (var link in editProduct.ProductPricesLinks)
            {
                if (!curList.Any(item => item.ID == link.ID))
                {
                    var productLink = Mapper.Map<ProductPricesLink>(link);

                    productLink.ProductID = productID;

                    ProductPricesLinks.Insert(productLink);
                }
                else
                {
                    ProductPricesLinks.Update(link);
                    curList.Remove(curList.Single(cls => cls.ID == link.ID));
                }
            }

            foreach (var item in curList)
                ProductPricesLinks.Delete(item.ID);
        }

        private static void SendMessageToRequests(EditProduct editProduct, int productID)
        {
            try
            {
                #region Send Message

                var emails = ProductRequests.GetNewEmails(productID).ToArray();
                var mobiles = ProductRequests.GetNewMobiles(productID).ToArray();

                EmailServices.ProductAvailable(editProduct.Title, emails);
                SMSServices.ProductAvailable(editProduct.Title, String.Join(",", mobiles));

                #endregion Send Message

                ProductRequests.UpdateStatus(emails, mobiles);
            }
            catch (Exception ex) { }
        }

        #endregion Methods
    }
}