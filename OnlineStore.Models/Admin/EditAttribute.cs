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
    public class EditAttribute
    {
        public EditAttribute()
        {
            ID = -1;

            Groups = new List<int>();
        }

        public int ID { get; set; }

        [Display(Name = "گروه ویژگی")]
        public int AttrGroupID { get; set; }

        [Display(Name = "ویژگی وابسته")]
        public int? DependentID { get; set; }

        public int? DependentOptionID { get; set; }

        [Display(Name = "گزینه ها")]
        public List<EditAttributeOption> Options { get; set; }

        public string JSONOptions
        {
            get
            {
                return JsonConvert.SerializeObject(Options);
            }
            set
            {
                Options = JsonConvert.DeserializeObject<List<EditAttributeOption>>(value);
            }
        }

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
        public string Title { get; set; }

        [Display(Name = "نوع")]
        public AttributeType AttributeType { get; set; }

        [Display(Name = "پسوند")]
        public string Posfix { get; set; }

        [Display(Name = "پیشوند")]
        public string Perfix { get; set; }

        [Display(Name = "قابل جستجو")]
        public bool IsSearchable { get; set; }

        [Display(Name = "ترتیب نمایش")]
        public int OrderID { get; set; }
    }
}
