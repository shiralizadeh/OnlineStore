using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineStore.Models.Enums
{
    public enum GiftCardType : byte
    {
        [Display(Name = "درصد")]
        Percent = 0,
        [Display(Name = "قیمت")]
        Price = 1,
        [Display(Name = "تعداد و درصد")]
        Count_Percent = 2,
        [Display(Name = "تعداد و قیمت")]
        Count_Price = 3,
        [Display(Name = "حداقل قیمت خرید و درصد")]
        Price_Percent = 4,
        [Display(Name = "حداقل قیمت خرید و قیمت")]
        Price_Price = 5
    }
}
