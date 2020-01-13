using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OnlineStore.EntityFramework;
using OnlineStore.Models.Admin;
using OnlineStore.Models.Enums;
using System.Text.RegularExpressions;
using OnlineStore.Providers;
using OnlineStore.Models.Public;

namespace OnlineStore.DataLayer
{
    public class AttributeValue : EntityBase, ICloneable
    {
        [ForeignKey("Product")]
        public int ProductID { get; set; }
        public Product Product { get; set; }

        [ForeignKey("Attribute")]
        public int AttributeID { get; set; }
        public Attribute Attribute { get; set; }

        [ForeignKey("AttributeOption")]
        public int? AttributeOptionID { get; set; }
        public AttributeOption AttributeOption { get; set; }

        public string Value { get; set; }

        public object Clone()
        {
            return new AttributeValue()
            {
                ProductID = this.ProductID,
                AttributeID = this.AttributeID,
                AttributeOptionID = this.AttributeOptionID,
                Value = this.Value
            };
        }
    }

    public static class AttributeValues
    {
        public static IList Get(int pageIndex, int pageSize, string pageOrder)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var query = from item in db.AttributeValues
                            select new
                            {
                                item.ID,
                                item.LastUpdate,
                            };

                if (!string.IsNullOrWhiteSpace(pageOrder))
                    query = query.OrderBy(pageOrder);

                query = query.Skip(pageIndex * pageSize).Take(pageSize);

                return query.ToList();
            }
        }

        public static int Count()
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var query = from item in db.AttributeValues
                            select item;

                return query.Count();
            }
        }

        public static AttributeValue GetByID(int id)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var attribute = db.AttributeValues.Where(item => item.ID == id).Single();

                return attribute;
            }
        }

        public static object GetValue(int productID, int attributeID)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                object value = null;

                var attr = Attributes.GetByID(attributeID);
                var attrValues = db.AttributeValues.Where(item => item.AttributeID == attributeID && item.ProductID == productID).ToList();

                if (attrValues.Count > 0)
                {
                    switch (attr.AttributeType)
                    {
                        case Models.Enums.AttributeType.Text:
                            value = attrValues[0].Value;
                            break;
                        case Models.Enums.AttributeType.Number:
                            value = attrValues[0].Value;
                            break;
                        case Models.Enums.AttributeType.SingleItem:
                            value = attrValues[0].AttributeOptionID;
                            break;
                        case Models.Enums.AttributeType.MultipleItem:
                            value = attrValues.Select(item => item.AttributeOptionID).ToList();
                            break;
                        case Models.Enums.AttributeType.Check:
                            value = true;
                            break;
                        case Models.Enums.AttributeType.MultilineText:
                            value = attrValues[0].Value;
                            break;
                        default:
                            break;
                    }
                }
                else if (attr.AttributeType == Models.Enums.AttributeType.Check)
                    value = false;

                return value;
            }
        }

        public static void Delete(int id)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var attribute = (from item in db.AttributeValues
                                 where item.ID == id
                                 select item).Single();

                db.AttributeValues.Remove(attribute);

                db.SaveChanges();
            }
        }

        public static void DeleteBy(int productID)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var attributes = (from item in db.AttributeValues
                                  where item.ProductID == productID
                                  select item).ToList();

                db.AttributeValues.RemoveRange(attributes);

                db.SaveChanges();
            }
        }

        public static void Insert(AttributeValue attribute)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                db.AttributeValues.Add(attribute);

                db.SaveChanges();
            }
        }

        public static void Update(AttributeValue attribute)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var orgAttributeValue = db.AttributeValues.Where(item => item.ID == attribute.ID).Single();

                orgAttributeValue.ProductID = attribute.ProductID;
                orgAttributeValue.AttributeID = attribute.AttributeID;
                orgAttributeValue.AttributeOptionID = attribute.AttributeOptionID;
                orgAttributeValue.Value = attribute.Value;
                orgAttributeValue.LastUpdate = attribute.LastUpdate;

                db.SaveChanges();
            }
        }

        public static string RenderValue(ViewAttribute item)
        {
            string result = String.Empty;

            var kvRegex = new Regex(@"^(\d+):(.+)");
            Match valueMatch = null;

            if (item.AttributeType == AttributeType.Text ||
                item.AttributeType == AttributeType.Number)
                valueMatch = kvRegex.Match(item.Value.ToString());

            switch (item.AttributeType)
            {
                case AttributeType.Text:
                    if (valueMatch.Success)
                    {
                        var key = int.Parse(valueMatch.Groups[1].Value);
                        var value = valueMatch.Groups[2].Value;
                        var list = new Dictionary<int, string>();

                        foreach (var postfixItem in item.Posfix.Split(',').Select(p => p.Trim()))
                        {
                            var pfMatch = kvRegex.Match(postfixItem);

                            list.Add(int.Parse(pfMatch.Groups[1].Value), pfMatch.Groups[2].Value);
                        }

                        result = value;
                        item.Posfix = list[key];
                    }
                    else
                    {
                        result = item.Value.ToString();
                    }

                    break;
                case AttributeType.Number:
                    if (valueMatch.Success)
                    {
                        var key = int.Parse(valueMatch.Groups[1].Value);
                        var value = float.Parse(valueMatch.Groups[2].Value);
                        var list = new Dictionary<int, string>();

                        foreach (var postfixItem in item.Posfix.Split(',').Select(p => p.Trim()))
                        {
                            var pfMatch = kvRegex.Match(postfixItem);

                            list.Add(int.Parse(pfMatch.Groups[1].Value), pfMatch.Groups[2].Value);
                        }

                        result = Math.Round(value, 1).ToString();
                        item.Posfix = list[key];
                    }
                    else
                    {
                        result = item.Value.ToString();
                    }

                    break;
                case AttributeType.SingleItem:
                    result = item.Options.FirstOrDefault(op => op.ID == (int)item.Value).Title;
                    if (result == "ندارد")
                        result = "<i class='fa fa-times'></i>";
                    break;
                case AttributeType.MultipleItem:
                    var options = item.Options.Where(op => ((IList)item.Value).Contains(op.ID));

                    if (options.Count() > 0)
                    {
                        foreach (var op in options)
                        {
                            result += op.Title + " - ";
                        }

                        result = result.Remove(result.Length - 2, 1);
                    }
                    break;
                case AttributeType.Check:
                    if ((bool)item.Value)
                    {
                        result = "<i class='fa fa-check'></i>";
                    }
                    else
                    {
                        result = "<i class='fa fa-times'></i>";
                    }
                    break;
                case AttributeType.MultilineText:
                    result = item.Value.ToString();

                    if (!Utilities.ContainsUnicodeCharacter(result))
                        result = "<span dir='ltr'>" + result + "</span>";

                    result = result.Replace("\n", "<br />");
                    break;
                default:
                    result = "نا معلوم";
                    break;
            }

            if (!OnlineStore.Providers.Utilities.ContainsUnicodeCharacter(result))
                result = "<span dir='ltr'>" + result + "</span>";

            return result;
        }

        public static List<AttributeValue> GetByProductID(int productID)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var query = db.AttributeValues.Where(item => item.ProductID == productID);

                return query.ToList();
            }
        }

        public static List<GroupOption> GetOptionIDsByGroup()
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var now = DateTime.Now;

                var query = from item in db.AttributeValues
                            where item.Attribute.IsSearchable &&
                                  now >= item.Product.PublishDate &&
                                  item.Product.IsInVisible == false &&
                                  item.Product.ProductStatus == ProductStatus.Approved
                            group item by item.Product.GroupID into g
                            select new GroupOption
                            {
                                GroupID = g.Key.Value,
                                Options = g.Where(av => av.AttributeOptionID.HasValue).GroupBy(av => av.AttributeOptionID).Select(av => av.Key.Value).ToList()
                            };

                return query.ToList();
            }
        }

    }
}
