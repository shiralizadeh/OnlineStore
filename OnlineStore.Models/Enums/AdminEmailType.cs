using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineStore.Models.Enums
{
    public enum AdminEmailType : byte
    {
        [Display(Name = "سفارش - جدید")]
        NewOrder = 0,
        [Display(Name = "پیام تماس با ما - جدید")]
        NewContactMessage = 1,
        [Display(Name = "نظر محصول - جدید")]
        NewProductComment = 2,
        [Display(Name = "نظر پست - جدید")]
        NewBlogComment = 3,
        [Display(Name = "پرسش - جدید")]
        NewProductQuestion = 4,
        [Display(Name = "متقاضی استخدام - جدید")]
        NewEmployment = 5,
        [Display(Name = "متقاضی همکاری - جدید")]
        NewColleague = 6
    }
}
