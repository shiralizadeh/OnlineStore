using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineStore.Models.Public
{
    public class RecentPost
    {
        public int ID { get; set; }
        public int GroupID { get; set; }
        public string Title { get; set; }
        public string GroupTitle { get; set; }
        public string Summary { get; set; }
        public string Image { get; set; }
        public int OrderID { get; set; }
        public DateTime LastUpdate { get; set; }

    }
}
