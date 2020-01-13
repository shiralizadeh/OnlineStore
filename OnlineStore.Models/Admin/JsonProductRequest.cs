using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineStore.Models.Admin
{
    public class JsonProductRequest
    {
        public List<string> Emails { get; set; }
        public List<string> Mobiles { get; set; }
    }
}
