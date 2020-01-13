using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineStore.Models.Enums
{
    public enum SupplyType : byte
    {
        [Display(Name = "ناشی از خرید")]
        Buy = 0,

        [Display(Name = "ناشی از ویرایش مدیر")]
        AdminEdit = 1
    }
}
