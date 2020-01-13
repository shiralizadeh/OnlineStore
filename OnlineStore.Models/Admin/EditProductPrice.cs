using OnlineStore.Models.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineStore.Models.Admin
{
    public class EditProductPrice
    {
        public EditProductPrice()
        {
            PriceType = PriceType.Sell;
        }

        [Display(Name = "قیمت")]
        public int Price { get; set; }

        [Display(Name = "نوع")]
        public PriceType PriceType { get; set; }

        [Display(Name = "توضیحات")]
        public string Description { get; set; }

        public DateTime LastUpdate { get; set; }
    }
}
