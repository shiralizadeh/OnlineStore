using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineStore.Models.Admin
{
    public class ViewAttrGroup
    {
        public int ID { get; set; }
        public string Title { get; set; }
        public string GroupsTitle { get; set; }
        public DateTime LastUpdate { get; set; }
        public int OrderID { get; set; }
    }
}
