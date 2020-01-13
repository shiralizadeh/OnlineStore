using Newtonsoft.Json;
using OnlineStore.Models.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineStore.Models.Admin
{
    public class EditGroup
    {
        public EditGroup()
        {
            ID = -1;
            Banners = new List<EditGroupBanner>();
        }

        public int ID { get; set; }

        [Display(Name = "عنوان")]
        public string Title { get; set; }

        [Display(Name = "عنوان انگلیسی")]
        public string TitleEn { get; set; }

        [Display(Name = "پیشوند فارسی")]
        public string Perfix { get; set; }

        [Display(Name = "پیشوند انگلیسی")]
        public string Perfix_En { get; set; }

        [Display(Name = "پدر")]
        public int? ParentID { get; set; }

        [Display(Name = "نوع گروه")]
        public GroupType GroupType { get; set; }

        [Display(Name = "عکس گروه")]
        public string Image { get; set; }

        [Display(Name = "عکس دکمه")]
        public string ButtonImage { get; set; }

        [Display(Name = "توضیحات")]
        public string Text { get; set; }

        [Display(Name = "عکس ها")]
        public List<EditGroupBanner> Banners { get; set; }
        public string JSONBanners
        {
            get
            {
                return JsonConvert.SerializeObject(Banners);
            }
            set
            {
                Banners = JsonConvert.DeserializeObject<List<EditGroupBanner>>(value);
            }
        }
    }
}
