using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineStore.Models.Public
{
    public class RecentComment
    {
        public int ID { get; set; }

        public string UserID { get; set; }

        public int ArticleID { get; set; }

        public string ArticleTitle { get; set; }

        public int GroupID { get; set; }

        public string UserName { get; set; }

        public string Text { get; set; }

        public string Subject { get; set; }

        public DateTime LastUpdate { get; set; }

    }
}
