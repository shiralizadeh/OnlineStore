using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineStore.Models.Admin
{
    public class ViewScoreParameters
    {
        public int ID { get; set; }
        public string Title { get; set; }
        public string GroupsTitle { get; set; }
        public bool IsActive { get; set; }
        public DateTime LastUpdate { get; set; }
    }
}
