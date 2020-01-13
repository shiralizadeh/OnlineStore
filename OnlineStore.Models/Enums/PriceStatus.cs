using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineStore.Models.Enums
{
    public enum PriceStatus : byte
    {
        [Display(Name = "معمولی")]
        Normal = 0,

        [Display(Name = "به زودی")]
        ComingSoon = 1,

        [Display(Name = "تماس بگیرید")]
        ContactUs = 2,
    }
}
