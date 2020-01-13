using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineStore.Models.Admin
{
    public class ViewProductSuggestion
    {
        public int ID { get; set; }

        public int ProductID { get; set; }

        public string FriendEmail { get; set; }

        public string Message { get; set; }

        public string IP { get; set; }

        public string UserID { get; set; }

        public string UserName { get; set; }

        public DateTime LastUpdate { get; set; }
    }
}
