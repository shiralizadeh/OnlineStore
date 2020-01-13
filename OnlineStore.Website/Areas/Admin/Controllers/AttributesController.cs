using OnlineStore.DataLayer;
using OnlineStore.Models;
using OnlineStore.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OnlineStore.Providers.Controllers;
using OnlineStore.Models.Enums;
using Attribute = OnlineStore.DataLayer.Attribute;
using AutoMapper;
using OnlineStore.Models.Admin;

namespace OnlineStore.Website.Areas.Admin.Controllers
{
    public class AttributesController : AdminController
    {
        public ActionResult Index()
        {
            var groups = AttrGroups.GetAll();

            return View(model: groups);
        }

        [HttpPost]
        public JsonResult Get(int pageIndex, int pageSize, string pageOrder, string title, int attrGroupID, List<int> groups, sbyte attributeType)
        {
            if (pageOrder.Trim() == "ID")
                pageOrder = "OrderID";

            AttributeType? type = null;

            if (attributeType != -1)
            {
                type = (AttributeType)attributeType;
            }

            var list = Attributes.Get(pageIndex, pageSize, pageOrder, title, attrGroupID, groups, type);

            int total = Attributes.Count(title, attrGroupID, groups, type);
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

        public JsonResult GetAttrGroups(List<int> groupIDs)
        {
            var jsonSuccessResult = new JsonSuccessResult();

            try
            {
                jsonSuccessResult.Data = AttrGroups.GetByGroupIDs(groupIDs);
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

        public JsonResult GetDependentAttr(List<int> groupIDs)
        {
            var jsonSuccessResult = new JsonSuccessResult();

            try
            {
                jsonSuccessResult.Data = Attributes.GetDependentByGroupIDs(groupIDs);
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

        public JsonResult GetAttrOptions(int attrID)
        {
            var jsonSuccessResult = new JsonSuccessResult();

            try
            {
                var attrType = Attributes.GetByID(attrID).AttributeType;

                object model;

                if (attrType == AttributeType.SingleItem || attrType == AttributeType.MultipleItem)
                {
                    model = new
                    {
                        Options = AttributeOptions.GetByAttributeID(attrID),
                        HasItems = true
                    };

                    jsonSuccessResult.Data = model;
                }
                else
                {
                    model = new
                    {
                        HasItems = false
                    };

                    jsonSuccessResult.Data = model;
                }

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
                Attributes.Delete(id);
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

        public ActionResult Edit(int? id)
        {
            EditAttribute editAttribute;

            if (id.HasValue)
            {
                editAttribute = Mapper.Map<EditAttribute>(Attributes.GetByID(id.Value));
                editAttribute.Groups = AttributeGroups.GetByAttributeID(editAttribute.ID).Select(item => item.GroupID).ToList();
            }
            else
                editAttribute = new EditAttribute();

            return View(editAttribute);
        }

        [HttpPost]
        public ActionResult Edit(EditAttribute editAttribute)
        {
            try
            {
                var attribute = Mapper.Map<Attribute>(editAttribute);

                attribute.LastUpdate = DateTime.Now;

                ViewBag.Success = true;

                int attributeID = attribute.ID;
                if (attributeID == -1)
                {
                    Attributes.Insert(attribute);
                    attributeID = attribute.ID;

                    SaveGroups(editAttribute, attribute.ID);
                    SaveOptions(editAttribute, attribute.ID);

                    UserNotifications.Send(UserID, String.Format("جدید - ویژگی '{0}'", attribute.Title), "/Admin/Attributes/Edit/" + attribute.ID, NotificationType.Success);
                    editAttribute = new EditAttribute();
                }
                else
                {
                    Attributes.Update(attribute);

                    SaveGroups(editAttribute, attribute.ID);
                    SaveOptions(editAttribute, attribute.ID);

                    editAttribute.Groups = AttributeGroups.GetByAttributeID(editAttribute.ID).Select(item => item.GroupID).ToList();
                    editAttribute.Options = AttributeOptions.GetByAttributeID(editAttribute.ID).Select(item => new EditAttributeOption() { ID = item.ID, AttributeID = item.AttributeID, Title = item.Title }).ToList();
                }
            }
            catch (Exception ex)
            {
                SetErrors(ex);
            }

            return ClearView(editAttribute);
        }

        private static void SaveGroups(EditAttribute editAttribute, int attributeID)
        {
            var curList = AttributeGroups.GetByAttributeID(attributeID);

            foreach (var groupID in editAttribute.Groups)
            {
                if (!curList.Any(item => item.GroupID == groupID))
                {
                    var attributeGroup = new AttributeGroup();

                    attributeGroup.AttributeID = attributeID;
                    attributeGroup.GroupID = groupID;

                    AttributeGroups.Insert(attributeGroup);
                }
                else
                {
                    curList.Remove(curList.Single(cls => cls.GroupID == groupID));
                }
            }

            foreach (var item in curList)
                AttributeGroups.Delete(item.ID);
        }

        private static void SaveOptions(EditAttribute editAttribute, int attributeID)
        {
            var curList = AttributeOptions.GetByAttributeID(attributeID);

            foreach (var option in editAttribute.Options)
            {
                if (!curList.Any(item => item.ID == option.ID))
                {
                    var attributeOption = new AttributeOption();

                    attributeOption.AttributeID = attributeID;
                    attributeOption.Title = option.Title;

                    AttributeOptions.Insert(attributeOption);
                }
                else
                {
                    var item = curList.Single(cls => cls.ID == option.ID);

                    curList.Remove(item);

                    item.Title = option.Title;
                    item.OrderID = option.OrderID;

                    AttributeOptions.Update(item);
                }
            }

            foreach (var item in curList)
                AttributeOptions.Delete(item.ID);
        }
    }
}