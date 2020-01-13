using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineStore.Models.Public
{
   public class ViewProductGroup
    {
        public int ID { get; set; }
        public int GroupID { get; set; }
        public string Title { get; set; }
        public string TitleEn { get; set; }
        public int? ParentID { get; set; }
        public string Perfix { get; set; }
    }
}
