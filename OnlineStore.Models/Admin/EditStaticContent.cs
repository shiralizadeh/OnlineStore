using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OnlineStore.Models.Enums;

namespace OnlineStore.Models.Admin
{
    public class EditStaticContent
    {
        public int ID { get; set; }

        [Display(Name = "کد")]
        public string Name { get; set; }

        [Display(Name = "عنوان")]
        public string Title { get; set; }

        [Display(Name = "متن")]
        public string SimpleContent { get; set; }

        [Display(Name = "متن")]
        public string EditorContent { get; set; }

        [Display(Name = "نوع")]
        public StaticContentType StaticContentType { get; set; }

        [Display(Name = "آخرین ویرایش")]
        public DateTime LastUpdate { get; set; }

    }
}
