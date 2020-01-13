using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineStore.Models.Admin
{
    public class EditScoreParameter
    {
        public EditScoreParameter()
        {
            ID = -1;
            Groups = new List<int>();
        }

        public int ID { get; set; }

        [Display(Name = "عنوان پارامتر")]
        public string Title { get; set; }

        [Display(Name = "فعال")]
        public bool IsActive { get; set; }

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
    }
}
