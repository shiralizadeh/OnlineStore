using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineStore.Models.Enums
{
    public enum SliderType : byte
    {
        [Display(Name = "اسلایدر")]
        Home = 0,
        [Display(Name = "آفرها")]
        Offer = 1

    }
}
