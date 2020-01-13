using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineStore.Models.Enums
{
    public enum GroupType : byte
    {
        [Display(Name = "محصولات")]
        Products = 0,
        [Display(Name = "مطالب وبلاگ")]
        Blogs = 1,
    }
}
