using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OnlineStore.Models.Enums;

namespace OnlineStore.Models.Admin
{
    public class ViewProductQuestion
    {
        public int ID { get; set; }

        public int ProductID { get; set; }

        public string ProductTitle { get; set; }

        public string Question { get; set; }

        public string UserID { get; set; }

        public string UserName { get; set; }

        public string UserFullName { get; set; }

        public DateTime DateTime { get; set; }

        public DateTime LastUpdate { get; set; }

        public QuestionStatus QuestionStatus { get; set; }

        public bool IsVisible { get; set; }

    }
}
