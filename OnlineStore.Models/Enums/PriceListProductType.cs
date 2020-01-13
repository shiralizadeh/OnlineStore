using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineStore.Models.Enums
{
    public enum PriceListProductType : byte
    {
        [Display(Name = "مشکی")]
        Type1 = 0,
        [Display(Name = "قرمز")]
        Type2 = 1
    }
}
