using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineStore.Models.Enums
{
    public enum SendType : byte
    {
        [Display(Name = "توسط مدیر سایت")]
        ByAdmin,

        [Display(Name = "توسط تولید کننده")]
        ByProducer
    }
}
