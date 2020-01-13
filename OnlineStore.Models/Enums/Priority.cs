using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineStore.Models.Enums
{
    public enum Priority : byte
    {
        [Display(Name ="کم")]
        Low = 0,

        [Display(Name = "متوسط")]
        Medium = 1,

        [Display(Name = "زیاد")]
        High = 2
    }
}
