using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OnlineStore.Models.Enums;

namespace OnlineStore.Models.Public
{
    public class JsonTrackOrder
    {
        public SendStatus SendStatus { get; set; }
        public string BillNumber { get; set; }
        public SendMethodType SendMethodType { get; set; }
    }
}
