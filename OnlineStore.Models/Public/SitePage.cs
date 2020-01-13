using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineStore.Models.Public
{
    public class SitePage
    {
        public int ID { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public List<BreadCrumbLink> BreadCrumb { get; set; }
    }
}
