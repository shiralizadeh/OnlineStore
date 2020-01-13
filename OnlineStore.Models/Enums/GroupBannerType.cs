using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineStore.Models.Enums
{
    public enum GroupBannerType : byte
    {
        [Display(Name = "بنر اصلی")]
        Type1 = 0,
    }
}
