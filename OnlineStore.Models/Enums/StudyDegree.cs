using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineStore.Models.Enum
{
    public enum StudyDegree : byte
    {
        [Display(Name = "دیپلم")]
        Diploma = 0,
        [Display(Name = "فوق دیپلم")]
        AssociateDegree = 1,
        [Display(Name = "لیسانس")]
        BA = 2,
        [Display(Name = "فوق لیسانس")]
        MA = 3,
        [Display(Name = "دکترا")]
        PHD = 4
    }
}
