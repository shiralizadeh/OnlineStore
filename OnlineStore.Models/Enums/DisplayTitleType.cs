using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineStore.Models.Enums
{
    public enum DisplayTitleType : Byte
    {
        [Display(Name = "عنوان فارسی")]
        Title_Fa = 1,

        [Display(Name = "عنوان انگلیسی")]
        Title_En = 2
    }
}
