using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OnlineStore.Models.Enums;
using OnlineStore.Models.Public;

namespace OnlineStore.Models.Admin
{
    public class EditScoreComment
    {
        public int ID { get; set; }

        [Display(Name = "کد کاربر")]
        public string UserID { get; set; }

        [Display(Name = "کد محصول")]
        public int ProductID { get; set; }

        [Display(Name = "عنوان محصول")]
        public string ProductTitle { get; set; }

        [Display(Name = "توضیحات")]
        public string Text { get; set; }

        [Display(Name = "وضعیت")]
        public ScoreCommentStatus ScoreCommentStatus { get; set; }

        [Display(Name = "امتیازات")]
        public List<ScoreValue> ScoreValues { get; set; }

        [Display(Name = "آخرین ویرایش")]
        public DateTime LastUpdate { get; set; }
    }
}
