using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineStore.Models.Enums
{
    public enum PriceListFieldName : byte
    {
        [Display(Name = "عنوان")]
        Title = 0,
        [Display(Name = "زیر عنوان")]
        SubTitle = 1,
        [Display(Name = "قیمت")]
        Price = 2,
        [Display(Name = "موجودی")]
        IsAvailable = 3
    }
}
