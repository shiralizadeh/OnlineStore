using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineStore.Models.Enums
{
    public enum AttributeType : byte
    {
        [Display(Name = "متن")]
        Text = 0,

        [Display(Name = "عدد")]
        Number = 1,

        [Display(Name = "تک انتخابی")]
        SingleItem = 2,

        [Display(Name = "چند انتخابی")]
        MultipleItem = 3,

        [Display(Name = "بله / خیر")]
        Check = 4,

        [Display(Name = "متن چند خطه")]
        MultilineText = 5,
    }
}
