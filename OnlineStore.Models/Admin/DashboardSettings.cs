using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineStore.Models.Admin
{
    public class DashboardSettings
    {
        public int SiteUsers { get; set; }
        public int Orders { get; set; }
        public int NewOrders { get; set; }
        public string Profit { get; set; }
        public List<ViewUserTask> Tasks { get; set; }
    }
}
