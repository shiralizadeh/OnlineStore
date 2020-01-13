using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OnlineStore.Models.Enums;
using Newtonsoft.Json;

namespace OnlineStore.Models.Public
{
    public class JsonProductVarient : PriceItem
    {
        [JsonIgnore]
        public int PriceID { get; set; }
        public List<JsonVarientAttribute> Attributes { get; set; }
        public PriceType PriceType { get; set; }
        public DateTime LastUpdate { get; set; }
    }

    public class JsonVarientAttribute
    {
        public int AttributeID { get; set; }
        public int AttributeOptionID { get; set; }
        public string AttributeOptionTitle { get; set; }
    }
}
