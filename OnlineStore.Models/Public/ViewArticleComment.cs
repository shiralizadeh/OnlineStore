using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineStore.Models.Public
{
    public class ViewArticleComment
    {
        public int ID { get; set; }

        public string Subject { get; set; }

        public string Text { get; set; }

        public string UserName { get; set; }

        public DateTime LastUpdate { get; set; }

        public string UserID { get; set; }

    }
}
