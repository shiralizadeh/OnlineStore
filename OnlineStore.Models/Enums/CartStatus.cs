using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineStore.Models.Enums
{
    public enum CartStatus
    {
        [Display(Name = "در حال خرید")]
        InProgress = 1,

        [Display(Name = "پرداخت موفق")]
        Success = 2,

        [Display(Name = "پرداخت ناموفق")]
        Fail = 3,

        [Display(Name = "در حال پرداخت")]
        DuringPay = 4,

        [Display(Name = "پرداخت در آینده")]
        FuturePay = 5,
    }
}
