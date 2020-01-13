using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineStore.Models.Public
{
    public class ViewProductComment
    {
        public string UserID { get; set; }

        public string UserName { get; set; }

        public string Text { get; set; }

        public DateTime LastUpdate { get; set; }

    }
}
