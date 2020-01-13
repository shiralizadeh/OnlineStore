using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineStore.Models.Enums
{
    public enum SendStatus
    {
        [Display(Name = "چک نشده")]
        NotChecked = 0,

        [Display(Name = "بررسی شده")]
        Checked = 4,

        [Display(Name = "ارسال شده")]
        Sent = 1,

        [Display(Name = "تحویل داده شد")]
        Delivered = 2,

        [Display(Name = "برگشت داده شد")]
        Returned = 3,
    }
}
