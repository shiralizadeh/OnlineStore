using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineStore.Models.Enums
{
    public enum StaticContentType
    {
        [Display(Name ="متنی ساده")]
        Text = 0,
        [Display(Name ="متنی پیشرفته")]
        Editor = 1
    }
}
