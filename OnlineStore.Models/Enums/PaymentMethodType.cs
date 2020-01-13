using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace OnlineStore.Models.Enums
{
    public enum PaymentMethodType : byte
    {
        [Display(Name = "پرداخت آنلاین توسط کارت های شتاب")]
        Online = 0,

        [Display(Name = "انتقال کارت به کارت شتاب")]
        Card = 1,

        [Display(Name = "پرداخت در محل توسط کارت خوان")]
        Home = 2,
    }
}
