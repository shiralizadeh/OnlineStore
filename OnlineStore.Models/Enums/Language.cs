using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineStore.Models.Enums
{
    public enum Language : byte
    {
        [Display(Name = "انگلیسی")]
        English = 0,
        [Display(Name = "فرانسوی")]
        French = 1,
        [Display(Name = "آلمانی")]
        German = 3,
        [Display(Name = "هیچکدام")]
        Nothing = 4

    }
}
