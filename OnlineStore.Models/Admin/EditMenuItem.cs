using Newtonsoft.Json;
using OnlineStore.Models.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineStore.Models.Admin
{
    public class EditMenuItem
    {
        public EditMenuItem()
        {
            ID = -1;
            Banners = new List<EditMenuItemBanner>();
        }

        public int ID { get; set; }

        [Display(Name = "پدر")]
        public int? ParentID { get; set; }

        [Display(Name = "عنوان")]
        public string Title { get; set; }

        [Display(Name = "محتوا")]
        public string Content { get; set; }

        [Display(Name = "لینک")]
        public string Link { get; set; }

        [Display(Name = "نوع آیتم")]
        public MenuItemType MenuItemType { get; set; }

        [Display(Name = "قابل نمایش")]
        public bool IsVisible { get; set; }

        [Display(Name = "نمایش در بردکرامب")]
        public bool ShowInBreadCrumb { get; set; }

        [Display(Name = "کلاس آیکن")]
        [MaxLength(50)]
        public string IconClass { get; set; }

        [Display(Name = "ترتیب نمایش")]
        public int OrderID { get; set; }

        public DateTime LastUpdate { get; set; }

        [Display(Name = "عکس ها")]
        public List<EditMenuItemBanner> Banners { get; set; }
        public string JSONBanners
        {
            get
            {
                return JsonConvert.SerializeObject(Banners);
            }
            set
            {
                Banners = JsonConvert.DeserializeObject<List<EditMenuItemBanner>>(value);
            }
        }
    }
}
