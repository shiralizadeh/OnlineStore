using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineStore.Models.Enums
{
    public enum DiscountType : byte
    {
        [Display(Name = "درصد تخفیف")]
        Percent = 0,

        [Display(Name = "قیمت بعد از تخفیف")]
        PriceAfter = 1,

        [Display(Name = "قیمت قبل از تخفیف")]
        PriceBefore = 2,

        [Display(Name = "قیمت قبل و بعد از تخفیف")]
        PriceBeforeAfter = 3
    }
}
