using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineStore.Models.Public
{
    public class ViewEmailSend
    {
        public int ID { get; set; }
        public ViewEmail From { get; set; }
        public int FromID { get; set; }
        public string To { get; set; }
        public string Subject { get; set; }
        public string Text { get; set; }
        public DateTime? SentDate { get; set; }
    }
}
