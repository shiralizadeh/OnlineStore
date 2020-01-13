using OnlineStore.Models.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineStore.Models.Admin
{
    public class EditProductSupply
    {
        [Display(Name = "تعداد")]
        public int Count { get; set; }

        [Display(Name = "نوع")]
        public SupplyType SupplyType { get; set; }

        [Display(Name = "توضیحات")]
        public string Description { get; set; }

        public DateTime LastUpdate { get; set; }
    }
}
