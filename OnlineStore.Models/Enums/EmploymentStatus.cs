using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineStore.Models.Enum
{
    public enum EmploymentStatus : byte
    {
        [Display(Name = "چک نشده")]
        NotChecked = 0,
        [Display(Name = "چک شده")]
        Checked = 1
    }
}
