using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OnlineStore.Models.Enums;

namespace OnlineStore.Models.Admin
{
    public class ViewProductComments
    {
        public int ID { get; set; }

        public int ProductID { get; set; }

        public string UserID { get; set; }

        public string UserName { get; set; }

        public string UserFullName { get; set; }

        public string Email { get; set; }

        public string Subject { get; set; }

        public CommentStatus CommentStatus { get; set; }

        public DateTime LastUpdate { get; set; }

    }
}
