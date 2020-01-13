using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineStore.Models.Enums
{
    public enum MenuItemBannerType : byte
    {
        [Display(Name = "بنر اصلی")]
        Type1 = 0,
        [Display(Name = "بنر بالا - چپ")]
        Type2 = 1,
        [Display(Name = "بنر پایین - چپ")]
        Type3 = 2,
        [Display(Name = "بنر پایین - وسط")]
        Type4 = 3,
        [Display(Name = "بنر پایین - راست")]
        Type5 = 4,
    }
}
