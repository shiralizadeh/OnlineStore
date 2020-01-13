using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace OnlineStore.Models.Enums
{
    public enum ArticleStatus : byte
    {
        [Display(Name = "چک نشده")]
        NotChecked = 0,

        [Display(Name = "رد شده")]
        Rejected = 1,

        [Display(Name = "تایید شده")]
        Approved = 2,
    }
}
