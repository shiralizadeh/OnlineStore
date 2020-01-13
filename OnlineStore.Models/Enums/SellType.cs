using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineStore.Models.Enums
{
    public enum SellType : byte
    {
        [Display(Name = "فروش فیزیکی")]
        Physical = 0,

        [Display(Name = "فروش دانلودی")]
        Downloadable = 1,
    }
}
