using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineStore.Models.Enums
{
    public enum ProductPointType : byte
    {
        [Display(Name = "قوت")]
        Strength = 1,

        [Display(Name = "ضعف")]
        Weakness = 2
    }
}
