using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineStore.Models.Enums
{
    public enum EmailSendStatus : byte
    {
        [Display(Name = "چک نشده")]
        NotChecked = 0,

        [Display(Name = "در حال ارسال")]
        Sending = 1,

        [Display(Name = "ارسال شده")]
        Sent = 2,

        [Display(Name = "ناموفق")]
        Failed = 3,
    }
}
