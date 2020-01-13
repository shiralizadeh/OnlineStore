using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineStore.Models.Enums
{
    public enum MenuItemType : byte
    {
        [Display(Name = "بدون لینک")]
        Normal = 0,
        [Display(Name = "لینک")]
        Link = 1,
        [Display(Name = "صفحه داخلی")]
        Page = 2
    }
}
