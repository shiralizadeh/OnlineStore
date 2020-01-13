using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OnlineStore.Models.Enums;

namespace OnlineStore.Models.Public
{
    public class ViewProductQuestion
    {
        public string Question { get; set; }

        public string Reply { get; set; }

        public string UserID { get; set; }

        public string UserName { get; set; }

        public string UserFullName { get; set; }

        public DateTime LastUpdate { get; set; }

    }
}
