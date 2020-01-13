using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineStore.Models.Enums
{
    public enum PriceType
    {
        [Display(Name = "خرید")]
        Buy = 0,

        [Display(Name = "فروش")]
        Sell = 1
    }
}
