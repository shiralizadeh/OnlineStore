using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineStore.Models.Enums
{
    public enum ContactUsMessageStatus : byte
    {
        [Display(Name = "چک نشده")]
        NotChecked = 0,
        [Display(Name = "چک شده")]
        Checked = 1,
        [Display(Name = "پاسخ داده شده")]
        Answered = 2,
    }
}
