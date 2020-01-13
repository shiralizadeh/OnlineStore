using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace OnlineStore.Models.Enums
{
    public enum SendMethodType : byte
    {
        [Display(Name = "پیک درون شهری فقط مشهد مقدس")]
        Free = 0,

        [Display(Name = "پست پیشتاز")]
        Post = 1,

        [Display(Name = "تیپاکس")]
        Tipax = 2,
    }
}
