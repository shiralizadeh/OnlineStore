using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineStore.Models.Enums
{
    public enum PriceListSectionType : byte
    {
        [Display(Name = "صفحه اول")]
        Page_01 = 0,
        [Display(Name = "صفحه دوم")]
        Page_02 = 1,
    }
}
