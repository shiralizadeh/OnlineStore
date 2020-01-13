using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineStore.Models.Enums
{
    public enum HomeBoxType
    {
        [Display(Name = "بخش افقی")]
        Horizontal = 1,

        [Display(Name = "بخش عمودی")]
        Vertical = 2,

        [Display(Name = "بخش گروه ها")]
        Group = 3,

    }
}
