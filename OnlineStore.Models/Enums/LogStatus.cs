using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineStore.Models.Enums
{
    public enum LogType : byte
    {
        [Display(Name = "خطا")]
        Error = 1,

        [Display(Name = "هشدار")]
        Warning = 2,

        [Display(Name = "اطلاعات")]
        Info = 3,

    }
}
