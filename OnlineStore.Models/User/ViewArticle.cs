using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OnlineStore.Models.Enums;

namespace OnlineStore.Models.User
{
    public class UserViewArticle
    {
        public int ID { get; set; }

        [Display(Name = "گروه مطلب")]
        public string GroupName { get; set; }

        [Display(Name = "عنوان")]
        public string Title { get; set; }

        [Display(Name = "تاریخ انتشار")]
        public DateTime PublishDate { get; set; }

        [Display(Name = "وضعیت")]
        public ArticleStatus ArticleStatus { get; set; }

        [Display(Name = "آخرین ویرایش")]
        public DateTime LastUpdate { get; set; }

        [Display(Name = "تعداد بازدید")]
        public int VisitCount { get; set; }

    }
}
