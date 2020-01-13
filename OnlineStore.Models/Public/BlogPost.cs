using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace OnlineStore.Models.Public
{
    public class BlogPost
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

        [Display(Name = "خلاصه")]
        public string Summary { get; set; }

        [Display(Name = "متن")]
        public string Text { get; set; }

        [Display(Name = "تصویر")]
        public string Image { get; set; }

        [Display(Name = "تاریخ")]
        public DateTime LastUpdate { get; set; }

        [Display(Name = "تاریخ انتشار")]
        public DateTime PublishDate { get; set; }

        [Display(Name = "تعداد بازدید")]
        public int VisitCount { get; set; }

        [Display(Name = "تعداد نظرات")]
        public int CommentsCount { get; set; }

        private float? _sumScore;
        public float? SumScore
        {
            get
            {
                return _sumScore;
            }
            set
            {
                _sumScore = value ?? 0;
            }
        }
        public int ScoreCount { get; set; }
        public float ArticleScore { get; set; }

    }
}
