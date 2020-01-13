using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OnlineStore.Models.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineStore.Models.Admin
{
    public class ViewArticle
    {
        public int ID { get; set; }

        [Display(Name = "گروه مطلب")]
        public string GroupName { get; set; }

        [Display(Name = "گروه مطلب")]
        public int GroupID { get; set; }

        [Display(Name = "کاربر")]
        public string UserID { get; set; }

        public string UserTitle { get; set; }

        [Display(Name = "عنوان")]
        public string Title { get; set; }

        [Display(Name = "تاریخ انتشار")]
        public DateTime PublishDate { get; set; }

        [Display(Name = "وضعیت")]
        public ArticleStatus ArticleStatus { get; set; }

        [Display(Name = "قابل نمایش")]
        public bool IsVisible { get; set; }

        [Display(Name = "نمایش در آخرین اخبار")]
        public bool IsLatestNews { get; set; }

        [Display(Name = "ترتیب نمایش")]
        public int OrderID { get; set; }

        [Display(Name = "آخرین ویرایش")]
        public DateTime LastUpdate { get; set; }
    }
}
