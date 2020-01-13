using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OnlineStore.Models.User;
using OnlineStore.Models.Enums;

namespace OnlineStore.Models.Public
{
    public class FactorSettings
    {
        public bool IsSuccess { get; set; }
        public Factor FactorInfo { get; set; }
        public ViewBuyerInfo BuyerInfo { get; set; }
        public List<ViewOrderItem> CartItems { get; set; }
        public string CompanyLogo { get; set; }
        public string CompanyWebsite { get; set; }
        public string CompanyEmail { get; set; }
        public string CompanyPhone { get; set; }
        public string CompanyName { get; set; }
    }
}
