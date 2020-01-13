using OnlineStore.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineStore.Models.Admin
{
    public class JsonPriceListLogGroup
    {
        public string ProductTitle { get; set; }
        public List<JsonPriceListLog> PriceListLogs { get; set; }

    }

    public class JsonPriceListLog
    {
        public string OldValue { get; set; }
        public string NewValue { get; set; }
        public PriceListFieldName PriceListField { get; set; }
        public string PriceListFieldName { get; set; }
        public string ColorClass { get; set; }
        public DateTime LastUpdate { get; set; }
    }
}
