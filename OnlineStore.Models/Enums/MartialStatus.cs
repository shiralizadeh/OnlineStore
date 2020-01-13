using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineStore.Models.Enum
{
    public enum MartialStatus : byte
    {
        [Display(Name = "مشمول")]
        Inductee = 0,
        [Display(Name = "معاف")]
        Exempt = 1,
        [Display(Name = "دارای کارت پایان خدمت")]
        Done = 2,
    }
}
