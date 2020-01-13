using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineStore.Models.Enums
{
    public enum ProductStatus : byte
    {
        [Display(Name = "چک نشده")]
        NotChecked = 0,

        [Display(Name = "رد شده")]
        Rejected = 1,

        [Display(Name = "تایید شده")]
        Approved = 2,
    }
}
