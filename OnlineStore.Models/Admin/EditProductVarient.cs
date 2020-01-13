using OnlineStore.Models.Enums;
using OnlineStore.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineStore.Models.Admin
{
    public class EditProductVarient
    {
        public EditProductVarient()
        {
            PriceType = PriceType.Sell;
            IsEnabled = true;
        }

        public int ID { get; set; }

        public bool IsEnabled { get; set; }

        public string Title { get; set; }

        public string PriceCode { get; set; }

        public int Price { get; set; }

        public int Count { get; set; }

        public string PriceTypeTitle
        {
            get
            {
                return Utilities.GetEnumDisplay(PriceType);
            }
        }

        public PriceType PriceType { get; set; }

        public DateTime? LastUpdate { get; set; }

        public List<EditProductVarientAttribute> Attributes { get; set; }
    }

    public class EditProductVarientAttribute
    {
        public int AttributeID { get; set; }
        public string AttributeOptionTitle { get; set; }
        public int? AttributeOptionID { get; set; }
    }
}
