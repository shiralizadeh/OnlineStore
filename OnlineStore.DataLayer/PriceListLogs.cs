using OnlineStore.Models.Admin;
using OnlineStore.Models.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineStore.DataLayer
{
    public class PriceListLog : EntityBase
    {
        public string OldValue { get; set; }
        public string NewValue { get; set; }
        public PriceListFieldName PriceListFieldName { get; set; }

        [ForeignKey("PriceListProduct")]
        public int PriceListProductID { get; set; }
        public PriceListProduct PriceListProduct { get; set; }
    }

    public static class PriceListLogs
    {
        public static void Inset(PriceListLog priceListLog)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                db.PriceListLogs.Add(priceListLog);

                db.SaveChanges();
            }
        }

        public static List<JsonPriceListLogGroup> Get(DateTime? fromDate, DateTime? toDate)
        {
            using (var db = OnlineStoreDbContext.Entity)
            {
                var query = from item in db.PriceListLogs
                            where ((fromDate.HasValue && item.LastUpdate >= fromDate) || !fromDate.HasValue) &&
                                  ((toDate.HasValue && item.LastUpdate <= toDate) || !toDate.HasValue)
                            group item by new { item.PriceListProductID, item.PriceListProduct.Title } into logs
                            select new JsonPriceListLogGroup
                            {
                                ProductTitle = logs.Key.Title,
                                PriceListLogs = (from l in logs
                                                 select new JsonPriceListLog
                                                 {
                                                     NewValue = l.NewValue,
                                                     OldValue = l.OldValue,
                                                     LastUpdate = l.LastUpdate,
                                                     PriceListField = l.PriceListFieldName
                                                 }).ToList()
                            };

                return query.ToList();
            }
        }
    }
}
