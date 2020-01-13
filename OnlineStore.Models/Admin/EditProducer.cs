using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineStore.Models.Admin
{
    public class EditProducer
    {
        public EditProducer()
        {
            ID = -1;

            Groups = new List<int>();
        }

        public int ID { get; set; }

        [Display(Name = "گروه ها")]
        public List<int> Groups { get; set; }

        public string JSONGroups
        {
            get
            {
                return JsonConvert.SerializeObject(Groups);
            }
            set
            {
                Groups = JsonConvert.DeserializeObject<List<int>>(value);
            }
        }

        [Display(Name = "عنوان")]
        [MaxLength(50)]
        public string Title { get; set; }

        [Display(Name = "عنوان انگلیسی")]
        [MaxLength(50)]
        public string TitleEn { get; set; }

        [Display(Name = "تصویر لوگو")]
        [MaxLength(200)]
        public string Filename { get; set; }

        [Display(Name = "متن")]
        public string Text { get; set; }

        [Display(Name = "ترتیب نمایش")]
        public int OrderID { get; set; }

        [Display(Name = "وزن نمایش")]
        public int Weight { get; set; }

        [Display(Name = "قابل نمایش")]
        public bool IsVisible { get; set; }
    }
}
