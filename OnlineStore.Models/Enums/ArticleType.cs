using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineStore.Models.Enums
{
    public enum ArticleType : byte
    {
        [Display(Name = "وبلاگ")]
        Blog = 1
    }
}
