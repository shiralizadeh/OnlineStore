using OnlineStore.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineStore.Models.Public
{
    public class PriceSettings
    {
        public PriceSettings()
        {
            IsUnavailable = false;
        }

        public bool SimplePrice { get; set; }
        public PriceStatus PriceStatus { get; set; }
        public List<PriceItem> Prices { get; set; }
        public bool IsUnavailable { get; set; }
        public bool IsFreeDelivery { get; set; }
    }
}
