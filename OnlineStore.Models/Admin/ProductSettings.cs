using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Public = OnlineStore.Models.Public;

namespace OnlineStore.Models.Admin
{
    public class ProductSettings
    {
        public List<Public.ViewProducer> Producers { get; set; }
        public List<UserShortInfo> Users { get; set; }
    }
}
