using OnlineStore.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineStore.Models.Admin
{
    public class JsonSimpleProduct
    {
        public int ID { get; set; }
        public string Title { get; set; }
        public string ImageFile { get; set; }
        public int OrderID { get; set; }
        public int Weight { get; set; }
        public PriceStatus PriceStatus { get; set; }
        public bool IsUnavailable { get; set; }
        public bool HasVarients { get; set; }

        public List<JsonShortProductVarient> Varients { get; set; }

        public DateTime? CreatedDate { get; set; }
        public DateTime LastUpdate { get; set; }
        public DateTime LastPriceUpdate { get; set; }
    }
}
